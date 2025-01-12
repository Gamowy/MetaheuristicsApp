namespace MetaheuristicsPlatform.Schemas
{
    public record TestRequest
    {
        public required string Algorithm { get; set; }
        public required int N { get; set; }
        public required int I { get; set; }
        public required string Fun { get; set; }
        public required int Dim { get; set; }
        public double[]? Parameters { get; set; }
    }
}
