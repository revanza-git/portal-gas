using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Admin.Models.User;
using Admin.Models.GCG;
using Admin.Interfaces.Repositories;
using Admin.Interfaces.Services;

namespace Admin.Controllers
{
    [Authorize]
    public class GCGController : Controller
    {
        UserManager<ApplicationUser> userManager;
        private IPelaporanGratifikasiRepository repoPg;
        private IGCGService gcgService;

        public GCGController(UserManager<ApplicationUser> _userManager,IPelaporanGratifikasiRepository _repoPg, IGCGService _gcgService)
        {
            userManager = _userManager;
            repoPg = _repoPg;
            gcgService = _gcgService;
        }

        public IActionResult PelaporanGratifikasi()
        {
            PelaporanGratifikasi pg;
            String UserID = userManager.GetUserId(User);
            int thisYear = DateTime.Now.Year;
            int thisMonth = DateTime.Now.Month;

            int lastMonth;
            int lastYear;

            if (thisMonth > 1)
            {
                thisMonth = thisMonth - 1;
            }
            else
            {
                thisMonth = 12;
                thisYear = thisYear - 1;
            }

            if (thisMonth > 1)
            {
                lastMonth = thisMonth - 1;
                lastYear = thisYear;
            }
            else
            {
                lastMonth = 12;
                lastYear = thisYear - 1;
            }

            switch(thisMonth)
            {
                case 1:
                    ViewBag.periode = "Januari " + thisYear;
                    break;
                case 2:
                    ViewBag.periode = "Februari " + thisYear;
                    break;
                case 3:
                    ViewBag.periode = "Maret " + thisYear;
                    break;
                case 4:
                    ViewBag.periode = "April " + thisYear;
                    break;
                case 5:
                    ViewBag.periode = "Mei " + thisYear;
                    break;
                case 6:
                    ViewBag.periode = "Juni " + thisYear;
                    break;
                case 7:
                    ViewBag.periode = "Juli " + thisYear;
                    break;
                case 8:
                    ViewBag.periode = "Agustus " + thisYear;
                    break;
                case 9:
                    ViewBag.periode = "September " + thisYear;
                    break;
                case 10:
                    ViewBag.periode = "Oktober " + thisYear;
                    break;
                case 11:
                    ViewBag.periode = "November " + thisYear;
                    break;
                case 12:
                    ViewBag.periode = "Desember " + thisYear;
                    break;
            }

            try
            {
                pg = repoPg.PelaporanGratifikasis.Where(x => x.Year == thisYear && x.Month == thisMonth && x.UserID == UserID).First();
            }
            catch (Exception)
            {
                pg = new PelaporanGratifikasi();
                pg.Month = thisMonth;
                pg.Year = thisYear;
                pg.AdaPemberianGratifikasi = -1;
                pg.AdaPenerimaanGratifikasi = -1;
                pg.AdaPermintaanGratifikasi = -1;
            }

            try
            {
                PelaporanGratifikasi last = repoPg.PelaporanGratifikasis.Where(x => x.Year == lastYear && x.Month == lastMonth && x.UserID == UserID).First();
                ViewBag.StatusPengisianPenerimaanGratifikasi = "Status terakhir pengisian: " + (last.AdaPenerimaanGratifikasi == 1 ? "":"tidak") + " ada penerimaan pada " + last.LastUpdated.ToString("dd/MM/yyyy");
                ViewBag.StatusPengisianPemberianGratifikasi = "Status terakhir pengisian: " + (last.AdaPemberianGratifikasi == 1 ? "" : "tidak") + " ada pemberian pada " + last.LastUpdated.ToString("dd/MM/yyyy");
                ViewBag.StatusPengisianPermintaanGratifikasi = "Status terakhir pengisian: " + (last.AdaPermintaanGratifikasi == 1 ? "" : "tidak") + " ada permintaan pada " + last.LastUpdated.ToString("dd/MM/yyyy");
            }
            catch (Exception)
            {
                ViewBag.StatusPengisianPenerimaanGratifikasi = "Status terakhir pengisian: tidak ada penerimaan";
                ViewBag.StatusPengisianPemberianGratifikasi = "Status terakhir pengisian: tidak ada pemberian";
                ViewBag.StatusPengisianPermintaanGratifikasi = "Status terakhir pengisian: tidak ada permintaan";
            }

            return View(pg);
        }

