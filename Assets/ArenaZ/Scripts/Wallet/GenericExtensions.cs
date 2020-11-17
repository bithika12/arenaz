using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

public static class GenericExtensions
{
    //public static T ObjectFromDictionary<T>(this T classObject, IDictionary<string, object> dict) where T : class
    //{
    //    Type type = typeof(T);
    //    T result = (T)Activator.CreateInstance(type);
    //    foreach (var item in dict)
    //    {
    //        type.GetProperty(item.Key).SetValue(result, item.Value, null);
    //    }
    //    return result;
    //}

    /*public static IDictionary<string, object> ObjectToDictionaryProperty<T>(this IDictionary<string, object> dictionary, T classObject) where T : class
    {
        IDictionary<string, object> dict = new Dictionary<string, object>();
        var indexer = new object[0];
        PropertyInfo[] properties = classObject.GetPropertyInfos();
        foreach (var element in properties)
        {
            var value = element.GetValue(classObject, indexer);

            if (element.GetCustomAttribute<JsonPropertyAttribute>() != null)
                dict.Add(element.GetCustomAttribute<JsonPropertyAttribute>().PropertyName, value);
            else
                dict.Add(element.Name, value);
        }
        return dict;
    }

    public static IDictionary<string, object> ObjectToDictionaryField<T>(this IDictionary<string, object> dictionary, T classObject) where T : class
    {
        IDictionary<string, object> dict = new Dictionary<string, object>();
        FieldInfo[] properties = classObject.GetFieldInfos();
        foreach (var element in properties)
        {
            var value = element.GetValue(null);

            if (element.GetCustomAttribute<JsonPropertyAttribute>() != null)
                dict.Add(element.GetCustomAttribute<JsonPropertyAttribute>().PropertyName, value);
            else
                dict.Add(element.Name, value);
        }
        return dict;
    }

    public static IDictionary<string, string> ObjectToDictionaryField<T>(this IDictionary<string, string> dictionary, T classObject) where T : class
    {
        IDictionary<string, string> dict = new Dictionary<string, string>();
        FieldInfo[] properties = classObject.GetFieldInfos();
        foreach (var element in properties)
        {
            var value = element.GetValue(null);

            if (element.GetCustomAttribute<JsonPropertyAttribute>() != null)
                dict.Add(element.GetCustomAttribute<JsonPropertyAttribute>().PropertyName, value.ToString());
            else
                dict.Add(element.Name, value.ToString());
        }
        return dict;
    }

    public static IList<string> GetObjectVariableName<T>(this IList<string> list, T classObject) where T : class
    {
        IList<string> nameList = new List<string>();
        FieldInfo[] properties = classObject.GetFieldInfos();
        foreach (var element in properties)
        {
            if (element.GetCustomAttribute<JsonPropertyAttribute>() != null)
                nameList.Add(element.GetCustomAttribute<JsonPropertyAttribute>().PropertyName);
            else
                nameList.Add(element.Name);
        }
        return nameList;
    }

    // Get Auto-Properties of an objects respect to BindingFlags
    public static PropertyInfo[] GetPropertyInfos<T>(this T classObject, BindingFlags bindingAttr)
    {
        Type myObjectType = classObject.GetType();
        return myObjectType.GetProperties(bindingAttr);
    }

    // Get Auto-Properties of an objects respect to BindingFlags
    public static PropertyInfo[] GetPropertyInfos<T>(this T classObject)
    {
        Type myObjectType = classObject.GetType();
        return myObjectType.GetProperties();
    }

    // Get Fields of an objects respect to BindingFlags
    public static FieldInfo[] GetFieldInfos<T>(this T classObject, BindingFlags bindingAttr)
    {
        Type myObjectType = classObject.GetType();
        return myObjectType.GetFields(bindingAttr);
    }

    // Get Fields of an objects respect to BindingFlags
    public static FieldInfo[] GetFieldInfos<T>(this T classObject)
    {
        Type myObjectType = classObject.GetType();
        return myObjectType.GetFields();
    }*/

    public static int GetLeadingInt(string input)
    {
        return Int32.Parse(new string(input.Trim().TakeWhile(c => char.IsDigit(c) || c == '.').ToArray()));
    }
}
