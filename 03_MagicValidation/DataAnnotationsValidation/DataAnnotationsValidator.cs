using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace _03_MagicValidation.DataAnnotationsValidation;

public static class DataAnnotationsValidator
{
    public static List<ValidationResult> Validate<T>(this T obj)
    {
        if (obj == null)
        {
            return [];
        }
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj, serviceProvider: null, items: null);

        _ = Validator.TryValidateObject(obj, context, results, true);

        return results;
    }
    
    public static List<ValidationResult> FixedValidate<T>(this T obj)
    {
        if (obj == null)
        {
            return [];
        }
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj, serviceProvider: null, items: null);

        _ = Validator.TryValidateObject(obj, context, results, true);
        
        foreach (var prop in obj.GetType().GetProperties()
                     .Where(p => p.CanRead && p.GetIndexParameters().Length == 0))
        {
            var value = prop.GetValue(obj);
            switch (value)
            {
                case null:
                    continue;
                case IEnumerable asEnumerable:
                {
                    foreach (var child in asEnumerable)
                    {
                        if (child == null)
                            continue;
                        results.AddRange(FixedValidate(child));
                    }
                    break;
                }
                default:
                    results.AddRange(FixedValidate(value));
                    break;
            }
        }

        return results;
    }
}