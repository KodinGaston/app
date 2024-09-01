using Microsoft.AspNetCore.Mvc;
using App.Jensen.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO; // Asegúrate de agregar esta línea para manejar archivos

namespace App.Jensen.Controllers
{
    public class AccountController : Controller
    {
        private readonly ContextUser _context;

        public AccountController(ContextUser context)
        {
            _context = context;
        }

        // Acción para mostrar el formulario de registro
        public IActionResult Register()
        {
            return View();
        }

        // Acción para manejar el envío del formulario de registro
        [HttpPost]
        public async Task<IActionResult> Register(User user, IFormFile profilePicture)
        {
            if (ModelState.IsValid)
            {
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    // Ruta de almacenamiento de imágenes en wwwroot/uploads
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    var filePath = Path.Combine(uploads, profilePicture.FileName);

                    // Crear el directorio de uploads si no existe
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    // Guardar el archivo en la ruta especificada
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(stream);
                    }

                    // Actualizar la URL de la foto de perfil del usuario
                    user.URLProfilPic = "/uploads/" + profilePicture.FileName;
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Agregar el mensaje de bienvenida a TempData
                TempData["SuccessMessage"] = "Welcome to your profile! You are logged in now! ♪♫";

                return RedirectToAction("Login"); // Redirige a la página de login después del registro
            }
            return View(user);
        }

        // Acción para mostrar el formulario de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Acción para manejar el envío del formulario de login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                // Redirigir a la página de perfil del usuario
                return RedirectToAction("Profile", new { userId = user.Id });
            }

            ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
            return View();
        }

        // Acción para mostrar el perfil del usuario
        public IActionResult Profile(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound("User not found."); // Devuelve un error 404 si el usuario no existe
            }
            return View(user);
        }

        // Acción para manejar la subida de una nueva imagen de perfil
        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(int userId, IFormFile profilePicture)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null && profilePicture != null && profilePicture.Length > 0)
            {
                // Ruta de almacenamiento de imágenes en wwwroot/uploads
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                var filePath = Path.Combine(uploads, profilePicture.FileName);

                // Crear el directorio de uploads si no existe
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                // Guardar el archivo en la ruta especificada
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                // Actualizar la URL de la foto de perfil del usuario
                user.URLProfilPic = "/uploads/" + profilePicture.FileName;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Profile picture updated successfully!";
                return RedirectToAction("Profile", new { userId }); // Redirige a la página de perfil después de la subida
            }

            TempData["ErrorMessage"] = "Failed to update profile picture.";
            return RedirectToAction("Profile", new { userId });
        }

        // Acción para cerrar sesión
        [HttpPost]
        public IActionResult Logout()
        {
            // Aquí podrías limpiar cualquier dato de sesión si estuvieras usando sesiones.
            // Para este ejemplo simple, solo redirigiremos al login.
            return RedirectToAction("Login");
        }
    }
}
