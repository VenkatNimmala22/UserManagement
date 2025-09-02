using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Text;

namespace UserManagementApp.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(IFormFile textFile)
        {
            if (textFile == null || textFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a text file to upload.";
                return View("Upload");
            }

            var lines = new List<string>();
            using (var reader = new StreamReader(textFile.OpenReadStream(), Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (line != null)
                        lines.Add(line);
                }
            }

            // Generate Excel file in memory
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Data");
                for (int i = 0; i < lines.Count; i++)
                {
                    worksheet.Cells[i + 1, 1].Value = lines[i];
                }
                package.Save();
            }
            stream.Position = 0;

            // Store the file in session for download
            HttpContext.Session.Set("ExcelFile", stream.ToArray());
            TempData["SuccessMessage"] = "Excel file created successfully!";
            return View("Upload");
        }

        [HttpGet]
        public IActionResult DownloadExcel()
        {
            var fileBytes = HttpContext.Session.Get("ExcelFile");
            if (fileBytes == null)
            {
                TempData["ErrorMessage"] = "No Excel file available for download. Please upload a text file first.";
                return RedirectToAction("Upload");
            }
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Converted.xlsx");
        }
    }
}
