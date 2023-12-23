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
    var order = new Order { Name = "����ΰ", Age = 10, Birthday = DateTime.Now, Hobbies = new string[] { "����", "����" } };
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
    method?.Invoke(t, new object[] { "�÷������Print" });
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
        Console.WriteLine($"������������{GetType().Name}��Print������{content}");
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