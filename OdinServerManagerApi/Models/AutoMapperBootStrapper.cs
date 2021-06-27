using AutoMapper;

namespace demoIdentityServer.Models
{
    public class AutoMapperBootStrapper : Profile
    {
        public AutoMapperBootStrapper()
        {
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