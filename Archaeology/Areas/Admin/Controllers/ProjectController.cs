using DataLayer;
using Domains;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Archaeology.Areas.Admin.Controllers
{
    [Route("Admin/Project")]
    [Area("Admin")]
    public class ProjectController : Controller
    {
        private readonly ArchaeologyDbContext Context = null;
        public ProjectController(ArchaeologyDbContext _context)
        {
            this.Context = _context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("ProjectList")]
        public IActionResult ProjectList(ProjectListModel model, int? paging, string pagingtype = "")
        {

            var search = "";
            if (model.TypeSearchName != null)
            {
                search = model.TypeSearchName;
            }

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

            var number = Context.Projects
                .Where(p => p.Title.Contains(search) || p.Description.Contains(search)).Count();
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

            var products = Context.Projects.Where(p => p.Title.Contains(search) || p.Description.Contains(search)).Skip(30 * pagenumberinsql).Take(30)

                .ToList();

            foreach (var VARIABLE in products)
            {
                model.Projectlist.Add(new Project()
                {
                    Title = VARIABLE.Title,

                    Description = VARIABLE.Description,

                    StartDate = VARIABLE.StartDate,

                    ID = VARIABLE.ID,
             
                });
            }
            return View(model);
        }

    }
}
