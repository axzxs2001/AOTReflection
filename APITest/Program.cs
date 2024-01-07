using System.Text.Json.Serialization;
using System.Text;
using APITest.Models;




var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();
app.MapGet("/testattribute", () =>
{
    var order = new Order { Name = "����ΰ", Age = 10, Birthday = DateTime.Now, Hobbies = new string[] { "����", "����" } };
    return GetString(order);

});
app.MapPost("/testattribute", (Person person) =>
{
    return GetString(person);
});

app.MapGet("/testinterface", () =>
{
    var item = new Item { ID = 1000001, Name = "ѡ������" };
    return GetString(item);

});

app.MapGet("/testentity", () =>
{
    var entity = new Entity { Name = "ʵ����", Description = "����AOT�����ʵ�������" };
    return GetString(entity);

});

app.MapGet("/testmethodattribute", () =>
{
    var testMehtodAtt = new TestMehtodAtt { Name = "����ΰ" };
    return GetString(testMehtodAtt);

});

[AOTReflectionHelper.MethodAttribute.AOTReflectionMethod]
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
    method?.Invoke(t, new object[] { "�÷������Print" });
}




app.Run();


[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(string[]))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}

class TestMehtodAtt
{
    public string Name { get; set; }

}




namespace AOTReflectionHelper.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public partial class AOTReflectionAttribute : System.Attribute
    {
    }
}
namespace AOTReflectionHelper.Interface
{
    public interface IAOTReflection
    {
    }
}

namespace AOTReflectionHelper.MethodAttribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AOTReflectionMethodAttribute : System.Attribute
    {
      
    }
}

