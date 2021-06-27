using AutoMapper;
using Newtonsoft.Json;
using Odin.Plugs.OdinCore.Models.Aop;
using Odin.Plugs.OdinCore.Models.ErrorCode;
using Odin.Plugs.OdinInject;
using Odin.Plugs.OdinMvcCore.OdinInject;
using Odin.Plugs.OdinNetCore.OdinSnowFlake.SnowFlakeInterface;
using OdinCore.Models.DbModels;

namespace OdinCore.Models
{
    public class AutoMapperBootStrapper : Profile
    {
        public AutoMapperBootStrapper()
        {
            CreateMap<ErrorCode_DbModel, ErrorCode_Model>()
                .ForMember(dest => dest.ShowMessage, opt => opt.MapFrom(src => src.CodeShowMessage))
                .ForMember(dest => dest.ErrorMessage, opt => opt.MapFrom(src => src.CodeErrorMessage));

            CreateMap<Aop_ApiInvokerRecord_Model, Aop_ApiInvokerRecord_DbModel>()
                .BeforeMap((source, dto) =>
                {
                    dto.Id = source.Id ?? (long)OdinInjectHelper.GetService<IOdinSnowFlake>().NextId();
                })
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<Aop_ApiInvokerCatch_Model, Aop_ApiInvokerCatch_DbModel>()
                .BeforeMap((source, dto) =>
                {
                    dto.Id = source.Id ?? (long)OdinInjectHelper.GetService<IOdinSnowFlake>().NextId();
                })
                .ForMember(dest => dest.StrParam, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.StrParam)))
                .ForMember(dest => dest.ExceptionMessage, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Ex)));

            //ApiCommentConfig转TF_Api_DbModel.
            // CreateMap<ApiCommentConfig, TF_Api_DbModel>()
            //     //映射发生之前
            //     .BeforeMap((source, dto) =>
            //     {
            //         //可以较为精确的控制输出数据格式
            //         //dto.CreateTime = Convert.ToDateTime(source.CreateTime).ToString("yyyy-MM-dd");
            //     })
            //     //映射发生之后
            //     .AfterMap((source, dto) =>
            //     {
            //         //code ...
            //     })
            //     .ForMember(dest => dest.AllowScope, opt => opt.MapFrom(src => src.AllowScope));

        }
    }
}