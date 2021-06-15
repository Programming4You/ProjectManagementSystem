using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using ProjectManagementSystem.Data;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories.Interfaces;
using ProjectManagementSystem.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ITaskRepository _taskRepository;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
               ILogger<AccountController> logger, ApplicationDbContext context, IMapper mapper, ITaskRepository taskRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _taskRepository = taskRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            AddRolesToRegisterViewModel();
            var vm = new RegisterViewModel();

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser user = _mapper.Map<ApplicationUser>(model);
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);

                    _logger.LogInformation("User created a new account.");
                    return RedirectToAction("Users");
                }
                AddErrors(result);
            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.IsPersistent = false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    return RedirectToAction("Projects", "Project");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login");
        }


        public ActionResult Users()
        {
            var users = _userManager.Users.ToList();

            return View(users);
        }

        public ActionResult Details(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            return View(user);
        }



        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            EditViewModel editViewModel = _mapper.Map<EditViewModel>(user);
            AddRolesToEditViewModel();

            return View(editViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Edit(EditViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.FindByIdAsync(userVM.Id).Result;
                user.Id = userVM.Id;
                user.UserName = userVM.UserName;
                user.Name = userVM.Name;
                user.Surname = userVM.Surname;
                user.RoleName = userVM.RoleName;

                await _userManager.UpdateAsync(user);

                var oldRoleId = _context.UserRoles.FirstOrDefault(r => r.UserId == userVM.Id).RoleId;
                var oldRole = _context.Roles.FirstOrDefault(r => r.Id == oldRoleId).Name;

                await _userManager.RemoveFromRoleAsync(user, oldRole);
                await _userManager.AddToRoleAsync(user, userVM.RoleName);

                return RedirectToAction("Users");
            }
            else
            {
                return View(userVM);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(ApplicationUser user)
        {

            var usr = _userManager.FindByIdAsync(user.Id).Result;
            var oldRoleId = _context.UserRoles.FirstOrDefault(r => r.UserId == usr.Id).RoleId;
            var oldRole = _context.Roles.FirstOrDefault(r => r.Id == oldRoleId).Name;
            _taskRepository.UnAssignTasks(user.Id);
            _userManager.DeleteAsync(usr);
            _userManager.RemoveFromRoleAsync(user, oldRole);
            return RedirectToAction("Users");
        }



        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


        public void AddRolesToRegisterViewModel()
        {
            var allRoles = _context.Roles.ToList();

            RegisterViewModel.Roles = allRoles.Select(s => new SelectListItem
            {
                Text = s.Name.ToString(),
                Value = s.Name
            });
        }

        public void AddRolesToEditViewModel()
        {
            var allRoles = _context.Roles.ToList();

            EditViewModel.Roles = allRoles.Select(s => new SelectListItem
            {
                Text = s.Name.ToString(),
                Value = s.Name
            });
        }

    }
}
