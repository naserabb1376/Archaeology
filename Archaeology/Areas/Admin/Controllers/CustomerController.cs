using DataLayer;
using Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AutoPartyadak.Areas.Admin.Controllers
{
    [Route("Admin/Customer")]
    [Area("Admin")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<UserRole> roleManager;
        private readonly ArchaeologyDbContext Context = null;

        public CustomerController(ArchaeologyDbContext _Context, UserManager<User> _userManager, RoleManager<UserRole> _roleManager)
        {
            this.Context = _Context;
            this.userManager = _userManager;
            this.roleManager = _roleManager;
        }

        public IActionResult Index()
        {
            return Redirect("/Admin/Customer/List");
        }

        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> ListAsync(CustomerRegisterModel model, int? paging, string pagingtype = "")
        {
            String Search = "";
            if (model.TypeSearchName != null)
            {
                Search = model.TypeSearchName;
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

            var users = userManager.Users.Where(p => p.FirstName.Contains(Search) || p.LastName.Contains(Search) ||
            p.PhoneNumber.Contains(Search) || p.UserName.Contains(Search) ||  p.Telephone.Contains(Search)).ToList();

            List<User> notguest = new List<User>();

            foreach (var itemgust in users)
            {     
                    modelroleguest.ListRoles = roleManager.Roles.ToList();

                    foreach (var items in modelroleguest.ListRoles)
                    {
                        if (await userManager.IsInRoleAsync(itemgust, items.Name).ConfigureAwait(false))
                        {
                            notguest.Add(itemgust);
                        }
                  }  
            }

            ///////////////////////////////////////
            int pagenumber = 0;
            if (paging != null && paging != 1)
            {
                pagenumber = (int)paging;
            }

            int pagenumberinsql = 0;
            if (pagenumber > 1)
            {
                pagenumberinsql = pagenumber - 1;
            }

            var number = notguest.Count();

            var page = number / 30;

            if (number % 30 != 0)
            {
                page = page + 1;
            }

            if (pagingtype != "")
            {
                if (pagingtype == "NEXT")
                {
                    if (pagenumber == page)
                    {
                        pagenumber = 0;
                        pagenumberinsql = 0;
                    }
                    else
                    {
                        if (pagenumber == 0 || pagenumber == 1)
                        {
                            if (page == 1)
                            {
                                pagenumber = 1;
                            }
                            else
                            {
                                pagenumber = 2;
                            }
                        }
                        else
                        {
                            pagenumber = pagenumber + 1;
                        }

                        pagenumberinsql = pagenumberinsql + 1;
                    }
                }
                else
                {
                    if (pagenumber == 1 || pagenumber == 0)
                    {
                        pagenumber = page;
                        pagenumberinsql = page - 1;
                    }
                    else
                    {
                        pagenumber = pagenumber - 1;
                        pagenumberinsql = pagenumberinsql - 1;
                    }
                }
            }

            for (int i = 1; i <= page; i++)
            {
                model.pagenumbers.Add(i);
            }

            if (pagenumber == 0)
            {
                model.page = 1;
            }
            else
            {
                model.page = pagenumber;
            }

            notguest = notguest.Skip(30 * pagenumberinsql).Take(30)
                .ToList();

            /////////////////////////////////

            var modelrole = new ManageCustomeRoleModel();
            string rolename = "";
            foreach (var item in notguest)
            {
                modelrole.ListRoles = roleManager.Roles.ToList();
                foreach (var items in modelrole.ListRoles)
                {
                    if (await userManager.IsInRoleAsync(item, items.Name))
                    {
                        rolename = items.Name;
                    }
                }

                model.customerlist.Add(new customerlist()
                {
                    Telephone = item.Telephone,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Id = item.Id,
                  
                    RoleName = rolename,

                    UserName = item.UserName
                });
            }
            return View(model);
        }

        [HttpGet]
        [Route("ManageRoles")]
        public async Task<IActionResult> ManageRoles(int id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            var model = new ManageCustomeRoleModel();

            model.fullName = user.FirstName + " " + user.LastName;
            model.Id = user.Id.ToString();

            model.ListRoles = roleManager.Roles.ToList();

            foreach (var item in model.ListRoles)
            {
                if (await userManager.IsInRoleAsync(user, item.Name))
                {
                    model.AssignedRoles.Add(item);
                }
            }

            return View(model);
        }

        [Route("SaveRole")]
        public async Task<IActionResult> SaveRole(string roleids, string userid)
        {
            if (roleids == null)
                return Ok();

            var listids = roleids.Split(";");
            var user = await userManager.FindByIdAsync(userid.ToString());
            var ListRoles = roleManager.Roles.ToList();
            foreach (var item in ListRoles)
            {
                if (listids.Contains(item.Id.ToString()))
                    await userManager.AddToRoleAsync(user, item.Name);
                else
                {
                    await userManager.RemoveFromRoleAsync(user, item.Name);
                }
            }
            return Ok();
        }
    }
}