# AOTReflection
 [中文](README.md/)

In the AOT project, there are issues with using reflection. This program package utilizes source generators to pre-fetch metadata for different types based on various dimensions. This approach aims to achieve smoother utilization of reflection-related functionalities.

The following code utilizes generic reflection for flexibility. However, when published as AOT, it's observed that the return value of GetString is empty (Note: local debugging returns results). This program package addresses and resolves this issue.
```C#
string GetString<T>(T t)
{
    var sb = new StringBuilder();
    var pros = typeof(T)?.GetProperties();
    foreach (var pro in pros)
    {
        if (pro != null)
        {
            if (pro.PropertyType.IsArray)
            {
                var arr = pro.GetValue(t) as string[];
                sb.Append($"{pro?.Name}:{string.Join(",", arr)};");
            }
            else
            {
                sb.Append($"{pro?.Name}:{pro?.GetValue(t)};");
            }
        }
    }
    InvockMethod(t);
    return sb.ToString();
}

void InvockMethod<T>(T t)
{
    var method = typeof(T)?.GetMethod("Print");
    method?.Invoke(t, new object[] { "User Reflection Print" });
}
```

**Explanation: This example is implemented based on the AOT template of MiniAPI.**

## AOTReflectionGenerator.Attribute
1. In the AOT project, install the AOTReflectionGenerator.Attribute NuGet package.
2. Define the AOTReflectionAttribute in the AOT project.
```C#
namespace AOTReflectionHelper.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public partial class AOTReflectionAttribute : System.Attribute
    {
    }
}
```
3. When defining entity classes, add the [AOTReflectionHelper.Attribute.AOTReflection] attribute to the classes that require reflection.
```C#
public class Parent
{
    public void Print(string content)
    {
        Console.WriteLine($"Reflection type name：{GetType().Name}，Print：{content}");
    }
}
[AOTReflectionHelper.Attribute.AOTReflection]
public struct Order //: Parent
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime Birthday { get; set; }
    public string[] Hobbies { get; set; }
}
[AOTReflectionHelper.Attribute.AOTReflection]
public class Person : Parent
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime Birthday { get; set; }
    public string[] Hobbies { get; set; }
}
```
4. Now you can use basic reflection functionalities in a generic way, such as the GetString<T>(T t) method.
```C#
app.MapGet("/testattribute", () =>
{
    var order = new Order { Name = "GuiSuWei", Age = 10, Birthday = DateTime.Now, Hobbies = new string[] { "Football", "Code" } };
    return GetString(order);

});
app.MapPost("/testattribute", (Person person) =>
{
    return GetString(person);
});
```

## AOTReflectionGenerator.Interface
1. In the AOT project, install the AOTReflectionGenerator.Attribute NuGet package.
2. Define the IAOTReflection interface in the AOT project.
```C#
namespace AOTReflectionHelper.Interface
{
    public interface IAOTReflection
    {
    }
}
```
3. When defining entity classes, simply inherit the IAOTReflection interface.
```C#
public class Item : IAOTReflection
{
    public int ID { get; set; }
    public string Name { get; set; }

    public Sex Sex { get; set; }
}
public enum Sex
{
    Male,
    Female
}
```

4. Now you can use basic reflection functionalities in a generic way, such as the GetString<T>(T t) method.
```C#
app.MapGet("/testinterface", () =>
{
    var item = new Item { ID = 1000001, Name = "ItemName" };
    return GetString(item);

});
```

## AOTReflectionGenerator.Entity
1. In an AOT (Ahead of Time) project, install the AOTReflectionGenerator.Entity package using the following 
> dotnet add package AOTReflectionGenerator.Entity --version 0.1.1

2. Simply define the entity classes directly.
```C#
public class Entity
{
    public string Name { get; set; }
    public string Description { get; set; }
}
```
3. Now you can utilize basic reflection functionalities in a generic manner, such as the GetString<T>(T t) method.
```C#
app.MapGet("/testentity", () =>
{
    var entity = new Entity { Name = "Entity",Description="
This is a test for AOT reflection entity classes." };
    return GetString(entity);

});
```
## AOTReflectionGenerator.MethodAttribute
1. In the AOT project, install the AOTReflectionGenerator.MethodAttribute NuGet package.
2. Define the AOTReflectionMethodAttribute in the AOT project.
```C#
namespace AOTReflectionHelper.MethodAttribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AOTReflectionMethodAttribute : System.Attribute
    {      
    }
}
```
3. Add the [AOTReflectionHelper.MethodAttribute.AOTReflectionMethod] attribute to the GetString<T>(T t) method to ensure that the source generator generates metadata for the type T used in the method.
```C#
[AOTReflectionHelper.MethodAttribute.AOTReflectionMethod]
string GetString<T>(T t)
{
  // ...
}
```
4. You can now use basic reflection functionalities in a generic way, such as the GetString<T>(T t) method.
```C#
app.MapGet("/testmethodattribute", () =>
{
    var testMethodAtt = new TestMethodAtt { Name = "John Doe" };
    return GetString(testMethodAtt);
});
```
Note that AOTReflectionGenerator.Interface, AOTReflectionGenerator.Entity, and AOTReflectionGenerator.MethodAttribute cannot be used simultaneously as they are all based on the MiniAPI AOT project template and involve partial classes of AppJsonSerializerContext. The implementation is done by overriding the GetHashCode method in the source generator for the AppJsonSerializerContext class to fetch reflection metadata.
