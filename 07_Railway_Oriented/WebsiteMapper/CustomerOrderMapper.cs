using _07_Railway_Oriented.DomainModel;
using _07_Railway_Oriented.WebsiteModel;

namespace _07_Railway_Oriented.WebsiteMapper;

public static class CustomerOrderMapper
{
    public static CustomerOrder Map(this WebsiteCustomerOrder websiteOrder)
    {
        return new CustomerOrder(
            websiteOrder.Number,
            websiteOrder.Positions.Select(p => p.Map())
                .Where(x => x.Quantity > 0)
                .ToArray(),
            websiteOrder.Customer.Map());
    }

    private static CustomerOrderPosition Map(this WebsiteCustomerOrderPosition websitePosition)
    {
        return new CustomerOrderPosition(
            websitePosition.ItemName,
            uint.TryParse(websitePosition.Quantity, out var quantity) ? quantity : 0);
    }
    
    private static Customer Map(this WebsiteCustomer websiteCustomer)
    {
        return new Customer(websiteCustomer.Name);
    }
}