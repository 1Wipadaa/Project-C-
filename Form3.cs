using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp4
{
    public partial class Form3 : Form
    {
        private MySqlConnection databaseConnection()
        {
            // สร้างการเชื่อมต่อกับฐานข้อมูล
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=mmigichanew;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        public Form3() //เรียกใช้เมธอด (อินิเฉอะลายคัมโพเนิน) เพื่อกำหนดค่าคอนโทรลต่าง ๆ ที่ถูกออกแบบไว้ใน Form Designer
        {
            InitializeComponent();
        }
        // เรียกโหลดข้อมูลเมื่อฟอร์มโหลด
        private void Form3_Load(object sender, EventArgs e)
        {
            LoadDataFromDatabase1(); 
            LoadDataFromDatabase2();
        }
        // ฟังก์ชันที่ใช้โหลดข้อมูลจากฐานในตารางmenucha
        private void LoadDataFromDatabase1()  
        {
            MySqlConnection conn = databaseConnection();  // เชื่อมต่อกับฐานข้อมูล
            DataSet ds = new DataSet();  // สร้าง DataSet เพื่อเก็บข้อมูลจากฐานข้อมูล

            conn.Open();  // เปิดการเชื่อมต่อกับฐานข้อมูล

            MySqlCommand cmd = conn.CreateCommand();  // สร้างคำสั่ง SQL
            cmd.CommandText = "SELECT name, price, quantity, image FROM `menucha`";  

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);  // สร้าง DataAdapter(ดาต้าอะแดปเตอร์)
            adapter.Fill(ds);  // เติมข้อมูลจากฐานข้อมูลลงใน DataSet

            conn.Close();  // ปิดการเชื่อมต่อกับฐานข้อมูล

            dataGridView1.DataSource = ds.Tables[0].DefaultView;  // แสดงข้อมูลใน DataGridView โดยใช้ข้อมูลจาก DataSet
        }
        // ฟังก์ชันที่ใช้โหลดข้อมูลจากฐานในตารางtopping
        private void LoadDataFromDatabase2()  
        {
            MySqlConnection conn = databaseConnection();  // เชื่อมต่อกับฐานข้อมูล
            DataSet ds = new DataSet();  // สร้าง DataSet เพื่อเก็บข้อมูลจากฐานข้อมูล

            conn.Open();  // เปิดการเชื่อมต่อกับฐานข้อมูล

            MySqlCommand cmd = conn.CreateCommand();  // สร้างคำสั่ง SQL
            cmd.CommandText = "SELECT name, price, quantity, image FROM `topping`";  // กำหนดคำสั่ง SQL เพื่อดึงข้อมูลจากตาราง `topping`

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);  // สร้าง DataAdapter เพื่อนำข้อมูลจากฐานข้อมูล
            adapter.Fill(ds);  // เติมข้อมูลจากฐานข้อมูลลงใน DataSet

            conn.Close();  // ปิดการเชื่อมต่อกับฐานข้อมูล

            dataGridView2.DataSource = ds.Tables[0].DefaultView;  // แสดงข้อมูลใน DataGridView2 โดยใช้ข้อมูลจาก DataSet
        }
        // ฟังก์ชันที่ทำงานเมื่อมีการคลิกที่เซลล์ใน dataGridView1
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)  
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)  // ตรวจสอบว่าได้คลิกที่เซลล์ข้อมูล (ที่ไม่ใช่หัวคอลัมน์)
            {
                string columnName = "image";  // กำหนดชื่อคอลัมน์ที่เก็บข้อมูลรูปภาพ

                // ตรวจสอบว่าคอลัมน์ "image" มีอยู่ใน dataGridView1 หรือไม่
                if (dataGridView1.Columns.Contains(columnName))
                {
                    // ถ้ามีข้อมูลในคอลัมน์ "image" ที่คลิก
                    if (dataGridView1.Rows[e.RowIndex].Cells[columnName].Value != null)
                    {
                        byte[] imageData = (byte[])dataGridView1.Rows[e.RowIndex].Cells[columnName].Value;  // ดึงข้อมูลรูปภาพจากเซลล์ในรูปแบบ byte array
                        if (imageData.Length > 0)  // ถ้าข้อมูลรูปภาพไม่ว่าง
                        {
                            // แปลง byte array เป็น MemoryStream(เมมเบอรี่สตรีม) (ข้อมูลที่เก็บในหน่วยความจำที่ไม่ใช่ไฟล์ เช่นข้อมูลไบต์ของภาพ)
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                // สร้างรูปภาพจาก MemoryStream
                                Image img = Image.FromStream(ms);

                                // ปรับขนาดรูปภาพใน PictureBox เพื่อให้แสดงในขนาดที่เหมาะสม
                                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;  // ปรับโหมดการแสดงผลเป็น Zoom
                                pictureBox1.Image = img;  // แสดงรูปภาพใน PictureBox
                            }
                        }
                    }
                }
            }
        }
        // ฟังก์ชันที่ทำงานเมื่อคลิกที่เซลล์ใน dataGridView2
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // ตรวจสอบว่าคลิกที่เซลล์ข้อมูล
            {
                string columnName = "image"; // ชื่อคอลัมน์ที่เก็บรูปภาพ

                // ตรวจสอบว่ามีคอลัมน์ที่ต้องการหรือไม่
                if (dataGridView2.Columns.Contains(columnName)) // ใช้ dataGridView2 แทน dataGridView1
                {
                    // ดึงข้อมูลรูปภาพจากคอลัมน์ที่เลือกในเซลล์
                    if (dataGridView2.Rows[e.RowIndex].Cells[columnName].Value != null) // ใช้ dataGridView2 แทน dataGridView1
                    {
                        byte[] imageData = (byte[])dataGridView2.Rows[e.RowIndex].Cells[columnName].Value; // ใช้ dataGridView2 แทน dataGridView1
                        if (imageData.Length > 0)
                        {
                            // แปลงข้อมูลรูปภาพจาก byte array เป็น MemoryStream
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                // สร้างรูปภาพจาก MemoryStream
                                Image img = Image.FromStream(ms);

                                // ปรับขนาดของ PictureBox เพื่อให้เต็มขนาด
                                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                pictureBox1.Image = img;
                            }
                        }
                    }
                }
            }
        }
        //ปุ่มสั่งสินค้า
        private void button2_Click(object sender, EventArgs e)
        {
            Form6 F6 = new Form6();
            F6.Show();
            this.Hide();
        }
        // ปุ่มย้อนกลับ
        private void button1_Click(object sender, EventArgs e)
        {

            Form1 F1 = new Form1();
            F1.Show();
            this.Hide();
        }
        // ปุ่มปิดโปรแกรม
        private void button4_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }

    }
}