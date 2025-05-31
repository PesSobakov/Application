using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.Server.Models.DTOs;
using Application.Server.Services;
using AutoMapper;
using System.Net;
using Application.Server.Models.CoworkingDatabase;
using FluentValidation.Results;
using FluentValidation;

namespace Application.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly ICoworkingDatabaseService _coworkingDatabaseService;
        private readonly IMapper _mapper;
        private readonly IValidator<LoginDto> _loginDtoValidator;

        public AuthController(ICoworkingDatabaseService coworkingDatabaseService, IMapper mapper, IValidator<LoginDto> loginDtoValidator)
        {
            _coworkingDatabaseService = coworkingDatabaseService;
            _mapper = mapper;
            _loginDtoValidator = loginDtoValidator;
        }

        [HttpGet]
        [ActionName("user")]
        public async Task<IActionResult> GetUser()
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                login = "test@gmail.com";
                var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, "test@gmail.com") };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
            }

            var response = await _coworkingDatabaseService.GetUser(login);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    string user = response.Data!.Email;
                    return Ok(new StringDto() { value = user });
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ActionName("user")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            ValidationResult validationResult = _loginDtoValidator.Validate(loginDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }

            var response = await _coworkingDatabaseService.Login(loginDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    User user = response.Data!;
                    var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email) };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return Ok();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
