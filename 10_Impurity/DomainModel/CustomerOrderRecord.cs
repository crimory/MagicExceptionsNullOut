using _10_Impurity.Errors;

namespace _10_Impurity.DomainModel;

public record CustomerOrderRec(
    string Number,
    CustomerOrderPositionRec[] Positions,
    CustomerRec Customer);

public record CustomerOrderPositionRec(
    string ItemName,
    uint Quantity);

public record CustomerRec(
    string Name);

public static class RecordLogUsage
{
    private static void LogUsageExample()
    {
        var order = new CustomerOrderRec("123",
            [
                new CustomerOrderPositionRec("Item 1", 5),
                new CustomerOrderPositionRec("Item 2", 3)
            ],
            new CustomerRec("John Doe"));
        var orderOrError = RailwayUtility.RailwayBind(order);
        LogInfo(orderOrError);
    }
    
    private static void LogInfo(ErrorOrOutput<CustomerOrderRec> orderOrError)
    {
        _ = orderOrError.Match(
            order =>
            {
                // order.Customer.Name = "Captain Pollution was here!";
                order.Positions
                    .SetValue(new CustomerOrderPositionRec("test", 123), 0);
                return true;
            },
            _ => true);
    }
}