namespace IWantApp.Endpoints.Products;

public class ProductGet
{
    public static string Template => "/products/{id:Guid}";
    public static string[] Method => new string[] { HttpMethod.Get.ToString( ) };
    public static Delegate Handle => Action;
    public static IResult Action([FromRoute] Guid id,ApplicationDbContext DbContext)
    {
        var product = DbContext.Products?.AsNoTracking( ).Include(p => p.Category).Where(p => p.Id == id);
        var response = product!.Select(c => new ProductResponse(c.Id, c.Name, c.Category!.Name, c.Description!,c.Price, c.HasStock, c.Active));
        if(product != null)
            return Results.Ok(response);
        return Results.NotFound( );
    }
}
