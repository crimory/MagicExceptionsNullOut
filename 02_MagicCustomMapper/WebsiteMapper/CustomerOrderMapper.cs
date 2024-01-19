using _02_MagicCustomMapper.WebsiteModel;

namespace _02_MagicCustomMapper.WebsiteMapper;

public static class CustomerOrderMapper
{
    public static DomainModel.CustomerOrder Map(this WebsiteCustomerOrder websiteOrder)
    {
        return new DomainModel.CustomerOrder(
            websiteOrder.Number,
            websiteOrder.Positions.Select(p => p.Map())
                .Where(x => x.Quantity > 0)
                .ToArray(),
            websiteOrder.Customer.Map());
    }

    private static DomainModel.CustomerOrderPosition Map(this WebsiteCustomerOrderPosition websitePosition)
    {
        return new DomainModel.CustomerOrderPosition(
            websitePosition.ItemName,
            uint.TryParse(websitePosition.Quantity, out var quantity) ? quantity : 0);
    }
    
    private static DomainModel.Customer Map(this WebsiteCustomer websiteCustomer)
    {
        return new DomainModel.Customer(websiteCustomer.Name);
    }
}