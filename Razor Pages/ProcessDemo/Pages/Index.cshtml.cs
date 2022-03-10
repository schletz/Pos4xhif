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
    private readonly QueuedWorker _worker;
    [TempData]
    public string? Message { get; set; }
    public int QueueLength => _worker.QueueLength;
    public IndexModel(QueuedWorker worker)
    {
        _worker = worker;
    }

    public IActionResult OnPost()
    {
        var username = Guid.NewGuid().ToString().Substring(0, 8);
        for (int i = 0; i < Count; i++)
        {
            var (success, message) = _worker.TryAddJob(new QueuedWorker.Jobinfo(username, Server));
            if (!success) { Message = message; break; }
        }
        return RedirectToPage();
    }
    public void OnGet()
    {

    }
}
