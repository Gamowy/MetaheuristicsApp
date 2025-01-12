namespace MetaheuristicsAPI.Schemas
{
    public record FitnessFunctionSchema(string FunctionName, bool AnyDims, double[]? Dims = null);
}
