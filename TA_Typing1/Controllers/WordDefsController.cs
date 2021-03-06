﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TA_Typing1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TA_Typing1.Controllers
{
    [Authorize]
    public class WordDefsController : Controller
    {
        private MyDbContext db = new MyDbContext();
        private UserManager<ApplicationUser> manager;

        public WordDefsController()
        {
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: WordDefs
        public ActionResult Index()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var wordDefs = db.WordDefs.Include(w => w.word);
            return View(wordDefs.ToList().Where(w => w.word.User.Id == currentUser.Id));
        }

        // GET: WordDefs/Details/5
        [ChildActionOnly]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordDef wordDef = db.WordDefs.Find(id);
            if (wordDef == null)
            {
                return HttpNotFound();
            }
            return View(wordDef);
        }

        // GET: WordDefs/Create
        public ActionResult Create(int wId)
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());

            Word word = db.Words.Find(wId);
            if (word == null)
            {
                return HttpNotFound();
            }
            else
            {
                Word currentWord = db.Words.First(w => w.Id == wId);
                if (currentWord.User.Id == currentUser.Id)
                {
                    ViewBag.currentWord = currentWord.Id;
                    ViewBag.currentWordContent = currentWord.WordDetail.WContext;
                    return View();
                }
                else
                {
                    return HttpNotFound();
                }

            }
        }

        // POST: WordDefs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WordDef wordDef)
        {
            ViewBag.currentWord = wordDef.wordId;

            if (ModelState.IsValid)
            {
                var currentUser = manager.FindById(User.Identity.GetUserId());
                Word currentWord = db.Words.First(w => w.Id == wordDef.wordId);

                if (currentWord.User.Id == currentUser.Id)
                {

                    ViewBag.redirectUrl = Url.Action("details", "words", new { id = wordDef.wordId });
                    db.WordDefs.Add(wordDef);
                    db.SaveChanges();

                    return PartialView("_RedirectPage");
                }

                return HttpNotFound();
            }
            else
            {
                return View();
            }
        }

        // GET: WordDefs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordDef wordDef = db.WordDefs.Find(id);
            if (wordDef == null)
            {
                return HttpNotFound();
            }
            ViewBag.wordId = new SelectList(db.Words, "Id", "WContext", wordDef.wordId);
            return View(wordDef);
        }

        // POST: WordDefs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,wType,wDefinition,wExample")] WordDef wordDef)
        {
            if (wordDef.wDefinition != null)
            {
                WordDef defwordFound = db.WordDefs.Find(wordDef.id);
                if (defwordFound != null)
                {
                    defwordFound.wType = wordDef.wType;
                    defwordFound.wDefinition = wordDef.wDefinition;
                    defwordFound.wExample = wordDef.wExample;
                    db.Entry(defwordFound).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true });
                }
                else
                    return View(wordDef);
            }

            ViewBag.wordId = new SelectList(db.Words, "Id", "WContext", wordDef.wordId);
            return View(wordDef);
        }

        // GET: WordDefs/Delete/5
        //[ChildActionOnly]
        public ActionResult Delete(int? id)
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordDef wordDef = db.WordDefs.Find(id);
            if (wordDef == null)
            {
                return HttpNotFound();
            }

            if (wordDef.word.User != currentUser)
            {
                return HttpNotFound();
            }

            ViewBag.wContext = wordDef.word.WordDetail.WContext;

            return View(wordDef);
        }

        // POST: WordDefs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WordDef wordDef = db.WordDefs.Find(id);
            db.WordDefs.Remove(wordDef);
            db.SaveChanges();

            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
