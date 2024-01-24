namespace _10_Impurity.OutSubstitute;

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
    
    internal void Match(Action<T> processSome, Action processNone)
    {
        switch (this)
        {
            case None:
                processNone();
                break;
            case Some some:
                processSome(some.Value);
                break;
        }
    }
}

public static class OutSubstitute
{
    public static DomainOption<uint> DiscriminatedUnionTryParse(this string input)
    {
        var tryParseResult = uint.TryParse(input, out var tryParseOutput);
        return tryParseResult
            ? new DomainOption<uint>.Some(tryParseOutput)
            : new DomainOption<uint>.None();
    }
}