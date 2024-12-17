# NET8 APIs

## Risorse Utili

**JWT**: https://jwt.io/
**guida**: https://learn.microsoft.com/it-it/aspnet/core/security/authentication/jwt-authn?view=aspnetcore-9.0&tabs=windows
**nuget package**: https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer
**esempio custom**: https://medium.com/@codewithankitsahu/authentication-and-authorization-in-net-8-web-api-94dda49516ee

## Descrizione Repository

[Il programma principale](Program.cs) implementa il nu package Microsoft.AspNetCore.Authentication.JwtBearer e presenta un esempio di uso con 2 API:

- /auth: Pubblica, permette di ottenere un JWT
- /test: Private, necessita di un JWT valido

Inoltre ho aggiunto una semplice funzione di decodifica del token che può essere usata per estrapolare l'id dell'utente da ogni richiesta autenticata (così da non dover includere l'id dell'utente negli args delle API)
