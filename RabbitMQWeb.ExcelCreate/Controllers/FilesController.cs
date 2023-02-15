using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RabbitMQWeb.ExcelCreate.Hubs;
using RabbitMQWeb.ExcelCreate.Models;

namespace RabbitMQWeb.ExcelCreate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {


        private readonly IHubContext<MyHub> _hubContext;
        private readonly AppDbContext _context;
        public FilesController(AppDbContext context, IHubContext<MyHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Upload(IFormFile file, int fileId)
        {
            if (file is not { Length: > 0 }) return BadRequest();

            var userFile = await _context.UserFiles.FirstOrDefaultAsync(x => x.Id == fileId);

            var filePath = file.FileName + Path.GetExtension(file.FileName);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filePath);

            await using FileStream stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatusType = FileStatusType.Completed;
            await _context.SaveChangesAsync();

            string jsonUserFile = JsonSerializer.Serialize(userFile);
            await _hubContext.Clients.User(userFile.UserId).SendAsync("CompletedFile", jsonUserFile);

            return Ok();
        }
    }
}
