using Admin.Data;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class VesselDataService : IVesselDataService
    {
        private ApplicationDbContext context;
        public VesselDataService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<VesselData> VesselData => context.VesselData;

        public IEnumerable<Vessel> Vessels => context.Vessels;

        public void Save(String id, String param, String value)
        {
            VesselData search = context.VesselData.FirstOrDefault(x => x.VesselDataID == id);
            if (search == null)
            {
                VesselData vd = new VesselData();
                vd.VesselDataID = id;
                vd.Date = DateTime.ParseExact(id.Substring(0, 4) + "-" + id.Substring(4, 2) + "-" + id.Substring(6, 2), "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                vd.VesselID = Int32.Parse(id.Substring(8, 2));
                switch (param)
                {
                    case "CargoID":
                        if (value.Length > 0)
                            vd.CargoID = Int32.Parse(value);
                        break;
                    case "Position":
                        vd.Position = value;
                        break;
                    case "NextPort":
                        vd.NextPort = value;
                        break;
                    case "ETANextPort":
                        vd.ETANextPort = value;
                        break;
                    case "WindSpeedDirection":
                        vd.WindSpeedDirection = value;
                        break;
                    case "CargoQuantityOnBoard":
                        vd.CargoQuantityOnBoard = Decimal.Parse(value);
                        break;
                    case "BoilOff":
                        vd.BoilOff = Decimal.Parse(value);
                        break;
                    case "BunkerROBHFO":
                        vd.BunkerROBHFO = Decimal.Parse(value);
                        break;
                    case "BunkerROBMDO":
                        vd.BunkerROBMDO = Decimal.Parse(value);
                        break;
                    case "BunkerROBMGO":
                        vd.BunkerROBMGO = Decimal.Parse(value);
                        break;
                    case "ConsumpHFO":
                        vd.ConsumpHFO = Decimal.Parse(value);
                        break;
                    case "ConsumpMDO":
                        vd.ConsumpMDO = Decimal.Parse(value);
                        break;
                    case "ConsumpMGO":
                        vd.ConsumpMGO = Decimal.Parse(value);
                        break;
                }
                vd.CreatedOn = DateTime.Now;
                vd.LastUpdated = DateTime.Now;
                context.VesselData.Add(vd);
            }
            else
            {
                switch (param)
                {
                    case "CargoID":
                        if(value.Length > 0)
                            search.CargoID = Int32.Parse(value);
                        break;
                    case "Position":
                        search.Position = value;
                        break;
                    case "NextPort":
                        search.NextPort = value;
                        break;
                    case "ETANextPort":
                        search.ETANextPort = value;
                        break;
                    case "WindSpeedDirection":
                        search.WindSpeedDirection = value;
                        break;
                    case "CargoQuantityOnBoard":
                        search.CargoQuantityOnBoard = Decimal.Parse(value);
                        break;
                    case "BoilOff":
                        search.BoilOff = Decimal.Parse(value);
                        break;
                    case "BunkerROBHFO":
                        search.BunkerROBHFO = Decimal.Parse(value);
                        break;
                    case "BunkerROBMDO":
                        search.BunkerROBMDO = Decimal.Parse(value);
                        break;
                    case "BunkerROBMGO":
                        search.BunkerROBMGO = Decimal.Parse(value);
                        break;
                    case "ConsumpHFO":
                        search.ConsumpHFO = Decimal.Parse(value);
                        break;
                    case "ConsumpMDO":
                        search.ConsumpMDO = Decimal.Parse(value);
                        break;
                    case "ConsumpMGO":
                        search.ConsumpMGO = Decimal.Parse(value);
                        break;
                }
                search.LastUpdated = DateTime.Now;
            }
            context.SaveChanges();
        }

        public List<VesselData> InitiateVesselData(DateTime date)
        {
            String selectedDate = date.ToString("yyyyMMdd");

            List<VesselData> vesselDataList = new List<VesselData>();
            IEnumerable<Vessel> vessels = context.Vessels;
            foreach (Vessel vessel in vessels)
            {
                VesselData vesselData = new VesselData();
                vesselData.VesselDataID = selectedDate + vessel.VesselID.ToString().PadLeft(2, '0');
                vesselData.Date = date;
                vesselData.VesselID = vessel.VesselID;
                VesselData tmp = context.VesselData.FirstOrDefault(x => x.VesselDataID == vesselData.VesselDataID);
                if (tmp != null)
                {
                    vesselData.CargoID = tmp.CargoID;
                    vesselData.Position = tmp.Position;
                    vesselData.NextPort = tmp.NextPort;
                    vesselData.ETANextPort = tmp.ETANextPort;
                    vesselData.WindSpeedDirection = tmp.WindSpeedDirection;
                    vesselData.CargoQuantityOnBoard = tmp.CargoQuantityOnBoard;
                    vesselData.BoilOff = tmp.BoilOff;
                    vesselData.BunkerROBHFO = tmp.BunkerROBHFO;
                    vesselData.BunkerROBMDO = tmp.BunkerROBMDO;
                    vesselData.BunkerROBMGO = tmp.BunkerROBMGO;
                    vesselData.ConsumpHFO = tmp.ConsumpHFO;
                    vesselData.ConsumpMDO = tmp.ConsumpMDO;
                    vesselData.ConsumpMGO = tmp.ConsumpMGO;
                }
                vesselDataList.Add(vesselData);
            }

            return (vesselDataList);
        }
    }
}
