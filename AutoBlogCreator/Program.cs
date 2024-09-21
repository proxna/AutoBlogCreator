using AutoBlogCreator.Middlewares;
using AutoBlogCreator.Models;
using AutoBlogCreator.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGitConnector, GitConnector>();
builder.Services.AddScoped<IArticleCreator, ArticleCreator>();
builder.Services.AddScoped<ILinkToImageRetriever, LinkToImageRetriever>();
builder.Services.AddScoped<IArticleAdjuster, ArticleAdjuster>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<ApiKeyMiddleware>();

app.UseMiddleware<ExceptionHandler>();

app.MapPost("/article", async (News news, IArticleCreator articleCreator) =>
{
    await articleCreator.CreateArticle(news);
    return Results.StatusCode(200);
});

app.MapGet("/artwork/{name}", async (string name, ILinkToImageRetriever linkRetriever) =>
    await linkRetriever.GetImageLink(name));

app.MapPost("/article/adjust", async (ArticleToAdjust article, IArticleAdjuster articleAdjuster) =>
    await articleAdjuster.AdjustArticle(article.Article));

app.Run();
