using MetaheuristicsAPI;
using MetaheuristicsAPI.Data;
using MetaheuristicsAPI.FitnessFunctions;
using MetaheuristicsAPI.Interfaces;
using MetaheuristicsAPI.Schemas;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

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


// Configure File Server for hosting reports
var rootPath = $"{Directory.GetCurrentDirectory()}/wwwroot";
var txtReportsPath = $"{rootPath}/data/txtReports";
var pdfReportsPath = $"{rootPath}/data/pdfReports";
if (!Directory.Exists(txtReportsPath))
{
    Directory.CreateDirectory(txtReportsPath);
}
if (!Directory.Exists(pdfReportsPath))
{
    Directory.CreateDirectory(pdfReportsPath);
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRequestTimeouts();


// GET: returns available algorithm names
app.MapGet("/algorithms", () => AlgorithmsProvider.GetAlgorithmNames())
.WithName("GetAlgorithms")
.WithDescription("Returns available algorithm names.")
.WithOpenApi();

// GET: returns parameters for given algoritm
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

// GET: returns fitness function names with available dimensions
app.MapGet("/functions", () => FitnessFunctionsProvider.GetFitnessfunctionsSchemas())
.WithName("GetFitnessFunctions")
.WithDescription("Returns available fitness function names and their domain dimensions.")
.WithOpenApi();

// GET: returns paths to reports
app.MapGet("reports/paths", () =>
{
    if (Directory.Exists(txtReportsPath) && Directory.Exists(pdfReportsPath))
    {
        string[] txtPaths = Directory.GetFiles(txtReportsPath).Select(path => Path.GetFileName(path)).ToArray();
        string[] pdfPaths = Directory.GetFiles(pdfReportsPath).Select(path => Path.GetFileName(path)).ToArray();
        ReportPaths paths = new ReportPaths
        {
            TxtPaths = txtPaths,
            PdfPaths = pdfPaths
        };
        return Results.Ok(paths);
    }
    else
    {
        Console.WriteLine(txtReportsPath);
        return Results.NotFound();
    }

})
.WithName("GetTxtReports")
.WithDescription("Returns paths to reports stored on the server.")
.Produces<ReportPaths[]>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

// POST: computes algorithm tests
app.MapPost("/test", async (TestRequest[] requests) =>
{
    TestResults[] results = new TestResults[requests.Length];
    try
    {
        for (int i = 0; i < requests.Length; i++)
        {
            IOptimizationAlgorithm? optimizationAlgorithm = AlgorithmsProvider.GetAlgorithm(requests[i].Algorithm, requests[i].N, requests[i].I);
            if (optimizationAlgorithm == null)
                return Results.BadRequest("Specified algorithm doesn't exist");

            fitnessFunction? fitnessFunction = FitnessFunctionsProvider.GetFitnessFunction(requests[i].Fun);
            if (fitnessFunction == null)
                return Results.BadRequest("Specified fitness function doesn't exist");

            double[][]? domain = FitnessFunctionsProvider.GetDomain(requests[i].Fun, requests[i].Dim);
            if (domain == null)
                return Results.BadRequest("Can't get domain for specified fitness function");

            if (domain.GetLength(0) != requests[i].Dim)
                return Results.BadRequest("Incorect dimension for specified fitness function");

            double[]? parameters = requests[i].Parameters;

            await Task.Run(() => optimizationAlgorithm.Solve(fitnessFunction, domain, requests[i].Parameters));

            TestResults result = new TestResults
            {
                AlgorithmName = optimizationAlgorithm.Name,
                FunctionName = requests[i].Fun,
                N = requests[i].N,
                I = requests[i].I,
                XBest = optimizationAlgorithm.XBest,
                FBest = optimizationAlgorithm.FBest,
                NumberOfEvaluationFitnessFunction = optimizationAlgorithm.NumberOfEvaluationFitnessFunction
            };
            results[i] = result;
        }
        TextFileReportWriter txtWriter = new TextFileReportWriter(results, rootPath);
        txtWriter.WriteTxt();
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Computing error: {ex.Message}");                   
        return Results.BadRequest($"Computing error: {ex.Message}");
    }
})
.WithName("PostTestRequest")
.WithDescription("Posts request for computing algorithm tests on given test functions with specified parametrs.")
.Produces<TestResults[]>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithRequestTimeout(TimeSpan.FromMinutes(10))
.WithOpenApi();

app.Run();