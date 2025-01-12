using MetaheuristicsAPI;
using MetaheuristicsAPI.Algorithms;
using MetaheuristicsAPI.FitnessFunctions;
using MetaheuristicsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRequestTimeouts();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRequestTimeouts();

app.MapPost("/test", async (TestRequest request) =>
{
    IOptimizationAlgorithm? optimizationAlgorithm = AlgorithmsProvider.GetAlgorithm(request.Algorithm, request.N, request.I);
    if (optimizationAlgorithm == null)
        return Results.BadRequest("Specified algorithm doesn't exist");

    fitnessFunction? fitnessFunction = FitnessFunctionsProvider.GetFitnessFunction(request.Fun);
    if (fitnessFunction == null)
        return Results.BadRequest("Specified fitness function doesn't exist");

    double[,]? domain = (request.Domain == null) ? FitnessFunctionsProvider.GetDomain(request.Fun, request.Dim) : request.Domain;
    if (domain == null)
        return Results.BadRequest("Can't get domain for specified fitness function");

    double[]? parameters = request.Parameters;

    await Task.Run(() => optimizationAlgorithm.Solve(fitnessFunction, domain, request.Parameters));

    TestResults results = new TestResults
    {
        XBest = optimizationAlgorithm.XBest,
        FBest = optimizationAlgorithm.FBest,
        NumberOfEvaluationFitnessFunction = optimizationAlgorithm.NumberOfEvaluationFitnessFunction
    };
    return Results.Ok(results);
})
.WithName("PostTestRequest")
.WithDescription("Posts request for testing a single algorithm on a given test function with specified parametrs and returns results.")
.Produces<TestResults>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithRequestTimeout(TimeSpan.FromMinutes(5))
.WithOpenApi();

app.Run();