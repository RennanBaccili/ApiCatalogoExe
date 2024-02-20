using ApiCatalogo.DTOs.AuthenticationsDTO;
using ApiCatalogo.Models;
using ApiCatalogo.Services.AuthenticationsServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiCatalogo.Controller;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    // token service é para gerar tokens
    private ITokenService _tokenService;
    // role manager é para criar e gerenciar roles
    private readonly RoleManager<IdentityRole> _roleManager;
    // user manager é para criar e gerenciar usuários
    private readonly UserManager<ApplicationUser> _userManager;
    // IConfiguration é para acessar o appsettings.json
    private readonly IConfiguration _config;

    public AuthController(ITokenService tokenService,
                          RoleManager<IdentityRole> roleManager,
                          UserManager<ApplicationUser> userManager,
                          IConfiguration config)
    {
        _tokenService = tokenService;
        _roleManager = roleManager;
        _userManager = userManager;
        _config = config;
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        // vejo se o usuariro existe
        var user = await _userManager.FindByNameAsync(model.Username!);
        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                //jti é um identificador único para o token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            // adiciono as roles do usuário a lista de claims
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            // nesse token eu coloco as claims e o tempo de expiração
            var token = _tokenService.GenerateAccessToken(authClaims, _config);

            var refreshToken = _tokenService.GenerateRefreshToken();

            // _ é uma convenção para variáveis que não serão usadas
            _ = int.TryParse(_config["JWT:RefreshTokenValidityInMinutes"],
                                  out int refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime =
                DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                RefreshToken = refreshToken
            });
        }
        return Unauthorized();
    }
}
