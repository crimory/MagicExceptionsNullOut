namespace _09_Out_Hidden_Output.OutSubstitute;

public abstract record DomainOption<T>
{
    private DomainOption()
    {
    }

    internal record None : DomainOption<T>;
    internal record Some(T Value) : DomainOption<T>;
    
    internal TOutput Match<TOutput>(Func<T, TOutput> processSome, Func<TOutput> processNone)
    {
        return this switch
        {
            None => processNone(),
            Some some => processSome(some.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public static class OutSubstitute
{
    public static bool ClassicTryParse(this string input, out uint output)
    {
        var tryParseResult = uint.TryParse(input, out var tryParseOutput);
        output = tryParseOutput;
        return tryParseResult;
    }
    
    public static (bool, uint) TupleTryParse(this string input)
    {
        var tryParseResult = uint.TryParse(input, out var tryParseOutput);
        return (tryParseResult, tryParseOutput);
    }
    
    public static (bool success, uint value) NamedTupleTryParse(this string input)
    {
        var tryParseResult = uint.TryParse(input, out var tryParseOutput);
        return (success: tryParseResult, value: tryParseOutput);
    }
    
    public static DomainOption<uint> DiscriminatedUnionTryParse(this string input)
    {
        var tryParseResult = uint.TryParse(input, out var tryParseOutput);
        return tryParseResult
            ? new DomainOption<uint>.Some(tryParseOutput)
            : new DomainOption<uint>.None();
    }
}