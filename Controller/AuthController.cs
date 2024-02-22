using ApiCatalogo.DTOs.AuthenticationsDTO;
using ApiCatalogo.Models;
using ApiCatalogo.Services.AuthenticationsServices;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    private readonly ILogger<AuthController> _logger;


    public AuthController(ITokenService tokenService,
                          RoleManager<IdentityRole> roleManager,
                          UserManager<ApplicationUser> userManager,
                          IConfiguration config,
                          ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _roleManager = roleManager;
        _userManager = userManager;
        _config = config;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> findAllUsersWithRoles()
    {
        var usersWithRoles = new List<UserWithRolesDto>();

        var users = await _userManager.Users.ToListAsync();

        foreach (var user in users)
        {
            // Esta chamada deve retornar uma lista vazia se não houver roles, em vez de null.
            var userRoles = await _userManager.GetRolesAsync(user) ?? new List<string>();
            var userWithRoles = new UserWithRolesDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = userRoles
            };
            usersWithRoles.Add(userWithRoles);
        }

        return Ok(usersWithRoles);
    }

    [HttpGet]
    [Route("GetRoles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return Ok(roles);
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

    [HttpPost] // Indica que este método de ação responde a solicitações HTTP POST.
    [Route("Register")] // Define a rota que será usada para acessar este método, que é "/Register".
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // Tenta encontrar um usuário existente com o mesmo nome de usuário.
        var userExists = await _userManager.FindByNameAsync(model.Username!);
        var emailExists = await _userManager.FindByEmailAsync(model.Email!);
        // Se um usuário com esse nome de usuário já existe, retorna um erro 500.
        if (userExists is not null || emailExists is not null)
            return StatusCode(StatusCodes.Status500InternalServerError,
                               new ResponseModel { Status = "Error", Message = "User or Email already exists!" });

        // Cria uma nova instância de ApplicationUser e define suas propriedades.
        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };
        // Tenta criar o usuário com a senha fornecida.
        var result = await _userManager.CreateAsync(user, model.Password!);
        // Se a criação do usuário falhar, retorna um erro 500.
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return StatusCode(StatusCodes.Status500InternalServerError,
                               new ResponseModel { Status = "Error", Message = $"User creation failed! Errors: {string.Join(", ", errors)}" });
        }

        // Se o usuário for criado com sucesso, retorna um código 200 com uma mensagem de sucesso.
        return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost] // Anotação para indicar que a ação do controlador responde a solicitações POST.
    [Route("refresh-token")] // Define a rota que será acessada, neste caso, "refresh-token".
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel is null) // Verifica se o modelo de token recebido é nulo.
        {
            return BadRequest("Invalid client request"); // Retorna um BadRequest se o tokenModel for nulo.
        }

        string? accessToken = tokenModel.AccessToken // Tenta obter o accessToken do tokenModel, se não estiver presente, lança uma exceção.
            ?? throw new ArgumentNullException(nameof(tokenModel));
        string? refreshToken = tokenModel.RefreshToken // Tenta obter o refreshToken do tokenModel, se não estiver presente, lança uma exceção.
            ?? throw new ArgumentNullException(nameof(tokenModel));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _config); // Obtém as credenciais do token expirado.

        if (principal == null) // Verifica se as credenciais são nulas.
        {
            return BadRequest("Invalid access token/refresh token"); // Retorna um BadRequest se as credenciais são nulas.
        }
        string username = principal.Identity.Name; // Obtém o nome de usuário do principal.

        var user = await _userManager.FindByNameAsync(username!); // Busca o usuário pelo nome de usuário.

        // Verifica se o usuário é nulo ou se o refresh token não corresponde ou expirou.
        if (user == null || user.RefreshToken != refreshToken
            || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Invalid access token/refresh token"); // Retorna um BadRequest se qualquer uma das condições acima for verdadeira.
        }

        // Gera um novo access token.
        var newAccessToken = _tokenService.GenerateAccessToken(
            principal.Claims.ToList(), _config);
        // Gera um novo refresh token.
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Atualiza o usuário com o novo refresh token e o tempo de expiração.
        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        // Retorna o novo access token e refresh token.
        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user is null) return BadRequest("Invalid client request");
        
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return Ok("Refresh Token has been revoked");
    }

    [HttpPost]
    [Route("CreateRole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        roleName = roleName.ToUpper();
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)
            {
                _logger.LogInformation(1, "Roles Added");
                return StatusCode(StatusCodes.Status200OK,
                    new ResponseModel { Status = "Success", Message = $"Role {roleName} added successfully" });
            }
            else
            {
                _logger.LogInformation(2, "Error");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ResponseModel { Status = "Error", Message = $"Issue adding the new {roleName} role" });
            }
        }
        return StatusCode(StatusCodes.Status400BadRequest,
            new ResponseModel { Status = "Error", Message = "Role already exist." });
    }
    [HttpPost]
    [Route("AddUserToRole")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        roleName = roleName.ToUpper();
        if (user != null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");
                return StatusCode(StatusCodes.Status200OK,
                    new ResponseModel { Status = "Success", Message = $"User {user.Email} added to the {roleName} role" });
            }
            else
            {
                _logger.LogInformation(1, $"Error: Unable to add user {user.Email} to the {roleName} role");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ResponseModel { Status = "Error", Message = $"Error: Unable to add user {user.Email} to the {roleName} role" });
            }
        }
        return BadRequest(new { error = "Unable to find user" });
    }
}
