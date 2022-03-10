using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProcessDemo.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProcessDemo.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Server { get; set; } = "www.google.com";
    [BindProperty]
    public int Count { get; set; } = 1;
    private readonly QueuedWorker _directoryReader;
    [TempData]
    public string? Message { get; set; }
    public IndexModel(QueuedWorker directoryReader)
    {
        _directoryReader = directoryReader;
    }

    public IActionResult OnPost()
    {
        var username = Guid.NewGuid().ToString().Substring(0, 8);
        for (int i = 0; i < Count; i++)
        {
            var (success, message) = _directoryReader.TryAddJob(new Jobinfo(username, Server));
            if (!success) { Message = message; break; }
        }
        return RedirectToPage();
    }
    public void OnGet()
    {

    }
}
