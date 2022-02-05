using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreManager.Application.Services;
using System.IO;
using System.Linq;

namespace StoreManager.Webapp.Pages.Import
{
    public class IndexModel : PageModel
    {
        private static string[] _allowedTextExtensions = { ".txt", ".csv" };
        private static string[] _allowedExcelExtensions = { ".xls", ".xlsx" };
        private readonly ProductImportService _importService;

        public IndexModel(ProductImportService importService)
        {
            _importService = importService;
        }

        [BindProperty]
        public IFormFile? UploadedFile { get; set; }
        [TempData]
        public string? ErrorMessage { get; set; }
        [TempData]
        public string? Message { get; set; }
        public IActionResult OnPostCsvImport()
        {
            var (success, message) = CheckUploadedFile(_allowedTextExtensions);
            if (!success)
            {
                ErrorMessage = message;
                return RedirectToPage();
            }

            using var stream = UploadedFile!.OpenReadStream();
            (success, message) = _importService.LoadCsv(stream);
            if (!success) { ErrorMessage = message; }
            else { Message = message; }
            return RedirectToPage();
        }

        public IActionResult OnPostExcelImport()
        {
            var (success, message) = CheckUploadedFile(_allowedExcelExtensions);
            if (!success)
            {
                ErrorMessage = message;
                return RedirectToPage();
            }

            using var stream = UploadedFile!.OpenReadStream();
            (success, message) = _importService.LoadExcel(stream);
            if (!success) { ErrorMessage = message; }
            else { Message = message; }
            return RedirectToPage();
        }

        private (bool success, string message) CheckUploadedFile(string[] allowedExtensions)
        {
            if (UploadedFile is null) { return (false, "Es wurde keine Datei hochgeladen."); }
            var extension = Path.GetExtension(UploadedFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return (false, $"Es dürfen nur Dateien mit der Erweiterung {string.Join(",", allowedExtensions)} hochgeladen werden.");
            }
            if (UploadedFile.Length > 1 << 20)
            {
                return (false, "Die Datei darf maximal 1 MB groß sein.");
            }
            return (true, string.Empty);
        }
    }
}
