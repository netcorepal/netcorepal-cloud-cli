using Cocona;

var builder = CoconaApp.CreateBuilder();
var app = builder.Build();

app.AddCommand(() =>
{
    Console.WriteLine("Welcome!");
});

app.Run();