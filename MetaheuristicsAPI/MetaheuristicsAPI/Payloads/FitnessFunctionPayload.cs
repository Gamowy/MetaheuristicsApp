namespace MetaheuristicsAPI.Payloads
{
    public record FitnessFunctionPayload(string FunctionName, bool AnyDims, double[]? Dims = null);
}
