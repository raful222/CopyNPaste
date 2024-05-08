using Microsoft.AspNetCore.Identity;

namespace CopyNPaste.Core.Entities.Identity
{
    public class ApplicationUserLogin : IdentityUserLogin<int>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
