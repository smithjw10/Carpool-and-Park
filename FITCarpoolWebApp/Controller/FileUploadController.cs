using DataAccessLibrary.Data.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FITCarpoolWebApp.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IUsersData _usersData;

        public FileUploadController(IUsersData usersData)
        {
            _usersData = usersData;
        }

        [HttpPost("upload/{userId}")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file, [FromRoute] int userId)
        {

            if (file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();

                    await UpdateUserProfilePictureAsync(userId, fileBytes);
                }
            }

            return Ok();
        }

        private async Task UpdateUserProfilePictureAsync(int userId, byte[] fileBytes)
        {
            await _usersData.UpdateUserProfilePicture(userId, fileBytes);
        }


        [HttpPost("upload/license/{userId}")]
        public async Task<IActionResult> UploadLicensePicture(IFormFile file, [FromRoute] int userId)
        {
            if (file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();

                    await UpdateUserLicensePictureAsync(userId, fileBytes);
                }
            }

            return Ok();
        }
        private async Task UpdateUserLicensePictureAsync(int userId, byte[] fileBytes)
        {
            await _usersData.UpdateUserLicensePicture(userId, fileBytes);
        }



        [HttpPost("upload/car/{userId}")]
        public async Task<IActionResult> UploadCarPicture(IFormFile file, [FromRoute] int userId)
        {

            if (file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();

                    await UpdateUserCarPictureAsync(userId, fileBytes);
                }
            }

            return Ok();
        }

        private async Task UpdateUserCarPictureAsync(int userId, byte[] fileBytes)
        {
            await _usersData.UpdateUserCarPicture(userId, fileBytes);
        }

        
    }
}
