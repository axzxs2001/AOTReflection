# AOTReflection

 [English](README_en.md/)

在AOT项目中，使用反射有很问题，本程序包利用源生成器，按不同的维度，提前获取对应类型的元数据，从而达到平滑使用Reflection部分功能。

如下面代码，利用泛型的反射，以达到灵活，但在当发布成AOT时，会发现GetString的返回值为空（注意：本地调试会返回结果），本程序包会解决掉这个问题。
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
    method?.Invoke(t, new object[] { "用反射调用Print" });
}
```

**说明：本例都是基于MiniAPI的AOT模版来实现的。**

## AOTReflectionGenerator.Attribute
1. 在AOT项目中，安装AOTReflectionGenerator.Attribute和AOTReflectionHelper.Attribute
> dotnet add package AOTReflectionGenerator.Attribute --version 0.1.1

> dotnet add package AOTReflectionHelper.Attribute --version 0.1.1
2. 定义实体类时，把需要Reflection的类加上[AOTReflectionHelper.Attribute.AOTReflection]特性
```C#
public class Parent
{
    public void Print(string content)
    {
        Console.WriteLine($"反射类型名：{GetType().Name}，Print参数：{content}");
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
3. 这时就可以按照泛型的方式来使用基本的反射功能了，如GetString<T>(T t)方法
```C#
app.MapGet("/testattribute", () =>
{
    var order = new Order { Name = "桂素伟", Age = 10, Birthday = DateTime.Now, Hobbies = new string[] { "足球", "代码" } };
    return GetString(order);

});
app.MapPost("/testattribute", (Person person) =>
{
    return GetString(person);
});
```

## AOTReflectionGenerator.Interface
1. 在AOT项目中，安装AOTReflectionGenerator.Interface和AOTReflectionHelper.Interface
> dotnet add package AOTReflectionGenerator.Interface --version 0.1.1

> dotnet add package AOTReflectionHelper.Interface --version 0.1.1

2. 定义实体类时，继承IAOTReflection接口即可
```C#
public class Item : IAOTReflection
{
    public int ID { get; set; }
    public string Name { get; set; }

    public Sex Sex { get; set; }
}
public enum Sex
{
    男,
    女
}
```

3. 这时就可以按照泛型的方式来使用基本的反射功能了，如GetString<T>(T t)方法
```C#
app.MapGet("/testinterface", () =>
{
    var item = new Item { ID = 1000001, Name = "选项名称" };
    return GetString(item);

});
```

## AOTReflectionGenerator.Entity
1. 在AOT项目中，安装AOTReflectionGenerator.Entity
> dotnet add package AOTReflectionGenerator.Entity --version 0.1.1

2. 直接定义实体类即可
```C#
public class Entity
{
    public string Name { get; set; }
    public string Description { get; set; }
}
```
3. 这时就可以按照泛型的方式来使用基本的反射功能了，如GetString<T>(T t)方法
```C#
app.MapGet("/testentity", () =>
{
    var entity = new Entity { Name = "实体类",Description="这是AOT反射的实体类测试" };
    return GetString(entity);

});
```

需要注意的是，AOTReflectionGenerator.Interface和AOTReflectionGenerator.Entity都是基于**MiniAPI AOT项目模版**的分部类AppJsonSerializerContext。具体实现是在源生成器中重写了AppJsonSerializerContext类的public override int GetHashCode()方法来完成反射元数据获取的，所以**AOTReflectionGenerator.Interface和AOTReflectionGenerator.Entity也不能同时使用**。


