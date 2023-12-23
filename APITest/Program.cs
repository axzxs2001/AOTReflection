using System.Text.Json.Serialization;
using System.Text;
using APITest.Models;
using AOTReflectionHelper;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();
app.MapGet("/test", () =>
{
    var order = new Order { Name = "桂素伟", Age = 10, Birthday = DateTime.Now, Hobbies = new string[] { "足球", "代码" } };
    InvockMethod(order);
    return GetString(order);

});
app.MapPost("/test", (Person person) =>
{
    return GetString(person);
});
string GetString<T>(T t) where T : Parent
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
    t.Print(sb.ToString());
    return sb.ToString();
}

void InvockMethod<T>(T t)
{
    var method = typeof(T)?.GetMethod("Print");
    method?.Invoke(t, new object[] { "用反射调用Print" });
}
app.Run();

[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(string[]))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}
public partial class Parent
{
    public void Print(string content)
    {      
        Console.WriteLine($"反射类型名：{GetType().Name}，Print参数：{content}");
    }
}

[AOTReflection]
public partial class Order : Parent
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime Birthday { get; set; }
    public string[] Hobbies { get; set; }
}


namespace APITest.Models
{
    [AOTReflection]
    public class Person : Parent
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public string[] Hobbies { get; set; }

    }
}