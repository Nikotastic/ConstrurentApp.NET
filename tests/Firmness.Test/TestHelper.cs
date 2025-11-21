using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Firmness.Test;

// Test helper methods for setting properties via reflection
public static class TestHelper
{
    public static void SetId<T>(T entity, Guid id) where T : class
    {
        var prop = typeof(T).GetProperty("Id");
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(entity, id);
            return;
        }
        // Fallback for protected setter via reflection
        var field = typeof(T).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(entity, id);
        }
    }
    
    // Test helper method for setting properties via reflection
    public static void SetProperty<T>(T entity, string propertyName, object value) where T : class
    {
        var prop = typeof(T).GetProperty(propertyName);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(entity, value);
            return;
        }
        // Fallback for protected setter via reflection
        var field = typeof(T).GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(entity, value);
        }
    }
}
