using Admin.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; // Change this to Microsoft.Data.SqlClient
using System.IO;
using System.Linq;
using Admin.Interfaces.Repositories;
using Admin.Models.Overtime;

namespace Admin.Repositories
{
    public class SDMRepository : ISDMRepository
    {
        private ApplicationDbContext context;
        private ICommonRepository crepository;

        public static IConfigurationRoot Configuration { get; set; }


        public SDMRepository(ApplicationDbContext ctx, ICommonRepository common)
        {
            context = ctx;
            crepository = common;

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public IEnumerable<Overtime> Overtime => context.Overtime;

        public void BatchApproval(string[] data)
        {

            foreach (var item in data)
            {
                Overtime search = context.Overtime.FirstOrDefault(x => x.OvertimeID == int.Parse(item));
                search.Status = 2;
            }

            context.SaveChanges();
        }

        public void SaveOvertime(Overtime overtime, string mode)
        {
            overtime.Department = crepository.GetJabatan().FirstOrDefault(x => x.JabatanID == overtime.Posisi).Department;
            overtime.Assigner = crepository.GetJabatan().FirstOrDefault(x => x.JabatanID == overtime.Posisi).Atasan;

            if (mode == "add")
            {

                context.Overtime.Add(overtime);
            }
            else
            {
                Overtime search = context.Overtime.FirstOrDefault(x => x.OvertimeID == overtime.OvertimeID);
                if (search != null)
                {
                    search.Name = overtime.Name;
                    search.Posisi = overtime.Posisi;
                    search.Department = overtime.Department;
                    search.Assigner = overtime.Assigner;
                    search.Tanggal = overtime.Tanggal;
                    search.WorkDescription = overtime.WorkDescription;

                    TimeSpan interval = overtime.JamAkhirLembur.Subtract(overtime.JamAwalLembur);
                    search.TotalHours = interval.TotalHours;

                    search.JamMulaiKerja = overtime.JamMulaiKerja;
                    search.JamSelesaiKerja = overtime.JamSelesaiKerja;
                    search.JamAwalLembur = overtime.JamAwalLembur;
                    search.JamAkhirLembur = overtime.JamAkhirLembur;
                    search.Status = overtime.Status;
                    search.Description = overtime.Description;
                }
            }
            context.SaveChanges();
        }

        public void DeleteOvertime(Overtime overtime)
        {
            context.Overtime.Remove(overtime);
            context.SaveChanges();
        }

        public List<Recap> getRecap(int bulan, string username, int tahun)
        {
            List<Recap> list = new List<Recap>();

            using (var conn = new SqlConnection(Configuration["Data:PortalNR:ConnectionString"]))
            {
                using (var cmd = new SqlCommand("rekap_lembur", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@bulan", bulan);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@tahun", tahun);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader()) // No change needed here
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // more code
                                var item = new Recap();

                                item.TanggalAbsensi = reader["TanggalAbsensi"].ToString();
                                item.JamMulaiKerja = reader["JamMulaiKerja"].ToString();
                                item.JamSelesaiKerja = reader["JamSelesaiKerja"].ToString();
                                item.KeteranganKerja = reader["KeteranganKerja"].ToString();
                                item.JamAwalLembur = reader["JamAwalLembur"].ToString();
                                item.JamAkhirLembur = reader["JamAkhirLembur"].ToString();
                                item.TotalJamLembur = reader["TotalJamLembur"].ToString();
                                item.KeteranganLembur = reader["KeteranganLembur"].ToString();
                                item.PemberiTugas = reader["PemberiTugas"].ToString();

                                list.Add(item);
                            }
                        }
                    }
                }

                return list;
            }
        }
    }
}
