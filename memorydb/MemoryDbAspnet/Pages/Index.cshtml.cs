using MemoryDbAspnet.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace MemoryDbAspnet.Pages;

public class IndexModel : PageModel
{
    public Stopwatch ServiceTimer { get; } = new Stopwatch();

    private readonly ILogger<IndexModel> _logger;
    private readonly IOrderService _svc;

    // Properties for display
    public bool IsPopulated { get; set; }
    public string FirstLettter { get; set; }
    public decimal TotalCost { get; set; }
    public decimal NumOrders { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IOrderService svc)
    {
        _logger = logger;
        _svc = svc;
    }

    public async Task OnGetDataAsync(string q)
    {
        OrderServiceTotalResult result;

        // Retrieve the data for the display
        ServiceTimer.Start();

        result = await _svc.GetOrdersForCustomerId(q);

        // Populate Page Data
        TotalCost = result.TotalCost;
        NumOrders = result.NumOrders;
        FirstLettter = q;
        IsPopulated = true;
        ServiceTimer.Stop();
    }

    public async Task OnGetAsync()
    {
        IsPopulated = false;
    }
}
