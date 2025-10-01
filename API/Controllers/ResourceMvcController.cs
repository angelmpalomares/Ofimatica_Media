using Application.Services.Interfaces;
using Infrastructure.Dtos;
using Infrastructure.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    public class ResourceMvcController(IResourceService resourceService) : Controller
    {
        private readonly IResourceService _resourceService = resourceService;

        [AllowAnonymous]
        public async Task<IActionResult> Index(ResourceFilterVm filters)
        {
            var result = await _resourceService.FilterResources(new GetResourceFiltersDto
            {
                Name = filters.Name,
                Author = filters.Author,
                ResourceType = filters.ResourceType,
                Year = filters.Year,
                PageNumber = filters.PageNumber,
                PageSize = filters.PageSize
            });
            return View(result);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View(new CreateResourceVm());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateResourceVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            try
            {
                await _resourceService.CreateResource(new CreateResourceDto
                {
                    Name = vm.Name,
                    Author = vm.Author,
                    Description = vm.Description,
                    Publication = vm.Publication,
                    ResourceType = vm.ResourceType
                });

                TempData["Success"] = "Recurso creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                var errorMessages = ex.Message.Split(", ");
                foreach (var error in errorMessages)
                {
                    var translatedError = error switch
                    {
                        "ErrorMessage.NameIsEmpty" => "El nombre es obligatorio",
                        "ErrorMessage.NameContainsScript" => "El nombre contiene caracteres no permitidos (< o >)",
                        "ErrorMessage.AuthorIsEmpty" => "El autor es obligatorio",
                        "ErrorMessage.DescriptionIsEmpty" => "La descripción es obligatoria",
                        "ErrorMessage.YearIsEmpty" => "El año de publicación es obligatorio",
                        "ErrorMessage.InvalidYear" => "El año de publicación no es válido",
                        _ => "Error de validación: " + error
                    };
                    ModelState.AddModelError(string.Empty, translatedError);
                }
                return View(vm);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el recurso. Por favor, inténtalo de nuevo.");
                return View(vm);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int resourceId)
        {
            try
            {
                var resource = await _resourceService.GetById(resourceId);

                var vm = new EditResourceVm
                {
                    ResourceId = resourceId,
                    Name = resource.Name,
                    Author = resource.Author,
                    Description = resource.Description,
                    Publication = resource.Publication,
                    ResourceType = resource.ResourceType
                };
                return View(vm);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int resourceId, EditResourceVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _resourceService.EditResource(resourceId, new EditResourceDto
                {
                    Name = vm.Name,
                    Author = vm.Author,
                    Description = vm.Description,
                    Publication = vm.Publication,
                    ResourceType = vm.ResourceType
                });

                TempData["Success"] = "Recurso actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                var errorMessages = ex.Message.Split(", ");
                foreach (var error in errorMessages)
                {
                    var translatedError = error switch
                    {
                        "ErrorMessage.NameIsEmpty" => "El nombre es obligatorio",
                        "ErrorMessage.NameContainsScript" => "El nombre contiene caracteres no permitidos (< o >)",
                        "ErrorMessage.AuthorIsEmpty" => "El autor es obligatorio",
                        "ErrorMessage.AuthorInvalidCharacters" => "El autor contiene caracteres no válidos",
                        "ErrorMessage.DescriptionIsEmpty" => "La descripción es obligatoria",
                        "ErrorMessage.DescriptionInvalidCharacters" => "La descripción contiene caracteres no válidos",
                        "ErrorMessage.YearIsEmpty" => "El año de publicación es obligatorio",
                        "ErrorMessage.InvalidYear" => "El año de publicación no puede ser futuro",
                        _ => "Error de validación: " + error
                    };
                    ModelState.AddModelError(string.Empty, translatedError);
                }
                return View(vm);
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Recurso no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al actualizar el recurso.");
                return View(vm);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int resourceId)
        {
            try
            {
                await _resourceService.DeleteResource(resourceId);
                TempData["Success"] = "Recurso eliminado.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
