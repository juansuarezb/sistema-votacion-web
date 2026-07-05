using ResultService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<VoteClient>(client =>
{
    var voteServiceUrl = builder.Configuration["Services:VoteService"];

    if (string.IsNullOrWhiteSpace(voteServiceUrl))
        throw new InvalidOperationException("Services:VoteService no está configurado.");

    client.BaseAddress = new Uri(voteServiceUrl);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("Result Service iniciado");

app.Run();