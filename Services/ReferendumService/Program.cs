using Microsoft.EntityFrameworkCore;
using ReferendumService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ReferendumDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReferendumDbContext>();

    try
    {
        db.Database.EnsureCreated();
        Console.WriteLine("✓ Referendum DB creada/verificada");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Error al crear Referendum DB: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("🚀 Referendum Service iniciado");

app.Run();