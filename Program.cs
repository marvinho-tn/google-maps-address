using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
}

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();
app.Run();