using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SecureIdentity.Password;

namespace Blog.Controllers
{
    [ApiController]
    [Route("v1")]
    public class AccountController : ControllerBase
    {

        [HttpPost("accounts")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model,
                                              [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };
            var password = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(password);
            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>("Este email ja está cadastrado."));
            }
            catch (Exception)
            {
                return StatusCode(400, new ResultViewModel<string>("Falha interna do servidor."));
            }
        }

        [HttpPost("accounts/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model,
                                               [FromServices] BlogDataContext context,
                                               [FromServices] TokenService tokenService)
        {
            if (!ModelState.IsValid)
                BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
            var user = await context.Users.AsNoTracking()
                                          .Include(x => x.Roles)
                                          .FirstOrDefaultAsync(x => x.Email == model.Email);
            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos."));

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos."));
            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch (Exception)
            { 
                return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor."));
            }

        }
        
      //  [Authorize(Roles = "user")]
      //  [HttpGet("user")]
      //  public IActionResult GetUser() => Ok(User.Identity.Name);
      //  [Authorize(Roles = "author")]
      //  [HttpGet("author")]
      //  public IActionResult GetAuthor() => Ok(User.Identity.Name);
     //   [Authorize(Roles = "admin")]
      //  [HttpGet("admin")]
      //  public IActionResult GetAdmin() => Ok(User.Identity.Name);
    }
}
