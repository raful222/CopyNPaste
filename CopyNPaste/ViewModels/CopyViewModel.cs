using System.ComponentModel.DataAnnotations;

namespace CopyNPaste.ViewModels
{
    public class CopyViewModel
    {
        public int Id { get; set; }
        public string? Subject { get; set; }

        public string? Body { get; set; }
        public string? Password { get; set; }
        public bool Policy { get; set; }
        public int? UserId { get; set; }

        public string[]? ColorCodes { get; set; }
        public int[]? SizeCodes { get; set; }
    }
}
