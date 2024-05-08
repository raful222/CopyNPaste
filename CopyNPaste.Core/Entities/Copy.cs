using CopyNPaste.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyNPaste.Core.Entities
{
    public class Copy : IAuditable
    {
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string? Subject { get; set; }

        public string? Body { get; set; }
        public string? Password { get; set; }
        public bool Policy { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set;  }
        public DateTime UpdatedAt { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
