namespace MetaheuristicsAPI.Schemas
{
    public record TestResults
    {
        public required string AlgorithmName { get; set; }
        public required string FunctionName { get; set; }
        public required double[] XBest { get; set; }
        public required double FBest { get; set; }
        public required int NumberOfEvaluationFitnessFunction { get; set; }

    }
}
