using Microsoft.EntityFrameworkCore;

namespace IWantApp.Endpoints.Orders;

public class OrderGet
{
    public static string Template => "/orders/{id:Guid}";
    public static string[] Method => new[] { HttpMethod.Get.ToString( ) };
    public static Delegate Handle => Action;

    [Authorize]
    public async static Task<IResult> Action(Guid id,ApplicationDbContext dbContext,HttpContext httpContext, UserManager<IdentityUser> userManager)
    {
        var clientClain = httpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier);
        var employeeClaim = httpContext.User.Claims.FirstOrDefault(claim => claim.Type == "EmployeeCode");

        var order = dbContext.Orders?.Include( o => o.Products).FirstOrDefault(o => o.Id == id);

        if(order!.ClientId != clientClain.Value && employeeClaim == null)
            return Results.Forbid( );

        var client = await userManager.FindByIdAsync(order.ClientId);

        var productsResponse = order!.Products.Select(p => new OrderProduct(p.Id,p.Name, p.Price));
        var orderResponse =
            new OrderResponse(
                order.Id,client.Email, productsResponse,order.Total, order.DeliveryAddress);

        return  Results.Ok(orderResponse);
        
    }
}
