using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetupTracker.Models;
using System.Net.Mail;
using System.Configuration;
using System.Text;

namespace MeetupTracker.Controllers
{
    public class AttendeesController : Controller
    {
        private MeetupModelContainer db = new MeetupModelContainer();

        // GET: Attendees
        public ActionResult Index()
        {
            return View(db.Attendees.ToList());
        }

        // GET: Attendees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendee attendee = db.Attendees.Find(id);
            if (attendee == null)
            {
                return HttpNotFound();
            }
            return View(attendee);
        }

        [Authorize]
        // GET: Attendees/Create
        public ActionResult Create(int? eventID)
        {
            var model = new Attendee();
            
            model.InviteDate = DateTime.Now;
            model.InvitationCode = Guid.NewGuid().ToString("N");

            if (eventID.HasValue)
            {
                model.MeetupEvent = db.MeetupEvents.Find(eventID);
                ViewBag.EventID = eventID;
            }

            return View(model);
        }

        // POST: Attendees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FullName,Company,EmailAddress,InviteDate,Rsvp,Attended,InvitationCode")] Attendee attendee)
        {


            if (ModelState.IsValid)
            {
                var eventidString = Request.Form["eventID"];
                // hack to associate the first event.
                if (attendee.MeetupEvent == null)
                {
                    if (string.IsNullOrWhiteSpace(eventidString))
                    {
                        attendee.MeetupEvent = db.MeetupEvents.FirstOrDefault();

                    }
                    else
                    {
                        var eventId = Convert.ToInt32(eventidString);
                        attendee.MeetupEvent = db.MeetupEvents.Find(eventId);
                    }
                }
                db.Attendees.Add(attendee);
                db.SaveChanges();

                if (!string.IsNullOrWhiteSpace(eventidString))
                {
                    return RedirectToAction("Attendees", "Events", new { id = eventidString });
                }

                return RedirectToAction("Index");
            }

            return View(attendee);
        }

        [Authorize]
        // GET: Attendees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendee attendee = db.Attendees.Find(id);
            if (attendee == null)
            {
                return HttpNotFound();
            }
            return View(attendee);
        }

        // POST: Attendees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FullName,Company,EmailAddress,InviteDate,Rsvp,Attended,InvitationCode")] Attendee attendee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attendee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(attendee);
        }
        [Authorize]
        // GET: Attendees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendee attendee = db.Attendees.Find(id);
            if (attendee == null)
            {
                return HttpNotFound();
            }
            return View(attendee);
        }

        // POST: Attendees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Attendee attendee = db.Attendees.Find(id);
            db.Attendees.Remove(attendee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // http://meetuptracker.azurewebsites.net/Attendees/Respond/1?eventId=1&value=YES
        [AllowAnonymous]
        public ActionResult Respond(int id, int eventId, string value)
        {
            var attendee = db.Attendees.Find(id);
            if (attendee != null)
            {
                if (attendee.MeetupEvent.Id == eventId)
                {
                    if (value == "YES")
                    {
                        attendee.Rsvp = true;
                        db.SaveChanges();
                    }
                    if (value == "NO")
                    {
                        attendee.Rsvp = false;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index", "Attendees");
        }

        [Authorize]
        public ActionResult Invite(int id)
        {
            var attendee = db.Attendees.Find(id);
            if (attendee != null)
            {
                if (!attendee.Rsvp.HasValue)
                {
                    // TODO: Abstract to a separate class / assembly / web job
                    var sendgriduser = ConfigurationManager.AppSettings["SENDGRID_USERNAME"];
                    var sendgridpassword = ConfigurationManager.AppSettings["SENDGRID_PASSWORD"];
                    var client = new SmtpClient();
                    var mailmsg = new MailMessage() { IsBodyHtml = true };

                    var bodycontent = new StringBuilder();
                    bodycontent.Append("<html><body>");
                    bodycontent.Append("<h2>Meetup Tracker</h2>");
                    bodycontent.Append("<p >You've been invited to an event, " + attendee.MeetupEvent.Title + "</p>");
                    bodycontent.Append("<p>Location: " + attendee.MeetupEvent.Location + "</p>");
                    bodycontent.Append("<p>Date / Time: " + attendee.MeetupEvent.EventDate + "</p>");
                    
                    bodycontent.Append("<h3>RSVP</h3>");
                    bodycontent.Append("<p>Are you planning to attend the event?</p>");

                    var yesUrl = Url.Action("Respond", "Attendees", new { id = attendee.Id, eventId = attendee.MeetupEvent.Id, value = "YES" });
                    var noUrl = Url.Action("Respond", "Attendees", new { id = attendee.Id, eventId = attendee.MeetupEvent.Id, value = "NO" });

                    bodycontent.Append("<p><a href='http://meetuptracker.azurewebsites.net" + yesUrl + "' class='response-link'>Yes</a></p>");
                    bodycontent.Append("<p><a href='http://meetuptracker.azurewebsites.net" + noUrl + "' class='response-link'>No</a></p>");
                    bodycontent.Append("<br /><br /><br />");
                    bodycontent.Append("<p>&copy; 2015 gfdata corp.</p>");
                    bodycontent.Append("</body></html>");

                    mailmsg.To.Add(attendee.EmailAddress);
                    mailmsg.Subject = "Meetup Invite: " + attendee.MeetupEvent.Title;
                    mailmsg.Body = bodycontent.ToString();
                    client.Credentials = new NetworkCredential { UserName = sendgriduser, Password = sendgridpassword };
                    client.Send(mailmsg);
                }
            }

            return RedirectToAction("Index", "Attendees");
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
