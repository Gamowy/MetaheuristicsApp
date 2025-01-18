namespace MetaheuristicsPlatform.Schemas
{
    public record ReportPaths
    {
        public required string[] TxtPaths { get; set; }
        public required string[] PdfPaths { get; set; }
    }
}
