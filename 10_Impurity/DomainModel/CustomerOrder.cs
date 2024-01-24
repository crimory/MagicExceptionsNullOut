namespace _10_Impurity.DomainModel;

public class CustomerOrder(
    string number,
    CustomerOrderPosition[] positions,
    Customer customer)
{
    public string Number { get; set; } = number;
    public CustomerOrderPosition[] Positions { get; set; } = positions;
    public Customer Customer { get; set; } = customer;
}

public class CustomerOrderPosition(
    string itemName,
    uint quantity)
{
    public string ItemName { get; set; } = itemName;
    public uint Quantity { get; set; } = quantity;
}

public class Customer(
    string name)
{
    public string Name { get; set; } = name;
}