namespace MetaheuristicsAPI.Schemas
{
    public record TestResults
    {
        public required string AlgorithmName { get; set; }
        public required string FunctionName { get; set; }
        public required int N { get; set; }
        public required int I { get; set; }
        public required double[]? Parameters { get; set; }
        public required double[] XBest { get; set; }
        public required double FBest { get; set; }
        public required int NumberOfEvaluationFitnessFunction { get; set; }
    }
}
