using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ShroomCity.Repositories;
using ShroomCity.Repositories.Implementations;
using ShroomCity.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ShroomCity.API.Middleware.Authentication;
using ShroomCity.API.Middleware.Exceptions;
using ShroomCity.Models.Constants;
using ShroomCity.Services.Implementations;
using ShroomCity.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


// Bind JwtConfiguration to configuration to inject as IOptions<JwtConfiguration>
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("TokenAuthentication"));

// Adding Authentication  
builder.Services.AddJwtAuthentication(builder.Configuration);
// Adding Authorization policies
builder.Services.AddAuthorization(options =>
{
    // Define role permissions inline
    var rolePermissions = new Dictionary<string, List<string>>
    {
        {RoleConstants.Admin, new List<string> { PermissionConstants.ReadMushrooms, PermissionConstants.WriteMushrooms, PermissionConstants.ReadResearchers, PermissionConstants.WriteResearchers }},
        {RoleConstants.Researcher, new List<string> { PermissionConstants.ReadMushrooms, PermissionConstants.WriteMushrooms, PermissionConstants.ReadResearchers }},
        {RoleConstants.Analyst, new List<string> { PermissionConstants.ReadMushrooms, PermissionConstants.ReadResearchers }},
    };

    // Create policies for roles based on permissions
    foreach (var role in rolePermissions)
    {
        options.AddPolicy($"{role.Key}", policyBuilder =>
        {
            policyBuilder.RequireAssertion(context =>
            {
                // Extract user's permissions from claims
                var userPermissions = context.User.Claims
                    .Where(c => c.Type == ClaimTypeConstants.PermissionClaimType)
                    .Select(c => c.Value)
                    .ToList();

                // Check if user has all the permissions associated with the role
                return role.Value.All(rolePermission => userPermissions.Contains(rolePermission));
            });
        });
    }
    // Also add individual permissions, if we require more granular control
    options.AddPolicy(PermissionConstants.WriteResearchers, policy => policy.RequireClaim(ClaimTypeConstants.PermissionClaimType, PermissionConstants.WriteResearchers));
    options.AddPolicy(PermissionConstants.ReadResearchers, policy => policy.RequireClaim(ClaimTypeConstants.PermissionClaimType, PermissionConstants.ReadResearchers));
    options.AddPolicy(PermissionConstants.WriteMushrooms, policy => policy.RequireClaim(ClaimTypeConstants.PermissionClaimType, PermissionConstants.WriteMushrooms));
    options.AddPolicy(PermissionConstants.ReadMushrooms, policy => policy.RequireClaim(ClaimTypeConstants.PermissionClaimType, PermissionConstants.ReadMushrooms));
});

// DB
builder.Services.AddDbContext<ShroomCityDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Database"),
        b => b.MigrationsAssembly("ShroomCity.Repositories")
        );
});

// REPOSITORIES

// Account
builder.Services.AddScoped<IAccountRepository, AccountRepository>(sp =>
{
    var salt = builder.Configuration.GetValue<string>("Salt");
    
    if (string.IsNullOrEmpty(salt))
    {
        throw new InvalidOperationException("The 'Salt' configuration value is missing or empty.");
    }

    var context = sp.GetRequiredService<ShroomCityDbContext>();
    var tokenRepository = sp.GetRequiredService<ITokenRepository>();
    return new AccountRepository(context, salt, tokenRepository);
});

// Token
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
// Mushroom
builder.Services.AddScoped<IMushroomRepository, MushroomRepository>();

// Researcher
builder.Services.AddScoped<IResearcherRepository, ResearcherRepository>();


// SERVICES

// Account
builder.Services.AddScoped<IAccountService, AccountService>();
// Token
builder.Services.AddScoped<ITokenService, TokenService>();

// Mushroom
builder.Services.AddScoped<IMushroomService, MushroomService>();

// External Mushroom
builder.Services.AddScoped<IExternalMushroomService, ExternalMushroomService>();

// Typed injection
builder.Services.AddHttpClient<IExternalMushroomService, ExternalMushroomService>(client => {
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ExternalApiBaseUrl") ?? "");
});


// Researcher
builder.Services.AddScoped<IResearcherService, ResearcherService>();


builder.Services.AddControllers(
    options =>
    {
        options.Filters.Add<GlobalExceptionFilter>();
    }
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

// TODO: Turn off Swagger in production
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
