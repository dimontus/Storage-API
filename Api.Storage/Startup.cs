using System.IO.Abstractions;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using AspNetCore.Infrastructure;
using AspNetCore.Infrastructure.Swagger;
using DataAccess.Configuration;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Api.DataAccess.Migrations;
using Api.DataAccess.Repositories;
using Api.Storage.Configuration;
using Api.Storage.Http;
using Api.Storage.Services;
using Api.Storage.Services.FileProviders;
using Api.Storage.Services.StorageProviders;

namespace Api.Storage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbOptions(Configuration);
            services.Configure<StorageSettings>(Configuration.GetSection("StorageSettings"));

            var type = Configuration.GetValue<StorageType>("StorageSettings:Type");

            switch (type)
            {
                case StorageType.FileSystem:
                    services
                        .AddScoped<IFileProvider, FileSystemProvider>()
                        .AddScoped<IStorageProvider, FileSystemStorageProvider>();
                    break;
                case StorageType.S3:
                    var s3Settings = Configuration.GetSection("StorageSettings:S3").Get<S3Settings>();

                    var config = new AmazonS3Config
                    {
                        ServiceURL = s3Settings.Endpoint,
                        ForcePathStyle = true,
                    };

                    var amazonS3Client = new AmazonS3Client(s3Settings.AccessKey, s3Settings.SecretKey, config);

                    services
                        .AddSingleton<IAmazonS3>(amazonS3Client)
                        .AddScoped<IFileProvider, S3Provider>()
                        .AddScoped<IStorageProvider, S3StorageProvider>();
                    break;
                default:
                    services
                        .AddScoped<IFileProvider, NoopProvider>()
                        .AddScoped<IStorageProvider, NoopStorageProvider>();
                    break;
            }

            services
                .AddScoped<IFileSystem, FileSystem>()
                .AddScoped<IStorageService, StorageService>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<IStorageRepository, StorageRepository>()
                .AddScoped<IFileRepository, FileRepository>()
                .AddSingleton<IActionResultExecutor<StorageFileResult>, StorageFileResultExecutor>();

            services.AddAutoMapper(typeof(Startup));

            //FIXME временно, пересмотреть значения
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            });

            //FIXME временно, пересмотреть значения
            services.Configure<FormOptions>(x =>
            {
                x.BufferBodyLengthLimit = long.MaxValue;
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            services.AddCors();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services.AddControllers();
            
            services.AddCloudSalCommonServices(Configuration, "SmartLogger Storage API");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseCloudSalMiddlewares();

            app.UseSwaggerConfiguration("SmartLogger Storage API");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
