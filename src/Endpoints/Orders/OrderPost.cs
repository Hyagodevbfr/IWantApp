using IWantApp.Domain.Orders;

namespace IWantApp.Endpoints.Orders;

public class OrderPost
{
    public static string Template => "/orders";
    public static string[] Method => new[] { HttpMethod.Post.ToString( ) };
    public static Delegate Handle => Action;

    [Authorize(Policy = "CpfPolicy")]
    public static async Task<IResult> Action(OrderRequest orderRequest, HttpContext httpContext, ApplicationDbContext DbContext)
    {
        var clientId = httpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        var clientName = httpContext.User.Claims.First(claim => claim.Type == "Name").Value;

        List<Product>? productsFound = null;
        if(orderRequest.ProductIds != null || orderRequest.ProductIds!.Any( ))
        {
            productsFound = DbContext.Products!.Where(p => orderRequest.ProductIds!.Contains(p.Id)).ToList();
        }

        var order = new Order(clientId,clientName,productsFound!,orderRequest.DeliveryAddress);
        if(!order.IsValid)
            return Results.ValidationProblem(order.Notifications.ConvertToProblemDetails( ));

        await DbContext.Orders!.AddRangeAsync(order);
        await DbContext.SaveChangesAsync();

        return Results.Created($"/orders/{order.Id}", order.Id);
    }
}
