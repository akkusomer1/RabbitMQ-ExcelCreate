using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQWeb.ExcelCreate.Models;
using RabbitMQWeb.ExcelCreate.Service;
using SharedLibrary;

namespace RabbitMQWeb.ExcelCreate.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private  readonly RabbitMQPublisher _rabbitMQPublisher;
        public ProductController(AppDbContext appDbContext, UserManager<IdentityUser> userManager, RabbitMQPublisher rabbitMqPublisher)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _rabbitMQPublisher = rabbitMqPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            string fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";

            UserFile file = new UserFile
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatusType = FileStatusType.Creating,
            };

            await _appDbContext.AddAsync(file);
           await _appDbContext.SaveChangesAsync();
            TempData["StartCreatingExcel"] = true;


            _rabbitMQPublisher.Publish(new CreateExcelMessage
            {
                FileId = file.Id
            });

           
            return RedirectToAction("Files");
        }


        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var files =await _appDbContext.UserFiles.Where(x => x.UserId == user.Id).ToListAsync();
            return View(files);
        }
    }
}
