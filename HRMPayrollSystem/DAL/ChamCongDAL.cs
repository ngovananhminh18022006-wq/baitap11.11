using System;
using System.Data;
using System.Data.SqlClient;

namespace HRMPayrollSystem.DAL
{
    public class ChamCongDAL
    {
        private string connectionString = "Data Source=LAPTOP-7I6A24E9;Initial Catalog=HRMDB;Integrated Security=True";

        // Sử dụng SqlDataAdapter nạp dữ liệu (Ngắt kết nối)
        public DataTable LayBangChamCongTheoThang(string thangNam)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT MaCong, MaNV, ThangNam, SoNgayCong, SoGioOT FROM BangChamCong WHERE ThangNam = @ThangNam";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@ThangNam", thangNam);
                    adapter.Fill(dt); // Đổ dữ liệu vào DataTable (Ngắt kết nối)
                }
            }
            return dt;
        }

        // Lưu dữ liệu sử dụng SqlCommandBuilder và adapter.Update()
        public bool LuuBangChamCong(DataTable dtChamCong)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT MaCong, MaNV, ThangNam, SoNgayCong, SoGioOT FROM BangChamCong";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    // Tự động phát sinh các câu lệnh INSERT, UPDATE, DELETE
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                    
                    try
                    {
                        // Đồng bộ dữ liệu 1 lượt từ DataTable xuống SQL Server
                        adapter.Update(dtChamCong);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi đồng bộ dữ liệu: " + ex.Message);
                    }
                }
            }
        }
    }
}
