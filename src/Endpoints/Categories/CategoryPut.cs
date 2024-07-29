namespace IWantApp.Endpoints.Categories;

public class CategoryPut
{
    public static string Template => "/categories/{id:Guid}";
    public static string[] Method => new[] { HttpMethod.Put.ToString( ) };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] Guid id, [FromBody]CategoryRequest categoryRequest, HttpContext http, ApplicationDbContext dbContext)
    {
        var userId = http.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        var category = dbContext.Categories?.Where(c => c.Id == id).FirstOrDefault();

        if(category == null)
            return Results.NotFound( );

        category!.EditInfo(categoryRequest.Name,categoryRequest.Active);

        if (!category.IsValid)
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails( ));

        category.EditedBy = userId;
        category.EditedOn = DateTime.Now;
        

        await dbContext.SaveChangesAsync( );
        return Results.Ok();
        
    }
}
