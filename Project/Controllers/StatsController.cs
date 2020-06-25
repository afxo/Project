using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core;
using Core.Models;
using Project.Models;

namespace Project.Controllers
{
    public class StatsController : Controller
    {
        // GET: Stats
        public ActionResult Statistics()
        {
            var stac = new StatisticsModel();
            using (var db = new Context())
            {
                stac.pubs = db.Publishments.ToList();
                stac.articles = db.Articles.ToList();
                stac.books = db.Books.ToList();
                stac.inbooks = db.Inbooks.ToList();
                stac.inproceedings = db.Inproceedings.ToList();
                stac.dictionary = YearsDictionary(db, db.Publishments.ToList(), stac);
                stac.piedict = TimePeriodDictionary(db, db.Publishments.ToList(), stac.dictionary);
                stac.unitdict = DepartmentStats();
                stac.average = ReturnAverage(stac);
            }

            return View(stac);
        }

        private int ReturnAverage(StatisticsModel stac)
        {
            var counter = 0;
            foreach (var item in stac.articles)
                counter = counter + item.author.Split(',').Length - 1;
            foreach (var item in stac.books)
                counter = counter + item.publisher.Split(',').Length - 1;
            foreach (var item in stac.inbooks)
                counter = counter + item.publisher.Split(',').Length - 1;
            foreach (var item in stac.inproceedings)
                counter = counter + item.editor.Split(',').Length - 1;

            return counter / stac.pubs.Count;
        }

        public Dictionary<string, int> DepartmentStats()
        {
            var restcoutner = 0;
            var unitdict = new Dictionary<string, int>();
            using (var db = new Context())
            {
                foreach (var unit in db.Units.ToList())
                {
                    unitdict.Add(unit.name, 0);
                    foreach (var pub in db.Publishments.ToList())
                        if (pub.unit_that_belongs == unit.ID)
                        {
                            restcoutner++;
                            unitdict[unit.name] += 1;
                        }
                }

                unitdict.Add("Ανεξάρτητες Δημοσιεύσεις", db.Publishments.ToList().Count - restcoutner);
            }

            return unitdict;
        }

        private Dictionary<string, int> TimePeriodDictionary(Context db, List<Publishment> toList,
            Dictionary<string, int> dict)
        {
            var piedict = new Dictionary<string, int>
            {
                {"2000-2004", 0},
                {"2005-2009", 0},
                {"2010-2014", 0},
                {"2015-2020", 0}
            };
            foreach (var index in dict)
                try
                {
                    if (int.Parse(index.Key) < 2005)
                        piedict["2000-2004"] += index.Value;
                    else if (int.Parse(index.Key) < 2010)
                        piedict["2005-2009"] += index.Value;
                    else if (uint.Parse(index.Key) < 2015)
                        piedict["2010-2014"] += index.Value;
                    else
                        piedict["2015-2020"] += index.Value;
                }
                catch (Exception e)
                {
                    continue;
                }


            return piedict;
        }

        private Dictionary<string, int> YearsDictionary(Context db, List<Publishment> items, StatisticsModel stac)
        {
            var resultDict = db.Publishments.GroupBy(f => f.year).Select(g => new {year = g.Key, count = g.Count()})
                .ToDictionary(k => k.year, i => i.count);
            return resultDict;
        }
    }
}