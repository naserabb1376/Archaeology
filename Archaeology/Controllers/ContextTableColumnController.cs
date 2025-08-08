using DataLayer;
using Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;

namespace Archaeology.Controllers
{
    [Route("ContextTableColumn")]
    public class ContextTableColumnController : Controller
    {
        private readonly ArchaeologyDbContext Context = null;

        public ContextTableColumnController(ArchaeologyDbContext _context)
        {
            this.Context = _context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Manage");
        }

        [HttpGet]
        [Route("Manage")]
        public IActionResult Manage(int tableId)
        {
            var table = Context.ContextTables
                .Include(t => t.Columns)
                .ThenInclude(c => c.ParentColumn) // برای گرفتن ParentTitle
                .FirstOrDefault(t => t.ID == tableId);

            if (table == null)
                return NotFound();

            var allColumns = table.Columns.ToList();

            var viewModel = new ManageTableColumnsViewModel
            {
                TableId = tableId,
                Columns = allColumns.Select(col => new ContextTableColumnViewModel
                {
                    Id = col.ID,
                    ColumnTitle = col.ColumnTitle,
                    Order = col.Order,
                    ParentColumnId = col.ParentColumnId,
                    ParentTitle = col.ParentColumn?.ColumnTitle
                }).ToList(),

                AddModel = new AddColumnModel
                {
                    TableId = tableId,
                    Order = (allColumns.Max(c => (int?)c.Order) ?? 0) + 1
                }
            };

            // ستون‌هایی که درخت والدشان حداکثر دو سطح دارد
            ViewBag.ColumnOptions = allColumns
                .Where(c =>
                {
                    int depth = 0;
                    var current = c;
                    while (current.ParentColumnId != null)
                    {
                        depth++;
                        current = allColumns.FirstOrDefault(x => x.ID == current.ParentColumnId);
                        if (depth > 2)
                            break;
                    }
                    return depth <= 2;
                })
                .Select(c => new SelectListItem
                {
                    Text = c.ColumnTitle,
                    Value = c.ID.ToString()
                }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(AddColumnModel model)
        {
            if (ModelState.IsValid)
            {
                var column = new ContextTableColumn
                {
                    ColumnTitle = model.ColumnTitle,
                    ContextTableId = model.TableId,
                    ParentColumnId = model.ParentColumnId,
                    Order = model.Order
                };

                Context.ContextTableColumns.Add(column);
                Context.SaveChanges();

                return RedirectToAction("Manage", new { tableId = model.TableId });
            }

            return RedirectToAction("Manage", new { tableId = model.TableId });
        }
    }
}