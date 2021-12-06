using AutoMapper;

using Api.DataAccess.Models;
using Api.Storage.Models;

namespace Api.Storage.Configuration
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<CreateStorageRequest, DataAccess.Models.Storage>();
            CreateMap<StorageInfo, DataAccess.Models.Storage>();
            CreateMap<DataAccess.Models.Storage, StorageInfo>();

            CreateMap<StorageFile, StorageFileInfo>()
                .ForMember(dest => dest.FileName, 
                    opts => opts.MapFrom(src => src.OriginalFileName));

            CreateMap<StorageFileInfo, StorageFile>();
        }
    }
}
