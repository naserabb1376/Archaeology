using Archaeology.Models;
using Aspose.Words;
using Aspose.Words.Tables;
using DataLayer;
using Domains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Archaeology.Controllers
{
    [Route("Context")]
    public class ContextController : Controller
    {
        private readonly ArchaeologyDbContext Context = null;
        private readonly IWebHostEnvironment _env;

        public ContextController(ArchaeologyDbContext _context, IWebHostEnvironment env)
        {
            this.Context = _context;
            _env = env;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpGet]
        [Route("List")]
        public IActionResult List(ContextListModel model, int? paging, string pagingtype = "")
        {
            var projects = Context.Projects.ToList();
            int projectid = 0;

            if (model.ProjectNameID == 0)
            {
                projectid = projects.Select(p => p.ID).LastOrDefault();
            }
            else
            {
                projectid = model.ProjectNameID;
            }
            foreach (var pitem in projects)
            {
                model.ProjectNames.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                {
                    Text = pitem.Title,
                    Value = pitem.ID.ToString()
                });
            }

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

            var number = Context.Contexts
                .Where(p => (p.Title.Contains(search) || p.Description.Contains(search)) && (p.ProjectId == projectid || projectid == 0)).Count();
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

            var Contexts = Context.Contexts.Where(p => (p.Title.Contains(search) || p.Description.Contains(search)) && (p.ProjectId == projectid || projectid == 0)).Skip(30 * pagenumberinsql).Take(30)
                .ToList();
            foreach (var VARIABLE in Contexts)
            {
                model.ContextLists.Add(new ContextModel()
                {
                    Title = VARIABLE.Title,
                    ProjectId = VARIABLE.ProjectId,
                    Description = VARIABLE.Description,

                    ID = VARIABLE.ID,
                });
            }

            model.ProjectNameID = projectid;
            return View(model);
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create(int? id)
        {
            ContextCreatModel model = new ContextCreatModel();

            if (id != null)
            {
                var context = Context.Contexts.Find(id);

                model.Title = context.Title;
                model.Description = context.Description;
                model.ProjectId = context.ProjectId;

                model.ID = context.ID;
            }

            var projects = Context.Projects.ToList();

            //model.Project.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
            //{
            //    Text = "موردی را انتخاب کنید",
            //    Value = "0"
            //});
            foreach (var pitem in projects)
            {
                model.Project.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                {
                    Text = pitem.Title,
                    Value = pitem.ID.ToString()
                });
            }

            return View(model);
        }

        [Route("Create")]
        [HttpPost]
        public IActionResult Create(ContextCreatModel model)
        {
            if (model != null)
            {
                Domains.Context newContext = new Domains.Context();

                if (model.ID != 0)
                {
                    newContext = Context.Contexts.Find(model.ID);
                }

                newContext.Title = model.Title;
                newContext.Description = model.Description;
                newContext.ProjectId = model.ProjectId;

                Context.Contexts.Update(newContext);
                Context.SaveChanges();
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var Contexts = Context.Contexts.Find(id);
                Context.Contexts.Remove(Contexts);
                Context.SaveChanges();
            }
            return RedirectToAction("List");
        }

        //////////////////////////////////////////////////

        [HttpGet]
        [Route("ExportToWord")]
        public IActionResult ExportToWord(int contextId)
        {
            var context = Context.Contexts.FirstOrDefault(c => c.ID == contextId);
            if (context == null)
                return NotFound();

            var displayItems = Context.ContextDisplayItems
                .Where(p => p.ContextId == contextId)
                .OrderBy(p => p.DisplayOrder)
                .ToList();

            var doc = new Aspose.Words.Document();
            var builder = new Aspose.Words.DocumentBuilder(doc);
            doc.FirstSection.PageSetup.RtlGutter = true;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            //builder.ParagraphFormat.Bidi = true;
            builder.Font.Name = "B Nazanin";
            builder.Font.Size = 14;

            int imageCounter = 1;
            int tableCounter = 1;

            // عنوان کانتکس
            builder.Bold = true;
            builder.Font.Size = 16;
            builder.Writeln($"کانتکس: {context.Title}");
            builder.Bold = false;
            builder.Font.Size = 12;
            builder.Writeln(context.Description);
            builder.Writeln("");

            foreach (var item in displayItems)
            {
                switch (item.ItemType)
                {
                    case DisplayItemType.Paragraph:
                        var paragraph = Context.ContextParagraphs.Find(item.ItemId);
                        if (paragraph != null)
                        {
                            builder.Font.Bold = true;
                            builder.Font.Size = 14;
                            builder.Writeln(paragraph.Title);

                            builder.Font.Bold = false;
                            builder.Font.Size = 12;
                            builder.Writeln(paragraph.Description);
                            builder.Writeln("");
                        }
                        break;

                    case DisplayItemType.Image:
                        var image = Context.ContextImages.Find(item.ItemId);
                        if (image != null)
                        {
                            string imagePath = Path.Combine(
                                _env.WebRootPath,
                                image.ImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                            );

                            if (!System.IO.File.Exists(imagePath))
                            {
                                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                                builder.Font.Italic = true;
                                builder.Writeln("(فایل تصویر یافت نشد)");
                                builder.Font.Italic = false;
                            }

                            if (System.IO.File.Exists(imagePath))
                            {
                                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                                //builder.ParagraphFormat.Bidi = false;
                                builder.InsertImage(imagePath, 400, 300);
                                builder.Writeln("");

                                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                                builder.Font.Italic = true;
                                builder.Writeln($"تصویر {imageCounter++} - {image.Caption}");
                                builder.Font.Italic = false;

                                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                                //builder.ParagraphFormat.Bidi = true;
                                builder.Writeln("");
                            }
                        }
                        break;

                    case DisplayItemType.Table:
                        var table = Context.ContextTables
                            .Include(t => t.Columns)
                            .Include(t => t.Rows)
                                .ThenInclude(r => r.Cells)
                            .FirstOrDefault(t => t.ID == item.ItemId);

                        if (table != null)
                        {
                            var columns = table.Columns.ToList();
                            var leafColumns = columns
                                .Where(c => !columns.Any(x => x.ParentColumnId == c.ID))
                                .OrderBy(c => c.Order)
                                .ToList();

                            var rowsList = table.Rows.ToList();

                            if (leafColumns.Count > 0 && rowsList.Count > 0)
                            {
                                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                                //builder.ParagraphFormat.Bidi = true;
                                builder.Font.Bold = true;
                                builder.Writeln($"جدول {tableCounter++}");
                                builder.Font.Bold = false;

                                builder.StartTable();

                                foreach (var col in leafColumns)
                                {
                                    builder.InsertCell();
                                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                                    builder.Font.Bold = true;
                                    builder.Write(col.ColumnTitle);
                                }
                                builder.EndRow();
                                builder.Font.Bold = false;

                                foreach (var row in rowsList)
                                {
                                    foreach (var col in leafColumns)
                                    {
                                        builder.InsertCell();
                                        var cellValue = row.Cells.FirstOrDefault(c => c.ColumnId == col.ID)?.Value ?? "";
                                        builder.Write(cellValue);
                                    }
                                    builder.EndRow();
                                }

                                builder.EndTable();
                                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                                builder.ParagraphFormat.Bidi = true;
                                builder.Writeln("");
                            }
                            else
                            {
                                builder.Font.Italic = true;
                                builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                                //builder.ParagraphFormat.Bidi = true;
                                builder.Writeln("(جدول فاقد ستون یا ردیف است)");
                                builder.Font.Italic = false;
                                builder.Writeln("");
                            }
                        }
                        break;
                }
            }

            var fileName = $"Context_{contextId}.docx";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            doc.Save(filePath);

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName);
        }

        // Export all contexts of a project to a single Word document
        [HttpGet]
        [Route("ExportProjectToWord")]
        public IActionResult ExportProjectToWord(int projectId)
        {
            var project = Context.Projects
                .Include(p => p.Contexts)
                .FirstOrDefault(p => p.ID == projectId);

            if (project == null)
                return NotFound();

            var doc = new Aspose.Words.Document();
            var builder = new Aspose.Words.DocumentBuilder(doc);

            doc.FirstSection.PageSetup.RtlGutter = true;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            //builder.ParagraphFormat.Bidi = true;
            builder.Font.Name = "B Nazanin";
            builder.Font.Size = 14;

            int imageCounter = 1;
            int tableCounter = 1;

            // عنوان پروژه
            builder.Font.Bold = true;
            builder.Font.Size = 18;
            builder.Writeln("پروژه: " + project.Title);
            builder.Font.Bold = false;
            builder.Font.Size = 12;
            builder.Writeln(project.Description);
            builder.Writeln("");

            foreach (var context in project.Contexts.OrderBy(c => c.ID))
            {
                builder.Font.Bold = true;
                builder.Font.Size = 16;
                builder.Writeln("کانتکس: " + context.Title);

                builder.Font.Bold = false;
                builder.Font.Size = 12;
                builder.Writeln(context.Description);
                builder.Writeln("");

                var displayItems = Context.ContextDisplayItems
                    .Where(p => p.ContextId == context.ID)
                    .OrderBy(p => p.DisplayOrder)
                    .ToList();

                foreach (var item in displayItems)
                {
                    switch (item.ItemType)
                    {
                        case DisplayItemType.Paragraph:
                            var paragraph = Context.ContextParagraphs.Find(item.ItemId);
                            if (paragraph != null)
                            {
                                builder.Font.Bold = true;
                                builder.Font.Size = 14;
                                builder.Writeln(paragraph.Title);

                                builder.Font.Bold = false;
                                builder.Font.Size = 12;
                                builder.Writeln(paragraph.Description);
                                builder.Writeln("");
                            }
                            break;

                        case DisplayItemType.Image:
                            var image = Context.ContextImages.Find(item.ItemId);
                            if (image != null)
                            {
                                string imagePath = Path.Combine(
                                    _env.WebRootPath,
                                    image.ImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                                );

                                if (System.IO.File.Exists(imagePath))
                                {
                                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                                    builder.InsertImage(imagePath, 400, 300);
                                    builder.Writeln("");

                                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                                    builder.Font.Italic = true;
                                    builder.Writeln($"تصویر {imageCounter++} - {image.Caption}");
                                    builder.Font.Italic = false;

                                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                                    //builder.ParagraphFormat.Bidi = true;
                                    builder.Writeln("");
                                }
                                else
                                {
                                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                                    builder.Font.Italic = true;
                                    builder.Writeln("(فایل تصویر یافت نشد)");
                                    builder.Font.Italic = false;
                                }
                            }
                            break;

                        case DisplayItemType.Table:
                            var table = Context.ContextTables
                                .Include(t => t.Columns)
                                .Include(t => t.Rows)
                                    .ThenInclude(r => r.Cells)
                                .FirstOrDefault(t => t.ID == item.ItemId);

                            if (table != null)
                            {
                                var columns = table.Columns.ToList();
                                var leafColumns = columns
                                    .Where(c => !columns.Any(x => x.ParentColumnId == c.ID))
                                    .OrderBy(c => c.Order)
                                    .ToList();

                                var rowsList = table.Rows.ToList();

                                if (leafColumns.Count > 0 && rowsList.Count > 0)
                                {
                                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                                    builder.Font.Bold = true;
                                    builder.Writeln($"جدول {tableCounter++}");
                                    builder.Font.Bold = false;

                                    builder.StartTable();

                                    foreach (var col in leafColumns)
                                    {
                                        builder.InsertCell();
                                        builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                                        builder.Font.Bold = true;
                                        builder.Write(col.ColumnTitle);
                                    }
                                    builder.EndRow();
                                    builder.Font.Bold = false;

                                    foreach (var row in rowsList)
                                    {
                                        foreach (var col in leafColumns)
                                        {
                                            builder.InsertCell();
                                            var cellValue = row.Cells.FirstOrDefault(c => c.ColumnId == col.ID)?.Value ?? "";
                                            builder.Write(cellValue);
                                        }
                                        builder.EndRow();
                                    }

                                    builder.EndTable();
                                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                                    //builder.ParagraphFormat.Bidi = true;
                                    builder.Writeln("");
                                }
                                else
                                {
                                    builder.Font.Italic = true;
                                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                                    builder.Writeln("(جدول فاقد ستون یا ردیف است)");
                                    builder.Font.Italic = false;
                                    builder.Writeln("");
                                }
                            }
                            break;
                    }
                }
            }

            var fileName = $"Project_{projectId}.docx";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            doc.Save(filePath);

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName);
        }

        public void BuildTableWithMultiHeader(DocumentBuilder builder, List<ContextTableColumn> allColumns, List<ContextTableRow> allRows)
        {
            // ستون‌هایی که خودشون فرزند ندارن (leaf)
            var leafColumns = allColumns
                .Where(c => !allColumns.Any(x => x.ParentColumnId == c.ID))
                .OrderBy(c => c.Order)
                .ToList();

            // نگاشت ستون‌ها به اطلاعات مسیر کامل (از والد تا خودش)
            var colDict = allColumns.ToDictionary(c => c.ID);
            var leafPaths = leafColumns.Select(leaf =>
            {
                var path = new List<ContextTableColumn>();
                var current = leaf;
                while (current != null)
                {
                    path.Insert(0, current);
                    current = current.ParentColumnId != null && colDict.ContainsKey(current.ParentColumnId.Value)
                        ? colDict[current.ParentColumnId.Value]
                        : null;
                }
                return path;
            }).ToList();

            int maxDepth = leafPaths.Max(p => p.Count);
            builder.StartTable();

            // 🧱 ساخت ردیف‌های هدر (سطح به سطح)
            for (int level = 0; level < maxDepth; level++)
            {
                ContextTableColumn lastCol = null;
                int span = 0;

                for (int i = 0; i <= leafPaths.Count; i++)
                {
                    ContextTableColumn currentCol = (i < leafPaths.Count && level < leafPaths[i].Count)
                        ? leafPaths[i][level]
                        : null;

                    if (lastCol != null && (currentCol == null || currentCol.ID != lastCol.ID))
                    {
                        builder.InsertCell();
                        builder.Write(lastCol.ColumnTitle);

                        // Horizontal merge
                        for (int j = 1; j < span; j++)
                        {
                            builder.InsertCell();
                            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                        }

                        // Vertical merge if this column path ends here
                        bool isLeafAtThisLevel = (level == leafPaths[i - 1].Count - 1);
                        builder.CellFormat.VerticalMerge = isLeafAtThisLevel ? CellMerge.First : CellMerge.None;

                        if (!isLeafAtThisLevel)
                        {
                            // سایر ردیف‌ها vertical previous می‌شن
                            for (int d = 1; d < (maxDepth - level); d++)
                            {
                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.Previous;
                                builder.EndRow();
                            }
                        }

                        span = 0;
                    }

                    if (i < leafPaths.Count)
                    {
                        lastCol = currentCol;
                        span++;
                    }
                }
                if (span > 0)
                    builder.EndRow();
            }

            // 🧾 داده‌ها فقط در leafColumns ریخته می‌شن
            foreach (var row in allRows)
            {
                foreach (var path in leafPaths)
                {
                    var col = path.Last();
                    builder.InsertCell();
                    var val = row.Cells.FirstOrDefault(c => c.ColumnId == col.ID)?.Value ?? "";
                    builder.Write(val);
                }
                builder.EndRow();
            }

            builder.EndTable();
        }
    }
}