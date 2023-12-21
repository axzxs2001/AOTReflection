using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();


app.MapGet("/", () =>
{
    using (var conn = new SqlConnection("Data Source=.;Initial Catalog=Test;Integrated Security=True"))
    {
        var todos = conn.Query<Todo>("select * from Todo");
        return todos;
    }
});


app.Run();


[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
class Todo
{
    public int ID { get; set; }
}
