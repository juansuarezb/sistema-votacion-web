using Microsoft.EntityFrameworkCore;
using VoteService.Data;
using VoteService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<VoteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<ReferendumClient>(client =>
{
    var referendumServiceUrl = builder.Configuration["Services:ReferendumService"];

    if (string.IsNullOrWhiteSpace(referendumServiceUrl))
        throw new InvalidOperationException("Services:ReferendumService no está configurado.");

    client.BaseAddress = new Uri(referendumServiceUrl);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VoteDbContext>();

    try
    {
        db.Database.EnsureCreated();
        Console.WriteLine("✓ Vote DB creada/verificada");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Error al crear Vote DB: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("Vote Service iniciado");

app.Run();