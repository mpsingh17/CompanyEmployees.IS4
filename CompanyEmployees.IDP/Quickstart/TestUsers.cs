// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace CompanyEmployees.IDP
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "a9ea0f25-b964-409f-bcce-c923266249b4",
                Username = "John",
                Password = "JohnPassword",
                Claims = new List<Claim>
                {
                    new Claim("given_name", "John"),
                    new Claim("family_name", "Doe"),
                    new Claim("nickname", "Peeta"),
                    new Claim("address", "37D Station Road, Takanini."),
                    new Claim("role", "Admin"),
                    new Claim("country", "USA")
                }
            },
            new TestUser
            {
                SubjectId = "c95ddb8c-79ec-488a-a485-fe57a1462340",
                Username = "Jane",
                Password = "JanePassword",
                Claims = new List<Claim>
                {
                    new Claim("given_name", "Jane"),
                    new Claim("family_name", "Doe"),
                    new Claim("address", "1 Station Road, Takanini."),
                    new Claim("role", "Visitor"),
                    new Claim("country", "USA")
                }
            }
        };
    }
}