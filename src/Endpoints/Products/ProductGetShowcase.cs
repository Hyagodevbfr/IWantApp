namespace IWantApp.Endpoints.Products;

public class ProductGetShowcase
{
    public static string Template => "/products/showcase";
    public static string[] Method => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static IResult Action(ApplicationDbContext dbContext, int page = 1, int row = 10, string? orderBy = "name")
    {
        if(row > 20)
            return Results.Problem(title: "Row with max 20", statusCode: 400);

        var queryBase = dbContext.Products?.AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.HasStock && p.Category!.Active);

        if(orderBy == "name")
            queryBase = queryBase?.OrderBy(p => p.Name);
        else if (orderBy == "price")
            queryBase = queryBase?.OrderBy(p => p.Price);
        else
            return Results.Problem(title: "Order by only by name or price",statusCode: 400);

        var queryFilter = queryBase?.Skip((page - 1) * row).Take(row);

        var products = queryFilter?.ToList( );
        var response = products?.Select(c => new ProductResponse(c.Id, c.Name, c.Category?.Name, c.Description!,c.Price, c.HasStock, c.Active));

        return Results.Ok(response);
    }
}
