using _10_Impurity.DomainModel;
using _10_Impurity.Errors;

namespace _10_Impurity.DomainModelValidation;

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
        p => p.Length >= 1
            ? new ValidOrNot.Valid()
            : new ValidOrNot.NonValid([
                CreateResult($"{nameof(CustomerOrder)}.{nameof(CustomerOrder.Positions)}",
                    "cannot be empty")
            ]),
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

    public static ErrorOrOutput<CustomerOrder> Validate(CustomerOrder order)
    {
        return order
            .ValidateInternal()
            .Match<ErrorOrOutput<CustomerOrder>>(
                () => new ErrorOrOutput<CustomerOrder>.ActualValue(order),
                results => new ErrorOrOutput<CustomerOrder>.Error(results));
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

    private static DomainError CreateResult(string memberName, string errorMessage)
    {
        return new DomainError($"Validation for {memberName}: {errorMessage}");
    }

    private abstract record ValidOrNot
    {
        private ValidOrNot()
        {
        }

        internal record Valid : ValidOrNot;

        internal record NonValid(DomainError[] Results) : ValidOrNot;
        
        internal TOutput Match<TOutput>(Func<TOutput> processValid, Func<DomainError[], TOutput> processNonValid)
        {
            return this switch
            {
                NonValid nonValid => processNonValid(nonValid.Results),
                Valid => processValid(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        internal DomainError[] GetResults()
        {
            return Match(
                Array.Empty<DomainError>,
                nonValid => nonValid);
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