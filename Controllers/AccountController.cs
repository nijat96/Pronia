using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.ViewModel.Autho;
using Pronia.ViewModel.Basket;
using System.Text.Json;


namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            var user = new AppUser
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                UserName = registerVM.Username,
                Email = registerVM.Email
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string link = Url.Action(nameof(ConfirmedEmail), "Account" , new {userId=user.Id, token }, Request.Scheme, Request.Host.ToString());

            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("nijatnabiyev96@gmail.com"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Test Email Subject";
            string body = string.Empty;

            using StreamReader reader = new StreamReader("wwwroot/template/verify-email.html");

            body= await reader.ReadToEndAsync();

            body= body.Replace("{{link}}", link);
            body=body.Replace("{{user_name}}", user.Name);
            body = body.Replace("{{app_name}}", "Pronia");
            body= body.Replace("{{year}}", DateTime.Now.Year.ToString());


            email.Body = new TextPart(TextFormat.Html) { Text = body };
            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("nijatnabiyev96@gmail.com","sham zgkt emqd lmng");
            smtp.Send(email);
            smtp.Disconnect(true);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }

            return RedirectToAction(nameof(VerifyEmail));
        }

        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginVM.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View(loginVM);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account is locked out. Please try again later.");
                return View(loginVM);
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or password is incorrect");
                return View(loginVM);
            }



            string? cookie = Request.Cookies["basket"];

            if (!string.IsNullOrEmpty(cookie))
            {
                List<BasketCookieItemVM>? cookieBasket = JsonSerializer.Deserialize<List<BasketCookieItemVM>>(cookie);
                if (cookieBasket != null && cookieBasket.Count > 0)
                {
                    var userDbBasket = await _context.BasketItems
                        .Where(b => b.AppUserId == user.Id)
                        .ToListAsync();

                    foreach(var cookieItem in cookieBasket)
                    {
                        var existDbItem = userDbBasket.FirstOrDefault(b => b.ProductId == cookieItem.Id);

                        if (existDbItem != null)
                        {
                            existDbItem.Count += cookieItem.Count;
                        }
                        else
                        {
                            _context.BasketItems.Add(new BasketItem
                            {
                                AppUserId = user.Id,
                                ProductId = cookieItem.Id,
                                Count = cookieItem.Count
                            });
                            
                        }
                    }

                    await _context.SaveChangesAsync();
                    Response.Cookies.Delete("basket");
                }
            }
            if (ReturnUrl is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(ReturnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmedEmail(string userId, string token)
        {
            if (userId is null || token is null) return BadRequest();
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Login));
        }
    }
}


