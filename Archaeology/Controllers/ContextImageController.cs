using DataLayer;
using Domains;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Archaeology.Controllers
{
    [Route("ContextImage")]
    public class ContextImageController : Controller
    {
        private readonly ArchaeologyDbContext Context = null;

        public ContextImageController(ArchaeologyDbContext _context)
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
        public IActionResult List(ContextImageListModel model, int? paging, int? ContextID, string pagingtype = "")
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

            var number = Context.ContextImages
                .Where(p => (p.Caption.Contains(search)) && (p.ContextId == ContextID || ContextID == 0)).Count();
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

            var Contexts = Context.ContextImages.Where(p => (p.Caption.Contains(search)) && (p.ContextId == ContextID || ContextID == 0)).Skip(30 * pagenumberinsql).Take(30)
                .ToList();
            foreach (var VARIABLE in Contexts)
            {
                model.ContextImageLists.Add(new ContextImageList()
                {
                    Caption = VARIABLE.Caption,
                    ContextId = VARIABLE.ContextId,
                    ImagePath = VARIABLE.ImagePath,

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
            ContextImageModel model = new ContextImageModel();

            {
                model.ContextId = ContextID;
            };

            return View(model);
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create(ContextImageModel model)
        {
            if (model.ImageFiles != null && model.ImageFiles.Count > 0)
            {
                int order = (Context.ContextDisplayItems
                    .Where(p => p.ContextId == model.ContextId)
                    .Max(p => (int?)p.DisplayOrder) ?? 0);

                for (int i = 0; i < model.ImageFiles.Count; i++)
                {
                    var file = model.ImageFiles[i];
                    var caption = model.Captions != null && i < model.Captions.Count ? model.Captions[i] : "";

                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        file.CopyTo(stream);

                    var newImage = new ContextImage
                    {
                        Caption = caption,
                        ContextId = model.ContextId,
                        ImagePath = "/Images/" + fileName
                    };

                    Context.ContextImages.Add(newImage);
                    Context.SaveChanges();

                    var newDisplay = new ContextDisplayItem
                    {
                        ContextId = model.ContextId,
                        ItemId = newImage.ID,
                        ItemType = DisplayItemType.Image,
                        ItemName = caption,
                        DisplayOrder = ++order
                    };

                    Context.ContextDisplayItems.Add(newDisplay);
                    Context.SaveChanges();
                }

                return RedirectToAction("List", "ContextImage", new { ContextID = model.ContextId });
            }

            ModelState.AddModelError("", "لطفاً تصویر انتخاب کنید.");
            return View(model);
        }

        [HttpGet]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var image = Context.ContextImages.FirstOrDefault(x => x.ID == id);
            if (image == null)
            {
                return NotFound();
            }

            // حذف فایل فیزیکی (اگر وجود داشته باشه)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImagePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // حذف رکورد از ContextImage
            Context.ContextImages.Remove(image);

            // حذف رکورد مرتبط از ContextDisplayItem
            var displayItem = Context.ContextDisplayItems.FirstOrDefault(d =>
                d.ItemId == id &&
                d.ItemType == DisplayItemType.Image &&
                d.ContextId == image.ContextId);

            if (displayItem != null)
            {
                Context.ContextDisplayItems.Remove(displayItem);
            }

            Context.SaveChanges();

            // برگشت به لیست تصاویر مربوط به همین کانتکست
            return RedirectToAction("List", "ContextImage", new { ContextID = image.ContextId });
        }
    }
}