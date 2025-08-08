using DataLayer;
using Domains;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Archaeology.Controllers
{
    [Route("ContextTable")]
    public class ContextTableController : Controller
    {
        private readonly ArchaeologyDbContext Context = null;

        public ContextTableController(ArchaeologyDbContext _context)
        {
            this.Context = _context;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List(ContextTableListModel model, int? paging, int? ContextID, string pagingtype = "")
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

            var number = Context.ContextTables
                .Where(p => (p.TableTitle.Contains(search)) && (p.ContextId == ContextID || ContextID == 0)).Count();
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

            var Contexts = Context.ContextTables.Where(p => (p.TableTitle.Contains(search)) && (p.ContextId == ContextID || ContextID == 0)).Skip(30 * pagenumberinsql).Take(30)
                .ToList();
            foreach (var VARIABLE in Contexts)
            {
                model.ContextTableLists.Add(new ContextTableList()
                {
                    TableTitle = VARIABLE.TableTitle,
                    ContextId = VARIABLE.ContextId,

                    ID = VARIABLE.ID,
                });
            }

            model.ContextID = ContextID.Value;
            return View(model);
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create(int? id, int ContextID)
        {
            ContextTableModel model = new ContextTableModel();
            model.ContextId = ContextID;
            if (id != null)
            {
                var ContextParagraphs = Context.ContextTables.Find(id);

                model.TableTitle = ContextParagraphs.TableTitle;

                model.ContextId = ContextParagraphs.ContextId;

                model.ID = ContextParagraphs.ID;
            }

            return View(model);
        }

        [Route("Create")]
        [HttpPost]
        public IActionResult Create(ContextTableModel model)
        {
            if (model != null)
            {
                Domains.ContextTable newContextTableModel = new Domains.ContextTable();

                if (model.ID != 0)
                {
                    newContextTableModel = Context.ContextTables.Find(model.ID);
                }

                newContextTableModel.TableTitle = model.TableTitle;

                newContextTableModel.ContextId = model.ContextId;

                Context.ContextTables.Update(newContextTableModel);
                Context.SaveChanges();

                ContextDisplayItem newitem = new ContextDisplayItem();
                var item = Context.ContextDisplayItems.Where(p => p.ContextId == model.ContextId && p.ItemId == model.ID && p.ItemType == DisplayItemType.Table).FirstOrDefault();
                if (item != null)
                {
                    newitem = item;
                }
                else
                {
                    var newitemlist = Context.ContextDisplayItems.Where(p => p.ContextId == model.ContextId).ToList();

                    if (newitemlist.Count == 0)
                    {
                        newitem.DisplayOrder = 1;
                    }
                    else
                    {
                        int maxOrder = newitemlist.Max(x => x.DisplayOrder);
                        newitem.DisplayOrder = maxOrder + 1;
                    }
                }

                newitem.ItemId = newContextTableModel.ID;
                newitem.ItemName = model.TableTitle;
                newitem.ContextId = model.ContextId;
                newitem.ItemType = DisplayItemType.Table;
                Context.ContextDisplayItems.Update(newitem);
                Context.SaveChanges();
            }

            return RedirectToAction("List", "ContextTable", new { ContextID = model.ContextId });
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            int Contextid = 0;
            if (id != null)
            {
                var ContextParagraph = Context.ContextTables.Find(id);
                Contextid = ContextParagraph.ContextId;
                Context.ContextTables.Remove(ContextParagraph);

                // حذف رکورد مرتبط از ContextDisplayItem
                var displayItem = Context.ContextDisplayItems.FirstOrDefault(d =>
                    d.ItemId == id &&
                    d.ItemType == DisplayItemType.Table &&
                    d.ContextId == ContextParagraph.ContextId);

                if (displayItem != null)
                {
                    Context.ContextDisplayItems.Remove(displayItem);
                }
                Context.SaveChanges();
            }
            return RedirectToAction("List", "ContextTable", new { ContextID = Contextid });
        }
    }
}