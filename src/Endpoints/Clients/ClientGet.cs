using IWantApp.Domain.Users;

namespace IWantApp.Endpoints.Employees;

public class ClientGet
{
    public static string Template => "/clients";
    public static string[] Method => new[] { HttpMethod.Get.ToString( ) };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static IResult Action(HttpContext http)
    {
        var user = http.User;
        var result = new
        {
           Id = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
           Name = user.Claims.First(c => c.Type == "Name").Value,
           Cpf = user.Claims.First(c => c.Type == "Cpf").Value
        };
        return Results.Ok(result);
    }
}
