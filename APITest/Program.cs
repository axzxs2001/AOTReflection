using System.Text.Json.Serialization;
using System.Text;
using APITest.Models;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();
app.MapGet("/test", () =>
{
    var order = new Order { Name = "桂素伟", Age = 10, Birthday = DateTime.Now, Hobbies = new string[] { "足球", "代码" } }; 
    return GetString(order);

});
app.MapPost("/test", (Person person) =>
{
    return GetString(person);
});
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
app.Run();

[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(string[]))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}