        [HttpPost]
        public IActionResult PelaporanGratifikasi(PelaporanGratifikasi pg)
        {
            if (ModelState.IsValid)
            {
                pg.UserID = userManager.GetUserId(User);
                repoPg.Save(pg);
                TempData["message"] = $"Laporan gratifikasi telah disimpan.";
                return RedirectToAction("PelaporanGratifikasi");
            }
            else
            {
                return View(pg);
            }
        }

        public IActionResult CodeOfConduct()
        {
            String UserID = userManager.GetUserId(User);
            
            CocCoi c = gcgService.GetCocCoi(DateTime.Now.Year, UserID);
            return View(c);
        }

        [HttpPost]
        public IActionResult CodeOfConduct(String submit)
        {
            String UserID = userManager.GetUserId(User);
            gcgService.Save(DateTime.Now.Year, UserID, "CoC");

            TempData["message"] = $"Pedoman Code of Conduct telah disetujui.";
            return RedirectToAction("CodeOfConduct");
        }

        public IActionResult ConflictOfInterest()
        {
            String UserID = userManager.GetUserId(User);

            CocCoi c = gcgService.GetCocCoi(DateTime.Now.Year, UserID);
            return View(c);
        }

        [HttpPost]
        public IActionResult ConflictOfInterest(String submit)
        {
            String UserID = userManager.GetUserId(User);
            gcgService.Save(DateTime.Now.Year, UserID, "CoI");
            TempData["message"] = $"Pedoman Conflict of Interest telah disetujui.";
            return RedirectToAction("ConflictOfInterest");
        }

