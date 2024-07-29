using IWantApp.Domain.Users;

namespace IWantApp.Endpoints.Employees;

public class ClientPost
{
    public static string Template => "/clients";
    public static string[] Method => new[] { HttpMethod.Post.ToString( ) };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ClientRequest clientRequest, UserCreator userCreator)
    {
        var userClaims = new List<Claim>
        {
            new("Cpf", clientRequest.Cpf),
            new("Name", clientRequest.Name)
        };

        (IdentityResult identity, string userId) result = await userCreator.Create(clientRequest.Email,clientRequest.Password, userClaims);

        if(!result.identity.Succeeded)
            return Results.ValidationProblem(result.identity.Errors.ConvertToProblemDetails());       

        return Results.Created($"/clients/{result.userId}", result.userId);
    }
}
