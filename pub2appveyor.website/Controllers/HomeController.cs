﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pub2appveyor.Services;
using pub2appveyor.website.Models;

namespace pub2appveyor.website.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string apiKey, ICollection<IFormFile> files)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(nameof(apiKey));

            var formFiles = Request.Form.Files;
            var pubFiles = formFiles.Where(x => x.FileName.ToLowerInvariant().EndsWith(".publishsettings")).ToList();

            if (!pubFiles.Any())
                throw new Exception("Please upload a .publishsettings file.");

            AppVeyorService service = new AppVeyorService(apiKey);
            var result = await service.Process(pubFiles);

            return View();
        }
    }
}
