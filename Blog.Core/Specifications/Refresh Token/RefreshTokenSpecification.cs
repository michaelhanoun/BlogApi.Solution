using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Specifications.Refresh_Token
{
    public class RefreshTokenSpecification :BaseSpecifications<RefreshToken>
    {
        public RefreshTokenSpecification(string refreshToken):base(RT=>RT.Token == refreshToken && RT.Revoked==null&& RT.Expires>=DateTime.UtcNow)
        {
            Includes.Add(Q=>Q.Include(RT=>RT.ApplicationUser).ThenInclude(U=>U.RefreshTokens));
        }
    }
}
