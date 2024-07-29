using IWantApp.Endpoints.Employees;
using IWantApp.Endpoints.Security;
using Microsoft.AspNetCore.Diagnostics;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using IWantApp.Endpoints.Products;
using IWantApp.Domain.Users;
using IWantApp.Endpoints.Orders;
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
      configuration
            .WriteTo.Console()
            .WriteTo.MSSqlServer(
                   context.Configuration["ConnectionString:IWantDb"],
                       sinkOptions: new MSSqlServerSinkOptions()
                        {
                           AutoCreateSqlTable = true,
                           TableName = "LogAPI"
                        });
});


builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionString:IWantDb"]);
builder.Services.AddIdentity<IdentityUser,IdentityRole>( ).AddEntityFrameworkStores<ApplicationDbContext>( );

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder( )
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser( )
        .Build( );

    options.AddPolicy("EmployeePolicy",p =>
        p.RequireAuthenticatedUser( ).RequireClaim("EmployeeCode"));

    options.AddPolicy("Employee007Policy",p =>
        p.RequireAuthenticatedUser( ).RequireClaim("EmployeeCode","007"));

    options.AddPolicy("CpfPolicy",p =>
        p.RequireAuthenticatedUser( ).RequireClaim("Cpf"));
         });

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters( )
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]))
    };
});

builder.Services.AddScoped<QueryAllUsersWithClaimName>( );
builder.Services.AddScoped<UserCreator>( );

builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen( );

var app = builder.Build( );
app.UseAuthentication( );
app.UseAuthorization( );

if(app.Environment.IsDevelopment( ))
{
    app.UseSwagger( );
    app.UseSwaggerUI( );
}

app.UseHttpsRedirection( );

app.MapMethods(CategoryPost.Template,CategoryPost.Method,CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template,CategoryGetAll.Method,CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template,CategoryPut.Method,CategoryPut.Handle);
app.MapMethods(EmployeePost.Template,EmployeePost.Method,EmployeePost.Handle);
app.MapMethods(EmployeeGetAll.Template,EmployeeGetAll.Method,EmployeeGetAll.Handle);
app.MapMethods(TokenPost.Template,TokenPost.Method,TokenPost.Handle);
app.MapMethods(ProductPost.Template,ProductPost.Method,ProductPost.Handle);
app.MapMethods(ProductGetAll.Template,ProductGetAll.Method,ProductGetAll.Handle);
app.MapMethods(ProductGet.Template,ProductGet.Method,ProductGet.Handle);
app.MapMethods(ProductPut.Template,ProductPut.Method,ProductPut.Handle);
app.MapMethods(ProductGetShowcase.Template,ProductGetShowcase.Method,ProductGetShowcase.Handle);
app.MapMethods(OrderPost.Template,OrderPost.Method,OrderPost.Handle);
app.MapMethods(ClientGet.Template,ClientGet.Method,ClientGet.Handle);
app.MapMethods(OrderGet.Template,OrderGet.Method,OrderGet.Handle);


app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext context) =>
{
    var error = context.Features?.Get<IExceptionHandlerFeature>( )?.Error;
    
    if(error != null)
    {
        // Verifica se a exceção é uma AggregateException
        if(error is AggregateException aggregateException)
        {
            // Itera sobre as exceções internas
            foreach(var innerException in aggregateException.InnerExceptions)
            {
                // Verifica se a exceção interna é uma SqlException
                if(innerException is SqlException)
                {
                    // Lidar com a exceção SQL
                    return Results.Problem(title: "Database out",statusCode: 500);
                }
            }
        }else if(error is BadHttpRequestException badHttpRequestException)
        {
            return Results.Problem(title: "Error to convert data to other type. See all the information sent",statusCode: 400);
        }
    }
    
    return Results.Problem(title: "An error ocurred", statusCode: 500);
});

app.Run( );