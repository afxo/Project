using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Core;
using Core.Models;
using Project.Models;

namespace Project.Controllers
{
    public class PublishedController : Controller
    {
        private readonly List<ArticleModel> articles = new List<ArticleModel>();
        private readonly List<BookModel> books = new List<BookModel>();
        private readonly List<InBookModel> inBooks = new List<InBookModel>();
        private readonly List<IncollectionModel> incollections = new List<IncollectionModel>();
        private readonly List<InProceedingModel> inproceedings = new List<InProceedingModel>();

        private readonly List<ProceedingModel> proceedings = new List<ProceedingModel>();

        //First we save the published to a list
        private readonly List<string> publishedElements = new List<string>();
        private readonly List<string> publishedValues = new List<string>();


        // GET: Published
        public ActionResult NewPublishment()
        {
            return View();
        }

        public ActionResult PubManager()
        {
            var owned = new List<Publishment>();

            using (var db = new Context())
            {
                var id = db.UserRoles.First(sh => sh.username == User.Identity.Name).ID;
                owned = db.Publishments.Where(sh => sh.uploadedby == id).ToList();
            }

            return View(owned);
        }

        public JsonResult AnalyzeXML(string link)
        {
            try
            {
                var x = XElement.Load(link);
                var contactNodes = x.Descendants().ToList();
                AnalyzeNodes(contactNodes);
                return Json(new
                {
                    articles,
                    proceedings,
                    inproceedings,
                    incollections,
                    books,
                    inbooks = inBooks
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new {success = false});
            }
        }

        private void AnalyzeNodes(List<XElement> contactNodes)
        {
            foreach (var i in contactNodes)
                if (i.Name == "r")
                {
                    var inside = i.NextNode;
                    //Console.WriteLine(inside.ToString());
                    var elements = (inside as XElement).Descendants().ToList();
                    foreach (var z in elements)
                    {
                        publishedElements.Add(z.Name.ToString());
                        publishedValues.Add(z.Value);
                    }

                    publishedElements.Add("end");
                    publishedValues.Add("end");
                }

            for (var i = 0; i < publishedElements.Count(); i++)
            {
                if (publishedElements[i].StartsWith("proceedings"))
                {
                    var pr = new ProceedingModel();
                    do
                    {
                        i++;
                        if (publishedElements[i].StartsWith("editor")) pr.author += "," + publishedValues[i];

                        if (publishedElements[i].StartsWith("title")) pr.title = publishedValues[i];

                        if (publishedElements[i].StartsWith("booktitle")) pr.booktitle = publishedValues[i];

                        if (publishedElements[i].StartsWith("volume")) pr.volume = "" + publishedValues[i];

                        if (publishedElements[i].StartsWith("series")) pr.series += "" + publishedValues[i];

                        if (publishedElements[i].StartsWith("isbn")) pr.ISBN += "" + publishedValues[i];
                    } while (publishedElements[i] != "end");

                    proceedings.Add(pr);
                }

                if (publishedElements[i].StartsWith("inproceedings"))
                {
                    var inpr = new InProceedingModel();

                    do
                    {
                        i++;
                        if (publishedElements[i].StartsWith("author")) inpr.author += "," + publishedValues[i];

                        if (publishedElements[i].StartsWith("number")) inpr.number += "," + publishedValues[i];

                        if (publishedElements[i].StartsWith("series")) inpr.series += "," + publishedValues[i];

                        if (publishedElements[i].StartsWith("title")) inpr.title = publishedValues[i];

                        if (publishedElements[i].StartsWith("booktitle")) inpr.booktitle += publishedValues[i];

                        if (publishedElements[i].StartsWith("volume")) inpr.volume += "" + publishedValues[i];

                        if (publishedElements[i].StartsWith("pages")) inpr.pages += "" + publishedValues[i];

                        if (publishedElements[i].StartsWith("ee")) inpr.ee += "" + publishedValues[i];

                        if (publishedElements[i].StartsWith("url")) inpr.url += publishedValues[i];
                        if (publishedElements[i].StartsWith("year")) inpr.year += "" + publishedValues[i];
                    } while (publishedElements[i] != "end");

                    inproceedings.Add(inpr);
                }

                if (publishedElements[i].StartsWith("incollection"))
                {
                    var incol = new IncollectionModel();

                    do
                    {
                        i++;
                        if (publishedElements[i].StartsWith("author")) incol.author += "," + publishedValues[i];

                        if (publishedElements[i].StartsWith("booktitle")) incol.booktitle += "" + publishedValues[i];

                        if (publishedElements[i].StartsWith("title")) incol.title += "" + publishedValues[i];

                        if (publishedElements[i].StartsWith("pages")) incol.pages += "" + publishedValues[i];

                        if (publishedElements[i].StartsWith("ee")) incol.ee += "" + publishedValues[i];

                        if (publishedElements[i].StartsWith("url")) incol.url += "" + publishedValues[i];

                        if (publishedElements[i].StartsWith("year")) incol.year += "" + publishedValues[i];
                    } while (publishedElements[i] != "end");

                    incollections.Add(incol);
                }

                if (publishedElements[i].StartsWith("article"))
                {
                    var article = new ArticleModel();

                    do
                    {
                        i++;
                        if (publishedElements[i].StartsWith("author")) article.author += "," + publishedValues[i];

                        if (publishedElements[i].StartsWith("title")) article.title = publishedValues[i];

                        if (publishedElements[i].StartsWith("pages")) article.pages += publishedValues[i];

                        if (publishedElements[i].StartsWith("volume")) article.volume += publishedValues[i];

                        if (publishedElements[i].StartsWith("journal")) article.journal += publishedValues[i];

                        if (publishedElements[i].StartsWith("ee")) article.ee += publishedValues[i];

                        if (publishedElements[i].StartsWith("url")) article.url += publishedValues[i];

                        if (publishedElements[i].StartsWith("year")) article.year += publishedValues[i];
                    } while (publishedElements[i] != "end");

                    articles.Add(article);

                    if (publishedElements[i].StartsWith("book"))
                    {
                        var book = new BookModel();

                        do
                        {
                            i++;
                            if (publishedElements[i].StartsWith("author")) book.author += "," + publishedValues[i];

                            if (publishedElements[i].StartsWith("title")) book.title += publishedValues[i];

                            if (publishedElements[i].StartsWith("volume")) book.volume += publishedValues[i];

                            if (publishedElements[i].StartsWith("publisher")) book.publisher += publishedValues[i];

                            if (publishedElements[i].StartsWith("number")) book.number += publishedValues[i];

                            if (publishedElements[i].StartsWith("year")) book.year += publishedValues[i];

                            if (publishedElements[i].StartsWith("edition")) book.edition += publishedValues[i];

                            if (publishedElements[i].StartsWith("address")) book.address += publishedValues[i];

                            if (publishedElements[i].StartsWith("month")) book.month += publishedValues[i];

                            if (publishedElements[i].StartsWith("note")) book.issn += publishedValues[i];
                            if (publishedElements[i].StartsWith("isbn")) book.isbn += publishedValues[i];
                            if (publishedElements[i].StartsWith("issn")) book.issn += publishedValues[i];
                        } while (publishedElements[i] != "end");

                        books.Add(book);
                    }
                }

                if (publishedElements[i].StartsWith("inbook"))
                {
                    var inbook = new InBookModel();

                    do
                    {
                        i++;
                        if (publishedElements[i].StartsWith("author")) inbook.author += "," + publishedValues[i];

                        if (publishedElements[i].StartsWith("title")) inbook.title = publishedValues[i];

                        if (publishedElements[i].StartsWith("publisher")) inbook.publisher += " " + publishedValues[i];

                        if (publishedElements[i].StartsWith("year")) inbook.year += " " + publishedValues[i];

                        if (publishedElements[i].StartsWith("chapter")) inbook.chapter += " " + publishedValues[i];
                        if (publishedElements[i].StartsWith("volume")) inbook.volume += " " + publishedValues[i];
                        if (publishedElements[i].StartsWith("isbn")) inbook.isbn += " " + publishedValues[i];
                        if (publishedElements[i].StartsWith("issn")) inbook.issn += " " + publishedValues[i];
                    } while (publishedElements[i] != "end");

                    inBooks.Add(inbook);
                }
            }
        }

        public JsonResult AddToDataBase(string link)
        {
            var counter = 0;
            AnalyzeXML(link);
            using (var db = new Context())
            {
                foreach (var item in articles)
                    if (PublishmenExists(item.title))
                    {
                        var pub = new Publishment
                        {
                            ID = Guid.NewGuid().ToString(),
                            type = "article",
                            title = item.title,
                            year = item.year,
                            uploadedby = db.UserRoles.Where(sh => sh.username.Contains(User.Identity.Name)).First().ID
                        };
                        counter++;
                        try
                        {
                            pub.unit_that_belongs = db.UnitMembers.First(sh => sh.Username.Contains(User.Identity.Name))
                                .ClassID;
                        }
                        catch (Exception e)
                        {
                            pub.unit_that_belongs = "independent";
                        }

                        var ar = new Article
                        {
                            ID = Guid.NewGuid().ToString(),
                            givenID = pub.ID,
                            mag_name = item.journal,
                            volume = item.volume,
                            pages = item.pages,
                            author = item.author
                        };
                        db.Publishments.Add(pub);
                        db.Articles.Add(ar);
                        db.SaveChanges();
                        AddPublshers(item.author);
                    }

                foreach (var item in books)
                    if (PublishmenExists(item.title))
                    {
                        counter++;

                        var pub = new Publishment
                        {
                            ID = Guid.NewGuid().ToString(),
                            type = "book",
                            title = item.title,
                            year = item.year,
                            uploadedby = db.UserRoles.Where(sh => sh.username.Contains(User.Identity.Name)).First().ID
                        };
                        counter++;
                        try
                        {
                            pub.unit_that_belongs = db.UnitMembers.First(sh => sh.Username.Contains(User.Identity.Name))
                                .ClassID;
                        }
                        catch (Exception e)
                        {
                            pub.unit_that_belongs = "independent";
                        }


                        var book = new Book
                        {
                            ID = Guid.NewGuid().ToString(),
                            givenID = pub.ID,
                            publisher = item.publisher,
                            volume = item.volume,
                            number = int.Parse(item.number),
                            series = item.series,
                            address = item.series,
                            edition = item.edition,
                            ISBN = item.isbn,
                            ISSN = item.issn
                        };
                        db.Books.Add(book);
                        db.Publishments.Add(pub);
                        db.SaveChanges();
                        AddPublshers(item.author);
                    }

                foreach (var item in inproceedings)
                    if (PublishmenExists(item.title))
                    {
                        counter++;

                        var pub = new Publishment
                        {
                            ID = Guid.NewGuid().ToString(),
                            type = "inproceeding",
                            title = item.title,
                            year = item.year,
                            uploadedby = db.UserRoles.Where(sh => sh.username.Contains(User.Identity.Name)).First().ID
                        };
                        counter++;
                        try
                        {
                            pub.unit_that_belongs = db.UnitMembers.First(sh => sh.Username.Contains(User.Identity.Name))
                                .ClassID;
                        }
                        catch (Exception e)
                        {
                            pub.unit_that_belongs = "independent";
                        }


                        var inpr = new Inproceeding
                        {
                            ID = Guid.NewGuid().ToString(),
                            givenID = pub.ID,
                            booktitle = item.booktitle,
                            editor = item.author,
                            pages = item.pages,
                            address = item.ee,
                            publisher = item.author
                        };
                        db.Publishments.Add(pub);
                        db.Inproceedings.Add(inpr);
                        db.SaveChanges();
                        AddPublshers(item.author);
                    }

                foreach (var item in inBooks)
                    if (PublishmenExists(item.title))
                    {
                        counter++;

                        var pub = new Publishment
                        {
                            ID = Guid.NewGuid().ToString(),
                            type = "inbook",
                            title = item.title,
                            year = item.year,
                            uploadedby = db.UserRoles.Where(sh => sh.username.Contains(User.Identity.Name)).First().ID
                        };
                        counter++;
                        try
                        {
                            pub.unit_that_belongs = db.UnitMembers.First(sh => sh.Username.Contains(User.Identity.Name))
                                .ClassID;
                        }
                        catch (Exception e)
                        {
                            pub.unit_that_belongs = "independent";
                        }


                        var inb = new Inbook
                        {
                            ID = Guid.NewGuid().ToString(),
                            givenID = pub.ID,
                            publisher = item.publisher,
                            chapter = item.chapter,
                            volume = item.volume,
                            ISBN = item.isbn,
                            ISSN = item.issn
                        };
                        db.Publishments.Add(pub);
                        db.Inbooks.Add(inb);
                        db.SaveChanges();
                        AddPublshers(item.author);
                    }
            }

            return Json(new {counter});
        }

        private void AddPublshers(string author)
        {
            var authors = author.Split(',');
            using (var db = new Context())
            {
                foreach (var a in authors)
                {
                    if (a == "") continue;
                    var name = a.Split(' ')[0];
                    var surrname = "";
                    try
                    {
                        surrname = a.Split(' ')[1];
                    }
                    catch (Exception E)
                    {
                        surrname = "";
                    }

                    if (EditorExists(name, surrname))
                    {
                        var wr = new Writer
                        {
                            name = name, surrname = surrname, ID = Guid.NewGuid().ToString(), orchidURL = "",
                            property = "", privateURL = ""
                        };
                        db.Writers.Add(wr);
                        db.SaveChanges();
                    }
                }
            }
        }

        private bool EditorExists(string name, string surrname)
        {
            using (var db = new Context())
            {
                try
                {
                    var same = db.Writers.Where(sh => name == sh.name && surrname == sh.surrname).First();
                    return false;
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }


        private bool PublishmenExists(string item)
        {
            using (var db = new Context())
            {
                try
                {
                    var same = db.Publishments.Where(sh => item == sh.title).First();
                    return false;
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }

        public JsonResult ReloadResults(string key)
        {
            var _temp = new List<Publishment>();
            using (var db = new Context())
            {
                _temp = db.Publishments.Where(sh => sh.title.Contains(key) || sh.year.Contains(key)).ToList();
            }

            return Json(new {results = _temp.Distinct().ToList()}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OpenPage(string title)
        {
            var pub = new PublishmentModel();
            using (var db = new Context())
            {
                var publishment = db.Publishments.Where(sh => sh.title == title).First();
                switch (publishment.type)
                {
                    case "article":
                        var article = db.Articles.Where(sh => sh.givenID == publishment.ID).First();
                        pub.medianame = article.mag_name;
                        pub.title = publishment.title;
                        pub.year = publishment.year;
                        pub.volume = article.volume;
                        pub.pages = article.pages;
                        pub.authors = article.author;
                        pub.adrresses = "-";
                        pub.ISBN = "-";
                        pub.ISSN = "-";
                        pub.chapter = "-";
                        pub.type = publishment.type;
                        pub.id = article.givenID;
                        pub.publihser_name = db.UserRoles.Where(sh => sh.ID == publishment.uploadedby).First().username;
                        try
                        {
                            pub.unit = db.Units.First(sh => sh.ID == publishment.unit_that_belongs).name;
                        }
                        catch (Exception e)
                        {
                            pub.unit = "Δεν ανήκει σε κάποια μονάδα";
                        }

                        break;
                    case "inproceeding":
                        var inproceeding = db.Inproceedings.Where(sh => sh.givenID == publishment.ID).First();
                        pub.authors = inproceeding.editor;
                        pub.medianame = inproceeding.booktitle;
                        pub.year = publishment.year;
                        pub.volume = "-";
                        pub.pages = inproceeding.pages;
                        pub.authors = inproceeding.publisher;
                        pub.adrresses = inproceeding.address;
                        pub.type = publishment.type;
                        pub.title = publishment.title;
                        pub.ISBN = "-";
                        pub.ISSN = "-";
                        pub.chapter = "-";
                        pub.id = inproceeding.givenID;
                        pub.publihser_name = db.UserRoles.Where(sh => sh.ID == publishment.uploadedby).First().username;
                        try
                        {
                            pub.unit = db.Units.First(sh => sh.ID == publishment.unit_that_belongs).name;
                        }
                        catch (Exception e)
                        {
                            pub.unit = "Δεν ανήκει σε κάποια μονάδα";
                        }

                        break;
                    case "inbook":
                        var inbook = db.Inbooks.First(sh => sh.givenID == publishment.ID);
                        pub.authors = inbook.publisher;
                        pub.medianame = "-";
                        pub.year = publishment.year;
                        pub.volume = inbook.volume;
                        pub.pages = "-";
                        pub.adrresses = "-";
                        pub.type = publishment.type;
                        pub.title = publishment.title;
                        pub.ISBN = inbook.ISBN;
                        pub.ISSN = inbook.ISSN;
                        pub.chapter = inbook.chapter;
                        pub.id = inbook.givenID;
                        pub.publihser_name = db.UserRoles.Where(sh => sh.ID == publishment.uploadedby).First().username;

                        try
                        {
                            pub.unit = db.Units.First(sh => sh.ID == publishment.unit_that_belongs).name;
                        }
                        catch (Exception e)
                        {
                            pub.unit = "Δεν ανήκει σε κάποια μονάδα";
                        }

                        break;
                    case "book":
                        var book = db.Books.First(sh => sh.givenID == publishment.ID);
                        pub.authors = book.publisher;
                        pub.medianame = "-";
                        pub.year = publishment.year;
                        pub.volume = book.volume;
                        pub.pages = "-";
                        pub.adrresses = "-";
                        pub.type = publishment.type;
                        pub.title = publishment.title;
                        pub.ISBN = book.ISBN;
                        pub.ISSN = book.ISSN;
                        pub.chapter = "-";
                        pub.id = book.givenID;
                        pub.publihser_name = db.UserRoles.Where(sh => sh.ID == publishment.uploadedby).First().username;

                        try
                        {
                            pub.unit = db.Units.First(sh => sh.ID == publishment.unit_that_belongs).name;
                        }
                        catch (Exception e)
                        {
                            pub.unit = "Δεν ανήκει σε κάποια μονάδα";
                        }

                        break;
                }

                return View(pub);
            }
        }

        public JsonResult UpdatePublishment(PublishmentModel model)
        {
            using (var db = new Context())
            {
                switch (model.type)
                {
                    case "article":
                        var article = db.Articles.First(sh => sh.givenID.Contains(model.id));
                        article.author = model.authors;
                        article.volume = model.volume;
                        article.pages = model.pages;
                        article.mag_name = model.medianame;
                        db.Articles.AddOrUpdate(article);
                        break;
                    case "inproceeding":
                        var inproceeding = db.Inproceedings.First(sh => sh.givenID.Contains(model.id));
                        inproceeding.publisher = model.authors;
                        inproceeding.pages = model.pages;
                        inproceeding.address = model.adrresses;
                        inproceeding.booktitle = model.medianame;
                        db.Inproceedings.AddOrUpdate(inproceeding);
                        break;
                    case "book":
                        var book = db.Books.First(sh => sh.givenID.Contains(model.id));
                        book.publisher = model.authors;
                        book.volume = model.volume;
                        book.ISBN = model.ISBN;
                        book.ISSN = model.ISSN;
                        break;
                    case "inbook":
                        var inbook = db.Inbooks.First(sh => sh.givenID.Contains(model.id));
                        inbook.publisher = model.authors;
                        inbook.volume = model.volume;
                        inbook.ISBN = model.ISBN;
                        inbook.ISSN = model.ISSN;
                        inbook.chapter = model.chapter;
                        break;
                }

                var pub = db.Publishments.First(sh => sh.title.Contains(model.title));
                pub.year = model.year;
                db.Publishments.AddOrUpdate(pub);
                db.SaveChanges();
            }

            return Json(new {success = true});
        }

        public JsonResult DeletePublishment(PublishmentModel model)
        {
            using (var db = new Context())
            {
                var pub = db.Publishments.First(sh => sh.title == model.title);
                db.Publishments.Remove(pub);
                switch (model.type)
                {
                    case "article":
                        var ar = db.Articles.First(sh => sh.givenID.Contains(model.id));
                        db.Articles.Remove(ar);
                        break;
                    case "inproceeding":
                        var inpr = db.Inproceedings.First(sh => sh.givenID.Contains(model.id));
                        db.Inproceedings.Remove(inpr);
                        break;
                    case "book":
                        var book = db.Books.First(sh => sh.givenID.Contains(model.id));
                        db.Books.Remove(book);
                        break;
                    case "inbook":
                        var inbook = db.Inbooks.First(sh => sh.givenID.Contains(model.id));
                        db.Inbooks.Remove(inbook);
                        break;
                }

                db.SaveChanges();
                return Json(new {success = true});
            }
        }

        public JsonResult ManualAdd(PublishmentModel model)
        {
            using (var db = new Context())
            {
                if (PublishmenExists(model.title))
                {
                    var pub = new Publishment
                    {
                        ID = Guid.NewGuid().ToString(),
                        uploadedby = db.UserRoles.First(sh => sh.username == User.Identity.Name).ID,
                        year = model.year,
                        type = model.type,
                        title = model.title
                    };
                    try
                    {
                        pub.unit_that_belongs = db.UnitMembers.First(sh => sh.Username.Contains(User.Identity.Name))
                            .ClassID;
                    }
                    catch (Exception e)
                    {
                        pub.unit_that_belongs = "independent";
                    }

                    db.Publishments.Add(pub);
                    db.SaveChanges();

                    switch (model.type)
                    {
                        case "article":
                            var ar = new Article
                            {
                                ID = Guid.NewGuid().ToString(),
                                author = model.authors,
                                givenID = pub.ID,
                                mag_name = model.medianame,
                                pages = model.pages,
                                volume = model.volume
                            };
                            db.Articles.Add(ar);
                            db.SaveChanges();
                            break;
                        case "inproceeding":
                            var inrp = new Inproceeding
                            {
                                ID = Guid.NewGuid().ToString(),
                                publisher = model.authors,
                                address = model.adrresses,
                                booktitle = model.medianame,
                                editor = model.authors,
                                givenID = pub.ID,
                                pages = model.pages
                            };
                            db.Inproceedings.Add(inrp);
                            db.SaveChanges();
                            break;
                        case "book":
                            var book = new Book
                            {
                                ID = Guid.NewGuid().ToString(),
                                publisher = model.authors,
                                address = model.adrresses,
                                edition = "",
                                givenID = pub.ID,
                                ISBN = model.ISBN,
                                ISSN = model.ISSN,
                                number = 0,
                                series = "",
                                volume = model.volume
                            };
                            db.Books.Add(book);
                            db.SaveChanges();
                            break;
                        case "inbook":
                        {
                            var inbook = new Inbook
                            {
                                ID = Guid.NewGuid().ToString(),
                                publisher = model.authors,
                                givenID = pub.ID,
                                ISBN = model.ISBN,
                                ISSN = model.ISSN,
                                chapter = model.chapter,
                                volume = model.volume
                            };
                            db.Inbooks.Add(inbook);
                            db.SaveChanges();
                            break;
                        }
                    }

                    AddPublshers(model.authors);
                }
            }

            return Json(new {success = true});
        }
    }
}