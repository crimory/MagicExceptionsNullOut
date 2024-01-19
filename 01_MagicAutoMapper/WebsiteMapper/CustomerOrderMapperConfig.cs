using _01_MagicAutoMapper.WebsiteModel;
using AutoMapper;

namespace _01_MagicAutoMapper.WebsiteMapper;

public static class CustomerOrderMapperConfig
{
    public static readonly MapperConfiguration Config = new(cfg =>
    {
        cfg.CreateMap<WebsiteCustomerOrder, DomainModel.CustomerOrder>();
        cfg.CreateMap<WebsiteCustomerOrderPosition, DomainModel.CustomerOrderPosition>();
        cfg.CreateMap<WebsiteCustomer, DomainModel.Customer>();
    });
}