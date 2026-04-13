using System.Linq.Expressions;
using System.Reflection;

namespace DeepSigma.Core.Utilities;

/// <summary>
/// Utility class for object-related operations.
/// </summary>
public class ObjectUtilities
{
    /// <summary>
    /// Checks if T and V are of the same type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <returns></returns>
    public static bool IsTypeT_TypeV<T, V>()
    {
        return typeof(V).IsAssignableFrom(typeof(T));
    }

    /// <summary>
    /// Converts object to Type T.
    /// </summary>
    /// <typeparam name="F">From Type</typeparam>
    /// <typeparam name="T">To Type</typeparam>
    /// <param name="object_instance"></param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException">If unable to convert object type and exception will be thrown.</exception>
    public static T CastAs<F, T>(F object_instance)
    {
        if (object_instance is T)
        {
            return (T)Convert.ChangeType(object_instance, typeof(T));
        }
        else
        {
            throw new InvalidCastException();
        }
    }

    /// <summary>
    /// Get property value by property selection expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="object_instance"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static V? GetPropertyValue<T, V>(T object_instance, Expression<Func<T, object>> predicate)
    {
        if (object_instance is null) return default;
        string property_name = GetPropertyName(predicate);
        return (V?)object_instance.GetType()?.GetProperty(property_name)?.GetValue(object_instance, null);
    }

    /// <summary>
    /// Get field value by property selection expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="object_instance"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static V? GetFieldValue<T, V>(T object_instance, Expression<Func<T, object>> predicate)
    {
        if (object_instance is null) return default;
        string property_name = GetPropertyName(predicate);
        return (V?)object_instance.GetType()?.GetField(property_name)?.GetValue(object_instance);
    }

    /// <summary>
    /// Gets a hash set list of properies of an object instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="object_instance"></param>
    /// <returns></returns>
    public static HashSet<PropertyInfo> GetAllPropertyInfos<T>(T object_instance)
    {
        if (object_instance is null)
        {
            return [];
        }
        return object_instance.GetType().GetProperties().ToHashSet();
    }

    /// <summary>
    /// Gets a hash set list of properies of an object instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="object_instance"></param>
    /// <returns></returns>
    public static HashSet<string> GetAllPropertyNames<T>(T object_instance)
    {
        HashSet<PropertyInfo> properties = GetAllPropertyInfos(object_instance).ToHashSet();
        HashSet<string> names = new HashSet<string>();
        foreach (PropertyInfo propertyInfo in properties)
        {
            names.Add(propertyInfo.Name);
        }
        return names;
    }

    /// <summary>
    /// Gets the property name of an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property"></param>
    /// <returns></returns>
    public static string GetPropertyName<T>(Expression<Func<T, object>> property)
    {
        if (property.Body is MemberExpression)
        {
            return ((MemberExpression)property.Body).Member.Name;
        }
        else
        {
            var op = ((UnaryExpression)property.Body).Operand;
            return ((MemberExpression)op).Member.Name;
        }
    }

    /// <summary>
    /// Converts a property name to an expression that can be used in LINQ queries.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property_name"></param>
    /// <returns></returns>
    public static Expression<Func<T, object>> PropertyNameToExpression<T>(string property_name)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        MemberExpression property = Expression.Property(parameter, property_name);
        Expression converted = Expression.Convert(property, typeof(object));
        return Expression.Lambda<Func<T, object>>(converted, parameter);
    }

    /// <summary>
    /// Converts object instance of type F to object instance of type T. Note: the T class must contain a constructor that accepts an instance of object F.
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="dtos"></param>
    /// <returns></returns>
    public static IEnumerable<T> ConvertToNewObject<F, T>(IEnumerable<F> dtos)
    {
        foreach (F dto in dtos)
        {
            var instance = Activator.CreateInstance(typeof(T), dto);
            if (instance is null)
            {
                throw new InvalidOperationException("Unable to convert from object of type F to T.");
            }
            yield return (T)instance;
        }
    }

    /// <summary>
    /// Converts object instance of type F to object instance of type T. Note: the T class must contain a constructor that accepts an instance of object F.
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="dto"></param>
    /// <returns></returns>
    public static T? ConvertToNewObject<F, T>(F dto) where F : notnull
    {
        return (T?)Activator.CreateInstance(typeof(T), dto);
    }

    /// <summary>
    /// Determines if an object instance contains a method by name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="method_name"></param>
    /// <returns></returns>
    public static bool HasMethod<T>(T obj, string method_name) where T : notnull
    {
        var type = obj.GetType();
        return type.GetMethod(method_name) != null;
    }

    /// <summary>
    /// Determines if an object instance contains a property by name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="property_name"></param>
    /// <returns></returns>
    public static bool HasProperty<T>(T obj, string property_name) where T : notnull
    {
        var type = obj.GetType();
        return type.GetProperty(property_name) != null;
    }

    /// <summary>
    /// Determines if an object instance contains a field by name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="field_name"></param>
    /// <returns></returns>
    public static bool HasField<T>(T obj, string field_name) where T : notnull
    {
        var type = obj.GetType();
        return type.GetField(field_name) != null;
    }


    /// <summary>
    /// Returns an object instance property/field value by name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">Object instance</param>
    /// <param name="attribute_name">Name of prperty or field </param>
    /// <returns></returns>
    public static T? GetPropertyOrFieldValue<T>(object obj, string attribute_name)
    {
        if (HasProperty(obj, attribute_name) == true)
        {
            return GetPropertyValue<T>(obj, attribute_name);
        }
        return GetFieldValue<T>(obj, attribute_name);
    }

    /// <summary>
    /// Gets the property value from an object by name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="selected_object"></param>
    /// <param name="property_name"></param>
    /// <returns></returns>
    public static T? GetPropertyValue<T>(object selected_object, string property_name)
    {
        if (selected_object is null) return default;
        return (T?)selected_object.GetType()?.GetProperty(property_name)?.GetValue(selected_object, null);
    }

    /// <summary>
    /// Gets the field value from an object by name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="selected_object"></param>
    /// <param name="field_name"></param>
    /// <returns></returns>
    public static T? GetFieldValue<T>(object selected_object, string field_name)
    {
        if (selected_object is null) return default;
        return (T?)selected_object.GetType()?.GetField(field_name)?.GetValue(selected_object);
    }

    /// <summary>
    /// Determine if an object instance has a property of type Z.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Z"></typeparam>
    /// <param name="object_instance"></param>
    /// <returns></returns>
    public static bool HasPropertyOfType<T, Z>(T object_instance) where T : notnull
    {
        PropertyInfo[] propertyInfos = object_instance.GetType().GetProperties();
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            if (propertyInfo.PropertyType == typeof(Z))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Converts an expression to a function delegate. For example, you can use this to convert an expression like x => x.PropertyName to a function that can be executed.
    /// 
    /// </summary>
    /// <typeparam name="TDelegate"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static TDelegate ExpressionToExecutableFunction<TDelegate>(Expression<TDelegate> expression) where TDelegate : Delegate
    {
        return expression.Compile();
    }
}
