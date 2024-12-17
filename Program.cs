/**
* Esempio di autenticazione JWT in ASP.NET Core
*
* 1. Installare pacchetti necessari:
* 
* dotnet add package Microsoft.AspNetCore.Authentication.JwtBeare
* https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer
*
* Questo pacchetto permette di decodificare facilmente il token (non è necessario per il funzionamento minimo)
* dotnet add package System.IdentityModel.Tokens.Jwt
* https://www.nuget.org/packages/system.identitymodel.tokens.jwt/
*
* 2. Aggiungere secret key in variabile d'ambiente
*
* Risorse:
* https://jwt.io/
* https://learn.microsoft.com/it-it/aspnet/core/security/authentication/jwt-authn?view=aspnetcore-9.0&tabs=windows
*
* (esempio struttura custom più avanzata: https://medium.com/@codewithankitsahu/authentication-and-authorization-in-net-8-web-api-94dda49516ee)
*/

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// 1. Aggiungere servizio di autenticazione
var secretKey = "Secret_Key"; // Inserire secret in variabile d'ambiente
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// 2. Aggiungere middleware di autenticazione
app.UseAuthentication();
app.UseAuthorization();

// 3. Metodo placeholder per autenticazione
bool AuthenticateCredentials(string username, string password)
{
    // Controllare i dati in database
    // ! la password va decodificata con lo stesso algoritmo di hashing e confrontata con quella memorizzata
    return username == "admin" && password == "password";
}

// 4. Genera JWT Token
string GenerateToken(string username)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
        Expires = DateTime.UtcNow.AddHours(1), // Scadenza token
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

// 5. API Endpoints
// Rotta: /auth -> Authentica, genera JWT e lo restituisce come risposta
// ! unica rotta pubblica
app.MapPost("/auth", (string username, string password) =>
{
    if (AuthenticateCredentials(username, password))
    {
        var token = GenerateToken(username);
        return Results.Ok(new { Token = token }); // ! metodo che restituisce automaticamente un codice 200 (OK)
    }
    return Results.Unauthorized(); // ! metodo che restituisce automaticamente un errore 401 (unauthorized)
});

// Rotta: /test -> Rotta protetta che richiede autenticazione
app.MapGet("/test", (ClaimsPrincipal user) =>
{
    // viene eseguito solo se l'utente è autenticato
    return Results.Ok(new { Message = $"Hello {user.Identity?.Name}, you accessed a protected route!" });
})
.RequireAuthorization(); // ! richiede autorizzazione


app.Run();


// FUNZIONI UTILI
// Decodifica JWT Token
object DecodeTokenPayload(string jwt)
{
    var handler = new JwtSecurityTokenHandler();
    var token = handler.ReadJwtToken(jwt);
    return new
    {
        payload = token.Payload
    };
}