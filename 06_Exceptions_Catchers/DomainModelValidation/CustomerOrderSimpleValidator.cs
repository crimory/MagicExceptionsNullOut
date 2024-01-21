using System.ComponentModel.DataAnnotations;
using _06_Exceptions_Catchers.DomainModel;

namespace _06_Exceptions_Catchers.DomainModelValidation;

public static class CustomerOrderSimpleValidator
{
    public static List<ValidationResult> Validate(this CustomerOrder order)
    {
        var results = new List<ValidationResult>();
        if (order.Number.Length is < 3 or > 50)
            results.Add(new ValidationResult(
                $"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Number)} should be between 3 and 50 characters long",
                new []{$"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Number)}"}));
        results.AddRange(order.Positions.Validate());
        results.AddRange(order.Customer.Validate());
        return results;
    }
    
    private static List<ValidationResult> Validate(this CustomerOrderPosition[] positions)
    {
        throw new Exception("Did you really think that was the only example?!");
        var results = new List<ValidationResult>();
        if (positions.Length < 1)
            results.Add(new ValidationResult(
                $"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Positions)} cannot be empty",
                new []{$"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Positions)}"}));
        foreach (var position in positions)
        {
            results.AddRange(position.Validate());
        }
        return results;
    }
    
    private static List<ValidationResult> Validate(this CustomerOrderPosition position)
    {
        var results = new List<ValidationResult>();
        if (position.ItemName.Length is < 3 or > 50)
            results.Add(new ValidationResult(
                $"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Positions)}.{nameof(CustomerOrderPosition.ItemName)} should be between 3 and 50 characters long",
                new []{$"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Positions)}[x].{nameof(CustomerOrderPosition.ItemName)}"}));
        if (position.Quantity is < 5 or > 1000)
            results.Add(new ValidationResult(
                $"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Positions)}.{nameof(CustomerOrderPosition.Quantity)} should be between 5 and 1000",
                new []{$"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Positions)}[x].{nameof(CustomerOrderPosition.Quantity)}"}));
        return results;
    }
    
    private static List<ValidationResult> Validate(this Customer customer)
    {
        var results = new List<ValidationResult>();
        if (customer.Name.Length is < 3 or > 50)
            results.Add(new ValidationResult(
                $"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Customer)}.{nameof(Customer.Name)} should be between 3 and 50 characters long",
                new []{$"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Customer)}.{nameof(Customer.Name)}"}));
        return results;
    }
}