using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SecretSanta.Models;

namespace SecretSanta.Controllers
{
    public class DashboardController : Controller
    {
        private SSContext _context;
        public DashboardController(SSContext context)
        {
            _context = context;
        }
        public Boolean isLogged()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return true;
            }
            return false;
        }
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        private static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return random.Next(min, max);
            }
        }
        public User GetLoggedUser()
        {
            return _context.Users.Include(user => user.MyEvents).Include(user => user.EventOrganized).Include(user => user.Wishlist).SingleOrDefault(user => user.UserId == (int)HttpContext.Session.GetInt32("UserId"));
        }
        public int GetUserId()
        {
            return (int)HttpContext.Session.GetInt32("UserId");
        }
        public Event GetEvent(int eventId)
        {
            return _context.Events.SingleOrDefault(Event => Event.EventId == eventId);
        }
        public Boolean isParticipant(User user, int eventId)
        {
            List<Participant> EventParticipants = _context.Participants.Where(part => part.EventId == eventId).ToList();
            foreach (Participant participant in EventParticipants)
            {
                if (participant.UserId == user.UserId)
                {
                    return true;
                }
            }
            return false;
        }
        public Boolean isSecretSanta(int recipientId)
        {
            List<SecretSantaModel> SecretSantas = _context.SecretSantaModels.Where(ss => ss.RecipientId == recipientId).ToList();
            foreach (SecretSantaModel secretsanta in SecretSantas)
            {
                if (secretsanta.UserId == GetUserId())
                {
                    return true;
                }
            }
            return false;
        }

        public IActionResult Index()
        {
            if (isLogged())
            {
                ViewBag.User = GetLoggedUser();
                // ViewBag.EventOrganized = GetLoggedUser().EventOrganized;
                ViewBag.EventOrganized = GetLoggedUser().EventOrganized.OrderBy(e => e.ExchangeDate).ToList();
                ViewBag.MyEvents = _context.Participants.Include(part => part.Event).Where(part => part.UserId == GetUserId()).OrderBy(e => e.Event.ExchangeDate).ToList();
                ViewBag.Success = TempData["Success"];
                ViewBag.DeleteEvent = TempData["DeleteEvent"];
                ViewBag.Error = TempData["Error"];
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult MyWishlist()
        {
            if (isLogged())
            {
                ViewBag.Wishlist = GetLoggedUser().Wishlist;
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult NewWish(WishViewModel model)
        {
            if (isLogged())
            {
                if (ModelState.IsValid)
                {
                    WishItem NewWishItem = new WishItem();
                    NewWishItem.Name = model.Name;
                    NewWishItem.UserId = GetUserId();
                    _context.WishItems.Add(NewWishItem);
                    _context.SaveChanges();
                    return RedirectToAction("MyWishlist");
                }
                return View("MyWishlist");
            }
            return RedirectToAction("Index", "Home");
        }
        [Route("/Dashboard/Wishlist/{recipientId}")]
        public IActionResult Wishlist(int recipientId)
        {
            if (isLogged())
            {
                if (isSecretSanta(recipientId))
                {
                    ViewBag.Recipient = _context.Users.SingleOrDefault(user => user.UserId == recipientId);
                    ViewBag.Wishlist = _context.WishItems.Where(wish => wish.UserId == recipientId).ToList();
                    return View();
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }
        [Route("/Dashboard/Event/{eventId}")]
        public IActionResult Event(int eventId)
        {
            if (isLogged())
            {
                if (isParticipant(GetLoggedUser(), eventId))
                {
                    ViewBag.Event = GetEvent(eventId);
                    ViewBag.EventMembers = _context.Participants.Include(part => part.User).Where(part => (part.EventId == eventId && part.Accepted == true)).ToList();
                    ViewBag.SecretSanta = _context.SecretSantaModels.Include(ss => ss.Recipient).Where(secret => (secret.EventId == eventId)).SingleOrDefault(ss => ss.UserId == GetUserId());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult NewEvent()
        {
            if (isLogged())
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult SubmitEvent(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                Event NewEvent = new Event();
                NewEvent.Name = model.Name;
                NewEvent.ExchangeDate = model.ExchangeDate;
                NewEvent.SpendLimit = model.SpendLimit;
                NewEvent.CreatedAt = DateTime.Now;
                NewEvent.OrganizerId = GetUserId();
                NewEvent.ExchangeLocation = model.ExchangeLocation;
                _context.Events.Add(NewEvent);
                _context.SaveChanges();
                Event CreatedEvent = _context.Events.Where(Event => (Event.OrganizerId == GetUserId())).Last();
                Participant newParticipant = new Participant();
                newParticipant.UserId = GetUserId();
                newParticipant.EventId = CreatedEvent.EventId;
                newParticipant.Accepted = true;
                _context.Participants.Add(newParticipant);
                _context.SaveChanges();
                TempData["Success"] = "Succesfully created a new event! Click 'Edit Event' to add participants";
                return RedirectToAction("Index");
            }
            return View("NewEvent");
        }
        [Route("/Dashboard/EditEvent/{eventId}")]
        public IActionResult EditEvent(int eventId)
        {
            if (!isLogged())
            {
                return RedirectToAction("Index", "Home");
            }
            if (GetEvent(eventId).OrganizerId == GetUserId())
            {
                ViewBag.Event = GetEvent(eventId);
                ViewBag.SecretSanta = _context.SecretSantaModels.Where(ss => ss.EventId == eventId).ToList();
                ViewBag.InvalidEmails = TempData["InvalidEmails"];
                ViewBag.EventMembers = _context.Participants.Include(part => part.User).Where(part => (part.EventId == eventId)).ToList();
                List<Participant> Participants = _context.Participants.Where(part => part.EventId == eventId).ToList();
                Boolean AllAccepted = true;
                foreach (Participant participant in Participants)
                {
                    if (!participant.Accepted)
                    {
                        AllAccepted = false;
                    }
                }
                if (Participants.Count <= 1)
                {
                    AllAccepted = false;
                }
                ViewBag.AllAccepted = AllAccepted;
                return View();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("/Dashboard/UpdateEvent/{eventId}")]
        public IActionResult UpdateEvent(EventViewModel model, string ParticiantEmails, int eventId)
        {
            if (ModelState.IsValid)
            {
                List<string> InvalidEmails = new List<string>();
                List<string> emails = ParticiantEmails.Split(',').ToList<string>();
                Event EditEvent = GetEvent(eventId);
                EditEvent.Name = model.Name;
                EditEvent.ExchangeDate = model.ExchangeDate;
                EditEvent.SpendLimit = model.SpendLimit;
                EditEvent.ExchangeLocation = model.ExchangeLocation;
                foreach (string email in emails)
                {
                    User participant = _context.Users.SingleOrDefault(user => user.Email == email.Trim());
                    if (participant == null)
                    {
                        InvalidEmails.Add(email.Trim());
                    }
                    else
                    {
                        if (!isParticipant(participant, eventId))
                        {
                            Participant newParticipant = new Participant();
                            newParticipant.UserId = participant.UserId;
                            newParticipant.EventId = EditEvent.EventId;
                            _context.Participants.Add(newParticipant);
                        }
                    }
                }
                _context.SaveChanges();

                if (InvalidEmails.Count > 0)
                {
                    string Error = null;
                    for (int i = 0; i < InvalidEmails.Count; i++)
                    {
                        if (InvalidEmails.Count == 1 || i == InvalidEmails.Count - 1)
                        {
                            Error += InvalidEmails[i];
                        }
                        else
                        {
                            Error += (InvalidEmails[i] + ", ");
                        }
                    }
                    TempData["InvalidEmails"] = Error;
                    return RedirectToAction("EditEvent");
                }
                return RedirectToAction("Index");
            }
            return View("EditEvent");
        }
        [Route("/Dashboard/AcceptInvite/{participantId}")]
        public IActionResult AcceptInvite(int participantId)
        {

            if (isLogged())
            {
                Participant AcceptInvite = _context.Participants.SingleOrDefault(part => part.ParticipantId == participantId);
                if (GetUserId() == AcceptInvite.UserId)
                {
                    AcceptInvite.Accepted = true;
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [Route("/Dashboard/GenerateSecretSanta/{eventId}")]
        public IActionResult GenerateSecretSanta(int eventId)
        {

            List<Participant> Participants = _context.Participants.Include(part => part.User).Where(part => part.EventId == eventId).ToList();
            List<User> Users = new List<User>();
            List<User> Recipients = new List<User>();
            foreach (Participant participant in Participants)
            {
                Users.Add(participant.User);
                Recipients.Add(participant.User);
            }
            int Total = Participants.Count;
            if (Total == 1)
            {
                TempData["Error"] = $"You must have at least 2 participants to generate secret santa";
            }
            else
            {
                for(int i=0; i<Users.Count; i++)
                {
                    int rand = RandomNumber(0, Users.Count);
                    User temp = Users[i]; 
                    Users[i] = Users[rand]; 
                    Users[rand] = temp;
                }
                User firstUser = Users[0];
                Users.Add(firstUser);
                for(int i=0; i<Users.Count-1; i++)
                {
                    SecretSantaModel NewSecretSanta = new SecretSantaModel();
                    User user = Users[i];
                    User recipient = Users[i+1];
                    NewSecretSanta.UserId = user.UserId;
                    NewSecretSanta.RecipientId = recipient.UserId;
                    NewSecretSanta.EventId = eventId;
                    _context.SecretSantaModels.Add(NewSecretSanta);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        [Route("/Dashboard/DeleteEvent/{eventId}")]
        public IActionResult DeleteEvent(int eventId)
        {
            if(isLogged() && (GetEvent(eventId).OrganizerId == GetUserId()))
            {
                Event DeleteEvent = GetEvent(eventId);
                _context.Events.Remove(DeleteEvent);
                List<Participant> RemoveParticipants = _context.Participants.Where(p => p.EventId == DeleteEvent.EventId).ToList();
                List<SecretSantaModel> RemoveSecretSantas = _context.SecretSantaModels.Where(ss => ss.EventId == DeleteEvent.EventId).ToList();
                foreach (Participant participant in RemoveParticipants)
                {
                    _context.Participants.Remove(participant);
                }
                foreach (SecretSantaModel secretsanta in RemoveSecretSantas)
                {
                    _context.SecretSantaModels.Remove(secretsanta);
                }
                _context.SaveChanges();
                TempData["DeleteEvent"] = $"Succesfully deleted {DeleteEvent.Name}";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}