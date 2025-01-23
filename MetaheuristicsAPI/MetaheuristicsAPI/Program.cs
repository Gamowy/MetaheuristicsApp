using MetaheuristicsAPI;
using MetaheuristicsAPI.FileHadlers;
using MetaheuristicsAPI.FileHandlers;
using MetaheuristicsAPI.FitnessFunctions;
using MetaheuristicsAPI.Interfaces;
using MetaheuristicsAPI.Schemas;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRequestTimeouts();

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure file hosting
var rootPath = $"{Directory.GetCurrentDirectory()}/wwwroot";
var txtReportsPath = $"{rootPath}/data/txtReports";
var pdfReportsPath = $"{rootPath}/data/pdfReports";
var algorithmStatesPath = $"AlgorithmStates";
if (!Directory.Exists(txtReportsPath))
{
    Directory.CreateDirectory(txtReportsPath);
}
if (!Directory.Exists(pdfReportsPath))
{
    Directory.CreateDirectory(pdfReportsPath);
}
if (!Directory.Exists(algorithmStatesPath))
{
    Directory.CreateDirectory(algorithmStatesPath);
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
        string[] txtPaths = Directory.GetFiles(txtReportsPath).OrderBy(f => new FileInfo(f).CreationTime).Select(path => Path.GetFileName(path)).ToArray();
        string[] pdfPaths = Directory.GetFiles(pdfReportsPath).OrderBy(f => new FileInfo(f).CreationTime).Select(path => Path.GetFileName(path)).ToArray();
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

SavedTestState? savedTestState = null;
bool stopTest = false;

// GET: return boolean value indicating whether test can be resumed
app.MapGet("/test/resume", ([FromQuery] bool testMultiple = false) =>
{
    if (savedTestState != null)
    {
        if (testMultiple == savedTestState.testMultiple)
            return Results.Ok(true);
    }
    return Results.Ok(false);
})
.WithName("CheckTestResumable")
.WithDescription("Returns boolean value indicating whether test can be resumed.")
.Produces<bool>(StatusCodes.Status200OK)
.WithOpenApi();

// POST: posts request to stop tests
app.MapPost("/test/stop", ([FromQuery] bool value) =>
{
    stopTest = value;
    return Results.Ok();
})
.WithName("StopTest")
.WithDescription("Stops the running test")
.Produces(StatusCodes.Status200OK)
.WithOpenApi();

// POST: computes algorithm tests
app.MapPost("/test", async (TestRequest[] requests, [FromQuery] bool testMultiple=false, [FromQuery] bool resumeTest = false) =>
{  
    try
    {
        TestResults[] results;
        int index = 0;
        if (resumeTest && savedTestState != null)
        {
            index = savedTestState.lastTestIndex; 
            requests = savedTestState.requests;
            results = savedTestState.results;
            testMultiple = savedTestState.testMultiple;
        }
        else
        {
            results = new TestResults[requests.Length];
            savedTestState = new SavedTestState(0, requests, results, testMultiple);
        }
  
        for (int i = index; i < requests.Length; i++)
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
                AlgorithmName = requests[i].Algorithm,
                FunctionName = requests[i].Fun,
                N = requests[i].N,
                I = requests[i].I,
                Parameters = parameters,
                XBest = optimizationAlgorithm.XBest,
                FBest = optimizationAlgorithm.FBest,
                NumberOfEvaluationFitnessFunction = optimizationAlgorithm.NumberOfEvaluationFitnessFunction
            };
            results[i] = result;
            savedTestState.results = results;
            savedTestState.lastTestIndex = i + 1;

            if (stopTest)
            {
                stopTest = false;
                return Results.BadRequest("Test stopped");
            }
        }
        TextFileReportWriter txtWriter = new TextFileReportWriter(results, rootPath, testMultiple);
        PdfFileReportWriter pdfWriter = new PdfFileReportWriter(results, rootPath, testMultiple);
        txtWriter.WriteTxt();
        pdfWriter.GenerateReport();

        TestResponse response = new TestResponse
        {
            Results = results,
            PdfFilePath = pdfWriter.fileName
        };
        
        savedTestState = null;
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Computing error: {ex.Message}");                   
        return Results.BadRequest($"Computing error: {ex.Message}");
    }
})
.WithName("PostTestRequest")
.WithDescription("Posts request for computing algorithm tests on given test functions with specified parametrs.")
.Produces <TestResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithRequestTimeout(TimeSpan.FromMinutes(10))
.WithOpenApi();

app.Run();