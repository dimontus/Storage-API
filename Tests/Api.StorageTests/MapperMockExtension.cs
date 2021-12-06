using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.StorageTests
{
    public static class MapperMockExtension
    {
        public static void CreateMap<TSource, TDestination>(this Mock<IMapper> mapperMock, TDestination result)
        {
            mapperMock.Setup(k => k.Map<TDestination>(It.IsAny<TSource>())).Returns(result);
        }
    }
}
