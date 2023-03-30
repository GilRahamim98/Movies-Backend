using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoviesAPI;
using MoviesAPI.APIBehavior;
using MoviesAPI.Filters;
using MoviesAPI.Helpers;
using NetTopologySuite;
using NetTopologySuite.Geometries;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ParseBadRequest));
}).ConfigureApiBehaviorOptions(BadRequestsBehavior.Parse);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDBContext>(options =>
                                                        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                                                        sqlOptions=>sqlOptions.UseNetTopologySuite()));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()
        .WithExposedHeaders(new string[] { "totalAmountOfRecords" });
    });
});
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton(provider=>new MapperConfiguration(config =>
{
    var geometryFactory = provider.GetRequiredService<GeometryFactory>();
    config.AddProfile(new AutoMapperProfiles(geometryFactory));
}).CreateMapper());
builder.Services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

builder.Services.AddScoped<IFileStorageService, InAppStorageService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
