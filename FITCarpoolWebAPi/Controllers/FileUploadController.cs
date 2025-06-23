using DataAccessLibrary.Data.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FITCarpoolWebAPi.Controllers
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

        // Adjusted to expect a userId query parameter
        [HttpPost("upload/profilepicture")]
        public async Task<IActionResult> UploadProfilePicture(List<IFormFile> files, [FromQuery] int userId)
        {
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(memoryStream);
                        byte[] fileBytes = memoryStream.ToArray();

                        // Now, use the userId along with the fileBytes to update the user's profile picture in the database
                        await UpdateUserProfilePictureAsync(userId, fileBytes);
                    }
                }
            }

            return Ok();
        }

        // This method needs to be implemented to update the user's profile picture in the database using userId and fileBytes
        private async Task UpdateUserProfilePictureAsync(int userId, byte[] fileBytes)
        {
            Console.WriteLine(userId + " - " + fileBytes.Length);
            await _usersData.UpdateUserProfilePicture(userId, fileBytes);
        }
    }
}
