using System.ComponentModel.DataAnnotations;

namespace MetaheuristicsAPI.Schemas
{
    public record ReportPaths
    {
        [Required]
        public required string[] TxtPaths { get; set; }
        [Required]
        public required string[] PdfPaths { get; set; }
    }
}
