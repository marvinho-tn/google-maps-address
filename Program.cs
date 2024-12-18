using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints();
app.MapOpenApi();
app.UseHttpsRedirection();
app.Run();