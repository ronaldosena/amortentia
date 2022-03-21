using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using backend.Data;
using backend.Dtos;
using backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
  [ServiceFilter(typeof(LogUserActivity))]
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IUsersRepository _repo;
    private readonly IMapper _mapper;
    public UsersController(IUsersRepository repo, IMapper mapper)
    {
      _mapper = mapper;
      _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
    {
      userParams.Username = User.FindFirst(ClaimTypes.Name).Value;
      var users = await _repo.GetUsers(userParams);
      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

      Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
      return Ok(usersToReturn);
    }

    [HttpGet("{username}", Name = "GetUser")]
    public async Task<IActionResult> GetUser(string username)
    {
      var user = await _repo.GetUser(username);
      var userToReturn = _mapper.Map<UserForDetailDto>(user);
      return Ok(userToReturn);
    }

    [HttpPut("{username}")]
    public async Task<IActionResult> UpdateUser(string username, UserForUpdateDto userForUpdateDto)
    {
      // Checks if the username trying to update the resource is the same as in the token
      if (username != User.FindFirst(ClaimTypes.Name).Value)
        return Unauthorized();

      if (await _repo.UpdateUser(username, userForUpdateDto))
        return NoContent();

      throw new Exception($"Updating user {username} failed on save");
    }
  }
}