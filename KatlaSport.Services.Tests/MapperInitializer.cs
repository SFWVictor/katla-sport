using System;
using AutoMapper;
using KatlaSport.Services.HiveManagement;
using KatlaSport.Services.ProductManagement;

namespace KatlaSport.Services.Tests
{
    public class MapperInitializer
    {
        private static readonly Lazy<MapperInitializer> _instanсeInitializer = new Lazy<MapperInitializer>(() => new MapperInitializer());

        private MapperInitializer()
        {
            Mapper.Reset();
            Mapper.Initialize(x =>
            {
                x.AddProfile(new HiveManagementMappingProfile());
                x.AddProfile(new ProductManagementMappingProfile());
            });
        }

        public static MapperInitializer Initialize()
        {
            return _instanсeInitializer.Value;
        }
    }
}
