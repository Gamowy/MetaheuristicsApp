namespace MetaheuristicsAPI.Schemas
{
    public record FitnessFunctionSchema(string FunctionName, bool AnyDims, int[]? Dims = null);
}
