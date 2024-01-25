namespace _11_Purity_Linq.Day05;

public static class Common
{
    internal record MapRange(
        long DestinationRangeStart,
        long SourceRangeStart,
        long RangeLength);

    internal record Mapping(
        string DestinationName,
        string SourceName,
        MapRange[] Ranges);

    internal abstract record SeedInput
    {
        private SeedInput()
        {
        }
        
        internal record Seed(long SeedNumber) : SeedInput;
        internal record SeedRange(long RangeStart, long RangeLength) : SeedInput;
    }

    private static long GetLowestLastCategory(long seed, List<Mapping> mappings)
    {
        return mappings.Aggregate(seed, (acc, mapping) => mapping.GetMappedValue(acc));
    }

    private static long GetMappedValue(this Mapping mapping, long sourceValue)
    {
        var matchingRange = mapping.Ranges.FirstOrDefault(x =>
            x.SourceRangeStart <= sourceValue && x.SourceRangeStart + x.RangeLength > sourceValue);
        if (matchingRange == null)
            return sourceValue;
        var sourceOffset = sourceValue - matchingRange.SourceRangeStart;
        return matchingRange.DestinationRangeStart + sourceOffset;
    }

    private static (SeedInput[] seeds, List<Mapping> mappings) ReadInput(string input, Func<string, SeedInput[]> readSeeds)
    {
        var initialGroups = input.Split($"{Environment.NewLine}{Environment.NewLine}");
        var seeds = readSeeds(initialGroups[0]);
        var mappings = initialGroups.Skip(1).Select(ReadMap).ToList();
        return (seeds, mappings);
    }

    private static Mapping ReadMap(string inputMap)
    {
        var lines = inputMap
            .Split(Environment.NewLine)
            .ToArray();
        var mapHeadings = lines[0].Split(' ')[0].Split('-');
        var ranges = lines
            .Skip(1)
            .Select(x =>
            {
                var a = x.Split(' ').Select(long.Parse).ToArray();
                return new MapRange(a[0], a[1], a[2]);
            })
            .ToArray();
        return new Mapping(mapHeadings[0], mapHeadings[2], ranges);
    }
    
    internal static long GetLowestLastCategory(string input, Func<string, SeedInput[]> readSeeds)
    {
        var (seeds, mappings) = ReadInput(input, readSeeds);
        return seeds
            .AsParallel()
            .SelectMany(GetActualSeed)
            .Select(x => GetLowestLastCategory(x, mappings))
            .Min();
    }
    
    private static long[] GetActualSeed(SeedInput input)
    {
        return input switch
        {
            SeedInput.Seed seed => [seed.SeedNumber],
            SeedInput.SeedRange seedRange => GetLongRange(seedRange.RangeStart, seedRange.RangeLength),
            _ => throw new ArgumentOutOfRangeException(nameof(input))
        };
    }

    private static long[] GetLongRange(long start, long length)
    {
        var output = new HashSet<long>();
        for (long i = 0; i < length; i++)
        {
            output.Add(start + i);
        }

        return output.ToArray();
    }
}

public static class Part1
{
    public static long GetLowestLastCategory(string input)
    {
        return Common.GetLowestLastCategory(input, ReadSeeds);
    }

    private static Common.SeedInput.Seed[] ReadSeeds(string inputLine)
    {
        return inputLine
            .Split(' ')
            .Skip(1)
            .Select(long.Parse)
            .Select(x => new Common.SeedInput.Seed(x))
            .ToArray();
    }
}

public static class Part2
{
    public static long GetLowestLastCategoryForRange(string input)
    {
        return Common.GetLowestLastCategory(input, ReadSeedRanges);
    }

    private static Common.SeedInput.SeedRange[] ReadSeedRanges(string inputLine)
    {
        return inputLine
            .Split(' ')
            .Skip(1)
            .Select(long.Parse)
            .Select((x, i) => (Value: x, Index: i))
            .GroupBy(x => x.Index / 2)
            .Select(x => x.Select(y => y.Value).ToArray())
            .Select(x => new Common.SeedInput.SeedRange(x[0], x[1]))
            .ToArray();
    }
}