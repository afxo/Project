using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;
using Core.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Project.Models;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UnitsManager()
        {
            using (var db = new Context())
            {
                return View(db.Units.ToList());
            }
        }

        public ActionResult NewUnit()
        {
           
            return View();

        }

        public new ActionResult Profile()
        {
            var profile = new ProfileModel();

            using (var db = new Context())
            {
                var user = db.UserRoles.First(sh => sh.username.Contains(User.Identity.Name));
                profile.ID = user.ID;
                profile.username = user.username;
                switch (user.role)
                {
                    case "dim":
                        try
                        {
                            var un = db.UnitMembers.First(sh => sh.Username == user.username);
                            profile.membership = db.Units.First(sh => sh.ID == un.ClassID).name;
                            profile.insitute = db.Units.First(sh => sh.ID == un.ClassID).institute;
                            profile.count = db.Publishments.Where(sh => sh.uploadedby == user.ID).ToList().Count;
                            profile.role = "Δημοσιεύον";
                        }
                        catch (Exception e)
                        {
                            profile.membership = "Δεν ανήκετε σε κάποια μονάδα";
                            profile.insitute = "-";
                            profile.count = db.Publishments.Where(sh => sh.uploadedby == user.ID).ToList().Count;
                            profile.role = "Δημοσιέυον";
                        }

                        break;
                    case "yp":
                        try
                        {
                            var um = db.UnitMasters.First(sh => sh.MasterID.Contains(user.username));
                            profile.membership = db.Units.First(sh => sh.ID == um.UnitID).name;
                            profile.insitute = db.Units.First(sh => sh.ID == um.UnitID).institute;
                            profile.role = "Υπεύθυνος";
                            profile.count = db.Publishments.Where(sh => sh.uploadedby == user.ID).ToList().Count;
                          
                        }
                        catch (Exception e)
                        {
                            profile.membership = "Η μονάδα δεν είναι πλέον διαθέσιμη";

                        }
                        break;
                }

                try
                {
                    profile.choises = new List<string>();
                    var writer = db.Writers.First(sh => sh.ID == user.ID);
                    profile.writername = writer.name;
                    profile.writersurrname = writer.surrname;
                    profile.writerorchidurl = writer.orchidURL;
                    profile.writerprivateurl = writer.privateURL;
                    profile.writerproperty = writer.property;
                }
                catch (Exception e)
                {
                    //if user not register as a writer return an empty form to complete
                    profile.writername = "";
                    profile.writersurrname = "";
                    profile.writerorchidurl = "";
                    profile.writerprivateurl = "";
                    profile.writerproperty = "";
                }

                profile.choises.Add("Τεχνικό Προσωπικό");
                profile.choises.Add("Καθηγητής");
                profile.choises.Add("Ερευνητής");
                profile.choises.Add("Ερευνητικό Προσωπικό");
                profile.avaliableUnits = db.Units.ToList();
            }


            return View(profile);
        }

        public ActionResult FirstLogin()
        {
            var listmodels = new List<DropItems>();
            using (var db = new Context())
            {
                try
                {
                    foreach (var item in db.UnitMasters)
                        listmodels.Add(new DropItems
                        {
                            ItemName = db.Units.AsEnumerable().Where(sh => sh.ID == item.UnitID).FirstOrDefault().name,
                            MasterID = item.MasterID
                        });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                  
                }
               
                return View(listmodels);
            }
        }


        public JsonResult CheckFirstTime(string username)
        {
            using (var db = new Context())
            {
                try
                {
                    var user = db.UserRoles.Where(c => username == c.username).FirstOrDefault();
                    if (user == null) return Json(new {success = true});
                }
                catch (Exception)
                {
                }

                return Json(new {success = false});
            }
        }

        public ActionResult Register(UserModel model)
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignOut()
        {
            var AuthenticationManager = HttpContext.GetOwinContext().Authentication;
            AuthenticationManager.SignOut();
            return Redirect("/Home");
        }

        public ActionResult LogUserIn(UserModel model)
        {
            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);
            var user = userManager.Find(model.username, model.password);

            if (user != null)
            {
                var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                authenticationManager.SignIn(new AuthenticationProperties {IsPersistent = false}, userIdentity);
                return Json(new {success = true, responseText = "Login"});
            }

            return Json(new
                {success = true, responseText = "Ο συνδιασμός Ονόματος Χρήστη και κωδικού δεν είναι έγκυρος"});
        }

        public ActionResult Delete(string id)
        {
            using (var db = new Context())
            {
                
                    var _todelete = db.Units.Where(c => id.Contains(c.ID)).FirstOrDefault();
                    db.Units.Remove(_todelete);
                    var _toremove = db.UnitMasters.Where(c => id.Contains(c.UnitID)).FirstOrDefault();
                    db.UnitMasters.Remove(_toremove);
                    var users_tochange = db.UnitMembers.Where(sh => sh.ClassID == id).ToList();
                    foreach (var item in users_tochange)
                    {
                        db.UnitMembers.Remove(item);
                    }
                    db.SaveChanges();
            }

            return RedirectToAction("UnitsManager");
        }


        [HttpPost]
        public ActionResult CreateUser(UserModel model)
        {
            var username = model.username;
            var password = model.password;
            var role = model.role;
            // Default UserStore constructor uses the default connection string named: DefaultConnection
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);

            var user = new IdentityUser {UserName = username};
            var result = manager.Create(user, password);


            return Json(new {success = true, responseText = result.Errors});
        }


        [HttpPost]
        public JsonResult NewUnit(Unit unit)
        {
            using (var db = new Context())
            {
                try
                {
                    unit.ID = Guid.NewGuid().ToString();
                    var un = new UnitMasters {UnitID = unit.ID, MasterID = "none", ID = Guid.NewGuid().ToString()};
                    db.Units.Add(unit);
                    db.UnitMasters.Add(un);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Json(new {success = false, responsemeeesage = e.ToString()});
                }


                return Json(new {success = true, responsemessage = "Προστέθηκε με επιτυχία!"});
            }
        }

        public JsonResult AssignRoles(UserModel model)
        {
            using (var db = new Context())
            {
                try
                {
                    var user = new UserRoles
                        {ID = Guid.NewGuid().ToString(), username = model.username, role = model.role};
                    db.UserRoles.AddOrUpdate(user);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Json(new {success = false, responsemessage = e.ToString()});
                }

                return Json(new {success = true, responsemessage = "Προστέθηκε με επιτυχία"});
            }
        }

        public JsonResult AddManager(RoleAssign model)
        {
            using (var db = new Context())
            {
                try
                {
                    var unit = db.Units.Where(sh => model.coursename.Contains(sh.name)).FirstOrDefault();
                    var unit_tomanage = db.UnitMasters.Where(sh => unit.ID.Contains(sh.UnitID)).FirstOrDefault();
                    unit_tomanage.MasterID = model.username;
                    db.UnitMasters.AddOrUpdate(unit_tomanage);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Json(new {success = false, responsemessage = e.ToString()});
                }

                return Json(new {success = true, responmessage = "completed"});
            }
        }

        public JsonResult SubscribeToTeams(SubscribeModel model)
        {
            using (var db = new Context())
            {
                try
                {
                    var item = new UnitMembers
                    {
                        ID = Guid.NewGuid().ToString(),
                        ClassID = db.Units.Where(sh => model.subscription.Contains(sh.name)).FirstOrDefault().ID,
                        Username = model.username
                    };
                    db.UnitMembers.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Json(new {success = false, responsemessage = e.ToString()});
                }

                return Json(new {success = true, responsemessage = "ok"});
            }
        }


        public JsonResult AddNewWriter(WriterModel model)
        {
            using (var db = new Context())
            {
                try
                {
                    var writer = new Writer
                    {
                        ID = db.UserRoles.Where(sh => model.username.Contains(sh.username)).FirstOrDefault().ID,
                        name = model.name,
                        surrname = model.surrname,
                        orchidURL = model.orchidURL,
                        privateURL = model.privateURL,
                        property = model.property
                    };
                    db.Writers.Add(writer);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Json(new {success = false, responsemessage = e.ToString()});
                }

                return Json(new {success = true, responsemessage = "ok"});
            }
        }

        public JsonResult UpdateTable(ProfileModel model)
        {
            using (var db = new Context())
            {
                try
                {
                    var writer = db.Writers.First(sh => sh.ID == model.ID);
                    writer.name = model.writername;
                    writer.surrname = model.writersurrname;
                    writer.orchidURL = model.writerorchidurl;
                    writer.privateURL = model.writerprivateurl;
                    writer.property = model.writerproperty;
                    db.Writers.AddOrUpdate(writer);
                    db.SaveChanges();
                }
                catch (Exception W)
                {
                    try
                    {
                        var writer = new Writer
                        {
                            ID = model.ID,
                            name = model.writername,
                            surrname = model.writersurrname,
                            orchidURL = model.writerorchidurl,
                            privateURL = model.writerprivateurl,
                            property = model.writerproperty
                        };
                        db.Writers.Add(writer);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        //den exei prothesi na ginei sugrafeas
                    }
                }

                UpdateUser(model);
            }

            return Json(new {success = true});
        }

        private void UpdateUser(ProfileModel model)
        {
            using (var db = new Context())
            {
                var username = db.UserRoles.First(sh => sh.ID == model.ID).username;
                try
                {
                    var member = db.UnitMembers.First(sh => sh.Username == username);
                    member.ClassID = db.Units.First(sh => sh.name.Contains(model.membership)).ID;
                    db.UnitMembers.AddOrUpdate(member);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    try
                    {
                        var member = new UnitMembers
                        {
                            ID = Guid.NewGuid().ToString(),
                            ClassID = db.Units.First(sh => sh.name.Contains(model.membership)).ID,
                            Username = username
                        };
                        db.UnitMembers.Add(member);
                        db.SaveChanges();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                }

                var id = db.UserRoles.First(sh => sh.username == username).ID;
                //Change Publishment unit owner
                foreach (var item in db.Publishments)
                    if (item.uploadedby == id)
                    {
                        item.unit_that_belongs = db.UnitMembers.First(sh => sh.Username == username).ClassID;
                        db.Publishments.AddOrUpdate(item);
                    }

                db.SaveChanges();
            }
        }
    }
}