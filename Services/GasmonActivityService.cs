using Admin.Data;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class GasmonActivityService : IGasmonActivityService
    {
        private ApplicationDbContext context;
        public GasmonActivityService(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        
        public IEnumerable<GasmonActivity> GasmonActivity => context.GasmonActivity;
        
        public void Save(Int32 source, String time, String remark)
        {
            GasmonActivity act = new GasmonActivity();
            act.Source = source;
            act.Date = DateTime.Now.Date;
            act.Time = time;
            act.Remark = remark;
            act.CreatedOn = DateTime.Now;
            act.LastUpdated = DateTime.Now;
            context.GasmonActivity.Add(act);
            context.SaveChanges();
        }

        public void Delete(GasmonActivity act)
        {
            context.GasmonActivity.Remove(act);
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
