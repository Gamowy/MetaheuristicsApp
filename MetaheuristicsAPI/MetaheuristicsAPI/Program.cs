using MetaheuristicsAPI;
using MetaheuristicsAPI.FitnessFunctions;
using MetaheuristicsAPI.Interfaces;
using MetaheuristicsAPI.Payloads;


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

// Endpoint for retriving available algorithms
app.MapGet("/algorithms", () => AlgorithmsProvider.GetAlgorithmNames())
.WithName("GetAlgorithms")
.WithDescription("Returns available algorithm names.")
.WithOpenApi();

// Endpoint for retriving algorithm parameters
app.MapGet("/algorithms/params/{algorithm}", (string algorithm) =>
{
    IOptimizationAlgorithm? optimizationAlgorithm = AlgorithmsProvider.GetAlgorithm(algorithm, 0, 0);
    if (optimizationAlgorithm != null)
        return Results.Ok(optimizationAlgorithm.ParamsInfo);
    return Results.NotFound();
})
.WithName("GetAlgorithmParams")
.WithDescription("Returns parameters for given algorithm")
.Produces<ParamInfo[]>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

// Endpoint for retriving available fitness functions with domain dimensions
app.MapGet("/functions", () => FitnessFunctionsProvider.GetFitnessfunctionsPayloads())
.WithName("GetFitnessFunctions")
.WithDescription("Returns available fitness function names and their domain dimensions.")
.WithOpenApi();

// Endpoint for testing a single algorithm
app.MapPost("/test", async (TestRequest request) =>
{
    try
    {
        IOptimizationAlgorithm? optimizationAlgorithm = AlgorithmsProvider.GetAlgorithm(request.Algorithm, request.N, request.I);
        if (optimizationAlgorithm == null)
            return Results.BadRequest("Specified algorithm doesn't exist");

        fitnessFunction? fitnessFunction = FitnessFunctionsProvider.GetFitnessFunction(request.Fun);
        if (fitnessFunction == null)
            return Results.BadRequest("Specified fitness function doesn't exist");

        double[][]? domain = FitnessFunctionsProvider.GetDomain(request.Fun, request.Dim);
        if (domain == null)
            return Results.BadRequest("Can't get domain for specified fitness function");

        if (domain.GetLength(0) != request.Dim)
            return Results.BadRequest("Incorect dimension for specified fitness function");

        double[]? parameters = request.Parameters;

        await Task.Run(() => optimizationAlgorithm.Solve(fitnessFunction, domain, request.Parameters));

        TestResults results = new TestResults
        {
            XBest = optimizationAlgorithm.XBest,
            FBest = optimizationAlgorithm.FBest,
            NumberOfEvaluationFitnessFunction = optimizationAlgorithm.NumberOfEvaluationFitnessFunction
        };
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Computing error: {ex.Message}");
    }
})
.WithName("PostTestRequest")
.WithDescription("Posts request for testing a single algorithm on a given test function with specified parametrs and returns results.")
.Produces<TestResults>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithRequestTimeout(TimeSpan.FromMinutes(5))
.WithOpenApi();

app.Run();