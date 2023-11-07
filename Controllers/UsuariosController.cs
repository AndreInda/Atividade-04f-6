using Exo.WebApi.Models;
using Exo.WebApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Exo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuariosController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }


        // get -> /api/usuarios
        [HttpGet]
        public IActionResult Listar()
        {
            return Ok(_usuarioRepository.Listar());
        }


        // // post -> /api/usuarios
        // [HttpPost]
        // public IActionResult Cadastrar(Usuario usuario)
        // {
        //    _usuarioRepository.Cadastrar(usuario);
        //    return StatusCode(201);
        // }

        //Novo codigo POST para auxiliar o metodo de login.
        public IActionResult Post(Usuario usuario)

        {
            Usuario usuarioBuscado = _usuarioRepository.Login(usuario.Email, usuario.Senha);
            if (usuarioBuscado == null)
            {
                return NotFound("E-mail ou senha Inválidos");
            }

            //Se o usuário for encontrado, segue a criação do token.

            //Define os dados que serão fornecidos no token - payload.
            var Claims = new[]
            {
                //Armazena na claim o e-mail usuario autenticado.
                new Claim(JwtRegisteredClaimNames.Email, usuarioBuscado.Email),


                //Armazena na claimo id do usuario autenticado.
                new Claim(JwtRegisteredClaimNames.Jti, usuarioBuscado.Id.ToString()),
            };
            //Define a chave de acesso ao token.
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("exiapi-chave-autenticacao"));

            //Define as credenciais do token.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Gera o token.
            var token = new JwtSecurityToken(
                issuer: "exoapi.webapi", //Emissor do token.
                audience: "exoapi.webapi", //Destinatario do token.
                claims: Claims, //Dados definidos acima.
                expires: DateTime.Now.AddMinutes(30), //tempo de expiração.
                signingCredentials: creds //Credenciais do token.
            );

            //Retorna ok com o token.
            return Ok( new{ token = new JwtSecurityTokenHandler().WriteToken(token)});
            //fim do novo codigo POST para auxilir o metodo login.
        }

        // get -> /api/usuarios/{id}
        [HttpGet("{id}")] // Faz a busca pelo ID.
        public IActionResult BuscarPorId(int id)
        {
            Usuario usuario = _usuarioRepository.BuscaPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        // put -> /api/usuarios/{id}
        // Atualiza.
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Usuario usuario)
        {
            _usuarioRepository.Atualizar(id, usuario);
            return StatusCode(204);
        }

        // delete -> /api/usuarios/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            try
            {
                _usuarioRepository.Deletar(id);
                return StatusCode(204);
            }
            catch (Exception e)
            {
                return BadRequest();
            }

        }

    }
}
