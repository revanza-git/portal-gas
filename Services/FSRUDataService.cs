using Admin.Data;
using Admin.Interfaces.Services;
using Admin.Models.Gasmon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class FSRUDataService : IFSRUDataService
    {
        private ApplicationDbContext context;
        public FSRUDataService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<FSRUData> FSRUData => context.FSRUData;

        public void Save(String id, String param, String value)
        {
            FSRUData search = context.FSRUData.FirstOrDefault(x => x.FSRUDataID == id);
            if (search == null)
            {
                FSRUData fsru = new FSRUData();
                fsru.FSRUDataID = id;
                fsru.Date = DateTime.ParseExact(id.Substring(0, 4) + "-" + id.Substring(4, 2) + "-" + id.Substring(6, 2), "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                fsru.Time = id.Substring(8, 2) + ":" + id.Substring(10, 2);
                fsru.FSRUID = Int32.Parse(id.Substring(12, 2));
                switch (param)
                {
                    case "Pressure":
                        fsru.Pressure = Decimal.Parse(value);
                        break;
                    case "Temperature":
                        fsru.Temperature = Decimal.Parse(value);
                        break;
                    case "Flow1":
                        fsru.Flow1 = Decimal.Parse(value);
                        break;
                    case "Flow2":
                        fsru.Flow2 = Decimal.Parse(value);
                        break;
                    case "RobLNG":
                        fsru.RobLNG = Decimal.Parse(value);
                        fsru.MMSCF = Decimal.Parse(value) / 47;
                        break;
                }
                fsru.CreatedOn = DateTime.Now;
                fsru.LastUpdated = DateTime.Now;
                context.FSRUData.Add(fsru);
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
                    case "Flow1":
                        search.Flow1 = Decimal.Parse(value);
                        break;
                    case "Flow2":
                        search.Flow2 = Decimal.Parse(value);
                        break;
                    case "RobLNG":
                        search.RobLNG = Decimal.Parse(value);
                        search.MMSCF = Decimal.Parse(value) / 47;
                        break;
                }
                search.LastUpdated = DateTime.Now;
            }
            context.SaveChanges();
        }

        public IEnumerable<FSRUData> InitiateFSRU(DateTime date)
        {
            String selectedDate = date.ToString("yyyyMMdd");
            Console.WriteLine("Date:" + selectedDate);
            List<FSRUData> fsruList = new List<FSRUData>();

            List<FSRUData> fsruListLine = context.FSRUData.Where(x => x.Date == date).ToList();

            for (int line = 1; line <= 1; line++)
            {
                for (int time = 1; time <= 24; time++)
                {
                    FSRUData fsruData = new FSRUData();
                    fsruData.FSRUDataID = selectedDate + time.ToString().PadLeft(2, '0') + "00" + line.ToString().PadLeft(2, '0');
                    fsruData.FSRUID = line;
                    fsruData.Time = time.ToString().PadLeft(2, '0') + ".00";
                    FSRUData tmp = fsruListLine.FirstOrDefault(x => x.FSRUDataID == fsruData.FSRUDataID);
                    if (tmp != null)
                    {
                        fsruData.Temperature = tmp.Temperature;
                        fsruData.Pressure = tmp.Pressure;
                        fsruData.Flow1 = tmp.Flow1;
                        fsruData.Flow2 = tmp.Flow2;
                        fsruData.RobLNG = tmp.RobLNG;
                        fsruData.MMSCF = tmp.MMSCF;
                    }
                    fsruList.Add(fsruData);
                }
            }
            return (fsruList);
        }
    }
}
