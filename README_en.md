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
1. In an AOT project, install the AOTReflectionGenerator.Attribute and AOTReflectionHelper.Attribute attributes.
> dotnet add package AOTReflectionGenerator.Attribute --version 0.1.1

> dotnet add package AOTReflectionHelper.Attribute --version 0.1.1
2. When defining entity classes, add the [AOTReflectionHelper.Attribute.AOTReflection] attribute to the classes that require reflection.
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
3. Now you can use basic reflection functionalities in a generic way, such as the GetString<T>(T t) method.
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
1. In an AOT project, install the AOTReflectionGenerator.Interface and AOTReflectionHelper.Interface packages.
> dotnet add package AOTReflectionGenerator.Interface --version 0.1.1

> dotnet add package AOTReflectionHelper.Interface --version 0.1.1

2. When defining entity classes, simply inherit the IAOTReflection interface.
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

3. Now you can use basic reflection functionalities in a generic way, such as the GetString<T>(T t) method.
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

It's important to note that both AOTReflectionGenerator.Interface and AOTReflectionGenerator.Entity are based on the **MiniAPI AOT project template's** partial class AppJsonSerializerContext. The specific implementation involves overriding the public override int GetHashCode() method in the source generator to achieve reflection metadata retrieval. Therefore, **AOTReflectionGenerator.Interface and AOTReflectionGenerator.Entity cannot be used simultaneously**.
