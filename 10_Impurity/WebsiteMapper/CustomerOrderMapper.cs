using _10_Impurity.DomainModel;
using _10_Impurity.WebsiteModel;

namespace _10_Impurity.WebsiteMapper;

public static class CustomerOrderMapper
{
    public static CustomerOrder Map(this WebsiteCustomerOrder websiteOrder)
    {
        return new CustomerOrder(
            websiteOrder.Number ?? string.Empty,
            (websiteOrder.Positions ?? []).Select(p => p.Map())
                .Where(x => x.Quantity > 0)
                .ToArray(),
            websiteOrder.Customer?.Map() ?? new Customer(string.Empty));
    }

    private static CustomerOrderPosition Map(this WebsiteCustomerOrderPosition websitePosition)
    {
        return new CustomerOrderPosition(
            websitePosition.ItemName ?? string.Empty,
            OutSubstitute.OutSubstitute
                .DiscriminatedUnionTryParse(websitePosition.Quantity ?? string.Empty)
                .Match<uint>(some => some, () => 0));
    }
    
    private static Customer Map(this WebsiteCustomer websiteCustomer)
    {
        return new Customer(websiteCustomer.Name ?? string.Empty);
    }
}