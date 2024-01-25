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
    private static bool ClassicTryParse(this string input, out uint output)
    {
        var tryParseResult = uint.TryParse(input, out var tryParseOutput);
        output = tryParseOutput;
        return tryParseResult;
    }

    private static (bool, uint) TupleTryParse(this string input)
    {
        var tryParseResult = uint.TryParse(input, out var tryParseOutput);
        return (tryParseResult, tryParseOutput);
    }

    private static (bool success, uint value) NamedTupleTryParse(this string input)
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

    private static void UsageExamples()
    {
        const string input = "123";
        
        // Classic TryParse
        Console.WriteLine(input.ClassicTryParse(out var output)
            ? $"Classic TryParse: Success! Output: {output}"
            : "Classic TryParse: Failed!");
        
        // Tuple TryParse
        var (success, value) = input.TupleTryParse();
        Console.WriteLine(success
            ? $"Tuple TryParse: Success! Output: {value}"
            : "Tuple TryParse: Failed!");
        
        // Named Tuple TryParse
        var result = input.NamedTupleTryParse();
        Console.WriteLine(result.success
            ? $"Named Tuple TryParse: Success! Output: {result.value}"
            : "Named Tuple TryParse: Failed!");
        
        // Discriminated Union TryParse
        Console.WriteLine(input.DiscriminatedUnionTryParse().Match(
            someValue => $"Discriminated Union TryParse: Success! Output: {someValue}",
            () => "Discriminated Union TryParse: Failed!"));
    }
}