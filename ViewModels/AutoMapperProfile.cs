using AutoMapper;
using DRM_Data;
using DRM_Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRM.ViewModels
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationEvaluationResult, ApplicationEvaluationResultViewModel>().ForMember(x => x.TotalRecords, o => o.Ignore());
        }
    }
}
