using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using roadmap_migrant.Models;

namespace roadmap_migrant.Controllers;

public class AccountController : Controller
{
    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(string email, string password, string confirmPassword)
    {
        bool isValid = true;

        var existingUser = HttpContext.Session.Keys
            .FirstOrDefault(k => k.StartsWith("User_") && k.EndsWith(email));

        if (existingUser != null)
        {
            isValid = false;
            ModelState.AddModelError("EmailError", "Пользователь с такой почтой уже зарегистрирован");
        }

        if (string.IsNullOrEmpty(password) || password.Length < 8)
        {
            isValid = false;
            ModelState.AddModelError("PasswordError", "Минимум 8 символов");
        }

        if (password != confirmPassword)
        {
            isValid = false;
            ModelState.AddModelError("ConfirmPasswordError", "Пароли не совпадают");
        }

        if (!isValid)
        {
            ViewData["Email"] = email;
            return View();
        }

        var user = new UserModel
        {
            Email = email,
            Password = password,
            Survey = new SurveyModel()
        };

        HttpContext.Session.SetString($"User_{email}", JsonSerializer.Serialize(user));

        return RedirectToAction("Login");
    }

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var userJson = HttpContext.Session.GetString($"User_{email}");

        if (!string.IsNullOrEmpty(userJson))
        {
            var user = JsonSerializer.Deserialize<UserModel>(userJson);

            if (user != null && user.Password == password)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                
                if (user.Survey.Citizenship != null)
                {
                    return RedirectToAction("ViewRoadmap", "Survey");
                }

                return RedirectToAction("FillForm", "Survey");
            }

            ModelState.AddModelError("PasswordError", "Неверный пароль");
        }
        else
        {
            ModelState.AddModelError("EmailError", "Пользователь с такой почтой не зарегистрирован");
        }

        ViewData["Email"] = email;

        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}