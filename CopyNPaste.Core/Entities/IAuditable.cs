using System;
using System.Collections.Generic;
using System.Text;

namespace CopyNPaste.Core.Entities
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }

        DateTime UpdatedAt { get; set; }
    }
}
