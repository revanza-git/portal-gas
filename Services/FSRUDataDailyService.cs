using Admin.Data;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class FSRUDataDailyService : IFSRUDataDailyService
    {
        private ApplicationDbContext context;
        public FSRUDataDailyService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<FSRUDataDaily> FSRUDataDaily => context.FSRUDataDaily;

        public void Save(String id, String param, String value)
        {
            FSRUDataDaily search = context.FSRUDataDaily.FirstOrDefault(x => x.FSRUDataDailyID == id);
            if (search == null)
            {
                FSRUDataDaily fsru = new FSRUDataDaily();
                fsru.FSRUDataDailyID = id;
                fsru.Date = DateTime.ParseExact(id.Substring(0, 4) + "-" + id.Substring(4, 2) + "-" + id.Substring(6, 2), "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                fsru.FSRUID = Int32.Parse(id.Substring(8, 2));
                switch (param)
                {
                    case "Pressure":
                        fsru.Pressure = Decimal.Parse(value);
                        break;
                    case "Temperature":
                        fsru.Temperature = Decimal.Parse(value);
                        break;
                    case "Rate":
                        fsru.Rate = Decimal.Parse(value);
                        break;
                    case "LNGTankInventory":
                        fsru.LNGTankInventory = Decimal.Parse(value);
                        break;
                    case "BoFM3":
                        fsru.BoFM3 = Decimal.Parse(value);
                        break;
                    case "BoFKg":
                        fsru.BoFKg = Decimal.Parse(value);
                        break;
                    case "DeliveredToORFKg":
                        fsru.DeliveredToORFKg = Decimal.Parse(value);
                        break;
                    case "DeliveredToORFM3":
                        fsru.DeliveredToORFM3 = Decimal.Parse(value);
                        break;
                }
                fsru.CreatedOn = DateTime.Now;
                fsru.LastUpdated = DateTime.Now;
                context.FSRUDataDaily.Add(fsru);
            }
            else
            {
                switch (param)
                {
                    case "Pressure":
                        search.Pressure = Decimal.Parse(value);
                        break;
                    case "Temperature":
                        search.Temperature = Decimal.Parse(value);
                        break;
                    case "Rate":
                        search.Rate = Decimal.Parse(value);
                        break;
                    case "LNGTankInventory":
                        search.LNGTankInventory = Decimal.Parse(value);
                        break;
                    case "BoFM3":
                        search.BoFM3 = Decimal.Parse(value);
                        break;
                    case "BoFKg":
                        search.BoFKg = Decimal.Parse(value);
                        break;
                    case "DeliveredToORFKg":
                        search.DeliveredToORFKg = Decimal.Parse(value);
                        break;
                    case "DeliveredToORFM3":
                        search.DeliveredToORFM3 = Decimal.Parse(value);
                        break;
                }
                search.LastUpdated = DateTime.Now;
            }
            context.SaveChanges();
        }

        public FSRUDataDaily InitiateFSRUDaily(DateTime date)
        {
            String selectedDate = date.ToString("yyyyMMdd");
            int line = 1;

            FSRUDataDaily fsruData = new FSRUDataDaily();
            fsruData.FSRUDataDailyID = selectedDate + line.ToString().PadLeft(2, '0');
            fsruData.FSRUID = line;
            FSRUDataDaily tmp = context.FSRUDataDaily.FirstOrDefault(x => x.FSRUDataDailyID == fsruData.FSRUDataDailyID);
            if (tmp != null)
            {
                fsruData.Temperature = tmp.Temperature;
                fsruData.Pressure = tmp.Pressure;
                fsruData.Rate = tmp.Rate;
                fsruData.LNGTankInventory = tmp.LNGTankInventory;
                fsruData.BoFKg = tmp.BoFKg;
                fsruData.BoFM3 = tmp.BoFM3;
                fsruData.DeliveredToORFKg = tmp.DeliveredToORFKg;
                fsruData.DeliveredToORFM3 = tmp.DeliveredToORFM3;
            }

            return (fsruData);
        }
    }
}
