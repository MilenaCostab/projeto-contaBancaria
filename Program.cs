﻿﻿﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ContaBancaria;

var builder = WebApplication.CreateBuilder(args);

string serverInstanceId = Guid.NewGuid().ToString();

builder.Services.AddDbContext<BancoDbContext>(options => options.UseSqlite("Data Source=banco.db"));

builder.Services.AddControllers();
builder.Services.AddScoped<ContaRepository, ContaRepositoryImpl>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BancoDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/login.html"));

app.MapGet("/api/instance", () => {
    return Results.Ok(serverInstanceId);
});

app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "login.html" }
});
app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
