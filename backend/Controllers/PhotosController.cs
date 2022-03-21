using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using backend.Data;
using backend.Dtos;
using backend.Helpers;
using backend.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace backend.Controllers
{
  [Authorize]
  [Route("api/users/{username}/photos")]
  [ApiController]
  public class PhotosController : ControllerBase
  {
    private readonly IUsersRepository _repo;
    private readonly IMapper _mapper;
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private Cloudinary _cloudinary;

    public PhotosController(IUsersRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
    {
      _cloudinaryConfig = cloudinaryConfig;
      _mapper = mapper;
      _repo = repo;

      Account acc = new Account(
          _cloudinaryConfig.Value.CloudName,
          _cloudinaryConfig.Value.ApiKey,
          _cloudinaryConfig.Value.ApiSecret
      );

      _cloudinary = new Cloudinary(acc);
    }

    [HttpGet("{photoId}", Name = "GetPhoto")]
    public async Task<IActionResult> GetPhoto(string username, string photoId)
    {
      var photoFromRepo = await _repo.GetPhoto(username, photoId);

      var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

      return Ok(photo);
    }

    [HttpPost]
    public async Task<IActionResult> AddPhotoForUser(string username, [FromForm]PhotoForCreationDto photoForCreationDto)
    {
      if (username != User.FindFirst(ClaimTypes.Name).Value)
        return Unauthorized();

      var file = photoForCreationDto.File;
      var uploadResult = new ImageUploadResult();

      if (file.Length > 0)
      {
        using (var stream = file.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams()
          {
            File = new FileDescription(file.Name, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
          };
          uploadResult = _cloudinary.Upload(uploadParams);
        }
      }

      photoForCreationDto.Url = uploadResult.Uri.ToString();
      photoForCreationDto.PublicId = uploadResult.PublicId;

      var photo = _mapper.Map<Photo>(photoForCreationDto);

      if (!await _repo.AnyMainPhoto(username))
        photo.IsMain = true;

      if (await _repo.AddPhoto(username, photo))
      {
        var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
        return CreatedAtRoute("GetPhoto", new { photoId = photo.PublicId }, photoToReturn);
      }

      return BadRequest("Could not add the photo");
    }

    [HttpPost("{photoId}/setMain")]
    public async Task<IActionResult> SetMainPhoto(string username, string photoId)
    {
      if (username != User.FindFirst(ClaimTypes.Name).Value)
        return Unauthorized();

      var photo = await _repo.GetPhoto(username, photoId);

      if (photo == null)
        return Unauthorized();

      if (await _repo.SetMainPhoto(username, photoId))
        return NoContent();

      return BadRequest("Could not set photo to main");
    }

    [HttpDelete("{photoId}")]
    public async Task<IActionResult> DeletePhoto(string username, string photoId)
    {
      if (username != User.FindFirst(ClaimTypes.Name).Value)
        return Unauthorized();

      if (await _repo.IsMainPhoto(username, photoId))
        return BadRequest("You cannot delete your main photo");

      var deleteParams = new DeletionParams(photoId);
      var result = _cloudinary.Destroy(deleteParams);

      if (result.Result == "ok")
      {
        if (await _repo.DeletePhoto(username, photoId))
        {
          return Ok();
        }
      }


      // if (photoFromRepo.PublicId == null)
      // {
      //   _repo.Delete(photoFromRepo);
      // }

      // if (await _repo.SaveAll())
      // return Ok();

      return BadRequest("Failed to delete the photo");
    }
  }
}