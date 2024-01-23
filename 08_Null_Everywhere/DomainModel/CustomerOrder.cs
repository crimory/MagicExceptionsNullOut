namespace _08_Null_Everywhere.DomainModel;

public record CustomerOrder(
    string Number,
    CustomerOrderPosition[] Positions,
    Customer Customer);

public record CustomerOrderPosition(
    string ItemName,
    uint Quantity);

public record Customer(
    string Name);