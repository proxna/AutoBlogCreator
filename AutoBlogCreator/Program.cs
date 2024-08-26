using AutoBlogCreator.Middlewares;
using AutoBlogCreator.Models;
using AutoBlogCreator.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGitConnector, GitConnector>();
builder.Services.AddScoped<IArticleCreator, ArticleCreator>();
builder.Services.AddScoped<ILinkToImageRetriever, LinkToImageRetriever>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<ApiKeyMiddleware>();

app.UseMiddleware<ExceptionHandler>();

app.MapPost("/addarticle", async (News news, IArticleCreator articleCreator) =>
{
    await articleCreator.CreateArticle(news);
    return Results.StatusCode(200);
});

app.MapGet("/artwork/{name}", async (string name, ILinkToImageRetriever linkRetriever) =>
    await linkRetriever.GetImageLink(name));

app.Run();
