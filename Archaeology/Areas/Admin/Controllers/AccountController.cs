using DataLayer;
using Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Security.Claims;

namespace Controllers
{
    [Route("Admin/Account")]
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly ArchaeologyDbContext Context = null;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<UserRole> roleManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, RoleManager<UserRole> roleManager
            , SignInManager<User> signInManager, ArchaeologyDbContext _context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.Context = _context;
        }

        [HttpGet]
        [Route("Register")]
        public async Task<IActionResult> RegisterAsync(int? id)
        {
            customerlist customerModel = new customerlist();
            if (id != null)
            {
                var customer = await Context.Users
                .Include(u => u.Address) // 👈 بارگذاری آدرس مرتبط
                 .FirstOrDefaultAsync(u => u.Id == id);

                customerModel.Id = customer.Id;
                var roleName = roleManager.Roles.ToList();

                foreach (var item in roleName)
                {
                    if (await userManager.IsInRoleAsync(customer, item.Name))
                    {
                        customerModel.RoleId = item.Id;
                    }
                }
                customerModel.CityName = customer.Address.CityName;
                customerModel.Telephone = customer.Telephone;
                customerModel.FirstName = customer.FirstName;
                customerModel.LastName = customer.LastName;
                customerModel.Street = customer.Address.Street;
                customerModel.PostalCode = customer.Address.PostalCode;
        
                customerModel.UserName = customer.UserName;

                customerModel.type = "edit";
            }

        


            ///////////////////////////////////////////////////////////
            string userrole = "";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int idd = int.Parse(userId);
            User selectcustomer = new User();
            selectcustomer = userManager.Users.FirstOrDefault(p => p.Id == idd);

            var modelroleguest = new ManageCustomeRoleModel();
            modelroleguest.ListRoles = roleManager.Roles.ToList();
            foreach (var items in modelroleguest.ListRoles)
            {
                if (await userManager.IsInRoleAsync(selectcustomer, items.Name))
                {
                    userrole = items.Name;
                }
            }

            /// ///////////////////////////////////

            var _Listrole = Context.Roles.ToList();
            foreach (var item in _Listrole)
            {
         

              
                        customerModel.RoleList.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                        {
                            Text = item.Name,
                            Value = item.Id.ToString()
                        });
                    
                
            }
            return View(customerModel);
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var customer = Context.Users.Find(id);
                var result = await userManager.DeleteAsync(customer);
                if (result.Succeeded)
                {
                    return RedirectToAction("List", "Customer");
                }
            }
            return RedirectToAction("List", "Customer");
        }

        [HttpPost]
        [CustomValidationFilter]
        [ValidateAntiForgeryToken]
        [Route("Register")]
        public async Task<IActionResult> Register(customerlist model)
        {
            Address newAddress = null;

            User customer = null;
            var roleName = Context.Roles.Find(model.RoleId);
            if (model.type == "edit")
            {
                customer = await userManager.Users
                .Include(x => x.Address)
                .FirstOrDefaultAsync(p => p.Id == model.Id);
            }
            else
            {
                customer = new User();
                newAddress = new Address();
            }

            customer.Telephone = model.Telephone;
            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.PasswordHash = model.Password;
            customer.UserName = model.UserName;

      
            customer.PhoneNumber = model.UserName;
            newAddress.CityName = model.CityName;
            newAddress.Street = model.Street;
            newAddress.PostalCode = model.PostalCode;
        

            if (model.type != "edit")
            {
                customer.Address = newAddress;

                var result = await userManager.CreateAsync(customer, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer, roleName.Name);
                    // await signInManager.SignInAsync(customer, isPersistent: false);
                    return RedirectToAction("List", "Customer");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }
            else
            {
                customer.Address.Street = newAddress.Street;
                customer.Address.PostalCode = newAddress.PostalCode;
                customer.Address.CityName = newAddress.CityName;
                var roles = userManager.GetRolesAsync(customer);
                foreach (var item in roles.Result)
                {
                    await userManager.RemoveFromRoleAsync(customer, item);
                }

                await userManager.AddToRoleAsync(customer, roleName.Name);
                Context.Addresses.Update(newAddress);
                await userManager.UpdateAsync(customer);

                if (model.Password != null)
                {
                    await userManager.RemovePasswordAsync(customer);

                    await userManager.AddPasswordAsync(customer, model.Password);
                }
                Context.Addresses.Update(customer.Address);
                Context.SaveChanges();
                return RedirectToAction("List", "Customer");
            }

            return View(model);
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            var model = new CustomerLoginModel();
            return View(model);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LogIn(CustomerLoginModel model)
        {
            var returnUrl = model.returnUrl ?? Url.Content("~/");

            var result = await signInManager.PasswordSignInAsync(model.UserName,
                model.Password, model.RememberMe, lockoutOnFailure: true);

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //int id = int.Parse(userId);

            if (result.Succeeded)
            {
                User selectcustomer = new User();

                selectcustomer = userManager.Users.Where(p => p.UserName == model.UserName).FirstOrDefault();

             
   

                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                return LocalRedirect("~/Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "کلمه کاربری یا رمز عبور اشتباست");
                return RedirectToAction("LogIn");
            }

            return View(model);
        }

        [HttpGet("/Account/Logout")]
        [Route("Logout")]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            await signInManager.SignOutAsync();
            return LocalRedirect(returnUrl);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckUserName(string UserName)
        {
            var isuser = await userManager.FindByNameAsync(UserName) == null;
            return Json(data: isuser);
        }
    }
}