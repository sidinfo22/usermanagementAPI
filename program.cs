using System.Text.Json;

using Microsoft.AspNetCore.HttpLogging;

using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;

using UserManagementSystem.Middleware;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All;
    });

    builder.Services.AddSerilog((services, config) => config
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(new ExpressionTemplate("[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}", theme: TemplateTheme.Code))
    );

    builder.Services.AddControllers();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/error");
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpLogging();
    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseMiddleware<TokenValidationMiddleware>();
    app.UseMiddleware<RequestResponseLoggingMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.MapControllers();

    app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: StatusCodes.Status500InternalServerError));
    app.MapGet("/division", (int numerator, int denominator) => numerator / denominator);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
