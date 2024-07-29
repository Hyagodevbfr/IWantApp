using IWantApp.Domain.Users;

namespace IWantApp.Endpoints.Employees;


public class EmployeePost
{
    public static string Template => "/employees";
    public static string[] Method => new[] { HttpMethod.Post.ToString( ) };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(EmployeeRequest employeeRequest, HttpContext http, UserCreator userCreator)
    {
        var userId = http.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

        var userClaims = new List<Claim>{
            new("EmployeeCode", employeeRequest.EmployeeCode),
            new("Name", employeeRequest.Name),
            new("CreatedBy", userId)
        };

        (IdentityResult identity, string userId) result = await userCreator.Create(employeeRequest.Email, employeeRequest.Password,  userClaims);

        if(!result.identity.Succeeded)
            return Results.ValidationProblem(result.identity.Errors.ConvertToProblemDetails( ));

        return Results.Created($"/employess/{result.userId}", result.userId);
    }
}
