using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Medpharma.Web.Helpers
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("UserFullName", user.FullName ?? ""));
            //identity.AddClaim(new Claim("UserLastName", user.LastName ?? ""));
            return identity;
        }
    }
}
