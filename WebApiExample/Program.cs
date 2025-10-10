
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using NSwag.Generation;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "La tua API";
    config.Version = "v1";
});

var app = builder.Build();

app.UseOpenApi(); // /swagger/v1/swagger.json
app.UseSwaggerUi(); // /swagger

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapPost("/agreements", async (AgParamQ paramQ) =>
{
    return Results.Ok(new
    {
        IdWallet = paramQ.IdWallet,
        EndDate = paramQ.EndDate,
        Sort = paramQ.Sort?.Select(s => new { By = s.By.ToString(), IsAscending = s.IsAscending })
    });
})
.WithName("GetAgreements")
.WithTags("Agreements")
.WithOpenApi(op => new(op)
{
    Summary = "Ricerca accordi",
    Description = "Esegue una ricerca sugli accordi usando parametri di filtro e ordinamento.",
    RequestBody = new OpenApiRequestBody
    {
        Description = "Parametri di ricerca",
        Required = true,
        Content =
        {
            ["application/json"] = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = nameof(AgParamQ)
                    }
                }
            }
        }
    },
    Responses =
    {
        ["200"] = new OpenApiResponse
        {
            Description = "Risultato della ricerca"
        }
    }
});




app.Run();

public class AgParamQ : BaseSort<AgSortField>
{
    public int? IdWallet { get; set; }
    public DateTime? EndDate { get; set; }
}

public class BaseSort<T> where T : Enum
{
    public IEnumerable<Sort<T>> Sort { get; set; }
}

public class Sort<T> where T : Enum
{
    public T By { get; set; }
    public bool IsAscending { get; set; }
}

public enum AgSortField
{
    Id,
    IdWallet,
    IdAgreement,
    IdProduct,
    IdCustomer,
    IdContractor,
    IdBranch,
    IdUser,
    DateFrom,
    DateTo,
    DateCreated,
    DateUpdated
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
