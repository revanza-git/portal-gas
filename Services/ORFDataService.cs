using Admin.Data;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class ORFDataService : IORFDataService
    {
        private ApplicationDbContext context;
        public ORFDataService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<ORFData> ORFData => context.ORFData;

        public void Save(String id,String param,String value)
        {
           ORFData search = context.ORFData.FirstOrDefault(x => x.ORFDataID == id);
            if(search == null)
            {
                ORFData orf = new ORFData();
                orf.ORFDataID = id;
                orf.Date = DateTime.ParseExact(id.Substring(0,4)+"-"+id.Substring(4,2)+"-"+id.Substring(6,2), "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                orf.Time = id.Substring(8, 2) + ":" + id.Substring(10, 2);
                orf.LineID = Int32.Parse(id.Substring(12, 2));
                switch (param)
                {
                    case "Volume":
                        orf.Volume = Decimal.Parse(value);
                        break;
                    case "VolumeA":
                        orf.VolumeA = Decimal.Parse(value);
                        break;
                    case "VolumeB":
                        orf.VolumeB = Decimal.Parse(value);
                        break;
                    case "VolumeC":
                        orf.VolumeC = Decimal.Parse(value);
                        break;
                    case "Flow":
                        orf.Flow = Decimal.Parse(value);
                        break;
                    case "FlowA":
                        orf.FlowA = Decimal.Parse(value);
                        break;
                    case "FlowB":
                        orf.FlowB = Decimal.Parse(value);
                        break;
                    case "FlowC":
                        orf.FlowC = Decimal.Parse(value);
                        break;
                    case "GHV":
                        orf.GHV = Decimal.Parse(value);
                        break;
                    case "Temperature":
                        orf.Temperature = Decimal.Parse(value);
                        break;
                }
                orf.CreatedOn = DateTime.Now;
                orf.LastUpdated = DateTime.Now;
                context.ORFData.Add(orf);
            } else { 
                switch (param)
                {
                    case "Volume":
                        search.Volume = Decimal.Parse(value);
                        break;
                    case "VolumeA":
                        search.VolumeA = Decimal.Parse(value);
                        break;
                    case "VolumeB":
                        search.VolumeB = Decimal.Parse(value);
                        break;
                    case "VolumeC":
                        search.VolumeC = Decimal.Parse(value);
                        break;
                    case "Flow":
                        search.Flow = Decimal.Parse(value);
                        break;
                    case "FlowA":
                        search.FlowA = Decimal.Parse(value);
                        break;
                    case "FlowB":
                        search.FlowB = Decimal.Parse(value);
                        break;
                    case "FlowC":
                        search.FlowC = Decimal.Parse(value);
                        break;
                    case "GHV":
                        search.GHV = Decimal.Parse(value);
                        break;
                    case "Temperature":
                        search.Temperature = Decimal.Parse(value);
                        break;
                }
                search.LastUpdated = DateTime.Now;
            }
            context.SaveChanges();
        }

        public IEnumerable<ORFData> InitiateORF(DateTime date)
        {
            String selectedDate = date.ToString("yyyyMMdd");
            Console.WriteLine("Date:"+selectedDate);
            List<ORFData> orfList = new List<ORFData>();

            List<ORFData> orfListLine = context.ORFData.Where(x => x.Date == date).ToList();

            for (int line = 1; line <= 4; line++)
            {
                for (int time = 1; time <= 24; time++)
                {
                    ORFData orfData = new ORFData();
                    orfData.ORFDataID = selectedDate + time.ToString().PadLeft(2, '0') + "00" + line.ToString().PadLeft(2, '0');
                    orfData.LineID = line;
                    orfData.Time = time.ToString().PadLeft(2, '0') + ".00";
                    ORFData tmp = orfListLine.FirstOrDefault(x => x.ORFDataID == orfData.ORFDataID);
                    if (tmp != null)
                    {
                        if (line == 3)
                        {
                            orfData.VolumeA = tmp.VolumeA;
                            orfData.VolumeB = tmp.VolumeB;
                            orfData.VolumeC = tmp.VolumeC;
                            orfData.FlowA = tmp.FlowA;
                            orfData.FlowB = tmp.FlowB;
                            orfData.FlowC = tmp.FlowC;
                            orfData.GHV = tmp.GHV;
                            orfData.Temperature = tmp.Temperature;
                        }
                        else
                        {
                            orfData.Volume = tmp.Volume;
                            orfData.Flow = tmp.Flow;
                            orfData.GHV = tmp.GHV;
                            orfData.Temperature = tmp.Temperature;
                        }
                    }
                    orfList.Add(orfData);
                }
            }
            return (orfList);
        }
    }
}
