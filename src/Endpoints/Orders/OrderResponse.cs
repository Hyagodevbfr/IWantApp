namespace IWantApp.Endpoints.Orders;

public record OrderResponse(
    Guid ClientId,string ClientEmail,
    IEnumerable<OrderProduct> Products, decimal Total,
    string DeliveryAddress);

    public record OrderProduct(Guid id, string name, decimal price);