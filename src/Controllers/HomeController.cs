using ToDo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using ToDo.Models.TaskViewModels;
using ToDo.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.IO;

namespace ToDo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        public HomeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string q = "")
        {
            var user = await _userManager.GetUserAsync(User);
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Account/Login");
            }
            return View(_mapper.Map<IEnumerable<Data.Task>, IEnumerable<TaskViewModel>>(await _context.Tasks.Where(item => item.UserId == user.Id && !item.IsCompleted && (string.IsNullOrEmpty(q) ? true : item.Name.ToLower().Contains(q))).ToListAsync()));
        }

        public async Task<IActionResult> Important(string q = "")
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_mapper.Map<IEnumerable<Data.Task>, IEnumerable<TaskViewModel>>(await _context.Tasks.Where(item => item.UserId == user.Id && item.IsImportant && !item.IsCompleted && (string.IsNullOrEmpty(q) ? true : item.Name.ToLower().Contains(q))).ToListAsync()));
        }

        public async Task<IActionResult> Planned(string q = "")
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_mapper.Map<IEnumerable<Data.Task>, IEnumerable<TaskViewModel>>(await _context.Tasks.Where(item => item.UserId == user.Id && item.DueDate != null && !item.IsCompleted && (string.IsNullOrEmpty(q) ? true : item.Name.ToLower().Contains(q))).ToListAsync()));
        }

        public async Task<IActionResult> Completed(string q = "")
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_mapper.Map<IEnumerable<Data.Task>, IEnumerable<TaskViewModel>>(await _context.Tasks.Where(item => item.UserId == user.Id && item.IsCompleted && (string.IsNullOrEmpty(q) ? true : item.Name.ToLower().Contains(q))).ToListAsync()));
        }

        public async Task<IActionResult> Create(string returnUrl = null)
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_mapper.Map<TaskViewModel>(_context.Tasks.First(item => item.UserId == user.Id && item.Id == id)));
        }

        [HttpPatch]
        public async Task<IActionResult> Complete(int id, string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var task = _context.Tasks.First(item => item.UserId == user.Id && item.Id == id);
            task.IsCompleted = !task.IsCompleted;
            _context.Update(task);
            await _context.SaveChangesAsync();
            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskViewModel model, string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var task =  _context.Tasks.First(item => item.UserId == user.Id && item.Id == model.Id);
            _mapper.Map(model, task);
            task.UserId = user.Id;
            _context.Update(task);
            _context.SaveChanges();
            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskViewModel model, string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.UserId = user.Id;
            _context.Tasks.Add(_mapper.Map<Data.Task>(model));
            await _context.SaveChangesAsync();
            return RedirectToLocal(returnUrl);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id, string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            _context.Remove(_context.Tasks.First(item => item.UserId == user.Id && item.Id == id));
            _context.SaveChanges();
            return RedirectToLocal(returnUrl);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
