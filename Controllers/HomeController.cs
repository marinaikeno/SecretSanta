using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // use Sessions
using Microsoft.EntityFrameworkCore; // use Entity
using Microsoft.AspNetCore.Identity;
using SecretSanta.Models;

namespace SecretSanta.Controllers
{
    public class HomeController : Controller
    {
        private SSContext _context { get; set; }
        public HomeController(SSContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitSignup(RegisterViewModel model)
        {
            User CheckUser = _context.Users.SingleOrDefault(user => user.Email == model.Email);
            if (CheckUser == null && ModelState.IsValid)
            {
                // Create new user
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                User NewUser = new User();
                NewUser.FirstName = model.FirstName;
                NewUser.LastName = model.LastName;
                NewUser.Email = model.Email;
                NewUser.Password = Hasher.HashPassword(NewUser, model.Password);
                _context.Users.Add(NewUser);
                _context.SaveChanges();
                // Grab User to sign in 
                User signedUser = _context.Users.SingleOrDefault(user => user.Email == model.Email);
                HttpContext.Session.SetInt32("UserId", signedUser.UserId);
                TempData["Success"] = $"You have succesfully signed up!";
                return RedirectToAction("Index", "Dashboard");
            }
            else if (CheckUser != null)
            {
                ViewBag.Invalid = "Email is already registered";
            }
            return View("Signup");
        }
        public IActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitSignin(string email, string password)
        {
            if (email != null && password != null)
            {
                User CheckUser = _context.Users.SingleOrDefault(user => user.Email == email);
                if (CheckUser != null)
                {
                    var Hasher = new PasswordHasher<User>();
                    if (0 != Hasher.VerifyHashedPassword(CheckUser, CheckUser.Password, password))
                    {
                        HttpContext.Session.SetInt32("UserId", CheckUser.UserId);
                        // if password matched
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
            }
            ViewBag.Invalid = "Email and/or password is invalid";
            return View("Signin");
        }
        public IActionResult Signout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
