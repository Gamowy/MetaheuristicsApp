using System.ComponentModel.DataAnnotations;

namespace MetaheuristicsAPI.Schemas
{
    public record TestResponse
    {
        [Required]
        public required TestResults[] Results { get; set; }
        public string? PdfFilePath { get ; set; }
    }
}
