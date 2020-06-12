﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CompanyEmployees.Client.Models;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Sockets;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Net;

namespace CompanyEmployees.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Privacy()
        {
            var idpClient = _httpClientFactory.CreateClient("IDPClient");
            var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var response = await idpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = metaDataResponse.UserInfoEndpoint,
                Token = accessToken
            });

            if (response.IsError)
            {
                throw new Exception("Problem while fetching data from UserInfoEndPoint.", response.Exception);
            }

            var addressClaim = response.Claims.FirstOrDefault(c => c.Type.Equals("address"));

            //Claim nickname = response.Claims.FirstOrDefault(c => c.Type.Equals(nameof(nickname)));

            User.AddIdentity(new ClaimsIdentity(new List<Claim>
            {
                new Claim(addressClaim.Type.ToString(), addressClaim.Value.ToString()),
                //new Claim(nickname.Type.ToString(), nickname.Value.ToString())
            }));

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Policy = "CanCreateAndModifyData")]
        public async Task<IActionResult> Companies()
        {
            var httpClient = _httpClientFactory.CreateClient("APIClient");

            var response = await httpClient.GetAsync("api/companies").ConfigureAwait(false);

            //response.EnsureSuccessStatusCode();
            if(response.IsSuccessStatusCode)
            {
                var companiesString = await response.Content.ReadAsStringAsync();
                var companies = JsonSerializer.Deserialize<List<CompanyViewModel>>(
                    companiesString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                _logger.LogInformation("{CompanyCount} has been returned.", companies.Count);

                return View(companies);
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            throw new Exception("There is a problem accessing the API.");
        }
    }
}
