namespace _07_Railway_Oriented.Errors;

internal record DomainError(string Message);

public abstract record ErrorOrOutput<T>
{
    private ErrorOrOutput()
    {
    }

    internal record Error(DomainError[] DomainErrors) : ErrorOrOutput<T>;
    internal record ActualValue(T Value) : ErrorOrOutput<T>;

    internal ErrorOrOutput<TK> PropagateError<TK>(Func<T, ErrorOrOutput<TK>> abc)
    {
        return this switch
        {
            Error error => new ErrorOrOutput<TK>.Error(error.DomainErrors),
            ActualValue actualValue => abc(actualValue.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public static class RailwayUtility
{
    public static ErrorOrOutput<T> WrapValue<T>(T value) =>
        new ErrorOrOutput<T>.ActualValue(value);
    
    public static ErrorOrOutput<TO> RailwayPropagate<TI, TO>(
        this ErrorOrOutput<TI> input,
        Func<TI, ErrorOrOutput<TO>> internalProcess)
    {
        return input.PropagateError(internalProcess);
    }
}