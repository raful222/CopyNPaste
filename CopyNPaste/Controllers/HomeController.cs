using CopyNPaste.Core.Data;
using CopyNPaste.Core.Entities;
using CopyNPaste.Models;
using CopyNPaste.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace CopyNPaste.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [Route("{id?}")]
        public IActionResult Index(Guid? id)
        {
            CopyViewModel model = new CopyViewModel();

            var user = User.Identity.Name;
            if (user != null)
            {
                var userid = _context.Users.Where(x => x.UserName == user).FirstOrDefault().Id;
                model.UserId = userid;
            }
            Load(model);
            if (id.HasValue)
            {
                var copy = _context.Copies.Where(x => x.Id == id).FirstOrDefault();
                if (copy != null)
                {
                    model.Subject = copy.Subject;
                    var text = ChangeToCopyStyle(copy.Body);
                    model.Body = text;
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult MakePaste(CopyViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var text = ChangeToHtmlStyle(model.Body);
                    var copy = new Copy()
                    {
                        Body = text,
                        Password = model.Password,
                        Policy = model.Policy,
                        Subject = model.Subject,
                        UserId = model.UserId,
                    };
                    _context.Add(copy);
                    _context.SaveChanges();
                    return RedirectToAction("Paste", new { Id = copy.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
            Load(model);
            return RedirectToAction("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void Load(CopyViewModel model)
        {
            string[] allColors = Enum.GetNames(typeof(System.Drawing.KnownColor));
            string[] preferredColors = { "Red", "Blue", "Green", "Yellow", "Orange", "Pink", "Cyan", "Brown" };

            Random rand = new Random();
            string[] finalColors = preferredColors
                .Concat(allColors.Except(preferredColors).OrderBy(_ => rand.Next()))
                .Take(25)
                .ToArray();

            int[] sizes = { 1, 2, 3, 4, 5, 6, 7 };
            model.SizeCodes = sizes;
            model.ColorCodes = finalColors;
        }

        public string ChangeToHtmlStyle(string body)
        {
            body = body.Replace("<BeStrong>", "<b>").Replace("</BeStrong>", "</b>"); // bold
            body = body.Replace("<BeItalic>", "<i>").Replace("</BeItalic>", "</i>"); // italic
            body = body.Replace("<BeStrike>", "<s>").Replace("</BeStrike>", "</s>"); // strike
            body = body.Replace("<BeUnderLine>", "<u>").Replace("</BeUnderLine>", "</u>"); // underline
            body = body.Replace("<BeQuotation>", "<q>").Replace("</BeQuotation>", "</q>"); // quatation
            body = body.Replace("<BeLink>", "<a target='_blank'  href='http://").Replace("</BeLink>", "'> link</a>"); //a link
            body = body.Replace("<BeSize=1>", "<h1>").Replace("</BeSize=1>", "</h1>"); // size 1
            body = body.Replace("<BeSize=2>", "<h2>").Replace("</BeSize=2>", "</h2>"); // size 2
            body = body.Replace("<BeSize=3>", "<h3>").Replace("</BeSize=3>", "</h3>"); // size 3
            body = body.Replace("<BeSize=4>", "<h4>").Replace("</BeSize=4>", "</h4>"); // size 4
            body = body.Replace("<BeSize=5>", "<h5>").Replace("</BeSize=5>", "</h5>"); // size 5
            body = body.Replace("<BeSize=6>", "<h6>").Replace("</BeSize=6>", "</h6>"); // size 6
            body = body.Replace("<BeSize=7>", "<h7>").Replace("</BeSize=7>", "</h7>"); // size 7

            //color
            string[] allColors = Enum.GetNames(typeof(System.Drawing.KnownColor));

            foreach (var color in allColors)
            {
                body = body.Replace($"<BeColor={color}>", $"<div style='color: {color};'>").Replace($"</BeColor={color}>", "</div>");
            }

            return body;
        }

        public string ChangeToCopyStyle(string body)
        {
            body = body.Replace("<b>", "<BeStrong>").Replace("</b>", "</BeStrong>"); // bold
            body = body.Replace("<i>", "<BeItalic>").Replace("</i>", "</BeItalic>"); // italic
            body = body.Replace("<s>", "<BeStrike>").Replace("</s>", "</BeStrike>"); // strike
            body = body.Replace("<u>", "<BeUnderLine>").Replace("</u>", "</BeUnderLine>"); // underline
            body = body.Replace("<q>", "<BeQuotation>").Replace("</q>", "</BeQuotation>"); // quotation
            body = body.Replace("<a target='_blank'  href='http://", "<BeLink>").Replace("'> link</a>", "</BeLink>"); // link
            body = body.Replace("<h1>", "<BeSize=1>").Replace("</h1>", "</BeSize=1>"); // size 1
            body = body.Replace("<h2>", "<BeSize=2>").Replace("</h2>", "</BeSize=2>"); // size 2
            body = body.Replace("<h3>", "<BeSize=3>").Replace("</h3>", "</BeSize=3>"); // size 3
            body = body.Replace("<h4>", "<BeSize=4>").Replace("</h4>", "</BeSize=4>"); // size 4
            body = body.Replace("<h5>", "<BeSize=5>").Replace("</h5>", "</BeSize=5>"); // size 5
            body = body.Replace("<h6>", "<BeSize=6>").Replace("</h6>", "</BeSize=6>"); // size 6
            body = body.Replace("<h7>", "<BeSize=7>").Replace("</h7>", "</BeSize=7>"); // size 7


            //color
            string[] allColors = Enum.GetNames(typeof(System.Drawing.KnownColor));

            foreach (var color in allColors)
            {
                body = body.Replace($"<div style='color: {color};'>", $"<BeColor={color}>").Replace("</div>", $"</BeColor={color}>");
            }

            return body;
        }
        [Route("Paste/{Id}")]
        public IActionResult Paste(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                return RedirectToAction("Index");
            }
            var pastePassword = HttpContext.Session.GetString("PastePassword"); 

            var paste = _context.Copies.Where(x => x.Id == Id).FirstOrDefault();
            if (paste == null)
            {
                return RedirectToAction("Index");

            }

            if (paste.Password != null && pastePassword != paste.Password)
            {
                var passwordPaste = new PasswordCheckViewModel { Id = Id };
                return View("PasswordPaste", passwordPaste);

            }
            return View(paste);
        }

        [HttpPost]
        public IActionResult CheckPassword(PasswordCheckViewModel model)
        {
            if (ModelState.IsValid)
            {
                var copy = _context.Copies.Where(x => x.Id == model.Id).FirstOrDefault();
                if(copy == null)
                {
                    return RedirectToAction("Index");
                }
                if (copy.Password == model.Password)
                {
                    HttpContext.Session.SetString("PastePassword", model.Password); 
                    return RedirectToAction("Paste", new { Id = copy.Id });
                }
            }
            ViewBag.Error = "Password Not Correct";
            return View("PasswordPaste", model);
        }
    }
}
 