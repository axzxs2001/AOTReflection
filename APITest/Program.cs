using System.Reflection;
using System.Text.Json;
using System;
using System.Text.Json.Serialization;
using System.Text;
using APITest.Models;
using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();
app.MapGet("/refstring", async () =>
{
    var person = new Order { Name = "¹ðËØÎ°", Age = 10, Birthday = DateTime.Now, Hobbies = new string[] { "×ãÇò", "´úÂë" } };
    return await GetStringAsync(person);

});
app.MapPost("/refstring", async (Person person) =>
{   
    return await GetStringAsync(person);
});
async Task<string> GetStringAsync<T>(T t) where T : Ent
{
    Console.WriteLine(t.GetType().FullName);
    var sb = new StringBuilder();
    var pros = typeof(T)?.GetProperties();
    foreach (var pro in pros)
    {
        Console.WriteLine(pro.Name);
        if (pro != null)
        {
            sb.Append($"{pro?.Name}:{pro?.GetValue(t)};");
        }
    }
    t.Print();
    return sb.ToString();
}


app.MapGet("/ref", async () =>
{
    return await GetPropertyAsync<Person>();
});
async Task<string[]> GetPropertyAsync<T>()
{
    var assembly = Assembly.GetExecutingAssembly();
    var stream = assembly.GetManifestResourceStream("APITest.typemember.json");
    var reader = new StreamReader(stream!);
    var json = await reader.ReadToEndAsync();
    var jsonSerializerOptions = new JsonSerializerOptions()
    {
        TypeInfoResolver = AppJsonSerializerContext.Default,
    };
    var typeModels = JsonSerializer.Deserialize<TypeModel[]>(json, jsonSerializerOptions);
    return typeModels.SingleOrDefault(s => s.TypeName == typeof(T).FullName).Properties.ToArray();
}
app.Run();

[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(TypeModel[]))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{

}

public class TypeModel
{
    public string TypeName { get; set; }
    public List<string> Properties { get; set; }
    public List<string> Types { get; set; }
}
public partial class Order: Ent
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime Birthday { get; set; }
    public string[] Hobbies { get; set; }

}
namespace APITest.Models
{
    public partial class Person: Ent
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public string[] Hobbies { get; set; }

    }
}

public partial class Ent
{
    public void Print()
    {
        Console.WriteLine(this.GetType().Name);
    }
}
