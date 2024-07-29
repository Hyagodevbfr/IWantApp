namespace IWantApp.Endpoints.Categories;

public class CategoryPost
{
    public static string Template => "/categories";
    public static string[] Method => new[] { HttpMethod.Post.ToString( ) };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(CategoryRequest categoryRequest, HttpContext http, ApplicationDbContext dbContext)
    {
        var userId = http.User.Claims.First(claim => claim.Type  == ClaimTypes.NameIdentifier).Value;

        var category = new Category(categoryRequest.Name, userId);

        if(!category.IsValid)
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        

        await dbContext.Categories!.AddAsync(category);
        await dbContext.SaveChangesAsync();

        return Results.Created($"/categories/{category.Id}", category.Id);
    }
}
