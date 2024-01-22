using _06_Exceptions_Catchers.DomainModel;
using _06_Exceptions_Catchers.WebsiteModel;

namespace _06_Exceptions_Catchers.WebsiteMapper;

public class CustomMapperException(string message) : Exception(message);

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
        throw new CustomMapperException("Exception to make your life so much easier!");
        return new Customer(websiteCustomer.Name);
    }
}