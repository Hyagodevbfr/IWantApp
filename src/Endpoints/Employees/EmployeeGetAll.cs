namespace IWantApp.Endpoints.Employees;

public class EmployeeGetAll
{
    public static string Template => $"/employees";
    public static string[] Method => new[] { HttpMethod.Get.ToString( ) };
    public static Delegate Handle => Action;

    [Authorize(Policy = "Employee007Policy")]
    public static async Task<IResult> Action(int? page, int? rows, QueryAllUsersWithClaimName claim)
    {
        if(page == null)
        {
            page = 1;
        }
        else if(rows == null || rows > 10)
        {
            rows = 10;
        }

        var result = await claim.Execute(page.Value,rows!.Value); ;
        return Results.Ok( result );
    }
}
