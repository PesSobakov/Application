﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.Server.Models.DTOs;
using Application.Server.Services;
using AutoMapper;
using System.Net;
using Application.Server.Models.CoworkingDatabase;
using FluentValidation;
using FluentValidation.Results;
using Application.Server.Models.ErrorResponses;
using Application.Server.Models.DTOs.GetBooking;
using Application.Server.Models.DTOs.GetBookings;

namespace Application.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class BookingController : ControllerBase
    {
        private readonly ICoworkingDatabaseService _coworkingDatabaseService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateBookingDto> _createBookingDtoValidator;
        private readonly IValidator<EditBookingDto> _editBookingDtoValidator;

        public BookingController(ICoworkingDatabaseService coworkingDatabaseService, IMapper mapper, IValidator<CreateBookingDto> createBookingDtoValidator, IValidator<EditBookingDto> editBookingDtoValidator)
        {
            _coworkingDatabaseService = coworkingDatabaseService;
            _mapper = mapper;
            _createBookingDtoValidator = createBookingDtoValidator;
            _editBookingDtoValidator = editBookingDtoValidator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            ValidationResult validationResult = _createBookingDtoValidator.Validate(createBookingDto);
            if (!validationResult.IsValid)
            {
                Dictionary<string, string> errors = new();
                validationResult.Errors.ForEach(error => errors.Add(error.PropertyName, error.ErrorMessage));
                return BadRequest(new ValidationError(errors));
            }

            var response = await _coworkingDatabaseService.CreateBooking(login, createBookingDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    if (response.ErrorMessages.Count != 0)
                    {
                        Dictionary<string, string> errors = new();
                        int counter = 1;
                        response.ErrorMessages.ForEach(error => errors.Add("error" + counter++.ToString(), error));
                        return BadRequest(new ValidationError(errors));
                    }
                    else return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _coworkingDatabaseService.DeleteBooking(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditBooking(int id, [FromBody] EditBookingDto editBoardDto)
        {
            //pretend this booking not exist when looking for awiolability
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            ValidationResult validationResult = _editBookingDtoValidator.Validate(editBoardDto);
            if (!validationResult.IsValid)
            {
                Dictionary<string, string> errors = new();
                validationResult.Errors.ForEach(error => errors.Add(error.PropertyName, error.ErrorMessage));
                return BadRequest(new ValidationError(errors));
            }

            var response = await _coworkingDatabaseService.EditBooking(login,id, editBoardDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    if (response.ErrorMessages.Count != 0)
                    {
                        Dictionary<string, string> errors = new();
                        int counter = 1;
                        response.ErrorMessages.ForEach(error => errors.Add("error" + counter++.ToString(), error));
                        return BadRequest(new ValidationError(errors));
                    }
                    else return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _coworkingDatabaseService.GetBooking(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    Booking booking = response.Data!;
                    Models.DTOs.GetBooking.BookingDto bookingDto = _mapper.Map<Models.DTOs.GetBooking.BookingDto>(booking);
                    return Ok(bookingDto);
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _coworkingDatabaseService.GetBookings(login);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    List<Booking> bookings = response.Data!;
                    List<Coworking> coworkings = bookings.Select(booking=>booking.Workspace).Select(workspace => workspace.Coworking).Distinct().ToList();
                    List<CoworkingDto> coworkingDtos = coworkings.Select(_mapper.Map<CoworkingDto>).ToList();
                    return Ok(coworkingDtos);
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [Route("[action]")]
        [ActionName("question")]
        [HttpPost]
        public async Task<IActionResult> BookingsQuestion([FromBody] StringDto question)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _coworkingDatabaseService.BookingsQuestion(login,question);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    StringDto responseText = response.Data!;
                    return Ok(responseText);
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
