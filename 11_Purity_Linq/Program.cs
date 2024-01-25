// See https://aka.ms/new-console-template for more information

var input = File.ReadAllText("Day05/input.txt");
var lowestLocation = _11_Purity_Linq.Day05.Part1.GetLowestLastCategory(input);
Console.WriteLine($"Day 05 Part 1: lowest location number is: {lowestLocation}");
var lowestLocationWithRanges = _11_Purity_Linq.Day05.Part2.GetLowestLastCategoryForRange(input);
Console.WriteLine($"Day 05 Part 2: lowest location number (according to seed ranges) is: {lowestLocationWithRanges}");