namespace _09_Out_Hidden_Output.DomainModel;

public record CustomerOrder(
    string Number,
    CustomerOrderPosition[] Positions,
    Customer Customer);

public record CustomerOrderPosition(
    string ItemName,
    uint Quantity);

public record Customer(
    string Name);