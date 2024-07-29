namespace IWantApp.Endpoints.Security;

public class TokenPost
{
    public static string Template => "/token";
    public static string[] Method => new[] { HttpMethod.Post.ToString( ) };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static IResult Action(LoginRequest loginRequest,IConfiguration configuration, UserManager<IdentityUser> userManager, ILogger<TokenPost> _logger, IWebHostEnvironment environment)
    {
        _logger.LogInformation($"Getting token - {DateTime.UtcNow}");
        _logger.LogError("Error");
        _logger.LogWarning("Warning");

       var user = userManager.FindByEmailAsync( loginRequest.Email ).Result;
        if(user == null)
            return Results.BadRequest( );

        if(!userManager.CheckPasswordAsync(user,loginRequest.Password).Result)
            return Results.BadRequest( );

        var claims = userManager.GetClaimsAsync(user).Result;
        var subject = new ClaimsIdentity(new Claim[]
         {
                 new (ClaimTypes.Email, loginRequest.Email),
                 new (ClaimTypes.NameIdentifier, user.Id),
         });
        subject.AddClaims(claims);

        var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {

            Subject = subject,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature),
            Audience = configuration["JwtBearerTokenSettings:Audience"],
            Issuer = configuration["JwtBearerTokenSettings:Issuer"],
            Expires = environment.IsDevelopment() ||environment.IsStaging()? DateTime.UtcNow.AddDays(10): DateTime.UtcNow.AddMinutes(60),
        };
        
        var tokenHandler = new JwtSecurityTokenHandler( );
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new {token = tokenHandler.WriteToken(token)});
    }
}
