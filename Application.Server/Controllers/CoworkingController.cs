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
    public class CoworkingController : ControllerBase
    {
        private readonly ICoworkingDatabaseService _coworkingDatabaseService;
        private readonly IMapper _mapper;
        public CoworkingController(ICoworkingDatabaseService coworkingDatabaseService, IMapper mapper)
        {
            _coworkingDatabaseService = coworkingDatabaseService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCoworkings()
        {
            var response = await _coworkingDatabaseService.GetCoworkings();
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok(response.Data!);
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
