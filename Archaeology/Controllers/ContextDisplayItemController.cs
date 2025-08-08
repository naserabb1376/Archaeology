using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Archaeology.Controllers
{
    [Route("ContextDisplayItem")]
    public class ContextDisplayItemController : Controller
    {
        private readonly ArchaeologyDbContext Context = null;

        public ContextDisplayItemController(ArchaeologyDbContext _context)
        {
            this.Context = _context;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            return RedirectToAction("Manage");
        }

        [HttpGet]
        [Route("Manage")]
        public IActionResult Manage(int contextId)
        {
            var items = Context.ContextDisplayItems
                .Where(i => i.ContextId == contextId)
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new ContextDisplayItemOrderViewModel.DisplayItemEntry
                {
                    Id = i.ID,
                    ItemName = i.ItemName,
                    ItemType = i.ItemType,
                    DisplayOrder = i.DisplayOrder
                }).ToList();

            var viewModel = new ContextDisplayItemOrderViewModel
            {
                ContextId = contextId,
                Items = items
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("MoveUp")]
        public IActionResult MoveUp(int id, int contextId)
        {
            var item = Context.ContextDisplayItems.FirstOrDefault(x => x.ID == id && x.ContextId == contextId);
            if (item == null) return NotFound();

            var above = Context.ContextDisplayItems
                .Where(x => x.ContextId == contextId && x.DisplayOrder < item.DisplayOrder)
                .OrderByDescending(x => x.DisplayOrder)
                .FirstOrDefault();

            if (above != null)
            {
                int temp = item.DisplayOrder;
                item.DisplayOrder = above.DisplayOrder;
                above.DisplayOrder = temp;

                Context.UpdateRange(item, above);
                Context.SaveChanges();
            }

            return RedirectToAction("Manage", new { contextId });
        }

        [HttpPost]
        [Route("MoveDown")]
        public IActionResult MoveDown(int id, int contextId)
        {
            var item = Context.ContextDisplayItems.FirstOrDefault(x => x.ID == id && x.ContextId == contextId);
            if (item == null)
                return NotFound();

            // پیدا کردن آیتم بعدی در ترتیب
            var nextItem = Context.ContextDisplayItems
                .Where(x => x.ContextId == contextId && x.DisplayOrder > item.DisplayOrder)
                .OrderBy(x => x.DisplayOrder)
                .FirstOrDefault();

            if (nextItem != null)
            {
                int temp = item.DisplayOrder;
                item.DisplayOrder = nextItem.DisplayOrder;
                nextItem.DisplayOrder = temp;

                Context.UpdateRange(item, nextItem);
                Context.SaveChanges();
            }

            return RedirectToAction("Manage", new { contextId });
        }
    }
}