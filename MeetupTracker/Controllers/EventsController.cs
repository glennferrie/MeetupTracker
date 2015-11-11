using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetupTracker.Models;

namespace MeetupTracker.Controllers
{
    public class EventsController : Controller
    {
        private MeetupModelContainer db = new MeetupModelContainer();

        // GET: Events
        public ActionResult Index()
        {
            return View(db.MeetupEvents.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MeetupEvent meetupEvent = db.MeetupEvents.Find(id);
            if (meetupEvent == null)
            {
                return HttpNotFound();
            }
            return View(meetupEvent);
        }

        [Authorize]
        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Location,EventDate")] MeetupEvent meetupEvent)
        {
            if (ModelState.IsValid)
            {
                db.MeetupEvents.Add(meetupEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(meetupEvent);
        }

        [Authorize]
        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MeetupEvent meetupEvent = db.MeetupEvents.Find(id);
            if (meetupEvent == null)
            {
                return HttpNotFound();
            }
            return View(meetupEvent);
        }


        public ActionResult Attendees(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MeetupEvent meetupEvent = db.MeetupEvents.Find(id);
            if (meetupEvent == null)
            {
                return HttpNotFound();
            }

            ViewBag.EventID = id;
            ViewBag.EventTitle = meetupEvent.Title;

            return View(meetupEvent.Attendees.ToList());
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Location,EventDate")] MeetupEvent meetupEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(meetupEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(meetupEvent);
        }

        [Authorize]
        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MeetupEvent meetupEvent = db.MeetupEvents.Find(id);
            if (meetupEvent == null)
            {
                return HttpNotFound();
            }
            return View(meetupEvent);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MeetupEvent meetupEvent = db.MeetupEvents.Find(id);
            db.MeetupEvents.Remove(meetupEvent);
            db.SaveChanges();
            return RedirectToAction("Index");
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
