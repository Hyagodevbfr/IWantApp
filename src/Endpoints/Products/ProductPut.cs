namespace IWantApp.Endpoints.Products;

public class ProductPut
{
    public static string Template => "/products/{id:Guid}";
    public static string[] Method => new[] { HttpMethod.Put.ToString( ) };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] Guid id,[FromBody] ProductRequest productRequest,HttpContext http,ApplicationDbContext dbContext)
    {
        var userId = http.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        var product = dbContext.Products?.Where(p => p.Id == id).FirstOrDefault( );

        if(product == null)
            return Results.NotFound( );

        product!.EditInfo(productRequest.Name,productRequest.Description!,productRequest.HasStock,productRequest.Active);

        if(!product.IsValid)
            return Results.ValidationProblem(product.Notifications.ConvertToProblemDetails( ));

        product.EditedBy = userId;
        product.EditedOn = DateTime.Now;


        await dbContext.SaveChangesAsync( );
        return Results.Ok( );
    }
}
