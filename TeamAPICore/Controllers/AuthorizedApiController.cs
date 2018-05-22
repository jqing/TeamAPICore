using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OAuth.Introspection;
using TeamAPICore.Models;

namespace TeamAPICore.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthIntrospectionDefaults.AuthenticationScheme, Roles = "Purchaser", Policy = "NeedCRMId")]
    public class AuthorizedApiController : Controller
    {
        protected string CRMContactID
        {
            get
            {
                var identity = User.Identity as ClaimsIdentity;
                var CRMContactID = string.Empty;
                var claim = identity.FindFirst("CRMContactId");
                if (claim != null)
                    CRMContactID = claim.Value;

                return CRMContactID;
            }
        }

        protected bool HasRole(string role)
        {
            var identity = User.Identity as ClaimsIdentity;
            return identity.HasClaim(identity.RoleClaimType, role);
        }

        protected UserInfo UserInfo
        {
            get
            {
                var identity = User.Identity as ClaimsIdentity;
                var roles = identity.Claims.Where(c => c.Type == identity.RoleClaimType).Select(c => c.Value).ToList();
                UserInfo info = new UserInfo();
                var claim = identity.FindFirst("fullname");
                if (claim != null)
                    info.Name = claim.Value;
                claim = identity.FindFirst("email");
                if (claim != null)
                    info.Email = claim.Value;
                info.Roles = roles;

                return info;
            }
        }

    }
}