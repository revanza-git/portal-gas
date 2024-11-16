using Admin.Data;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class ORFDataDailyService : IORFDataDailyService
    {
        private ApplicationDbContext context;
        public ORFDataDailyService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<ORFDataDaily> ORFDataDaily => context.ORFDataDaily;

        public void Save(String id, String param, String value)
        {
            ORFDataDaily search = context.ORFDataDaily.FirstOrDefault(x => x.ORFDataDailyID == id);
            if (search == null)
            {
                ORFDataDaily orf = new ORFDataDaily();
                orf.ORFDataDailyID = id;
                orf.Date = DateTime.ParseExact(id.Substring(0, 4) + "-" + id.Substring(4, 2) + "-" + id.Substring(6, 2), "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                orf.LineID = Int32.Parse(id.Substring(8, 2));
                switch (param)
                {
                    case "Pressure":
                        orf.Pressure = Decimal.Parse(value);
                        break;
                    case "Temperature":
                        orf.Temperature = Decimal.Parse(value);
                        break;
                    case "DailyNet":
                        orf.DailyNet = Decimal.Parse(value);
                        break;
                    case "HeatingValue":
                        orf.HeatingValue = Decimal.Parse(value);
                        break;
                    case "DailyEnergy":
                        orf.DailyEnergy = Decimal.Parse(value);
                        break;
                    case "CO2":
                        orf.CO2 = Decimal.Parse(value);
                        break;
                    case "Ethane":
                        orf.Ethane = Decimal.Parse(value);
                        break;
                    case "Methane":
                        orf.Methane = Decimal.Parse(value);
                        break;
                    case "Nitrogen":
                        orf.Nitrogen = Decimal.Parse(value);
                        break;
                    case "Propane":
                        orf.Propane = Decimal.Parse(value);
                        break;
                    case "Water":
                        orf.Water = Decimal.Parse(value);
                        break;
                    case "iPentane":
                        orf.iPentane = Decimal.Parse(value);
                        break;
                    case "nPentane":
                        orf.nPentane = Decimal.Parse(value);
                        break;
                    case "iButane":
                        orf.iButane = Decimal.Parse(value);
                        break;
                    case "nButane":
                        orf.nButane = Decimal.Parse(value);
                        break;
                    case "nHexane":
                        orf.nHexane = Decimal.Parse(value);
                        break;
                }
                orf.CreatedOn = DateTime.Now;
                orf.LastUpdated = DateTime.Now;
                context.ORFDataDaily.Add(orf);
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
                    case "DailyNet":
                        search.DailyNet = Decimal.Parse(value);
                        break;
                    case "HeatingValue":
                        search.HeatingValue = Decimal.Parse(value);
                        break;
                    case "DailyEnergy":
                        search.DailyEnergy = Decimal.Parse(value);
                        break;
                    case "CO2":
                        search.CO2 = Decimal.Parse(value);
                        break;
                    case "Ethane":
                        search.Ethane = Decimal.Parse(value);
                        break;
                    case "Methane":
                        search.Methane = Decimal.Parse(value);
                        break;
                    case "Nitrogen":
                        search.Nitrogen = Decimal.Parse(value);
                        break;
                    case "Propane":
                        search.Propane = Decimal.Parse(value);
                        break;
                    case "Water":
                        search.Water = Decimal.Parse(value);
                        break;
                    case "iPentane":
                        search.iPentane = Decimal.Parse(value);
                        break;
                    case "nPentane":
                        search.nPentane = Decimal.Parse(value);
                        break;
                    case "iButane":
                        search.iButane = Decimal.Parse(value);
                        break;
                    case "nButane":
                        search.nButane = Decimal.Parse(value);
                        break;
                    case "nHexane":
                        search.nHexane = Decimal.Parse(value);
                        break;
                }
                search.LastUpdated = DateTime.Now;
            }
            context.SaveChanges();
        }

        public List<ORFDataDaily> InitiateORFDaily(DateTime date)
        {
            String selectedDate = date.ToString("yyyyMMdd");
            
            List<ORFDataDaily> orfDataList = new List<ORFDataDaily>();
            for (int line = 1; line <= 4; line++)
            {
                ORFDataDaily orfData = new ORFDataDaily();
                orfData.ORFDataDailyID = selectedDate + line.ToString().PadLeft(2, '0');
                orfData.LineID = line;
                ORFDataDaily tmp = context.ORFDataDaily.FirstOrDefault(x => x.ORFDataDailyID == orfData.ORFDataDailyID);
                if (tmp != null)
                {
                    orfData.Temperature = tmp.Temperature;
                    orfData.Pressure = tmp.Pressure;
                    orfData.DailyNet = tmp.DailyNet;
                    orfData.HeatingValue = tmp.HeatingValue;
                    orfData.DailyEnergy = tmp.DailyEnergy;
                    orfData.CO2 = tmp.CO2;
                    orfData.Ethane = tmp.Ethane;
                    orfData.Methane = tmp.Methane;
                    orfData.Nitrogen = tmp.Nitrogen;
                    orfData.Propane = tmp.Propane;
                    orfData.Water = tmp.Water;
                    orfData.iPentane = tmp.iPentane;
                    orfData.nPentane = tmp.nPentane;
                    orfData.iButane = tmp.iButane;
                    orfData.nButane = tmp.nButane;
                    orfData.nHexane = tmp.nHexane;
                }
                orfDataList.Add(orfData);
            }
            return (orfDataList);
        }
    }
}
