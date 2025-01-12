using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MetaheuristicsAPI.Payloads
{
    public record TestRequest
    {
        [Required]
        public required string Algorithm { get; set; }
        [Required]
        public required int N { get; set; }
        [Required]
        public required int I { get; set; }
        [Required]
        public required string Fun { get; set; }
        [Required]
        public required int Dim { get; set; }
        [DefaultValue(null)]
        public double[]? Parameters { get; set; }
    }
}
