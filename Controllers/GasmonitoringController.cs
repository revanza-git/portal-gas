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
        private readonly IORFDataService _orfDataService;
        private readonly IFSRUDataService _fsruDataService;
        private readonly IFSRUDataDailyService _fsruDataDailyService;
        private readonly IORFDataDailyService _orfDataDailyService;
        private readonly ITUGBoatsDataService _tugBoatsDataService;
        private readonly IVesselDataService _vesselDataService;
        private readonly IGasmonActivityService _gasmonActivityService;
        private readonly IGasmonParameterService _gasmonParameterService;

        public GasmonitoringController(
            IORFDataService orfDataService,
            IFSRUDataService fsruDataService,
            IFSRUDataDailyService fsruDataDailyService,
            IORFDataDailyService orfDataDailyService,
            ITUGBoatsDataService tugBoatsDataService,
            IVesselDataService vesselDataService,
            IGasmonActivityService gasmonActivityService,
            IGasmonParameterService gasmonParameterService)
        {
            _orfDataService = orfDataService;
            _fsruDataService = fsruDataService;
            _fsruDataDailyService = fsruDataDailyService;
            _orfDataDailyService = orfDataDailyService;
            _tugBoatsDataService = tugBoatsDataService;
            _vesselDataService = vesselDataService;
            _gasmonActivityService = gasmonActivityService;
            _gasmonParameterService = gasmonParameterService;
        }

        public ViewResult EntryData()
        {
            ViewData["Title"] = "Entry Data";
            return View("EntryData");
        }

        public ViewResult SetupParameter()
        {
            ViewData["Title"] = "Setup Parameter Dashboard";
            int year = DateTime.Now.Year;
            _gasmonParameterService.InitParams(year);
            var parameters = _gasmonParameterService.GasmonParameter.Where(x => x.Tahun == year).ToList();
            ViewData["cargo"] = _gasmonParameterService.Cargo.Where(x => x.Tahun == year).ToList();
            return View("SetupParameter", parameters);
        }

        public ViewResult GetParams()
        {
            int year = DateTime.Now.Year;
            var parameters = _gasmonParameterService.GasmonParameter.Where(x => x.Tahun == year).ToList();
            ViewData["cargo"] = _gasmonParameterService.Cargo.Where(x => x.Tahun == year).ToList();
            return View("Params", parameters);
        }

        private string MonthlyReport()
        {
            return "Under Construction";
        }

        public ViewResult ManagementDashboard()
        {
            int year = DateTime.Now.Year;
            DateTime today = DateTime.Now.Date;
            _gasmonParameterService.InitParams(year);
            ViewData["Title"] = "Management Dashboard";
            ViewData["target_pasokan"] = _gasmonParameterService.Cargo.Count(x => x.Tahun == year && x.IsTarget == 1);
            ViewData["realisasi_pasokan"] = _vesselDataService.VesselData.Count(x => x.Date.Year == year && x.CargoID > 0);
            ViewData["target_penjualan"] = GetParameterValue("target_penjualan", year);
            ViewData["realisasi_penjualan"] = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == year).Sum(x => x.DailyEnergy);
            ViewData["target_bog"] = GetParameterValue("target_bog", year);
            ViewData["realisasi_bog"] = _fsruDataDailyService.FSRUDataDaily.Where(x => x.Date.Year == year).Sum(x => x.BoFM3) + _vesselDataService.VesselData.Where(x => x.Date.Year == year).Sum(x => x.BoilOff);
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = today;
            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;
            ViewData["jsonPasokan"] = GetPasokanByPeriod(startDate, endDate);
            ViewData["jsonPenjualan"] = GetPenjualanByPeriod(startDate, endDate);

            return View("ManagementDashboard");
        }

        public ViewResult OperationalDashboard()
        {
            int year = DateTime.Now.Year;
            DateTime today = DateTime.Now.Date;
            _gasmonParameterService.InitParams(year);
            ViewData["Title"] = "Operational Dashboard";
            ViewData["target_pasokan"] = _gasmonParameterService.Cargo.Count(x => x.Tahun == year && x.IsTarget == 1);
            ViewData["realisasi_pasokan"] = _vesselDataService.VesselData.Count(x => x.Date.Year == year && x.CargoID > 0);
            ViewData["target_penjualan"] = GetParameterValue("target_penjualan", year);
            ViewData["realisasi_penjualan"] = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == year).Sum(x => x.DailyEnergy);
            ViewData["realisasi_penjualan1"] = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == year && x.LineID == 1).Sum(x => x.DailyEnergy);
            ViewData["realisasi_penjualan2"] = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == year && x.LineID == 2).Sum(x => x.DailyEnergy);
            ViewData["realisasi_penjualan3"] = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == year && x.LineID == 3).Sum(x => x.DailyEnergy);
            ViewData["realisasi_penjualan4"] = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == year && x.LineID == 4).Sum(x => x.DailyEnergy);
            ViewData["jsonDailyEnergy"] = GetDailyEnergyData();

            ViewData["orfData"] = _orfDataService.ORFData.Where(x => x.Date == today).ToList();
            ViewData["orfDataDaily"] = _orfDataDailyService.ORFDataDaily.Where(x => x.Date == today).ToList();

            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = today;
            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;
            ViewData["jsonROB"] = GetROBByPeriod(startDate, endDate);
            ViewData["jsonBOG"] = GetBOGByPeriod(startDate, endDate);
            ViewData["jsonRegasRate"] = GetRegasRateByPeriod(startDate, endDate);
            return View("OperationalDashboard");
        }

        public ViewResult GetContent(string date)
        {
            DateTime dt = ParseDate(date);
            DateTime yesterday = dt.AddDays(-1);
            DateTime yesterday2 = dt.AddDays(-2);
            var orfList = _orfDataService.InitiateORF(dt).ToList();
            var fsruList = _fsruDataService.InitiateFSRU(dt).ToList();
            var fsruDaily = _fsruDataDailyService.InitiateFSRUDaily(dt);
            var orfDaily = _orfDataDailyService.InitiateORFDaily(dt);
            var orfYesterday = _orfDataService.ORFData.Where(x => x.Time == "24:00" && x.Date == yesterday).ToList();
            var orfYesterday2 = _orfDataService.ORFData.Where(x => x.Time == "24:00" && x.Date == yesterday2).ToList();
            var tugList = _tugBoatsDataService.InitiateTUGBoatsData(dt);
            var boatList = _tugBoatsDataService.Boats.ToList();
            var vdList = _vesselDataService.InitiateVesselData(dt);
            var vesselList = _vesselDataService.Vessels.ToList();
            var activityList = _gasmonActivityService.GasmonActivity.Where(x => x.Date == dt).ToList();
            var cargoList = _gasmonParameterService.Cargo.Where(x => x.Tahun == dt.Year).ToList();

            decimal initialRate = 0;
            decimal cumulativeRate = initialRate + _fsruDataDailyService.FSRUDataDaily.Where(x => x.Date < dt).Sum(x => x.Rate);
            ViewData["FSRU"] = fsruList;
            ViewData["ORF"] = orfList;
            ViewData["ORFYesterday"] = orfYesterday;
            ViewData["ORFYesterday2"] = orfYesterday2;
            ViewData["FSRUDaily"] = fsruDaily;
            ViewData["ORFDaily"] = orfDaily;
            ViewData["FSRUCummulativeRate"] = cumulativeRate;
            ViewData["TUGBoats"] = tugList;
            ViewData["Boats"] = boatList;
            ViewData["VesselD"] = vdList;
            ViewData["Vessel"] = vesselList;
            ViewData["Activity"] = activityList;
            ViewData["Cargo"] = cargoList;

            return View("Content");
        }

        public decimal GetVolumeBefore(string id)
        {
            decimal result = 0;
            DateTime dt = ParseDateFromId(id);
            int time = int.Parse(id.Substring(8, 2));
            if (time == 1)
            {
                dt = dt.AddDays(-1);
                time = 24;
            }
            else
            {
                time -= 1;
            }

            string idBefore = dt.ToString("yyyyMMdd") + time.ToString().PadLeft(2, '0') + "00" + id.Substring(12, 2);
            try
            {
                var orfData = _orfDataService.ORFData.FirstOrDefault(x => x.ORFDataID == idBefore);
                if (orfData != null)
                {
                    result = orfData.Volume;
                }
            }
            catch (Exception) { }

            return result;
        }

        public void UpdateData(string id, string type, string param, string value)
        {
            switch (type)
            {
                case "FSRU":
                    _fsruDataService.Save(id, param, value);
                    break;
                case "ORF":
                    _orfDataService.Save(id, param, value);
                    break;
                case "FSRUDaily":
                    _fsruDataDailyService.Save(id, param, value);
                    break;
                case "ORFDaily":
                    _orfDataDailyService.Save(id, param, value);
                    break;
                case "TUGBoats":
                    _tugBoatsDataService.Save(id, param, value);
                    break;
                case "Vessel":
                    _vesselDataService.Save(id, param, value);
                    break;
            }
        }

        public string AddActivity(int source, string time, string remark)
        {
            DateTime dt = DateTime.Now.Date;
            _gasmonActivityService.Save(source, time, remark);
            var activities = _gasmonActivityService.GasmonActivity.Where(x => x.Date == dt && x.Source == source).ToList();
            return GenerateActivityHtml(activities);
        }

        public string DeleteActivity(int id)
        {
            var response = new JsonResponse();
            var activity = _gasmonActivityService.GasmonActivity.FirstOrDefault(x => x.ActivityID == id);
            if (activity == null)
            {
                response.Status = false;
                return JsonConvert.SerializeObject(response);
            }

            _gasmonActivityService.Delete(activity);
            response.Status = true;
            response.Container = GetActivityContainer(activity.Source);
            response.Content = GenerateActivityHtml(_gasmonActivityService.GasmonActivity.Where(x => x.Date == activity.Date && x.Source == activity.Source).ToList());
            return JsonConvert.SerializeObject(response);
        }

        public void UpdateParameter(string id, string value)
        {
            _gasmonParameterService.Save(id, int.Parse(value));
        }

        public string AddCargo(string code, string date, int isTarget)
        {
            var cargo = new Cargo
            {
                Date = ParseDate(date),
                Tahun = DateTime.Now.Year,
                IsTarget = isTarget,
                Code = code,
                CreatedOn = DateTime.Now,
                LastUpdated = DateTime.Now
            };
            _gasmonParameterService.AddCargo(cargo);

            var cargos = _gasmonParameterService.Cargo.Where(x => x.Tahun == cargo.Tahun).ToList();
            return GenerateCargoHtml(cargos);
        }

        public string DeleteCargo(int id)
        {
            var cargo = _gasmonParameterService.Cargo.FirstOrDefault(x => x.CargoID == id);
            if (cargo != null)
            {
                _gasmonParameterService.DeleteCargo(cargo);
            }

            var cargos = _gasmonParameterService.Cargo.Where(x => x.Tahun == DateTime.Now.Year).ToList();
            return GenerateCargoHtml(cargos);
        }

        public string[] GetDailyEnergyData()
        {
            var jsons = new string[5];
            int startYear = DateTime.Now.Year - 1;
            DateTime startDate = new DateTime(startYear, 1, 1);
            DateTime endDate = DateTime.Now.Date;

            var orfs = _orfDataDailyService.ORFDataDaily.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();

            for (int lineId = 1; lineId <= 4; lineId++)
            {
                var graphs = new List<GasmonGraph1>();
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    var graph = new GasmonGraph1
                    {
                        date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10),
                        value = orfs.FirstOrDefault(x => x.Date == dt && x.LineID == lineId)?.DailyEnergy ?? 0
                    };
                    graphs.Add(graph);
                }
                jsons[lineId] = JsonConvert.SerializeObject(graphs);
            }
            return jsons;
        }

        public string GetROBByPeriod(DateTime startDate, DateTime endDate)
        {
            var graphs = new List<GasmonGraph1>();
            var fsrus = _fsruDataDailyService.FSRUDataDaily.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            int diffDays = (endDate - startDate).Days;

            if (diffDays <= 31)
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    var graph = new GasmonGraph1
                    {
                        date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10),
                        value = fsrus.FirstOrDefault(x => x.Date == dt)?.LNGTankInventory ?? 0
                    };
                    graphs.Add(graph);
                }
            }
            else
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                {
                    var graph = new GasmonGraph1
                    {
                        date = dt.Date.ToString("yyyy-MM").Substring(0, 7),
                        value = fsrus.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).Sum(y => y.LNGTankInventory)
                    };
                    graphs.Add(graph);
                }
            }
            return JsonConvert.SerializeObject(graphs);
        }

        public string GetBOGByPeriod(DateTime startDate, DateTime endDate)
        {
            var graphs = new List<GasmonGraph1>();
            var fsrus = _fsruDataDailyService.FSRUDataDaily.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            var vds = _vesselDataService.VesselData.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            int diffDays = (endDate - startDate).Days;

            if (diffDays <= 31)
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    var graph = new GasmonGraph1
                    {
                        date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10),
                        value = (fsrus.FirstOrDefault(x => x.Date == dt)?.BoFM3 ?? 0) + (vds.FirstOrDefault(x => x.Date == dt)?.BoilOff ?? 0)
                    };
                    graphs.Add(graph);
                }
            }
            else
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                {
                    var graph = new GasmonGraph1
                    {
                        date = dt.Date.ToString("yyyy-MM").Substring(0, 7),
                        value = fsrus.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).Sum(y => y.BoFM3)
                    };
                    graphs.Add(graph);
                }
            }
            return JsonConvert.SerializeObject(graphs);
        }

        public string GetRegasRateByPeriod(DateTime startDate, DateTime endDate)
        {
            var graphs = new List<GasmonGraph1>();
            var fsrus = _fsruDataDailyService.FSRUDataDaily.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            int diffDays = (endDate - startDate).Days;

            if (diffDays <= 31)
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    var graph = new GasmonGraph1
                    {
                        date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10),
                        value = fsrus.FirstOrDefault(x => x.Date == dt)?.Rate ?? 0
                    };
                    graphs.Add(graph);
                }
            }
            else
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                {
                    var graph = new GasmonGraph1
                    {
                        date = dt.Date.ToString("yyyy-MM").Substring(0, 7),
                        value = fsrus.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).Sum(y => y.Rate)
                    };
                    graphs.Add(graph);
                }
            }
            return JsonConvert.SerializeObject(graphs);
        }

        public string GetPasokanByPeriod(DateTime startDate, DateTime endDate)
        {
            var graphs = new List<GasmonGraph3>();
            var vds = _vesselDataService.VesselData.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            int diffDays = (endDate - startDate).Days;

            if (diffDays <= 31)
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    var graph = new GasmonGraph3
                    {
                        date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10),
                        target = 10,
                        realisasi = 5
                    };
                    graphs.Add(graph);
                }
            }
            else
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                {
                    var graph = new GasmonGraph3
                    {
                        date = dt.Date.ToString("yyyy-MM").Substring(0, 7),
                        target = 10,
                        realisasi = 5
                    };
                    graphs.Add(graph);
                }
            }
            return JsonConvert.SerializeObject(graphs);
        }

        public string GetPenjualanByPeriod(DateTime startDate, DateTime endDate)
        {
            var graphs = new List<GasmonGraph3>();
            var orfs = _orfDataDailyService.ORFDataDaily.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            int diffDays = (endDate - startDate).Days;

            if (diffDays <= 31)
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    var graph = new GasmonGraph3
                    {
                        date = dt.Date.ToString("yyyy-MM-dd").Substring(0, 10),
                        realisasi = orfs.FirstOrDefault(x => x.Date == dt)?.DailyEnergy ?? 0,
                        target = _gasmonParameterService.GasmonParameter.FirstOrDefault(x => x.Tahun == dt.Year && x.ParameterID == "target_penjualan")?.Value / 365 ?? 0
                    };
                    graphs.Add(graph);
                }
            }
            else
            {
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                {
                    var graph = new GasmonGraph3
                    {
                        date = dt.Date.ToString("yyyy-MM").Substring(0, 7),
                        realisasi = orfs.Where(x => x.Date.Month == dt.Month && x.Date.Year == dt.Year).Sum(y => y.DailyEnergy),
                        target = _gasmonParameterService.GasmonParameter.FirstOrDefault(x => x.Tahun == dt.Year && x.ParameterID == "target_penjualan")?.Value / 12 ?? 0
                    };
                    graphs.Add(graph);
                }
            }
            return JsonConvert.SerializeObject(graphs);
        }

        public string GetDashboardData(int tahun)
        {
            var graph = new GasmonGraph2
            {
                target_pasokan = _gasmonParameterService.Cargo.Count(x => x.Tahun == tahun),
                realisasi_pasokan = _vesselDataService.VesselData.Count(x => x.Date.Year == tahun && x.CargoID > 0)
            };

            graph.prosentase_pasokan = graph.target_pasokan == 0 ? 0 : graph.realisasi_pasokan * 100 / graph.target_pasokan;

            graph.target_penjualan = _gasmonParameterService.GasmonParameter.FirstOrDefault(x => x.ParameterID == "target_penjualan" && x.Tahun == tahun)?.Value ?? 0;
            graph.realisasi_penjualan = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun).Sum(x => x.DailyEnergy);
            graph.prosentase_penjualan = graph.target_penjualan == 0 ? 0 : (int)Math.Round(graph.realisasi_penjualan * 100 / graph.target_penjualan);

            graph.realisasi_penjualan1 = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 1).Sum(x => x.DailyEnergy);
            graph.realisasi_penjualan2 = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 2).Sum(x => x.DailyEnergy);
            graph.realisasi_penjualan3 = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 3).Sum(x => x.DailyEnergy);
            graph.realisasi_penjualan4 = _orfDataDailyService.ORFDataDaily.Where(x => x.Date.Year == tahun && x.LineID == 4).Sum(x => x.DailyEnergy);

            return JsonConvert.SerializeObject(graph);
        }

        private int GetParameterValue(string parameterId, int year)
        {
            return _gasmonParameterService.GasmonParameter.FirstOrDefault(x => x.ParameterID == parameterId && x.Tahun == year)?.Value ?? 0;
        }

        private DateTime ParseDate(string date)
        {
            var dates = date.Split('/');
            return new DateTime(int.Parse(dates[2]), int.Parse(dates[1]), int.Parse(dates[0])).Date;
        }

        private DateTime ParseDateFromId(string id)
        {
            return DateTime.ParseExact(id.Substring(0, 8), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        }

        private string GenerateActivityHtml(List<GasmonActivity> activities)
        {
            var html = "";
            foreach (var act in activities)
            {
                html += "<tr>";
                html += $"<td class='activity'>{act.Time}</td><td class='activity'>{act.Remark}</td><td><button onclick=\"DeleteActivity('{act.ActivityID}')\"><i class=\"fa fa-minus\"></i></button></td>";
                html += "</tr>";
            }
            return html;
        }

        private string GetActivityContainer(int source)
        {
            return source switch
            {
                1 => "act_fsru_content",
                2 => "act_orf_content",
                >= 61 and <= 79 => $"act_carrier{source}_content",
                >= 81 and <= 99 => $"act_tug{source}_content",
                _ => null
            };
        }

        private string GenerateCargoHtml(List<Cargo> cargos)
        {
            var html = "";
            foreach (var c in cargos)
            {
                var isTarget = c.IsTarget == 1 ? "checked" : "";
                html += "<tr>";
                html += $"<td class=''>{c.Tahun}</td><td class=''>{c.Code}</td><td class=''>{c.Date:dd/MM/yyyy}</td><td><input {isTarget} type='checkbox' disabled/></td><td><button onclick=\"deleteCargo('{c.CargoID}')\"><i class=\"fa fa-minus\"></i></button></td>";
                html += "</tr>";
            }
            return html;
        }
    }
}
