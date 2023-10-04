using AutoMapper;

namespace EmployeeManagement.Api.Helper.AutoMapper
{

    public class MyMappingHelper<TSource, TDestination> : Profile
    {
        public MyMappingHelper()
        {
            CreateMap<TSource, TDestination>();
        }
    }
}
