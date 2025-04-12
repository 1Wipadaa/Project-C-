using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp4
{
    public partial class Form10 : Form
    {
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=mmigichanew;charset=utf8;";
            return new MySqlConnection(connectionString);
        }

        public Form10()
        {
            InitializeComponent();  // เรียกใช้งานฟังก์ชันที่สร้างโดยอัตโนมัติจากการออกแบบฟอร์มเพื่อเริ่มต้นและตั้งค่า UI ของฟอร์ม
            LoadAllCustomers();  // เรียกฟังก์ชัน ออลคัสโตเมอร์) เพื่อโหลดข้อมูลทั้งหมดของลูกค้าเมื่อฟอร์มเปิด
            // ตั้งค่า textBox ให้ว่างในตอนเริ่มต้น
            textBox2.Text = string.Empty;  // กำหนดให้ textBox2,3,4 ว่างเปล่าในตอนเริ่มต้น
            textBox3.Text = string.Empty;  
            textBox4.Text = string.Empty;  
        }
        //ดึงข้อมูลทั้งหมดจากตาราง name ในฐานข้อมูลและแสดงผลใน DataGridView 
        private void LoadAllCustomers()
        {
            using (MySqlConnection conn = databaseConnection())  // เปิดการเชื่อมต่อฐานข้อมูล
            {
                string query = "SELECT * FROM name";  // ดึงข้อมูลทั้งหมดจากตาราง "name"
                MySqlCommand cmd = new MySqlCommand(query, conn);  // โดยใช้การเชื่อมต่อที่เปิดไว้

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);  // สร้าง MySqlDataAdapter เพื่อดึงข้อมูลจากฐานข้อมูล
                DataTable dt = new DataTable();  // สร้าง DataTable เพื่อเก็บผลลัพธ์
                adapter.Fill(dt);  // ดึงข้อมูลจากฐานข้อมูลและเก็บลงใน DataTable

                dataGridView1.DataSource = dt;  // แสดงข้อมูลใน DataGridView1

                // ถ้ามีผลลัพธ์ แสดงข้อมูลใน TextBox
                if (dt.Rows.Count > 0)
                {
                    textBox2.Text = dt.Rows[0]["username"].ToString();  // แสดงชื่อผู้ใช้จากแถวแรกใน textBox2,3,4
                    textBox3.Text = dt.Rows[0]["email"].ToString();  
                    textBox4.Text = dt.Rows[0]["number"].ToString();  
                }
                else
                {
                    MessageBox.Show("ไม่พบข้อมูลลูกค้า");  // ถ้าไม่พบข้อมูลในฐานข้อมูล จะแสดงข้อความแจ้งเตือน
                }
            }
        }
        //ปุ่มค้นหาข้อมูลลูกค้า 
        private void button7_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text;  // ดึงข้อความที่กรอกใน textBox1 ซึ่งเป็นข้อความที่จะใช้ในการค้นหา

            // ตรวจสอบว่ากรอกข้อความหรือไม่
            if (!string.IsNullOrEmpty(searchText))  // ตรวจสอบว่า searchText ไม่ว่างหรือไม่
            {
                SearchCustomer(searchText);  // เรียกใช้ฟังก์ชัน SearchCustomer และส่งค่า searchText เป็นพารามิเตอร์ไปค้นหาลูกค้าที่ตรงกับข้อความ
            }
            else
            {
                MessageBox.Show("กรุณากรอกเบอร์โทรศัพท์หรือชื่อเพื่อค้นหา");  // ถ้าไม่กรอกข้อความ จะแสดงกล่องข้อความแจ้งเตือน
            }
        }
        //ตรวจสอบว่ามีข้อมูลลูกค้าที่มีชื่อผู้ใช้ (username) ที่ระบุอยู่ในฐานข้อมูลหรือไม่ โดยการใช้ SQL query เพื่อค้นหาชื่อผู้ใช้ในตาราง name
        private bool CustomerExists(string username)
        {
            //เปิดการเชื่อมต่อกับฐานข้อมูลโดยใช้ฟังก์ชัน ซึ่งจะคืนค่าการเชื่อมต่อ MySQL
            using (MySqlConnection conn = databaseConnection())
            {
                //สร้างคำสั่ง SQL ที่ใช้ในการค้นหาว่ามีชื่อผู้ใช้ (username) ที่ตรงกับที่ระบุในตาราง name หรือไม่
                //โดยใช้ฟังก์ชัน COUNT(*)เคาท เพื่อคำนวณจำนวนแถวที่ตรงกับเงื่อนไข.
                string query = "SELECT COUNT(*) FROM name WHERE username = @username";
                MySqlCommand cmd = new MySqlCommand(query, conn); //สร้างคำสั่ง SQL (cmd) และกำหนดให้เชื่อมต่อกับฐานข้อมูลที่เปิดไว้ (conn)
                cmd.Parameters.AddWithValue("@username", username);//กำหนดค่าพารามิเตอร์ @username ในคำสั่ง SQL
                                                                   //เป็นชื่อผู้ใช้ (username) ที่ถูกส่งเข้าไปในฟังก์ชัน

                conn.Open(); //เปิดการเชื่อมต่อกับฐานข้อมูล.
                int count = Convert.ToInt32(cmd.ExecuteScalar()); //ค่าผลลัพธ์จาก ExecuteScalar จะถูกแปลงเป็น int
                                                                  //โดยใช้ Convert.ToInt32(แปลงเป็นจำนวนเต็ม) เพื่อให้สามารถนำไปเปรียบเทียบได้
                return count > 0; // ถ้ามีข้อมูลลูกค้า จะคืนค่า true 
                //ถ้าผลลัพธ์เป็น 0 แสดงว่าไม่มีข้อมูลลูกค้าดังกล่าวในฐานข้อมูล ดังนั้นจะคืนค่า false
            }
        }
        //การค้นหาข้อมูลลูกค้าจากฐานข้อมูลโดยการใช้เบอร์โทรศัพท์หรือชื่อผู้ใช้ที่กรอกเข้ามา
        private void SearchCustomer(string searchText)
        {
            using (MySqlConnection conn = databaseConnection())  // เชื่อมต่อกับฐานข้อมูล
            {
                // ค้นหาข้อมูลลูกค้า โดยใช้เบอร์โทรศัพท์หรือชื่อที่ตรงกับ searchText
                string query = "SELECT * FROM name WHERE number LIKE @searchText OR username LIKE @searchText";
                MySqlCommand cmd = new MySqlCommand(query, conn);  // สร้างคำสั่ง SQL
                cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");  // เพิ่มพารามิเตอร์เพื่อค้นหาข้อความในเบอร์โทรศัพท์หรือชื่อ

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);  // ใช้ MySqlDataAdapter เพื่อดึงข้อมูลจากฐานข้อมูล
                DataTable dt = new DataTable();  // สร้าง DataTable เพื่อเก็บผลลัพธ์ที่ได้จากการค้นหา
                adapter.Fill(dt);  // ดึงข้อมูลจากฐานข้อมูลและเติมลงใน DataTable

                dataGridView1.DataSource = dt;  // แสดงข้อมูลใน DataGridView1

                // ถ้ามีผลลัพธ์ แสดงข้อมูลใน TextBox
                if (dt.Rows.Count > 0)
                {
                    textBox2.Text = dt.Rows[0]["username"].ToString();  // แสดงชื่อผู้ใช้ใน textBox2
                    textBox3.Text = dt.Rows[0]["email"].ToString();  // แสดงอีเมลใน textBox3
                    textBox4.Text = dt.Rows[0]["number"].ToString();  // แสดงเบอร์โทรศัพท์ใน textBox4
                }
                else
                {
                    MessageBox.Show("ไม่พบข้อมูลลูกค้า");  // หากไม่พบข้อมูล จะแสดงข้อความแจ้งเตือน
                }
            }
        }
        //อัปเดตข้อมูลของลูกค้าในฐานข้อมูล
        private void UpdateCustomer(string oldUsername, string newUsername, string email, string phoneNumber)
        {
            // ตรวจสอบว่ามีข้อมูลลูกค้าก่อนทำการอัพเดท
            if (!CustomerExists(oldUsername)) //ฟังก์ชัน CustomerExists ที่จะเช็คว่าในฐานข้อมูลมีชื่อผู้ใช้ที่ต้องการแก้ไขหรือไม่
            {
                MessageBox.Show("ไม่พบลูกค้าที่มีชื่อผู้ใช้นี้");
                return;
            }

            // ตรวจสอบว่า username ใหม่ไม่ซ้ำกับที่มีอยู่ในระบบ
            if (CustomerExists(newUsername) && oldUsername != newUsername)
            {
                //ถ้าชื่อผู้ใช้ใหม่ (newUsername) ซ้ำกับชื่อผู้ใช้ที่มีอยู่ในระบบและชื่อผู้ใช้เดิม (oldUsername) ไม่เหมือนกับชื่อผู้ใช้ใหม่
                //ก็จะแสดงข้อความเตือนให้ผู้ใช้เลือกชื่อใหม่
                MessageBox.Show("ชื่อผู้ใช้นี้มีอยู่ในระบบแล้ว กรุณาใช้ชื่ออื่นค่ะ");
                return;
            }
            //เปิดการเชื่อมต่อกับฐานข้อมูลโดยเรียกใช้ฟังก์ชัน databaseConnection ซึ่งจะคืนค่าการเชื่อมต่อ MySQL
            using (MySqlConnection conn = databaseConnection())
            {
                string query = "UPDATE name SET username = @newUsername, email = @email, number = @number WHERE username = @oldUsername";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@oldUsername", oldUsername); // ใช้ชื่อเดิมเพื่อค้นหาข้อมูล
                cmd.Parameters.AddWithValue("@newUsername", newUsername); // ใช้ชื่อใหม่เพื่ออัปเดต
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@number", phoneNumber); // ใช้ phoneNumber เป็น string
                //เปิดการเชื่อมต่อฐานข้อมูลเพื่อให้สามารถดำเนินการคำสั่ง SQL ได้
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery(); //เพื่อส่งคำสั่ง SQL ไปยังฐานข้อมูล โดยไม่ต้องการผลลัพธ์กลับมา(เช่นการแทรกหรืออัปเดตข้อมูล).
                if (rowsAffected > 0)
                {
                    
                    MessageBox.Show("แก้ไขข้อมูลลูกค้าสำเร็จ");
                    LoadAllCustomers(); // โหลดข้อมูลใหม่หลังการแก้ไข
                }
                //ถ้าไม่มีแถวข้อมูลที่ได้รับผลกระทบ (แสดงว่าไม่มีการเปลี่ยนแปลงข้อมูลหรือเกิดข้อผิดพลาด)
                else
                {
                    MessageBox.Show("ไม่สามารถแก้ไขข้อมูลลูกค้าได้");
                }
                // รีเซ็ตฟอร์ม ถ้าอัปเดตสำเร็จ

                textBox2.Clear(); //ชื่อผู้ใช้
                textBox3.Clear(); //อีเมล
                textBox4.Clear(); //เบอร์โทร
            }
        }
        //ปุ่มบันทึกข้อมูลเมื่อแก้ไขข้อมูล
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox2.Text;  // ดึงค่าจาก textBox2 (ชื่อผู้ใช้) มาเก็บในตัวแปร username
            string email = textBox3.Text;  // ดึงค่าจาก textBox3 (อีเมล) มาเก็บในตัวแปร email
            string phoneNumber = textBox4.Text;  // ดึงค่าจาก textBox4 (เบอร์โทรศัพท์) มาเก็บในตัวแปร phoneNumber
            string oldUsername = dataGridView1.CurrentRow.Cells["username"].Value.ToString(); // ดึงชื่อเดิมจาก DataGridView

            UpdateCustomer(oldUsername, username, email, phoneNumber); // ส่งชื่อเดิมไปอัปเดต
        }
        //หน้าที่จัดการกับเหตุการณ์ที่เกิดขึ้นเมื่อผู้ใช้คลิกที่เซลล์ภายใน DataGridView
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // ตรวจสอบว่าแถวที่คลิกมีข้อมูล (RowIndex ต้อง >= 0)
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells["username"].Value != null)
            //ตรวจสอบว่า RowIndex แถวที่คลิกต้องเป็นแถวที่มีข้อมูลจริง ๆ ไม่ใช่แถวที่ว่าง
            //ตรวจสอบว่าเซลล์ในคอลัมน์ "username" ที่แถวที่เลือกมีค่าที่ไม่เป็น null ซึ่งหมายความว่ามีข้อมูลในเซลล์นั้น
            {

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex]; //เมื่อเงื่อนไขข้างต้นถูกต้องแล้ว, แถวที่ถูกคลิกจะถูกดึงมาเป็นตัวแปร row ซึ่งเป็นประเภท DataGridViewRow.
                textBox2.Text = row.Cells["username"].Value.ToString(); //แปลงค่าจาก Value ของเซลล์เป็น string และแสดงใน textBox2.
                textBox3.Text = row.Cells["email"].Value.ToString(); //แปลงค่าจาก Value ของเซลล์เป็น string และแสดงใน textBox3.
                textBox4.Text = row.Cells["number"].Value.ToString(); //แปลงค่าจาก Value ของเซลล์เป็น string และแสดงใน textBox4.
            }
        }
        //ปุ่มกลับหน้าหลัก
        private void button3_Click(object sender, EventArgs e)
        {
            Form5 F5 = new Form5();
            F5.Show();
            this.Hide();
        }
        // ปุ่มปิดโปรแกรม
        private void button5_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }
    }
 }