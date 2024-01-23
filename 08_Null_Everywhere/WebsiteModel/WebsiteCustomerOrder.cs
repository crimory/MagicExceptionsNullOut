namespace _08_Null_Everywhere.WebsiteModel;

public record WebsiteCustomerOrder(
    string? Number,
    WebsiteCustomerOrderPosition[]? Positions,
    WebsiteCustomer? Customer);

public record WebsiteCustomerOrderPosition(string? ItemName, string? Quantity);
public record WebsiteCustomer(string? Name);