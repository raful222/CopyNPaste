using Microsoft.AspNetCore.Identity;

namespace CopyNPaste.Core.Entities.Identity
{
    public class ApplicationRoleClaim : IdentityRoleClaim<int>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}
