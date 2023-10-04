using EmployeeApiConsumer.Models;
using EmployeeConsumer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;

namespace EmployeeApiConsumer.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;
        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<IActionResult> GetConnectedUsers()
        {
            var connectedUsers = ChatHub.connectedUsers;
            string userListJson = HttpContext.Session.GetString("UserList")!;
            List<User> userList = new List<User>();
            if (!string.IsNullOrEmpty(userListJson))
            {
                userList = JsonConvert.DeserializeObject<List<User>>(userListJson)!;
            }
            else
            {
                var allUser = await _chatService.GetUsersAsync();
                string userListAll = JsonConvert.SerializeObject(allUser);
                HttpContext.Session.SetString("UserList", userListAll);
            }
            var users = new List<UserStatus>();

            foreach (var user in userList)
            {
                bool isConnected = connectedUsers.ContainsValue(user.UserName);
                string connectionId = connectedUsers.FirstOrDefault(x => x.Value == user.UserName).Key;
                users.Add(new UserStatus { UserName = user.UserName, ConnectionId = connectionId, IsConnected = isConnected });
            }

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    // Generate a unique file name or use the original file name
                    string fileName = file.FileName;/*+ Path.GetExtension(file.FileName);*/
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName.Trim());

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Append))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return Json("<a href='/Chat/DownloadFile?fileName=" + fileName + "'>" + fileName + "</a>");
                }
                catch (Exception ex)
                {
                    return Json("Error uploading file: " + ex.Message);
                }
            }

            return Json("No file selected");
        }

        public IActionResult DownloadFile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                // Get the file path based on the provided fileName
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    // Read the file contents
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                    // Determine the content type based on the file extension
                    string contentType = GetContentType(fileName);

                    // Provide the file for download
                    return File(fileBytes, contentType, fileName);
                }
            }

            return Json("No file selected");
        }

        private string GetContentType(string fileName)
        {
            // Map file extensions to content types
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out string? contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        public IActionResult Notification()
        {
            return View();
        } 
    }
}