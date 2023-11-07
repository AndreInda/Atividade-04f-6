using Exo.WebApi.Contexts;
using Exo.WebApi.Repositories;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ExoContext, ExoContext>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Forma de Atuenticação.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
//Parametros de validação do token.
.AddJwtBearer("JwtBearer", Options =>
{
    Options.TokenValidationParameters = new TokenValidationParameters
    {
        //valida quem esta solicitando.
        ValidateIssuer = true,
        //valida quem eta recebendo.
        ValidateAudience = true,
        //Define se o tempo deexpiração será validado.
        ValidateLifetime = true,
        //Criptografia e valdação da chave de autenticação.
        IssuerSigningKey = new
        SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("exoapi-chave-autenticacao")),
        //valida o tempo de expiração do token.
        ClockSkew = TimeSpan.FromMinutes(30),
        //Nome do issuer, da origem.
        ValidIssuer = "exoapi.webapi",
        //nome do audience, para o destino.
        ValidAudience = "exiapi.webapi"
    };
});
builder.Services.AddTransient<ProjetoRepository, ProjetoRepository>();
builder.Services.AddTransient<UsuarioRepository, UsuarioRepository>();

var app = builder.Build();

app.UseRouting();

//Habilita a autenticação.
app.UseAuthentication();

//Habilita a autorização.
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
