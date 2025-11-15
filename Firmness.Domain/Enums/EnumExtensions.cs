using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Firmness.Domain.Enums;


// Extension methods for working with ENUMs

public static class EnumExtensions
{
    // Obtains the DisplayName of an enum if it exists, otherwise returns the name
    public static string GetDisplayName(this Enum enumValue)
    {
        var displayAttribute = enumValue.GetType()
            .GetField(enumValue.ToString())?
            .GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? enumValue.ToString();
    }

 
    // Obtains the Description of an enum if it exists, otherwise returns the name
    public static string GetDescription(this Enum enumValue)
    {
        var displayAttribute = enumValue.GetType()
            .GetField(enumValue.ToString())?
            .GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Description ?? enumValue.ToString();
    }
    
    // Converts an enum to string
    public static string ToStringValue(this Enum enumValue)
    {
        return enumValue.ToString().ToUpperInvariant();
    }
    
    // Converts string to enum
    public static T ParseEnum<T>(this string value) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, true, out var result))
        {
            return result;
        }
        return default;
    }
}

