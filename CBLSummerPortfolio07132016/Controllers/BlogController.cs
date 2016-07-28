using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using System.Web;
using System.IO;
using CBLSummerPortfolio07132016.Models;
using X.PagedList;


namespace CBLSummerPortfolio07132016.Controllers
{
   [RequireHttps]

    public class BlogController : Controller
    {
        
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Blog
       
        [Authorize(Roles = "Admin")]
        public ActionResult AdminBlogIndex(int? page)
        {
            int pageSize = 100;//the number of posts you want to display per page
            int pageNumber = (page ?? 1);

            return View(db.Posts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }
        // Get: Blog
        public ActionResult Index(int? page, string query)
        {
            int pageSize = 5;//the number of posts you want to display per page
            int pageNumber = (page ?? 1);
            ViewBag.Query = query;
            var qposts = db.Posts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
            {
                qposts = qposts.Where(p => p.Title.Contains(query) || p.BodyText.Contains(query) || p.Comments.Any(c => c.BodyText.Contains(query) || c.Author.DisplayName.Contains(query) || c.UpdateReason.Contains(query)));
            }
            var posts = qposts.Where(p => p.Published).OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize);
            return View(posts);
        }

        // GET: Blog/Details/5
        public ActionResult Details(string slug)

        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }
        public ActionResult Latest()

        {
            var qposts = db.Posts.AsQueryable();//set as queryable
            var posts = qposts.Where(p => p.Published).OrderByDescending(p => p.Created);//order by date
            var slug = posts.FirstOrDefault();//use first one
            return RedirectToAction("Details", new { slug = slug.Slug});
        }
        // GET: Blog/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
      
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Slug,BodyText,MediaUrl,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            blogPost.Created = new DateTimeOffset(DateTime.Now);
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title.");
                    return View(blogPost);
                }
                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique.");
                    return View(blogPost);
                }

                //restsricting the valid file formats to images only
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/assets/images/blog/"), fileName));
                    blogPost.MediaUrl = "~/assets/images/blog/" + fileName;
                }
                blogPost.Created = new DateTimeOffset(DateTime.Now);
                blogPost.Slug = Slug;
                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: Blog/Edit/5
        
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: Blog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Slug,BodyText,MediaUrl,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {

            if (ModelState.IsValid)
            {
                var Id = blogPost.Id;
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                var SlugAlreadyExists = db.Posts.Where(p => p.Id == Id && p.Slug == Slug).Select(p => p.Slug);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title.");
                    return View(blogPost);
                }
                if (!SlugAlreadyExists.Any())
                {
                    if (db.Posts.Any(p => p.Slug == Slug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique.");
                        return View(blogPost);
                    }
                }
                //restricting the valid file formats to images only

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/assets/images/blog/"), fileName));
                    blogPost.MediaUrl = "~/assets/images/blog/" + fileName;
                }
                //db.Entry(blogPost).State = EntityState.Modified;
                blogPost.Updated = new DateTimeOffset(DateTime.Now);
                blogPost.Slug = Slug;


                db.Posts.Attach(blogPost);
                db.Entry(blogPost).Property("Published").IsModified = true;
                db.Entry(blogPost).Property("Title").IsModified = true;
                db.Entry(blogPost).Property("BodyText").IsModified = true;



                if (blogPost.MediaUrl != null)
                {
                    db.Entry(blogPost).Property("MediaUrl").IsModified = true;
                }
                db.Entry(blogPost).Property("Slug").IsModified = true;
                db.Entry(blogPost).Property("Updated").IsModified = true;



                //db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminBlogIndex");
            }
            return View(blogPost);
        }

        // GET: Blog/Delete/5
        
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: Blog/Delete/5
        
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("AdminBlogIndex");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);

        }



        // //GET: Blog/Commment
        //[Authorize(Roles = "Admin")]
        // public ActionResult Comment()
        // {
        //     return View();
        // }


        //// POST: Blog/Comment
        [HttpPost]
        
        public ActionResult Comment([Bind(Include = "Id,BlogPostId, BodyText")] Comment comment)
        {

            if (ModelState.IsValid)
            {
                comment.AuthorId = User.Identity.GetUserId();
                comment.Created = DateTimeOffset.Now;
                db.Comments.Add(comment);
                db.SaveChanges();

            }
            var post = db.Posts.Find(comment.BlogPostId);
            return RedirectToAction("Details", new { slug = post.Slug });

        }

        // GET: Blog/Edit Comment
        [Authorize]
       
        public ActionResult EditComment(int Id)
        {
            Comment comment = db.Comments.FirstOrDefault(p => p.Id == Id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }
        //POST: Edit Comment
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditComment([Bind(Include = "Id,Updated,BodyText,BlogPostId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Updated = DateTimeOffset.Now;
                db.Comments.Attach(comment);

                db.Entry(comment).Property("BodyText").IsModified = true;
                db.Entry(comment).Property("Updated").IsModified = true;

                //db.Entry(comment).State = EntityState.Modified; // To change the entire record

                db.SaveChanges();

            }
            var post = db.Posts.Find(comment.BlogPostId);
            return RedirectToAction("Details", new { slug = post.Slug });
        }




        // GET: Blog/DeleteComment/5
        [Authorize]
        
        public ActionResult DeleteComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);

        }

        // POST: Blog/DeleteCommentConfirmed/5
        [Authorize]
        
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCommentConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            var post = db.Posts.Find(comment.BlogPostId);
            return RedirectToAction("Details", new { slug = post.Slug });
        }

    }
}
