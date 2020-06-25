using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using Core;
using Core.Models;
using Project.Models;

namespace Project.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin

        public ActionResult WriterManager()
        {
            var awm = new AdminWriterModel();
            var writerList = new List<Writer>();
            using (var db = new Context())
            {
                writerList = db.Writers.ToList();
            }

            awm.writer = writerList;
            awm.counter = CountWriter(writerList);
            return View(awm);
        }

        private Dictionary<string, int> CountWriter(List<Writer> writerList)
        {
            var finaldict = new Dictionary<string, int>();
            foreach (var writer in writerList)
            {
                var fullname = writer.name + " " + writer.surrname;
                finaldict.Add(fullname, 0);
                using (var db = new Context())
                {
                    finaldict[fullname] += db.Articles.Count(sh => sh.author.Contains(fullname));
                    finaldict[fullname] += db.Books.Count(sh => sh.publisher.Contains(fullname));
                    finaldict[fullname] += db.Inbooks.Count(sh => sh.publisher.Contains(fullname));
                    finaldict[fullname] += db.Inproceedings.Count(sh => sh.publisher.Contains(fullname));
                }
            }

            return finaldict;
        }

        public ActionResult UserManager()
        {
            var model = new List<AdminUserModel>();
            using (var db = new Context())
            {
                foreach (var item in db.UserRoles.ToList())
                {
                    var aum = new AdminUserModel
                    {
                        ID = item.ID,
                        username = item.username,
                        role = item.role
                    };
                    switch (item.role)
                    {
                        case "dim":
                            try
                            {
                                var classId = db.UnitMembers.First(sh => sh.Username == item.username).ClassID;
                                aum.membership = db.Units.First(sh => sh.ID == classId).name;
                                // ReSharper disable once ReplaceWithSingleCallToFirst
                                aum.insitute = db.Units.Where(sh => sh.ID == classId).First().institute;
                            }
                            catch (Exception e)
                            {
                                aum.membership = "Δεν ανοικει σε κάποια μονάδα";
                                aum.insitute = "-";
                            }

                            break;

                        case "yp":
                            try
                            {
                                var unitId = db.UnitMasters.First(sh => sh.MasterID == item.username).UnitID;
                                aum.membership = db.Units.First(sh => sh.ID == unitId).name;
                                aum.insitute = db.Units.First(sh => sh.ID == unitId).institute;
                               
                            }
                            catch (Exception e)
                            {
                                aum.membership = "Η μονάδα ως υπεύθυνος δεν είναι πλέον διαθέσιμη";
                                aum.insitute = "-";
                            }
                            break;
                    }

                    model.Add(aum);
                }
            }

            return View(model);
        }


        public ActionResult Delete(string username)
        {
            using (var db = new Context())
            {
                var _todelete = db.UserRoles.Where(c => username.Contains(c.username)).First();
                db.UserRoles.Remove(_todelete);
                if (_todelete.role == "yp")
                {
                    var master = _todelete.username;
                    var _tochange = db.UnitMasters.Where(c => c.MasterID == master).First();
                    _tochange.MasterID = "none";
                    db.UnitMasters.AddOrUpdate(_tochange);
                }

                if (_todelete.role == "dim")
                    try
                    {
                        var _removeUser = db.UnitMembers.Where(c => c.Username == username).First();
                        db.UnitMembers.Remove(_removeUser);
                    }
                    catch (Exception e)
                    {
                        //do nothing he was just a regular member
                    }

                db.SaveChanges();
            }

            return RedirectToAction("UserManager");
        }

        public ActionResult WriterEdit(string name, string surrname)
        {
            var wr = new Writer();
            using (var db = new Context())
            {
                try
                {
                    wr = db.Writers.First(sh => sh.name == name && sh.surrname == surrname);
                }
                catch (Exception e)
                {
                    wr = new Writer
                    {
                        name = null
                    };
                }
            }

            return View(wr);
        }


        public ActionResult UpdateWriter(ProfileModel model)
        {
            using (var db = new Context())
            {
                try
                {
                    var wr = db.Writers.First(sh => sh.ID == model.ID);
                    wr.surrname = model.writersurrname;
                    wr.name = model.writername;
                    wr.orchidURL = model.writerorchidurl;
                    wr.privateURL = model.writerprivateurl;
                    wr.property = model.writerproperty;
                    db.Writers.AddOrUpdate(wr);
                }
                catch (Exception e)
                {
                    var wr = new Writer
                    {
                        ID = Guid.NewGuid().ToString(),
                        surrname = model.writersurrname,
                        name = model.writername,
                        orchidURL = model.writerorchidurl,
                        privateURL = model.writerprivateurl,
                        property = model.writerproperty
                    };
                    db.Writers.Add(wr);
                }

                db.SaveChanges();
            }

            return Json(new {success = true});
        }

        public ActionResult DeleteWriter(Writer wr)
        {
            var _todelete = new Writer();
            using (var db = new Context())
            {
                _todelete = db.Writers.Where(u => wr.ID.Contains(u.ID)).FirstOrDefault();
                db.Writers.Remove(_todelete);
                db.SaveChanges();
            }

            return RedirectToAction("WriterManager");
        }
    }
}