﻿using AutoMapper;
using EmployeeManagement.Entities.Models.DTOModels;
using EmployeeManagement.Entities.Models.PayloadModel;

namespace EmployeeManagement.Helper
{
    
    public class MyMappingHelper<TSource, TDestination> : Profile
    {
        public MyMappingHelper()
        {
            CreateMap<TSource, TDestination>();
        }
    }  
}
