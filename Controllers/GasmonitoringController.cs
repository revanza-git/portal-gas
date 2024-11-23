using Admin.Interfaces.Services;
using Admin.Models.Gasmon;
using Admin.Models.Tugboat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Admin.Controllers
{
    [Authorize]
    public class GasmonitoringController : Controller
    {
        private IORFDataService orfDataService;
        private IFSRUDataService fsruDataService;
        private IFSRUDataDailyService fsruDataDailyService;
        private IORFDataDailyService orfDataDailyService;
        private ITUGBoatsDataService tugBoatsDataService;
        private IVesselDataService vesselDataService;
        private IGasmonActivityService gasmonActivityService;
        private IGasmonParameterService gasmonParameterService;

        public GasmonitoringController(IORFDataService _orfDataService, IFSRUDataService _fsruDataService, IFSRUDataDailyService _fsruDataDailyService, IORFDataDailyService _orfDataDailyService, ITUGBoatsDataService _tugBoatsDataService, IVesselDataService _vesselDataService, IGasmonActivityService _gasmonActivityService,IGasmonParameterService _gasmonParameterService)
        {
            this.orfDataService = _orfDataService;
            this.fsruDataService = _fsruDataService;
            this.fsruDataDailyService = _fsruDataDailyService;
            this.orfDataDailyService = _orfDataDailyService;
            this.tugBoatsDataService = _tugBoatsDataService;
            this.vesselDataService = _vesselDataService;
            this.gasmonActivityService = _gasmonActivityService;
            this.gasmonParameterService = _gasmonParameterService;
        }

        public ViewResult EntryData()
        {
            ViewData["Title"] = "Entry Data";
            return View("EntryData");
        }

        public ViewResult SetupParameter()
        {
            ViewData["Title"] = "Setup Parameter Dashboard";
            Int32 tahun = DateTime.Now.Year;
            gasmonParameterService.InitParams(tahun);
            List<GasmonParameter> paramaters = gasmonParameterService.GasmonParameter.Where(x => x.Tahun == tahun).ToList();
            ViewData["cargo"] = gasmonParameterService.Cargo.Where(x => x.Tahun == tahun).ToList();
            return View("SetupParameter",paramaters);
        }

        public ViewResult GetParams()
        {
            Int32 tahun = DateTime.Now.Year;
            List<GasmonParameter> paramaters = gasmonParameterService.GasmonParameter.Where(x => x.Tahun == tahun).ToList();
            ViewData["cargo"] = gasmonParameterService.Cargo.Where(x => x.Tahun == tahun).ToList();

            return View("Params",paramaters);
        }

        String MonthlyReport()
        {
            return "Under Construction";
        }

        public ViewResult ManagementDashboard()
        {
            Int32 tahun = DateTime.Now.Year;
            DateTime today = DateTime.Now.Date;
            gasmonParameterService.InitParams(tahun);
            ViewData["Title"] = "Management Dashboard";
            ViewData["target_pasokan"] = gasmonParameterService.Cargo.Where(x => x.Tahun == tahun && x.IsTarget == 1).Count();
            ViewData["realisasi_pasokan"] = (Decimal) vesselDataService.VesselData.Where(x => x.Date.Year == tahun && x.CargoID > 0).GroupBy(x => x.CargoID).Count();
            ViewData["target_penjualan"] = gasmonParameterService.GasmonParameter.FirstOrDefault(x => x.ParameterID == "target_penjualan" && x.Tahun == tahun).Value;
            ViewData["realisasi_penjualan"] = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun).Sum(x => x.DailyEnergy);
            ViewData["target_bog"] = gasmonParameterService.GasmonParameter.FirstOrDefault(x => x.ParameterID == "target_bog" && x.Tahun == tahun).Value;
            ViewData["realisasi_bog"] = fsruDataDailyService.FSRUDataDaily.Where(x => x.Date.Year == tahun).Sum(x => x.BoFM3) + vesselDataService.VesselData.Where(x => x.Date.Year == tahun).Sum(x => x.BoilOff);
            DateTime StartDate = new DateTime(tahun, 1, 1);
            DateTime EndDate = today;
            ViewData["startDate"] = StartDate;
            ViewData["endDate"] = EndDate;
            ViewData["jsonPasokan"] = getPasokanByPeriod(StartDate, EndDate);
            ViewData["jsonPenjualan"] = getPenjualanByPeriod(StartDate, EndDate);

            return View("ManagementDashboard");
        }

        public ViewResult OperationalDashboard()
        {
            Int32 tahun = DateTime.Now.Year;
            DateTime today = DateTime.Now.Date;
            gasmonParameterService.InitParams(tahun);
            ViewData["Title"] = "Operational Dashboard";
            ViewData["target_pasokan"] = gasmonParameterService.Cargo.Where(x => x.Tahun == tahun && x.IsTarget == 1).Count();
            ViewData["realisasi_pasokan"] = (Decimal)vesselDataService.VesselData.Where(x => x.Date.Year == tahun && x.CargoID > 0).GroupBy(x => x.CargoID).Count();
            ViewData["target_penjualan"] = gasmonParameterService.GasmonParameter.FirstOrDefault(x => x.ParameterID == "target_penjualan" && x.Tahun == tahun).Value;
            ViewData["realisasi_penjualan"] = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun).Sum(x => x.DailyEnergy);
            ViewData["realisasi_penjualan1"] = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 1).Sum(x => x.DailyEnergy);
            ViewData["realisasi_penjualan2"] = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 2).Sum(x => x.DailyEnergy);
            ViewData["realisasi_penjualan3"] = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 3).Sum(x => x.DailyEnergy);
            ViewData["realisasi_penjualan4"] = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 4).Sum(x => x.DailyEnergy);
            ViewData["jsonDailyEnergy"] = getDailyEnergyData();

            ViewData["orfData"] = (List<ORFData>) orfDataService.ORFData.Where(x => x.Date == today).ToList();
            ViewData["orfDataDaily"] = (List<ORFDataDaily>) orfDataDailyService.ORFDataDaily.Where(x => x.Date == today).ToList();

            DateTime StartDate = new DateTime(tahun, 1, 1);
            DateTime EndDate = today;
            ViewData["startDate"] = StartDate;
            ViewData["endDate"] = EndDate;
            ViewData["jsonROB"] = getROBByPeriod(StartDate, EndDate);
            ViewData["jsonBOG"] = getBOGByPeriod(StartDate, EndDate);
            ViewData["jsonRegasRate"] = getRegasRateByPeriod(StartDate, EndDate);
            return View("OperationalDashboard");
        }

        public ViewResult GetContent(String date)
        {
            String[] dates = date.Split('/');
            DateTime dt = new DateTime(Int32.Parse(dates[2]),Int32.Parse(dates[1]),Int32.Parse(dates[0])).Date;
            DateTime yesterday = dt.AddDays(-1);
            DateTime yesterday2 = dt.AddDays(-2);
            List<ORFData> orfList = orfDataService.InitiateORF(dt).ToList();
            List<FSRUData> fsruList = fsruDataService.InitiateFSRU(dt).ToList();
            FSRUDataDaily fsruDaily = fsruDataDailyService.InitiateFSRUDaily(dt);
            List<ORFDataDaily> orfDaily = orfDataDailyService.InitiateORFDaily(dt);
            List<ORFData> orfYesterday = orfDataService.ORFData.Where(x => x.Time == "24:00" && x.Date == yesterday).ToList();
            List<ORFData> orfYesterday2 = orfDataService.ORFData.Where(x => x.Time == "24:00" && x.Date == yesterday2).ToList();
            List<TUGBoatsData> tugList = tugBoatsDataService.InitiateTUGBoatsData(dt);
            List<Boat> boatList = tugBoatsDataService.Boats.ToList();
            List<VesselData> vdList = vesselDataService.InitiateVesselData(dt);
            List<Vessel> vesselList = vesselDataService.Vessels.ToList();
            List<GasmonActivity> activityList = gasmonActivityService.GasmonActivity.Where(x => x.Date == dt).ToList();
            List<Cargo> cargoList = gasmonParameterService.Cargo.Where(x => x.Tahun == dt.Year).ToList();

            Decimal InitialRate = 0;
            Decimal CummulativeRate = InitialRate + fsruDataDailyService.FSRUDataDaily.Where(x => x.Date < dt).Sum(x => x.Rate);
            ViewData["FSRU"] = fsruList;
            ViewData["ORF"] = orfList;
            ViewData["ORFYesterday"] = orfYesterday;
            ViewData["ORFYesterday2"] = orfYesterday2;
            ViewData["FSRUDaily"] = fsruDaily;
            ViewData["ORFDaily"] = orfDaily;
            ViewData["FSRUCummulativeRate"] = CummulativeRate;
            ViewData["TUGBoats"] = tugList;
            ViewData["Boats"] = boatList;
            ViewData["VesselD"] = vdList;
            ViewData["Vessel"] = vesselList;
            ViewData["Activity"] = activityList;
            ViewData["Cargo"] = cargoList;

            return View("Content");
        }

        public Decimal GetVolumeBefore(String id)
        {
            Decimal result = 0;

            DateTime dt = DateTime.ParseExact(id.Substring(0, 4) + "-" + id.Substring(4, 2) + "-" + id.Substring(6, 2), "yyyy-MM-dd",
                                     System.Globalization.CultureInfo.InvariantCulture);
            Int32 time = Int32.Parse(id.Substring(8, 2));
            if (time == 1)
            {
                dt = dt.AddDays(-1);
                time = 24;
            }
            else
            {
                time = time - 1;
            }

            string idBefore = dt.ToString("yyyyMMdd") + time.ToString().PadLeft(2, '0') + "00" + id.Substring(12, 2);
            try
            {
                result = orfDataService.ORFData.FirstOrDefault(x => x.ORFDataID == idBefore).Volume;
            } catch (Exception) { }

            return result;
        }

        public void UpdateData(String id,String type,String param,String value)
        {
            switch (type)
            {
                case "FSRU":
                    fsruDataService.Save(id, param, value);
                    break;
                case "ORF":
                    orfDataService.Save(id, param, value);
                    break;
                case "FSRUDaily":
                    fsruDataDailyService.Save(id, param, value);
                    break;
                case "ORFDaily":
                    orfDataDailyService.Save(id, param, value);
                    break;
                case "TUGBoats":
                    tugBoatsDataService.Save(id, param, value);
                    break;
                case "Vessel":
                    vesselDataService.Save(id, param, value);
                    break;
            }
        }

        public String AddActivity(Int32 source, String time, String remark)
        {
            DateTime dt = DateTime.Now.Date;
            gasmonActivityService.Save(source,time,remark);
            List<GasmonActivity> acts = gasmonActivityService.GasmonActivity.Where(x => x.Date == dt && x.Source == source).ToList();
            String html = "";
            foreach(GasmonActivity act in acts)
            {
                html += "<tr>";
                html += "<td class='activity'>"+act.Time+"</td><td class='activity'>"+ act.Remark+ "</td><td><button onclick=\"DeleteActivity('"+act.ActivityID+"')\"><i class=\"fa fa-minus\"></i></button></td>";
                html += "</tr>";
            }
            return html;
        }

        public String DeleteActivity(Int32 id)
        {
            JsonResponse resp = new JsonResponse();
            String json;

            GasmonActivity act = gasmonActivityService.GasmonActivity.FirstOrDefault(x => x.ActivityID == id);
            if(act == null)
            {
                resp.Status = false;
                json = JsonConvert.SerializeObject(resp);
                return json;
            }

            gasmonActivityService.Delete(act);

            resp.Status = true;
            if(act.Source == 1)
            {
                resp.Container = "act_fsru_content";
            }
            else if(act.Source == 2)
            {
                resp.Container = "act_orf_content";
            }
            else if(act.Source >= 61 && act.Source <=79)
            {
                resp.Container = "act_carrier"+act.Source+"_content";
            }
            else if(act.Source >= 81 && act.Source <= 99)
            {
                resp.Container = "act_tug" + act.Source + "_content";
            }
            else
            {
                resp.Status = false;
                json = JsonConvert.SerializeObject(resp);
                return json;
            }

            String html = "";
            List<GasmonActivity> acts = gasmonActivityService.GasmonActivity.Where(x => x.Date == act.Date && x.Source == act.Source).ToList();
            foreach (GasmonActivity a in acts)
            {
                html += "<tr>";
                html += "<td class='activity'>" + a.Time + "</td><td class='activity'>" + a.Remark + "</td><td><button onclick=\"DeleteActivity('" + a.ActivityID + "')\"><i class=\"fa fa-minus\"></i></button></td>";
                html += "</tr>";
            }
            resp.Content = html;
            json = JsonConvert.SerializeObject(resp);
            return json;
        }

        public void UpdateParameter(String id,String value)
        {
            gasmonParameterService.Save(id,Int32.Parse(value));
        }

        public String AddCargo(String code, String date,int isTarget)
        {
            DateTime dt = DateTime.Now.Date;
            Cargo cargo = new Cargo();
            String[] tgl = date.Split('/');
            cargo.Date = new DateTime(Int32.Parse(tgl[2]), Int32.Parse(tgl[1]), Int32.Parse(tgl[0]));
            cargo.Tahun = cargo.Date.Year;
            cargo.IsTarget = isTarget;
            cargo.Code = code;
            cargo.CreatedOn = DateTime.Now;
            cargo.LastUpdated = DateTime.Now;
            gasmonParameterService.AddCargo(cargo);

            List<Cargo> cargos = gasmonParameterService.Cargo.Where(x => x.Tahun == cargo.Tahun).ToList();
            String html = "";
            String IsTarget = "";
            foreach (Cargo c in cargos)
            {
                IsTarget = c.IsTarget == 1 ? "checked" : "";
                html += "<tr>";
                html += "<td class=''>" + c.Tahun + "</td><td class=''>" + c.Code + "</td><td class=''>" + c.Date.ToString("dd/MM/yyyy") + "</td><td><input "+ IsTarget +" type='checkbox' disabled/></td><td><button onclick=\"deleteCargo('"+ c.CargoID +"')\"><i class=\"fa fa-minus\"></i></button></td>";
                html += "</tr>";
            }
            return html;
        }

        public String DeleteCargo(Int32 id)
        {
            int tahun = DateTime.Now.Year;
            Cargo cargo = gasmonParameterService.Cargo.FirstOrDefault(x => x.CargoID == id);
            if (cargo != null)
            {
                gasmonParameterService.DeleteCargo(cargo);
                tahun = cargo.Tahun;
            }

            List<Cargo> cargos = gasmonParameterService.Cargo.Where(x => x.Tahun == tahun).ToList();
            String html = "";
            String IsTarget = "";
            foreach (Cargo c in cargos)
            {
                IsTarget = c.IsTarget == 1 ? "checked" : "";
                html += "<tr>";
                html += "<td class=''>" + c.Tahun + "</td><td class=''>" + c.Code + "</td><td class=''>" + c.Date.ToString("dd/MM/yyyy") + "</td><td><input "+ IsTarget+" type='checkbox' disabled></td><td><button onclick=\"deleteCargo('" + c.CargoID + "')\"><i class=\"fa fa-minus\"></i></button></td>";
                html += "</tr>";
            }
            return html;
        }

        public String[] getDailyEnergyData()
        {
            String[] jsons = new String[5];
           
            Int32 StartYear = DateTime.Now.Year - 1;
            DateTime startDate = new DateTime(StartYear, 1, 1);
            DateTime endDate = DateTime.Now.Date;

            List<ORFDataDaily> orfs = orfDataDailyService.ORFDataDaily.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();

            for (Int32 LineID = 1; LineID <= 4; LineID++)
            {
                List<GasmonGraph1> graphs = new List<GasmonGraph1>();
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    GasmonGraph1 graph = new GasmonGraph1();
                    graph.date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10);
                    ORFDataDaily orf = orfs.FirstOrDefault(x => x.Date == dt && x.LineID == LineID);
                    graph.value = orf == null ? 0 : orf.DailyEnergy;
                    graphs.Add(graph);
                }
                jsons[LineID] = JsonConvert.SerializeObject(graphs);
            }
            return jsons;
        }

        public String getROBByPeriod(DateTime startDate,DateTime endDate)
        {
            String json = "";
            Int32 diffDays = (endDate - startDate).Days;
            List<FSRUDataDaily> fsrus = fsruDataDailyService.FSRUDataDaily.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();

            List<GasmonGraph1> graphs = new List<GasmonGraph1>();
            if (diffDays <= 31)
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    GasmonGraph1 graph = new GasmonGraph1();
                    graph.date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10);
                    FSRUDataDaily fsru = fsrus.FirstOrDefault(x => x.Date == dt);
                    graph.value = fsru == null ? 0 : fsru.LNGTankInventory;
                    graphs.Add(graph);
                }
            }
            else
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                {
                    GasmonGraph1 graph = new GasmonGraph1();
                    graph.date = dt.Date.ToString("yyyy-MM").Substring(0, 7);
                    Decimal value = fsrus.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).Sum(y => y.LNGTankInventory);
                    graph.value = value;
                    graphs.Add(graph);
                }
            }
            json = JsonConvert.SerializeObject(graphs);

            return json;
        }

        public String getBOGByPeriod(DateTime startDate, DateTime endDate)
        {
            String json = "";
            Int32 diffDays = (endDate - startDate).Days;
            List<FSRUDataDaily> fsrus = fsruDataDailyService.FSRUDataDaily.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            List<VesselData> vds = vesselDataService.VesselData.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            List<GasmonGraph1> graphs = new List<GasmonGraph1>();
            if (diffDays <= 31)
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    GasmonGraph1 graph = new GasmonGraph1();
                    graph.date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10);
                    FSRUDataDaily fsru = fsrus.FirstOrDefault(x => x.Date == dt);
                    VesselData vd = vds.FirstOrDefault(x => x.Date == dt);
                    graph.value = (fsru == null ? 0 : fsru.BoFM3) + (vd == null ? 0 : vd.BoilOff);
                    graphs.Add(graph);
                }
            }
            else
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                {
                    GasmonGraph1 graph = new GasmonGraph1();
                    graph.date = dt.Date.ToString("yyyy-MM").Substring(0, 7);
                    Decimal value = fsrus.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).Sum(y => y.BoFM3);
                    graph.value = value;
                    graphs.Add(graph);
                }
            }
            json = JsonConvert.SerializeObject(graphs);

            return json;
        }

        public String getRegasRateByPeriod(DateTime startDate, DateTime endDate)
        {
            String json = "";
            Int32 diffDays = (endDate - startDate).Days;
            List<FSRUDataDaily> fsrus = fsruDataDailyService.FSRUDataDaily.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();

            List<GasmonGraph1> graphs = new List<GasmonGraph1>();
            if (diffDays <= 31)
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    GasmonGraph1 graph = new GasmonGraph1();
                    graph.date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10);
                    FSRUDataDaily fsru = fsrus.FirstOrDefault(x => x.Date == dt);
                    graph.value = fsru == null ? 0 : fsru.Rate;
                    graphs.Add(graph);
                }
            }
            else
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                {
                    GasmonGraph1 graph = new GasmonGraph1();
                    graph.date = dt.Date.ToString("yyyy-MM").Substring(0, 7);
                    Decimal value = fsrus.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).Sum(y => y.Rate);
                    graph.value = value;
                    graphs.Add(graph);
                }
            }
            json = JsonConvert.SerializeObject(graphs);

            return json;
        }

        public String getPasokanByPeriod(DateTime startDate, DateTime endDate)
        {
            String json = "";
            Int32 diffDays = (endDate - startDate).Days;
            List<VesselData> vds = vesselDataService.VesselData.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();

            List<GasmonGraph3> graphs = new List<GasmonGraph3>();
            if (diffDays <= 31)
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    GasmonGraph3 graph = new GasmonGraph3();
                    graph.date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10);
                    //VesselData vd = fsrus.FirstOrDefault(x => x.Date == dt);
                    graph.target = 10;
                    graph.realisasi = 5;
                    graphs.Add(graph);
                }
            }
            else
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                {
                    GasmonGraph3 graph = new GasmonGraph3();
                    graph.date = dt.Date.ToString("yyyy-MM").Substring(0, 7);
                    //Decimal value = fsrus.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).Sum(y => y.Rate);
                    graph.target = 10;
                    graph.realisasi = 5;
                    graphs.Add(graph);
                }
            }
            json = JsonConvert.SerializeObject(graphs);

            return json;
        }

        public String getPenjualanByPeriod(DateTime startDate, DateTime endDate)
        {
            String json = "";
            Int32 diffDays = (endDate - startDate).Days;
            List<ORFDataDaily> orfs = orfDataDailyService.ORFDataDaily.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();

            List<GasmonGraph3> graphs = new List<GasmonGraph3>();
            if (diffDays <= 31)
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    GasmonGraph3 graph = new GasmonGraph3();
                    graph.date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10);
                    try
                    {
                        graph.realisasi = orfs.FirstOrDefault(x => x.Date == dt).DailyEnergy;
                    }
                    catch (Exception)
                    {
                        graph.realisasi = 0;
                    }
                    graph.target = gasmonParameterService.GasmonParameter.FirstOrDefault(x => x.Tahun == dt.Year && x.ParameterID == "target_penjualan").Value / 365;
                    graphs.Add(graph);
                }
            }
            else
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                {
                    GasmonGraph3 graph = new GasmonGraph3();
                    graph.date = dt.Date.ToString("yyyy-MM").Substring(0, 7);
                    try
                    {
                        graph.realisasi = orfs.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).Sum(y => y.DailyEnergy);
                    }
                    catch(Exception)
                    {
                        graph.realisasi = 0;
                    }
                    graph.target = gasmonParameterService.GasmonParameter.FirstOrDefault(x => x.Tahun == dt.Year && x.ParameterID == "target_penjualan").Value / 12;
                    graphs.Add(graph);
                }
            }
            json = JsonConvert.SerializeObject(graphs);

            return json;
        }

        public String getDashboardData(int tahun)
        {
            String json;
            GasmonGraph2 graph = new GasmonGraph2();

            graph.target_pasokan = gasmonParameterService.Cargo.Where(x => x.Tahun == tahun).Count();
            graph.realisasi_pasokan = vesselDataService.VesselData.Where(x => x.Date.Year == tahun && x.CargoID > 0).GroupBy(x => x.CargoID).Count();

            if (graph.target_pasokan == 0)
                graph.prosentase_pasokan = 0;
            else
                graph.prosentase_pasokan = graph.realisasi_pasokan * 100 / graph.target_pasokan;

            try
            {
                graph.target_penjualan = gasmonParameterService.GasmonParameter.FirstOrDefault(x => x.ParameterID == "target_penjualan" && x.Tahun == tahun).Value;
            }
            catch (Exception)
            {
                graph.target_penjualan = 0;
            }

            try
            {
                graph.realisasi_penjualan = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun).Sum(x => x.DailyEnergy);
            } 
            catch(Exception)
            {
                graph.realisasi_penjualan = 0;
            }

            if (graph.target_penjualan == 0)
                graph.prosentase_penjualan = 0;
            else
                graph.prosentase_penjualan = (int) Math.Round(graph.realisasi_penjualan * 100 / graph.target_penjualan);

            try
            {
                graph.realisasi_penjualan1 = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 1).Sum(x => x.DailyEnergy);
            }
            catch (Exception)
            {
                graph.realisasi_penjualan1 = 0;
            }
            try
            {
                graph.realisasi_penjualan2 = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 2).Sum(x => x.DailyEnergy);
            }
            catch (Exception)
            {
                graph.realisasi_penjualan2 = 0;
            }
            try
            {
                graph.realisasi_penjualan3 = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 3).Sum(x => x.DailyEnergy);
            }
            catch (Exception)
            {
                graph.realisasi_penjualan3 = 0;
            }
            try
            {
                graph.realisasi_penjualan4 = orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 4).Sum(x => x.DailyEnergy);
            }
            catch(Exception)
            {
                graph.realisasi_penjualan4 = 0;
            }

            json = JsonConvert.SerializeObject(graph);

            return json;
        }
    }
}
