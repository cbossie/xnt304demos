using MemoryDbAspnet.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics;
using System.Text.Json;

namespace MemoryDbAspnet.Pages;

public class IndexModel : PageModel
{
    public Stopwatch ServiceTimer { get; } = new Stopwatch();

    private readonly ILogger<IndexModel> _logger;
    private readonly IOrderService _svc;
    private readonly IDistributedCache _cache;


    // Properties for display
    public bool IsPopulated { get; set; }
    public string FirstLettter { get; set; }
    public decimal TotalCost { get; set; }
    public decimal NumOrders { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IOrderService svc, IDistributedCache cache)
    {
        _logger = logger;
        _svc = svc;
        _cache = cache;
    }

    public async Task OnGetDataAsync(string q)
    {
        OrderServiceTotalResult result;

        // Retrieve the data for the display
        ServiceTimer.Start();

        //Updated cache retrieval logic
        var cachedData = await _cache.GetStringAsync(q);
        if (cachedData is null)
        {
            result = await _svc.GetOrdersForCustomerId(q);
            await _cache.SetStringAsync(q, JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(15) }
                );
        }
        else
        {
            result = JsonSerializer.Deserialize<OrderServiceTotalResult>(cachedData);
        }

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
