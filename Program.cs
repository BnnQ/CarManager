using CarManager.Utils.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();
var app = builder.Build();

app.Configure();
app.Run();
