﻿using BusinessLogic.Interfaces;
using DataAccess.Models;
        public string GetJwtToken(Aspnetuser aspnetuser)
        {
            var role = aspnetuser.Aspnetuserroles.FirstOrDefault(x => x.Userid == aspnetuser.Id);
            int roleid = role.Roleid;
            var claims = new List<Claim>
                claims,

                // Corrected access to the validatedToken

                jwtSecurityToken = (JwtSecurityToken)validatedToken;