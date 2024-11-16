using Admin.Data;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class TUGBoatsDataService : ITUGBoatsDataService
    {
        private ApplicationDbContext context;
        public TUGBoatsDataService(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        
        public IEnumerable<TUGBoatsData> TUGBoatsData => context.TUGBoatsData;

        public IEnumerable<Boat> Boats => context.Boats;

        public void Save(String id, String param, String value)
        {
            TUGBoatsData search = context.TUGBoatsData.FirstOrDefault(x => x.TUGBoatsDataID == id);
            if (search == null)
            {
                TUGBoatsData tgu = new TUGBoatsData();
                tgu.TUGBoatsDataID = id;
                tgu.Date = DateTime.ParseExact(id.Substring(0, 4) + "-" + id.Substring(4, 2) + "-" + id.Substring(6, 2), "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                tgu.BoatID = Int32.Parse(id.Substring(8, 2));
                switch (param)
                {
                    case "FuelOilConsumption":
                        tgu.FuelOilConsumption = Decimal.Parse(value);
                        break;
                    case "FuelOilROB":
                        tgu.FuelOilROB = Decimal.Parse(value);
                        break;
                }
                tgu.CreatedOn = DateTime.Now;
                tgu.LastUpdated = DateTime.Now;
                context.TUGBoatsData.Add(tgu);
            }
            else
            {
                switch (param)
                {
                    case "FuelOilConsumption":
                        search.FuelOilConsumption = Decimal.Parse(value);
                        break;
                    case "FuelOilROB":
                        search.FuelOilROB = Decimal.Parse(value);
                        break;
                }
                search.LastUpdated = DateTime.Now;
            }
            context.SaveChanges();
        }

        public List<TUGBoatsData> InitiateTUGBoatsData(DateTime date)
        {
            String selectedDate = date.ToString("yyyyMMdd");

            List<TUGBoatsData> tugDataList = new List<TUGBoatsData>();
            IEnumerable<Boat> boats = context.Boats;
            foreach(Boat boat in boats)
            {
                TUGBoatsData tugData = new TUGBoatsData();
                tugData.TUGBoatsDataID = selectedDate + boat.BoatID.ToString().PadLeft(2, '0');
                tugData.Date = date;
                tugData.BoatID = boat.BoatID;
                TUGBoatsData tmp = context.TUGBoatsData.FirstOrDefault(x => x.TUGBoatsDataID == tugData.TUGBoatsDataID);
                if (tmp != null)
                {
                    tugData.FuelOilConsumption = tmp.FuelOilConsumption;
                    tugData.FuelOilROB = tmp.FuelOilROB;
                }
                tugDataList.Add(tugData);
            }
            
            return (tugDataList);
        }
        
    }
}
