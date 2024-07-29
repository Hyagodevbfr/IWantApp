namespace IWantApp.Endpoints.Products;

public class ProductGetAll
{
    public static string Template => "/products";
    public static string[] Method => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static IResult Action(ApplicationDbContext dbContext)
    {
        var products = dbContext.Products?.AsNoTracking( ).Include(p => p.Category).OrderBy(p => p.Name).ToList( );
        var response = products?.Select(c => new ProductResponse(c.Id, c.Name, c.Category?.Name, c.Description!, c.Price, c.HasStock, c.Active));

        return Results.Ok(response);
    }
}
