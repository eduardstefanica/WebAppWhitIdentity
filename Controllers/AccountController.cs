using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp8WhitIdentity.Models;
using WebAppWithIdentity.ViewModels;

namespace WebAppWithIdentity.Controllers
{
    [EnableCors(PolicyName = "enablecorsfromreact")]
    public class AccountController : Controller
    {
        // For Login - Logout
        private readonly SignInManager<AppUser> signInManager;

        // For Register
        private readonly UserManager<AppUser> userManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        //GET
        public IActionResult Login() //visualizza la pagina di login ,
        {
            return View(); //restituisce una vista "View" associata al nome Login
        }
        [HttpPost]// rappresenta la richiesta POST per elaborare la richiesta di login quando l'utente invia il modulo di login.
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)//Verifica se il modello è valido utilizzando ModelState.IsValid.
            {
                //Login 
                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);//Se il modello è valido, tenta di eseguire il login utilizzando signInManager.PasswordSignInAsync.
                if (result.Succeeded)//Se il login è riuscito (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");// reindirizza l'utente alla pagina "Index" del controller "Home".
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");//Se il login non è riuscito, aggiunge un errore al modello (ModelState.AddModelError) 
                return View(model);//restituisce la vista di login con il modello (LoginVM) per consentire all'utente di correggere le credenziali.
            }
            ModelState.AddModelError(string.Empty, "ModelState is not Invalid");
            return View(model);
        }
        [HttpPost]
        public async Task<bool> LoginFromReact([FromForm] LoginVM model)
        {
            //Login
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
            return result.Succeeded;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Username,
                    Address = model.Address
                };
                // Register
                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // N.B. Remember to update the ConfirmedMail in the AppUser class (Models/AppUser.cs) IdentityUser
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            ModelState.AddModelError(string.Empty, "ModelState is not valid");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<bool> LogoutFromReact()
        {
            try
            {
                await signInManager.SignOutAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
