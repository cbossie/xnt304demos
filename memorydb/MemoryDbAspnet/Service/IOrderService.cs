namespace MemoryDbAspnet.Service;

public interface IOrderService
{
    Task<OrderServiceTotalResult> GetOrdersForCustomerId(string character);
}
