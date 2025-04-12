using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form16 : Form
    {
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=mmigichanew;charset=utf8;";
            return new MySqlConnection(connectionString);
        }

        public Form16()  // ตัวคอนสตรัคเตอร์สำหรับฟอร์ม Form16
        {
            InitializeComponent();  // ทำการตั้งค่าเริ่มต้นให้กับฟอร์มและควบคุมต่าง ๆ ที่อยู่ในฟอร์ม

            // กำหนดเหตุการณ์ KeyPress สำหรับ textBox1
                                               // แฮน'เดลอะ
            textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);  // เมื่อผู้ใช้กดปุ่มใน textBox1 จะเรียกใช้งาน method textBox1_KeyPress

            // กำหนดเหตุการณ์ TextChanged สำหรับ textBox1
            textBox1.TextChanged += new EventHandler(textBox1_TextChanged);  // เมื่อข้อความใน textBox1 เปลี่ยนแปลง จะเรียกใช้งาน method textBox1_TextChanged
        }
        //ตรวจสอบกรอกข้อมูลเข้าสู่ระบบ
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) 
        {       //อักขระ
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))  // ตรวจสอบว่าเป็นตัวเลขหรือไม่ 
            {
                e.Handled = true;  // หากไม่ใช่ตัวเลข ให้หยุดการทำงาน และปฎิเสธการพิมพ์ตัวอักษรนั้น
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)  // เมื่อข้อความใน textBox1 เปลี่ยนแปลง
        {
            if (textBox1.Text.Length > 10)  // ถ้าข้อความใน textBox1 มีความยาวมากกว่า 10 ตัว
            {
                textBox1.Text = textBox1.Text.Substring(0, 10);  // ตัดข้อความใน textBox1 ให้เหลือแค่ 10 ตัวแรก
                textBox1.SelectionStart = textBox1.Text.Length;  // เลื่อนเคอร์เซอร์ไปยังตำแหน่งสุดท้ายของข้อความ
            }
        }
        // ปุ่มเข้าสู่ระบบ
        private void button1_Click(object sender, EventArgs e)  
        {
        if (string.IsNullOrEmpty(textBox1.Text))  // ตรวจสอบว่า textBox1 ว่างหรือไม่
        {
            MessageBox.Show("กรุณากรอกหมายเลขก่อนเข้าสู่ระบบ");  // ถ้าหากว่างให้แสดงข้อความเตือน
            return;  // หยุดการทำงานของฟังก์ชัน
        }

        string searchText = textBox1.Text;  // เก็บค่าจาก textBox1 ลงในตัวแปร searchText
        try
        {
        using (MySqlConnection conn = databaseConnection())  // เปิดการเชื่อมต่อกับฐานข้อมูล
        {
            conn.Open();  // เปิดการเชื่อมต่อ

            // ดึงข้อมูลจากตาราง name เพื่อล็อกอิน
            string sqlName = "SELECT * FROM name WHERE number = @number";  
            using (MySqlCommand cmdName = new MySqlCommand(sqlName, conn))
            {
                cmdName.Parameters.AddWithValue("@number", searchText);  // เพิ่ม parameter number
                using (MySqlDataReader reader = cmdName.ExecuteReader())  // ใช้ reader เพื่ออ่านข้อมูลจากฐานข้อมูล
                {
                    if (reader.Read())  // ถ้ามีข้อมูลในฐานข้อมูล
                    {
                        string name = reader.GetString("username");  // ดึงชื่อผู้ใช้
                        string num = reader.GetString("number");  // ดึงหมายเลขโทรศัพท์
                        string email = reader.GetString("email");  // ดึงอีเมล

                        reader.Close();  // ปิด reader ก่อนทำการ insert ข้อมูล

                        //บันทึกข้อมูลการเข้าสู่ระบบลงใน login
                        string insertlogin = "INSERT INTO login (number, username, email, date) VALUES (@number, @username, @email, NOW())";
                        using (MySqlCommand insertCmd = new MySqlCommand(insertlogin, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@number", num);
                            insertCmd.Parameters.AddWithValue("@username", name);
                            insertCmd.Parameters.AddWithValue("@email", email);
                            insertCmd.ExecuteNonQuery();
                        }

                        // เปิดฟอร์ม 3
                        Form3 form3 = new Form3();
                        form3.Show();  // แสดงฟอร์ม 3
                        this.Hide();  // ซ่อนฟอร์มปัจจุบัน
                        return;  // หยุดการทำงาน
                    }
                }
            }
            // ไม่พบข้อมูลใน name
            MessageBox.Show("ไม่พบข้อมูล กรุณาสมัครสมาชิก");  // ถ้าไม่พบข้อมูล ให้แสดงข้อความแจ้งเตือน
            }
        }
        catch (Exception ex)
        {
        MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);  // ถ้ามีข้อผิดพลาด ให้แสดงข้อความข้อผิดพลาด
        }
    }
        //ปุ่มย้อนกลับ
        private void button3_Click(object sender, EventArgs e)
        {
            Form1 F1 = new Form1();
            F1.Show();
            this.Hide();
        }
        //ปุ่มออกจากโปรแกรม
        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}