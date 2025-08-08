using DataLayer;
using Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers
{
    [Route("ContextTableRow")]
    public class ContextTableRowController : Controller
    {
        private readonly ArchaeologyDbContext Context = null;

        public ContextTableRowController(ArchaeologyDbContext _context)
        {
            this.Context = _context;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            return RedirectToAction("Manage");
        }

        [HttpGet]
        [Route("Manage")]
        public IActionResult Manage(int tableId)
        {
            var columns = Context.ContextTableColumns
                .Where(c => c.ContextTableId == tableId)
                .ToList();

            // فقط ستون‌هایی که خودشون SubColumn ندارن (یعنی Leaf هستن)
            var leafColumns = columns
                .Where(c => !columns.Any(sub => sub.ParentColumnId == c.ID))
                .OrderBy(c => c.Order)
                .ToList();

            var viewModel = new TableRowEditModel
            {
                TableId = tableId,
                Cells = leafColumns.Select(c => new CellInputModel
                {
                    ColumnId = c.ID,
                    ColumnTitle = c.ColumnTitle,
                    FullPathTitles = GetFullPath(c, columns)
                }).ToList()
            };
            // بارگذاری ردیف‌ها و سلول‌های آن‌ها
            var rows = Context.ContextTableRows
                .Where(r => r.ContextTableId == tableId)
                .Include(r => r.Cells)
                .ToList();

            viewModel.ExistingRows = rows.Select(r => new TableRowEditModel.ExistingRowViewModel
            {
                RowId = r.ID,
                CellValues = leafColumns.Select(col =>
                    r.Cells.FirstOrDefault(c => c.ColumnId == col.ID)?.Value ?? "").ToList()
            }).ToList();

            return View(viewModel);
        }

        private List<string> GetFullPath(ContextTableColumn col, List<ContextTableColumn> all)
        {
            var path = new List<string>();
            var current = col;

            while (current != null)
            {
                path.Insert(0, current.ColumnTitle);
                current = all.FirstOrDefault(x => x.ID == current.ParentColumnId);
            }

            return path;
        }

        [HttpPost]
        [Route("Manage")]
        public IActionResult Manage(TableRowEditModel model)
        {
            if (model.Cells == null || !model.Cells.Any())
                return RedirectToAction("Manage", new { tableId = model.TableId });

            var newRow = new ContextTableRow
            {
                ContextTableId = model.TableId
            };

            Context.ContextTableRows.Add(newRow);
            Context.SaveChanges();

            foreach (var cell in model.Cells)
            {
                if (!string.IsNullOrWhiteSpace(cell.Value))
                {
                    Context.ContextTableCells.Add(new ContextTableCell
                    {
                        RowId = newRow.ID,
                        ColumnId = cell.ColumnId,
                        Value = cell.Value
                    });
                }
            }

            Context.SaveChanges();

            return RedirectToAction("Manage", new { tableId = model.TableId });
        }
    }
}