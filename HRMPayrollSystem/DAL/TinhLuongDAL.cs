using System;
using System.Data;
using System.Data.SqlClient;

namespace HRMPayrollSystem.DAL
{
    public class TinhLuongDAL
    {
        private string connectionString = "Data Source=LAPTOP-7I6A24E9;Initial Catalog=HRMDB;Integrated Security=True";

        public bool ChotLuongThang(string thangNam)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // Lấy danh sách chấm công kết hợp với lương cơ bản của NV
                    string queryLayData = @"
                        SELECT c.MaNV, c.SoNgayCong, c.SoGioOT, n.LuongCoBan, n.HeSoLuong 
                        FROM BangChamCong c
                        JOIN NhanVien n ON c.MaNV = n.MaNV
                        WHERE c.ThangNam = @ThangNam";
                    
                    SqlCommand cmdLayData = new SqlCommand(queryLayData, conn, trans);
                    cmdLayData.Parameters.AddWithValue("@ThangNam", thangNam);

                    DataTable dtData = new DataTable();
                    using (SqlDataReader reader = cmdLayData.ExecuteReader())
                    {
                        dtData.Load(reader);
                    }

                    // Duyệt từng dòng để tính lương và Insert
                    foreach (DataRow row in dtData.Rows)
                    {
                        int maNV = Convert.ToInt32(row["MaNV"]);
                        double soNgayCong = Convert.ToDouble(row["SoNgayCong"]);
                        double soGioOT = Convert.ToDouble(row["SoGioOT"]);
                        decimal luongCoBan = Convert.ToDecimal(row["LuongCoBan"]);
                        double heSoLuong = Convert.ToDouble(row["HeSoLuong"]);

                        // Lưu ý: Đề bài ghi "- SoNgayCong" có thể là lỗi đánh máy của GV, đúng ra phải là "*" (nhân ngày công). 
                        // Ở đây code dùng phép tính Nhân (*) để lương ra đúng số học.
                        decimal thucLinh = (luongCoBan * (decimal)heSoLuong / 22m) * (decimal)soNgayCong + (decimal)(soGioOT * 50000);

                        string queryInsert = "INSERT INTO BangLuong (MaNV, ThangNam, ThucLinh) VALUES (@MaNV, @ThangNam, @ThucLinh)";
                        SqlCommand cmdInsert = new SqlCommand(queryInsert, conn, trans);
                        cmdInsert.Parameters.AddWithValue("@MaNV", maNV);
                        cmdInsert.Parameters.AddWithValue("@ThangNam", thangNam);
                        cmdInsert.Parameters.AddWithValue("@ThucLinh", thucLinh);
                        
                        cmdInsert.ExecuteNonQuery();
                    }

                    // Thành công -> Commit
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // Lỗi (ví dụ: đã chốt lương rồi, vi phạm Unique Key) -> Rollback
                    trans.Rollback();
                    throw new Exception("Lỗi chốt lương, đã hoàn tác: " + ex.Message);
                }
            }
        }
    }
}
