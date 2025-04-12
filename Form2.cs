using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp4
{
    public partial class Form2 : Form
    {
        private MySqlConnection databaseConnection()
        {
            // สร้างการเชื่อมต่อกับฐานข้อมูล
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=mmigichanew;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        public Form2()
        {
            //Constructor (คอนสตัคเจอร์) ของฟอร์ม Form1
            InitializeComponent();  //เรียกใช้เมธอด (อินิเฉอะลายคัมโพเนิน) เพื่อกำหนดค่าคอนโทรลต่าง ๆ ที่ถูกออกแบบไว้ใน Form Designer

            // กำหนดเหตุการณ์ KeyPress สำหรับ textBox1 textBox2 textBox3
            textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);  // เมื่อกดปุ่มใน textBox1 จะเรียกใช้ method textBox1_KeyPress
            textBox2.KeyPress += new KeyPressEventHandler(textBox2_KeyPress);  // เมื่อกดปุ่มใน textBox2 จะเรียกใช้ method textBox2_KeyPress
            textBox3.KeyPress += new KeyPressEventHandler(textBox3_KeyPress);  // เมื่อกดปุ่มใน textBox3 จะเรียกใช้ method textBox3_KeyPress

            // กำหนดเหตุการณ์ Textเซ๊ง สำหรับ textBox1 และ textBox2
            textBox1.TextChanged += new EventHandler(textBox1_TextChanged);  // เมื่อข้อความใน textBox1 ,textBox2 เปลี่ยนแปลง จะเรียกใช้ method textBox1 textBox2_TextChanged
            textBox2.TextChanged += new EventHandler(textBox2_TextChanged);  
           //+= ใช้ในการผูกอีเวนต์(KeyPress ของ textBox1) กับเมธอดที่ต้องการให้ทำงานเมื่อเหตุการณ์นั้นเกิดขึ้น(textBox1_KeyPress).
        }
        //สำหรับกรอกusername
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //ให้สามารถกรอกตัวอักษรและสัญลักษณ์ได้
            if (textBox1.Text.Length >= 10 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // ปฎิเสธการกรอกได้ไม่เกิน (10ตัว)
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // ตรวจสอบความยาวของข้อความที่กรอกใน textBox1 มีความยากเกิน 10 ตัวไหม
            if (textBox1.Text.Length > 10)  // หากข้อมูลใน textBox1 มีความยาวมากกว่า 10 ตัว
            {
                // กรอกข้อความได้แค่10ตัว และตัดข้อความที่เกิน 10 ตัวออก
                textBox1.Text = textBox1.Text.Substring(0, 10); //(subสเติง) 

                // เลื่อนตำแหน่งเคอร์เซอร์ไปยังตำแหน่งสุดท้ายของข้อความที่เหลือ
                textBox1.SelectionStart = textBox1.Text.Length;  
            }
        }
        
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //กรอกได้เฉพาะตัวเลข เช่น เบอร์โทร
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // ปฏิเสธการกรอกตัวอักษรที่ไม่ใช่ตัวเลข
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // ตรวจสอบความยาวของข้อความที่กรอกใน textBox2 มีความยากเกิน 10 ตัวไหม
            if (textBox2.Text.Length > 10)
            {
                // กรอกข้อความได้แค่10ตัว และตัดข้อความที่เกิน 10 ตัวออก
                textBox2.Text = textBox2.Text.Substring(0, 10);
                // เลื่อนตำแหน่งเคอร์เซอร์ไปยังตำแหน่งสุดท้ายของข้อความ
                textBox2.SelectionStart = textBox2.Text.Length;
            }
        } 
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // จำกัดให้ textBox3 สามารถกรอกได้เฉพาะตัวอักษรและตัวเลข เช่น @, ., และ - 
            // และปฏิเสธอักขระภาษาไทยหรืออักขระที่ไม่ได้รับอนุญาต
            if ((!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '@' && e.KeyChar != '.' && e.KeyChar != '-' && !char.IsControl(e.KeyChar)) ||
                (e.KeyChar >= 0x0E00 && e.KeyChar <= 0x0E7F)) // ตรวจสอบว่าเป็นตัวอักษรภาษาไทยหรือไม่
            {
                e.Handled = true; // ปฏิเสธอักขระที่ไม่ถูกต้อง
            }
        }
        // ปุ่มลงทะเบียน
        private void button2_Click(object sender, EventArgs e)
        {
            // รับค่าจาก TextBox
            string username = textBox1.Text;  //  (ชื่อผู้ใช้)
            string number = textBox2.Text;    //  (หมายเลขโทรศัพท์)
            string email = textBox3.Text;     //  (อีเมล)

            // ตรวจสอบความยาวของเบอร์โทรศัพท์
            if (number.Length != 10)  // เช็คว่าเบอร์โทรศัพท์มีความยาว 10 ตัวหรือไม่
            {
                MessageBox.Show("กรุณากรอกเบอร์โทรศัพท์ให้ครบ 10 ตัว");  // ถ้าไม่ครบ 10 ตัว จะแสดงข้อความแจ้งเตือน
                return;  // หยุดการทำงานของฟังก์ชันหากเงื่อนไขไม่ถูกต้อง
            }

            // เชื่อมต่อฐานข้อมูล
            using (MySqlConnection conn = databaseConnection())  // เปิดการเชื่อมต่อกับฐานข้อมูล
            {
                conn.Open();  // เปิดการเชื่อมต่อ
                MySqlTransaction transaction = conn.BeginTransaction();  
                try
                {
                    // ตรวจสอบข้อมูลซ้ำในตาราง history   //เคาท                //แว
                    string checkHistory = "SELECT COUNT(*) FROM history WHERE username = @username";  
                    using (MySqlCommand cmdCheckHistory = new MySqlCommand(checkHistory, conn, transaction))
                    {
                        cmdCheckHistory.Parameters.AddWithValue("@username", username);  // เพิ่ม parameter username

                        int countHistory = Convert.ToInt32(cmdCheckHistory.ExecuteScalar());  // นับจำนวนแถวที่ตรงกับ username
                        if (countHistory > 0)  // ถ้ามีข้อมูลซ้ำ 
                        {
                            MessageBox.Show("ชื่อนี้มีการใช้งานแล้ว กรุณากรอกชื่อใหม่ค่ะ");  // แจ้งเตือนให้กรอกชื่อใหม่
                            transaction.Rollback();  
                            return;
                        }
                    }
                    // ตรวจสอบข้อมูลซ้ำในตาราง history สำหรับเบอร์โทรศัพท์ ว่ามีข้อมูลซ้ำหรือไม่
                                                          //เคาท
                    string checkNumberInHistory = "SELECT COUNT(*) FROM history WHERE number = @number";  
                    using (MySqlCommand cmdCheckNumberInHistory = new MySqlCommand(checkNumberInHistory, conn, transaction))
                    {
                        cmdCheckNumberInHistory.Parameters.AddWithValue("@number", number);  // เพิ่ม parameter number
                        // นับจำนวนแถวที่ตรงกับหมายเลขโทรศัพท์
                        int countNumberInHistory = Convert.ToInt32(cmdCheckNumberInHistory.ExecuteScalar());  
                        if (countNumberInHistory > 0)  // ถ้ามีเบอร์ซ้ำ
                        {
                            MessageBox.Show("มีเบอร์นี้มีการใช้งานแล้ว กรุณากรอกเบอร์ใหม่ค่ะ");  // แจ้งเตือนให้กรอกเบอร์ใหม่
                            transaction.Rollback();  // ยกเลิก เทรนแซค'เชิน
                            return;
                        }
                    }

                    // ตรวจสอบว่า username ซ้ำในตาราง name(สมาชิก) หรือไม่
                    string checkName = "SELECT COUNT(*) FROM name WHERE username = @username";  
                    using (MySqlCommand cmdCheckName = new MySqlCommand(checkName, conn, transaction))
                    {
                        cmdCheckName.Parameters.AddWithValue("@username", username);  // เพิ่ม parameter username

                        int countName = Convert.ToInt32(cmdCheckName.ExecuteScalar());  // นับจำนวนแถวที่ตรงกับ username
                        if (countName > 0)  // ถ้ามีข้อมูลซ้ำ
                        {
                            MessageBox.Show("ชื่อนี้มีการใช้งานแล้ว กรุณากรอกชื่อใหม่ค่ะ");  // แจ้งเตือนให้กรอกชื่อใหม่
                            transaction.Rollback();  // ยกเลิก transaction 
                            return;
                        }
                    }
                    // ตรวจสอบข้อมูลซ้ำในตาราง name สำหรับเบอร์โทรศัพท์ ซ้ำในตาราง name หรือไม่
                    string checkNumberInName = "SELECT COUNT(*) FROM name WHERE number = @number";  
                    using (MySqlCommand cmdCheckNumberInName = new MySqlCommand(checkNumberInName, conn, transaction))
                    {
                        cmdCheckNumberInName.Parameters.AddWithValue("@number", number);  // เพิ่ม parameter number

                        int countNumberInName = Convert.ToInt32(cmdCheckNumberInName.ExecuteScalar());  // นับจำนวนแถวที่ตรงกับหมายเลขโทรศัพท์
                        if (countNumberInName > 0)  // ถ้ามีเบอร์ซ้ำ
                        {
                            MessageBox.Show("มีเบอร์นี้มีการใช้งานแล้ว กรุณากรอกเบอร์ใหม่ค่ะ");  // แจ้งเตือนให้กรอกเบอร์ใหม่
                            transaction.Rollback();  // ยกเลิก transaction
                            return;
                        }
                    }
                    // คำสั่ง SQL เพื่อเพิ่มข้อมูลในตาราง name
                    string sqlName = "INSERT INTO name (username, number, email) VALUES (@username, @number, @email)";  // คำสั่ง SQL สำหรับเพิ่มข้อมูล
                    using (MySqlCommand cmdName = new MySqlCommand(sqlName, conn, transaction))
                    {
                        cmdName.Parameters.AddWithValue("@username", username);  // เพิ่ม parameter username
                        cmdName.Parameters.AddWithValue("@number", number);  // เพิ่ม parameter number
                        cmdName.Parameters.AddWithValue("@email", email);  // เพิ่ม parameter email

                        cmdName.ExecuteNonQuery();  // ดำเนินการคำสั่ง SQL
                    }

                    // ยืนยัน (แทรนแซคเชิน)
                    transaction.Commit();  // ยืนยันการทำธุรกรรมทั้งหมด
                    //เมื่อสมาชิกเสร็จ
                    // เคลียร์ข้อมูลใน TextBox
                    textBox1.Clear();  
                    textBox2.Clear();  
                    textBox3.Clear(); 

                    MessageBox.Show("สมัครเรียบร้อยแล้วค่ะ");  

                    // กด ok เปลี่ยนไปยังหน้า Form1
                    Form1 form1 = new Form1();  // สร้างฟอร์มใหม่
                    form1.Show(); 
                    this.Hide();  
                }
                catch (Exception ex)
                {
                    // ยกเลิก เทรนแซค'เชิน หากเกิดข้อผิดพลาด
                    transaction.Rollback();  // ยกเลิก transaction
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);  // แสดงข้อความข้อผิดพลาด
                }
            }
        }
        //ปุ่มกลับหน้าหลัก
        private void button3_Click(object sender, EventArgs e)
        {
            // เปิด Form1 และซ่อน Form2
            Form1 F1 = new Form1();
            F1.Show();
            this.Hide();
        }
        //ปุ่มออกจากโปรแกรม
        private void button4_Click(object sender, EventArgs e)
        {
            // ปิดโปรแกรม
            Application.Exit();
        }
    }
}