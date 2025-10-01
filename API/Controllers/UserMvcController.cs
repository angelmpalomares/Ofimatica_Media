using Application.Services.Interfaces;
using Infrastructure.Dtos;
using Infrastructure.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    public class UserMvcController(IUserService userService) : Controller
    {
        private readonly IUserService _userService = userService;

        [AllowAnonymous]
        public IActionResult Register()
        {
            TempData.Remove("Success");
            TempData.Remove("Error");
            return View(new RegisterUserVm());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            
            try
            {
                await _userService.RegisterUser(new RegisterUserDto
                {
                    Username = vm.Username,
                    Password = vm.Password,
                    Email = vm.Email,
                    Name = vm.Name,
                    Surname = vm.Surname
                });

                TempData["Success"] = "Usuario registrado.";
                return RedirectToAction("Index", "Home");
            }
            catch (ValidationException ex)
            {
                var errorMessages = ex.Message.Split(", ");
                foreach (var error in errorMessages)
                {
                    var translatedError = error switch
                    {
                        "ErrorMessage.EmailIsEmpty" => "El email es obligatorio",
                        "ErrorMessage.WrongEmailFormat" => "El formato del email no es válido",
                        "ErrorMessage.PasswordIsEmpty" => "La contraseña es obligatoria",
                        "ErrorMessage.PasswordDoesntMeetRequirements" =>
                            "La contraseña debe tener al menos 12 caracteres, incluyendo mayúsculas, minúsculas, números y caracteres especiales",
                        _ => error
                    };
                    ModelState.AddModelError(string.Empty, translatedError);
                }
                return View(vm);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al registrar el usuario. Por favor, inténtalo de nuevo.");
                return View(vm);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(UserFilterVm filterVm)
        {
            var users = await _userService.FilterUsers(new GetUserFiltersDto
            {
                Name = filterVm.Name,
                Username = filterVm.Username,
                IsActive = filterVm.IsActive,
                PageNumber = filterVm.PageNumber,
                PageSize = filterVm.PageSize
            });
            ViewData["PageNumber"] = filterVm.PageNumber;
            ViewData["PageSize"] = filterVm.PageSize;
            ViewData["Total"] = users.Total;
            return View(users.Items);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            TempData.Remove("Success");
            TempData.Remove("Error");
            return View(new LoginVm());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            var dto = new LoginDto { Username = vm.Username, Password = vm.Password };

            try
            {
                var token = await _userService.Login(dto);

                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return RedirectToAction("Index", "ResourceMvc");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                await _userService.ActivateUser(id);
                TempData["Success"] = "Usuario activado.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                await _userService.DeactivateUser(id);
                TempData["Success"] = "Usuario desactivado.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Login", "UserMvc");
        }
    }
}
