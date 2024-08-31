using Microsoft.AspNetCore.Mvc;
using EZ_Site.Data;
using EZ_Site.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace EZ_Site.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        
        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            List<User> objUserList = _db.Users.ToList();
            return View(objUserList);
        }

        public IActionResult Register()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]


        public IActionResult Users()
        {
            var users = _db.Users.ToList();
            return View(users);
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {            
            if(user.Login.ToLower() == "admin")
            {
                user.Role = "Admin";
            }
            else
            {
                user.Role = "User";
            }
            _db.Add(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");  
            
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (user.Login.ToLower() == "admin")
            { 
                user.Role = "Admin";
            }
            else
            { 
                user.Role = "User";
            }            
            _db.Add(user);
            await _db.SaveChangesAsync();
            TempData["success"] = "Пользователь добавлени успешно.";
            return RedirectToAction("Login");            
        }
        
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            User? userFromDb = _db.Users.Find(id);
            if(userFromDb == null)
            {
                return NotFound();
            }
            return View(userFromDb);
            
        }
        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                _db.Users.Update(user);
                _db.SaveChanges();
                TempData["success"] = "Юзер изменен успешно";
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            User? userFromDb = _db.Users.Find(id);
            if (userFromDb == null)
            {
                return NotFound();
            }
            return View(userFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            User? userFromDb = _db.Users.Find(id);
            if(userFromDb == null)
            {
                return NotFound();
            }
            _db.Users.Remove(userFromDb);
            _db.SaveChanges();
            TempData["success"] = "Юзер успешно удален";
            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string login, string password) 
        {
            User user = _db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            //проверка
            if (user == null)
            {
                Console.WriteLine($"Прользователя нет: login={login}, password={password}");
            }
            else
            {
                Console.WriteLine($"Пользователь вошёл: {user.Login}");
            }
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (user != null)
            {
                //добавление куки пользователя
                var conf = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(conf, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProtects = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProtects);
                if(user.Role == "Admin")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Game");
                }
            }
            ModelState.AddModelError("", "Неверный логин или пароль");
            return View();
        }

        

        

        

        public IActionResult Game()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            var login = User.Identity.Name;
            ViewBag.Login = login;
            return View();
        }
        public async Task<IActionResult> Exit()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
    }
    }

    
}
