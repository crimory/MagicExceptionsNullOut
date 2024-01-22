using System.ComponentModel.DataAnnotations;
using _06_Exceptions_Catchers.DomainModel;

namespace _06_Exceptions_Catchers.DomainModelValidation;

public class CustomValidationException(string message) : Exception(message);

public static class CustomerOrderSimpleValidator
{
    private static readonly Func<CustomerOrder, ValidOrNot>[] CustomerOrderValidators =
    [
        o => o.Number.Length is >= 3 and <= 50
            ? new ValidOrNot.Valid()
            : new ValidOrNot.NonValid([
                CreateResult($"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Number)}",
                    "should be between 3 and 50 characters long")
            ]),
        o => o.Positions.Validate(),
        o => o.Customer.Validate()
    ];

    private static readonly Func<CustomerOrderPosition[], ValidOrNot>[] CustomerOrderPositionsValidators =
    [
        p =>
        {
            throw new CustomValidationException("Did you really think that was the only example?!");
            return p.Length >= 1
                ? new ValidOrNot.Valid()
                : new ValidOrNot.NonValid([
                    CreateResult($"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Positions)}",
                        "cannot be empty")
                ]);
        },
        p => p
            .Select(x => x.Validate())
            .CombineMultipleValidOrNot()
    ];

    private static readonly Func<CustomerOrderPosition, ValidOrNot>[] CustomerOrderPositionValidators =
    [
        p => p.ItemName.Length is >= 3 and <= 50
            ? new ValidOrNot.Valid()
            : new ValidOrNot.NonValid([
                CreateResult(
                    $"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Positions)}[x].{nameof(CustomerOrderPosition.ItemName)}",
                    "should be between 3 and 50 characters long")
            ]),
        p => p.Quantity is >= 5 and <= 1000
            ? new ValidOrNot.Valid()
            : new ValidOrNot.NonValid([
                CreateResult(
                    $"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Positions)}[x].{nameof(CustomerOrderPosition.Quantity)}",
                    "should be between 5 and 1000")
            ])
    ];

    private static readonly Func<Customer, ValidOrNot>[] CustomerValidators =
    [
        p => p.Name.Length is >= 3 and <= 50
            ? new ValidOrNot.Valid()
            : new ValidOrNot.NonValid([
                CreateResult($"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Customer)}.{nameof(Customer.Name)}",
                    "should be between 3 and 50 characters long")
            ])
    ];

    public static ValidationResult[] Validate(this CustomerOrder order)
    {
        return order.ValidateInternal() switch
        {
            ValidOrNot.NonValid nonValid => nonValid.Results,
            ValidOrNot.Valid => Array.Empty<ValidationResult>(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static ValidOrNot ValidateInternal(this CustomerOrder order)
    {
        return CustomerOrderValidators
            .Select(x => x(order))
            .CombineMultipleValidOrNot();
    }

    private static ValidOrNot Validate(this CustomerOrderPosition[] positions)
    {
        return CustomerOrderPositionsValidators
            .Select(x => x(positions))
            .CombineMultipleValidOrNot();
    }

    private static ValidOrNot Validate(this CustomerOrderPosition position)
    {
        return CustomerOrderPositionValidators
            .Select(x => x(position))
            .CombineMultipleValidOrNot();
    }

    private static ValidOrNot Validate(this Customer customer)
    {
        return CustomerValidators
            .Select(x => x(customer))
            .CombineMultipleValidOrNot();
    }

    private static ValidationResult CreateResult(string memberName, string errorMessage)
    {
        return new ValidationResult($"{memberName} {errorMessage}", new[] { memberName });
    }

    private abstract record ValidOrNot
    {
        private ValidOrNot()
        {
        }

        internal record Valid : ValidOrNot;

        internal record NonValid(ValidationResult[] Results) : ValidOrNot;

        internal ValidationResult[] GetResults()
        {
            return this switch
            {
                Valid => Array.Empty<ValidationResult>(),
                NonValid nonValid => nonValid.Results,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    private static ValidOrNot CombineMultipleValidOrNot(this IEnumerable<ValidOrNot> input)
    {
        var inputs = input as ValidOrNot[] ?? input.ToArray();
        return inputs.All(x => x is ValidOrNot.Valid)
            ? new ValidOrNot.Valid()
            : new ValidOrNot.NonValid(inputs.SelectMany(x => x.GetResults()).ToArray());
    }
}