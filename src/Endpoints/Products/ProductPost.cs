namespace IWantApp.Endpoints.Products;

public class ProductPost
{
    public static string Template => "/products";
    public static string[] Method => new[] { HttpMethod.Post.ToString( ) };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(ProductRequest productRequest, HttpContext http, ApplicationDbContext dbContext)
    {
        var userId = http.User.Claims.First(claim => claim.Type  == ClaimTypes.NameIdentifier).Value;
        var category = await dbContext.Categories!.FirstOrDefaultAsync(c => c.Id == productRequest.CategoryId);
        
        var product = new Product(productRequest.Name, category, productRequest.Description,productRequest.Price, productRequest.HasStock, userId);

        if(!product.IsValid)
            return Results.ValidationProblem(product.Notifications.ConvertToProblemDetails());
        

        await dbContext.Products!.AddAsync(product);
        await dbContext.SaveChangesAsync();

        return Results.Created($"/categories/{product.Id}", product.Id);
    }
}
