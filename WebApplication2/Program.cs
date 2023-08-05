using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("SlnDbContext");

builder.Services.AddDbContext<SlnDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

app.Run();
