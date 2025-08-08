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
            return RedirectToAction("ProjectList");
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

            var Projects = Context.Projects.Where(p => p.Title.Contains(search) || p.Description.Contains(search)).Skip(30 * pagenumberinsql).Take(30)

                .ToList();

            foreach (var VARIABLE in Projects)
            {
                model.Projectlist.Add(new ProjectModel()
                {
                    Title = VARIABLE.Title,

                    Description = VARIABLE.Description,

                    StartDate = VARIABLE.StartDate.ToPersian(),

                    ID = VARIABLE.ID,
                });
            }
            return View(model);
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create(int? id)
        {
            ProjectModel model = new ProjectModel();

            if (id != null)
            {
                var Project = Context.Projects.Find(id);

                model.Title = Project.Title;
                model.Description = Project.Description;
                model.StartDate = Project.StartDate.ToPersian();

                model.ID = Project.ID;
            }

            return View(model);
        }

        [Route("Create")]
        [HttpPost]
        public IActionResult Create(ProjectModel model)
        {
            if (model != null)
            {
                Project newProject = new Project();

                if (model.ID != 0)
                {
                    newProject = Context.Projects.Find(model.ID);
                }

                newProject.Title = model.Title;
                newProject.Description = model.Description;
                newProject.StartDate = DateTime.Now;

                Context.Projects.Update(newProject);
                Context.SaveChanges();
            }

            return RedirectToAction("ProjectList");
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var Project = Context.Projects.Find(id);
                Context.Projects.Remove(Project);
                Context.SaveChanges();
            }
            return RedirectToAction("ProjectList", "Project");
        }
    }
}