namespace MetaheuristicsAPI.Schemas
{
    public record TestResults
    {
        public required double[] XBest { get; set; }
        public required double FBest { get; set; }
        public required int NumberOfEvaluationFitnessFunction { get; set; }

    }
}
