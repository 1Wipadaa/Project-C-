using iTextSharp.text;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WindowsFormsApp4;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp4
{
    public partial class SalesReportForm : Form
    {
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=mmigichanew;charset=utf8;";
            return new MySqlConnection(connectionString);
        }


        public SalesReportForm()
        {
            InitializeComponent();  //เรียกใช้เมธอด (อินิเฉอะลายคัมโพเนิน) เพื่อกำหนดค่าคอนโทรลต่าง ๆ ที่ถูกออกแบบไว้ใน Form Designer
            LoadComboBoxes();  // โหลดลงComboBoxes
        }
        //โหลดข้อมูลที่จำเป็นลงใน ComboBox
        private void LoadComboBoxes()
        {
            //ฟังก์ชันนี้จะทำการตรวจสอบว่า ComboBox ทั้งสามตัวถูกสร้างและไม่เป็น null หรือยังไม่ได้รับการกำหนดค่า (อินิเฉอะลาย) หรือไม่
            if (comboBox1 == null || comboBox2 == null || comboBox3 == null)
            {
                MessageBox.Show("ComboBox not initialized properly.");
                return;
                //หากมี ComboBox ตัวใดตัวหนึ่งเป็น null จะแสดงข้อความเตือนว่า "ComboBox not initialized พรอพ'เพอลี."
                //และออกจากฟังก์ชัน
            }

            // โหลดวัน
            // ใช้ลูป for เพื่อเพิ่มรายการวันตั้งแต่ 1 ถึง 31 ลงใน comboBox1
            for (int day = 1; day <= 31; day++)
            {
                comboBox1.Items.Add(day.ToString());
            }
            //สร้างอาเรย์ thaiMonths ซึ่งประกอบด้วยชื่อเดือนในภาษาไทย

            //จากนั้นใช้ลูป foreach เพื่อเพิ่มชื่อเดือนแต่ละเดือนใน comboBox2.
            string[] thaiMonths = new string[]
            {
                "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน",
                "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"
            };

            foreach (string month in thaiMonths)
            {
                comboBox2.Items.Add(month);
            }

            // โหลดปี เริ่มจากปีปัจจุบัน (DateTime.Now.Year) และใช้ลูป for เพื่อลดปีลงจากปีปัจจุบันไปจนถึงปี 2000.
            // โดยเพิ่มปีแต่ละปีลงใน comboBox3:
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= 2000; year--)
            {
                comboBox3.Items.Add(year.ToString());
            }
        }
        //รับเดือนซึ่งจะใช้ในการทำงานกับข้อมูลที่เกี่ยวข้องกับเดือนในรูปแบบตัวเลข
        private int GetMonthNumber(string thaiMonth) //ฟังก์ชันนี้รับค่าอินพุตเป็น string ที่แทนชื่อเดือนในภาษาไทย เช่น "มกราคม", "กุมภาพันธ์", เป็นต้น
        {
            string[] thaiMonths = new string[]
            {
                "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน",
                "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"
            };

            return Array.IndexOf(thaiMonths, thaiMonth) + 1; // แปลงเป็นตัวเลขเดือน (1-12)
            //ฟังก์ชัน Array.IndexOf ใช้ค้นหาตำแหน่งของ thaiMonth(ชื่อเดือนที่ถูกส่งเข้ามา) ในอาเรย์ thaiMonths
            //ซึ่งจะคืนค่าเป็นดัชนี(index) ของชื่อเดือนในอาเรย์นั้น
        }
        //ปุ่มค้นหา
        private void button1_Click(object sender, EventArgs e)
        {
            // เชื่อมต่อกับฐานข้อมูล
            using (MySqlConnection conn = databaseConnection())
            {
                try
                {
                    conn.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล

                    // รับค่าจาก ComboBox และแปลงเป็นตัวเลข
                    int selectedDay = comboBox1.SelectedItem != null ? int.Parse(comboBox1.SelectedItem.ToString()) : 0;
                    int selectedMonth = comboBox2.SelectedItem != null ? GetMonthNumber(comboBox2.SelectedItem.ToString()) : 0;
                    int selectedYear = comboBox3.SelectedItem != null ? int.Parse(comboBox3.SelectedItem.ToString()) : 0;

                    // ประกาศตัวแปรสำหรับเก็บคำสั่ง SQL และประเภทสรุปยอด
                    string query = "";
                    string sumQuery = "";
                    string summaryType = "";

                    // ตรวจสอบเงื่อนไขการค้นหาจากค่าที่ผู้ใช้เลือก
                    if (selectedDay > 0 && selectedMonth > 0 && selectedYear > 0)
                    {
                        // ค้นหายอดรายวัน
                        query = "SELECT * FROM history WHERE DAY(`DATE`) = @day AND MONTH(`DATE`) = @month AND YEAR(`DATE`) = @year";
                        sumQuery = "SELECT SUM(sum) AS TotalSum FROM history WHERE DAY(`DATE`) = @day AND MONTH(`DATE`) = @month AND YEAR(`DATE`) = @year";
                        summaryType = "รายวัน";
                    }
                    else if (selectedMonth > 0 && selectedYear > 0)
                    {
                        // ค้นหายอดรายเดือน
                        query = "SELECT * FROM history WHERE MONTH(`DATE`) = @month AND YEAR(`DATE`) = @year";
                        sumQuery = "SELECT SUM(sum) AS TotalSum FROM history WHERE MONTH(`DATE`) = @month AND YEAR(`DATE`) = @year";
                        summaryType = "รายเดือน";
                    }
                    else if (selectedYear > 0)
                    {
                        // ค้นหายอดรายปี
                        query = "SELECT * FROM history WHERE YEAR(`DATE`) = @year";
                        sumQuery = "SELECT SUM(sum) AS TotalSum FROM history WHERE YEAR(`DATE`) = @year";
                        summaryType = "รายปี";
                    }
                    else
                    {
                        // แจ้งเตือนหากไม่ได้เลือกวัน เดือน หรือ ปีที่ถูกต้อง
                        MessageBox.Show("กรุณาเลือกวัน เดือน หรือปีให้ถูกต้อง");
                        return;
                    }

                    // สร้างคำสั่ง SQL เพื่อดึงข้อมูลจากฐานข้อมูล
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // เพิ่มค่าพารามิเตอร์ให้กับคำสั่ง SQL ตามเงื่อนไขที่กำหนด
                    if (query.Contains("@day")) cmd.Parameters.AddWithValue("@day", selectedDay);
                    if (query.Contains("@month")) cmd.Parameters.AddWithValue("@month", selectedMonth);
                    cmd.Parameters.AddWithValue("@year", selectedYear);

                    // ใช้ MySqlDataAdapter เพื่อดึงข้อมูลไปแสดงใน DataGridView
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt; // แสดงข้อมูลใน dataGridView1

                    // สร้างคำสั่ง SQL เพื่อคำนวณยอดรวม
                    MySqlCommand sumCmd = new MySqlCommand(sumQuery, conn);
                    if (sumQuery.Contains("@day")) sumCmd.Parameters.AddWithValue("@day", selectedDay);
                    if (sumQuery.Contains("@month")) sumCmd.Parameters.AddWithValue("@month", selectedMonth);
                    sumCmd.Parameters.AddWithValue("@year", selectedYear);

                    // ใช้ ExecuteScalar เพื่อดึงค่าผลรวมจากฐานข้อมูล
                    object totalResult = sumCmd.ExecuteScalar();
                    decimal totalIncome = (totalResult != DBNull.Value && totalResult != null) ? Convert.ToDecimal(totalResult) : 0;

                    // แสดงผลยอดรวมใน TextBox
                    tb_income.Text = totalIncome.ToString("N0"); // แสดงยอดรวมแบบมีจุลภาคคั่น
                    tb_inT.Text = summaryType; // แสดงประเภทของยอดสรุป เช่น รายวัน รายเดือน หรือ รายปี
                }
                catch (Exception ex)
                {
                    // แจ้งเตือนเมื่อเกิดข้อผิดพลาด
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        // ปุ่มปิดโปรแกรม
        private void button5_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }
        //ปุ่มย้อนกลับ
        private void button3_Click(object sender, EventArgs e)
        {

            Form5 form5 = new Form5();
            form5.Show();
            this.Hide();
        }

    }
}