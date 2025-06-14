using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Application.Server.Services;
using AutoMapper;
using System.Net;
using Application.Server.Models.CoworkingDatabase;
using Application.Server.Models.DTOs.GetWorkspaces;
using System.Security.Claims;

namespace Application.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class WorkspaceController : ControllerBase
    {
        private readonly ICoworkingDatabaseService _coworkingDatabaseService;
        private readonly IMapper _mapper;
        public WorkspaceController(ICoworkingDatabaseService coworkingDatabaseService, IMapper mapper)
        {
            _coworkingDatabaseService = coworkingDatabaseService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkspaces(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();

            var response = await _coworkingDatabaseService.GetWorkspaces(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok(response.Data!);
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
