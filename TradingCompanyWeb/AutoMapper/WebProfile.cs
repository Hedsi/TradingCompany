// AutoMapper/WebProfile.cs
using AutoMapper;
using TradingCompany.DTO;
using TradingCompanyWeb.ViewModels;

namespace TradingCompanyWeb.AutoMapper
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            CreateMap<Product, ProductViewModel>();
            CreateMap<ProductViewModel, Product>();
        }
    }
}