        public async Task<IActionResult> Report(int ReportType=0,int Year=0,int Month=0)
        {
            String UserID = userManager.GetUserId(User);
            ApplicationUser login = await userManager.FindByIdAsync(UserID);
            if (login.GCGAdmin == false)
                return RedirectToAction("Index", "Home");

            ViewData["Title"] = "GCG Report";

            if (Year == 0)
                Year = DateTime.Now.Year;
            if (Month == 0)
            {
                Month = DateTime.Now.Month;
                if (Month > 1)
                {
                    Month = Month - 1;
                }
                else
                {
                    Month = 12;
                    Year = Year - 1;
                }
            }

            ViewBag.ReportType = ReportType;
            ViewBag.Month = Month;
            ViewBag.Year = Year;

            switch(ReportType)
            {
                case 0:
                    ViewData["Title"] = "Laporan CoC, CoI, dan Gratifikasi Periode " + getPeriod(Month, Year);
                    break;
                case 1:
                    ViewData["Title"] = "Laporan Code of Conduct";
                    break;
                case 2:
                    ViewData["Title"] = "Laporan Conflict of Interest";
                    break;
                case 3:
                    ViewData["Title"] = "Laporan Gratifikasi Periode " + getPeriod(Month, Year);
                    break;
            }

            String[] ReportTypes = { "All","Code of Conduct","Conflict of Interest","Gratifikasi","Penerimaan Gratifikasi","Pemberian Gratifikasi","Permintaan Gratifikasi" };
            ViewBag.ReportTypes = ReportTypes;

            List<LaporanGratifikasi> laporanGratifikasi = new List<LaporanGratifikasi>();
            IEnumerable<ApplicationUser> users = userManager.Users.Where(x => x.GCG == true).ToList();
            foreach(ApplicationUser user in users)
            {
                LaporanGratifikasi lg = new LaporanGratifikasi();
                CocCoi c = gcgService.GetCocCoi(Year,user.Id);
                lg.Nama = user.Name;
                lg.CoC = c.CoC ? "Sudah" : "Belum";
                lg.CoI = c.CoI ? "Sudah" : "Belum";
                if(ReportType == 1 && c.CoCSignedTime != null)
                {
                    lg.WaktuPelaporan = c.CoCSignedTime.ToString();
                }
                if (ReportType == 2 && c.CoISignedTime != null)
                {
                    lg.WaktuPelaporan = c.CoISignedTime.ToString();
                }
                try
                {
                    PelaporanGratifikasi pelaporan = repoPg.PelaporanGratifikasis.FirstOrDefault(x => x.Year == Year && x.Month == Month && x.UserID == user.Id);
                    lg.PenerimaanGratifikasi = pelaporan.AdaPenerimaanGratifikasi == 1 ? "Ada" : "Tidak ada";
                    lg.PemberianGratifikasi = pelaporan.AdaPemberianGratifikasi == 1 ? "Ada" : "Tidak ada";
                    lg.PermintaanGratifikasi = pelaporan.AdaPermintaanGratifikasi == 1 ? "Ada" : "Tidak ada";
                    if (ReportType == 0 || ReportType == 3)
                    {
                        lg.WaktuPelaporan = pelaporan.LastUpdated.ToString("dd/MM/yyyy");
                    }
                }
                catch (Exception)
                {
                    lg.PenerimaanGratifikasi = "Belum dilaporkan";
                    lg.PemberianGratifikasi = "Belum dilaporkan";
                    lg.PermintaanGratifikasi = "Belum dilaporkan";
                }
                laporanGratifikasi.Add(lg);
            }
            int CoCCount = gcgService.CocCoi.Where(x => x.Year == Year && x.CoC == true).Count();
            int CoICount = gcgService.CocCoi.Where(x => x.Year == Year && x.CoI == true).Count();
            ViewBag.CoCSigned = CoCCount;
            ViewBag.CoCUnsigned = users.Count() - CoCCount;
            ViewBag.CoISigned = CoICount;
            ViewBag.CoIUnsigned = users.Count() - CoICount;
            try
            {
                IEnumerable<PelaporanGratifikasi> pg = repoPg.PelaporanGratifikasis.Where(x => x.Year == Year && x.Month == Month);
                ViewBag.GratifikasiReported = pg.Count();
                ViewBag.GratifikasiNotReported = users.Count() - pg.Count();
                ViewBag.AdaPenerimaanGratifikasi = pg.Where(x => x.AdaPenerimaanGratifikasi == 1).Count();
                ViewBag.TidakAdaPenerimaanGratifikasi = pg.Where(x => x.AdaPenerimaanGratifikasi == 0).Count();
                ViewBag.AdaPemberianGratifikasi = pg.Where(x => x.AdaPemberianGratifikasi == 1).Count();
                ViewBag.TidakAdaPemberianGratifikasi = pg.Where(x => x.AdaPemberianGratifikasi == 0).Count();
                ViewBag.AdaPermintaanGratifikasi = pg.Where(x => x.AdaPermintaanGratifikasi == 1).Count();
                ViewBag.TidakAdaPermintaanGratifikasi = pg.Where(x => x.AdaPermintaanGratifikasi == 0).Count();
            }
            catch(Exception)
            {
                ViewBag.GratifikasiReported = 0;
                ViewBag.GratifikasiNotReported = users.Count();
                ViewBag.AdaPenerimaanGratifikasi = 0;
                ViewBag.TidakAdaPenerimaanGratifikasi = 0;
                ViewBag.AdaPemberianGratifikasi = 0;
                ViewBag.TidakAdaPemberianGratifikasi = 0;
                ViewBag.AdaPermintaanGratifikasi = 0;
                ViewBag.TidakAdaPermintaanGratifikasi = 0;
            }

            List<LaporanGratifikasiDetail> lgds = new List<LaporanGratifikasiDetail>();
            switch (ReportType)
            {
                case 4:
                    {
                        ViewBag.JudulLaporanGratifikasi = "Laporan Penerimaan Gratifikasi Periode " + getPeriod(Month, Year);
                        ViewData["Title"] = ViewBag.JudulLaporanGratifikasi;
                        List<PelaporanGratifikasi> pgs = repoPg.PelaporanGratifikasis.Where(x => x.Year == Year && x.Month == Month && x.AdaPenerimaanGratifikasi == 1).ToList();
                        foreach (PelaporanGratifikasi pg in pgs)
                        {
                            LaporanGratifikasiDetail lgd = new LaporanGratifikasiDetail();
                            lgd.Nama = userManager.FindByIdAsync(pg.UserID).Result.Name;
                            lgd.Deskripsi = pg.DeskripsiPenerimaanGratifikasi;
                            lgd.WaktuPelaporan = pg.LastUpdated.ToString("dd/MM/yyyy");
                            lgds.Add(lgd);
                        }
                        break;
                    }
                case 5:
                    {
                        ViewBag.JudulLaporanGratifikasi = "Laporan Pemberian Gratifikasi Periode " + getPeriod(Month, Year);
                        ViewData["Title"] = ViewBag.JudulLaporanGratifikasi;
                        List<PelaporanGratifikasi> pgs = repoPg.PelaporanGratifikasis.Where(x => x.Year == Year && x.Month == Month && x.AdaPemberianGratifikasi == 1).ToList();
                        foreach (PelaporanGratifikasi pg in pgs)
                        {
                            LaporanGratifikasiDetail lgd = new LaporanGratifikasiDetail();
                            lgd.Nama = userManager.FindByIdAsync(pg.UserID).Result.Name;
                            lgd.Deskripsi = pg.DeskripsiPemberianGratifikasi;
                            lgd.WaktuPelaporan = pg.LastUpdated.ToString("dd/MM/yyyy");
                            lgds.Add(lgd);
                        }
                        break;
                    }
                case 6:
                    {
                        ViewBag.JudulLaporanGratifikasi = "Laporan Permintaan Gratifikasi Periode " + getPeriod(Month, Year);
                        ViewData["Title"] = ViewBag.JudulLaporanGratifikasi;
                        List<PelaporanGratifikasi> pgs = repoPg.PelaporanGratifikasis.Where(x => x.Year == Year && x.Month == Month && x.AdaPermintaanGratifikasi == 1).ToList();
                        foreach (PelaporanGratifikasi pg in pgs)
                        {
                            LaporanGratifikasiDetail lgd = new LaporanGratifikasiDetail();
                            lgd.Nama = userManager.FindByIdAsync(pg.UserID).Result.Name;
                            lgd.Deskripsi = pg.DeskripsiPermintaanGratifikasi;
                            lgd.WaktuPelaporan = pg.LastUpdated.ToString("dd/MM/yyyy");
                            lgds.Add(lgd);
                        }
                        break;
                    }
            }
            ViewBag.LaporanGratifikasiDetail = lgds;

            return View("ListLaporanGratifikasi",laporanGratifikasi);
        }

        private String getPeriod(int Month,int Year)
        {
            switch (Month)
            {
                case 1:
                    return "Januari " + Year;
                case 2:
                    return "Februari " + Year;
                case 3:
                    return "Maret " + Year;
                case 4:
                    return "April " + Year;
                case 5:
                    return "Mei " + Year;
                case 6:
                    return "Juni " + Year;
                case 7:
                    return "Juli " + Year;
                case 8:
                    return "Agustus " + Year;
                case 9:
                    return "September " + Year;
                case 10:
                    return "Oktober " + Year;
                case 11:
                    return "November " + Year;
                case 12:
                    return "Desember " + Year;
                default:
                    return Month + " " + Year;
            }
        }
    }
}