﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace CompanyEmployees.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "User roles(s)", new List<string> { "role" })
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[] 
            { };
        
        public static IEnumerable<Client> Clients =>
            new Client[] 
            {
                new Client
                {
                    ClientName = "CompanyEmployeeClient",
                    ClientId = "companyemployeeclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string> { "https://localhost:5010/signin-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles"
                    },
                    ClientSecrets =
                    {
                        new Secret("CompanyEmployeeClientSecret".Sha512())
                    },
                    RequirePkce = true,
                    PostLogoutRedirectUris = new List<string> { "https://localhost:5010/signout-callback-oidc" }
                    //RequireConsent = false
                }
            };
        
    }
}