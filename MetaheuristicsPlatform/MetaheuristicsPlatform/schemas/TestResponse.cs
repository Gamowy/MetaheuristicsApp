namespace MetaheuristicsPlatform.Schemas
{
    public record TestResponse
    {
        public required TestResults[] Results { get; set; }
        public string? PdfFilePath { get; set; }
    }
}
