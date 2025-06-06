using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Application.Server.Services;
using AutoMapper;
using System.Net;
using Application.Server.Models.CoworkingDatabase;
using Application.Server.Models.DTOs.GetWorkspaces;

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

        [HttpGet]
        public async Task<IActionResult> GetWorkspaces()
        {
            var response = await _coworkingDatabaseService.GetWorkspaces();
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    List<Workspace> workspaces = response.Data!;
                    List<WorkspaceDto> workspaceDtos = workspaces.Select(_mapper.Map<WorkspaceDto>).ToList();
                    return Ok(workspaceDtos);
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
