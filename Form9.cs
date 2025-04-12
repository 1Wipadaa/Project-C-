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
    public partial class Form9 : Form
    {
        private MySqlConnection databaseConnection()
        {
            // สร้างการเชื่อมต่อกับฐานข้อมูล
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=mmigichanew;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        public Form9()
        {
            InitializeComponent(); //เรียกใช้เมธอด (อินิเฉอะลายคัมโพเนิน) เพื่อกำหนดค่าคอนโทรลต่าง ๆ ที่ถูกออกแบบไว้ใน Form Designer
        }
        private void Form9_Load(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
            LoadMenuNames();
        }
        private void LoadMenuNames()
        {
            // ล้างข้อมูลที่มีอยู่ใน comboBox1 เพื่อไม่ให้ซ้ำซ้อน
            comboBox1.Items.Clear();

            // สร้างการเชื่อมต่อกับฐานข้อมูล
            using (MySqlConnection conn = databaseConnection())
            {
                // เปิดการเชื่อมต่อฐานข้อมูล
                conn.Open();

                // ดึงชื่อเมนูจากตาราง menucha และ topping โดยรวมผลลัพธ์เข้าด้วยกัน (ไม่ซ้ำกัน)
                string query = "SELECT name FROM menucha UNION SELECT name FROM topping";

                // สร้างคำสั่ง SQL ด้วย query ที่เขียนไว้
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                // ใช้ MySqlDataReader เพื่ออ่านข้อมูลที่ได้จากฐานข้อมูล
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    // วนลูปอ่านข้อมูลทีละแถวจนกว่าจะจบ
                    while (reader.Read())
                    {
                        // เพิ่มชื่อเมนูแต่ละชื่อลงใน comboBox1
                        comboBox1.Items.Add(reader.GetString("name"));
                    }
                }
            }
        }
        // เมื่อมีการเลือกชื่อสินค้าจาก comboBox1 เพื่อแก้ไขสินค้าต่อไป
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ดึงค่าชื่อสินค้าที่ถูกเลือกจาก comboBox1
            string selectedName = comboBox1.SelectedItem?.ToString();

            // ถ้าไม่ได้เลือกชื่อสินค้า ให้หยุดทำงานทันที
            if (string.IsNullOrEmpty(selectedName)) return;

            // เชื่อมต่อกับฐานข้อมูล
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();

                // ค้นหาข้อมูลจากทั้ง menucha และ topping ที่มีชื่อตรงกับที่เลือก
                string query = @"SELECT 'menucha' AS source, price, quantity, image FROM menucha WHERE name = @name
                         UNION 
                         SELECT 'topping', price, quantity, image FROM topping WHERE name = @name";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // ส่งพารามิเตอร์ชื่อสินค้าเข้าไปในคำสั่ง SQL
                    cmd.Parameters.AddWithValue("@name", selectedName);

                    // อ่านผลลัพธ์ที่ได้จากการรันคำสั่ง SQL
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // ถ้าพบข้อมูล
                        {
                            // แสดงราคาสินค้าใน textBox3
                            textBox3.Text = reader["price"].ToString();

                            // แสดงจำนวนสินค้าใน textBox4
                            textBox4.Text = reader["quantity"].ToString();

                            // ตรวจสอบว่ามีรูปภาพหรือไม่
                            if (reader["image"] != DBNull.Value)
                            {
                                // แปลงข้อมูลรูปภาพจาก byte[] เป็นรูปภาพแล้วแสดงใน pictureBox1
                                byte[] imgData = (byte[])reader["image"];
                                using (MemoryStream ms = new MemoryStream(imgData))
                                {
                                    pictureBox1.Image = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                // ถ้าไม่มีรูปภาพ ให้ลบรูปภาพที่แสดงอยู่
                                pictureBox1.Image = null;
                            }

                            // เก็บชื่อของตาราง (menucha หรือ topping) ไว้ใน comboBox1.Tag
                            comboBox1.Tag = reader["source"].ToString();
                        }
                        else // ถ้าไม่พบข้อมูล
                        {

                            MessageBox.Show("ไม่พบข้อมูลของเมนูนี้");

                            //ล้างข้อมูลและลบรูปที่แสดงอยู่
                            textBox3.Text = "";
                            textBox4.Text = "";
                            pictureBox1.Image = null;
                        }
                    }
                }
            }
        }
        // ปุ่มแก้ไขข้อมูล เพื่ออัปเดตข้อมูลสินค้า
        private void button4_Click(object sender, EventArgs e)
        {
            // ดึงชื่อสินค้าที่ถูกเลือกจาก comboBox1
            string selectedName = comboBox1.SelectedItem?.ToString();

            // ถ้ายังไม่ได้เลือกรายการสินค้า ให้แสดงข้อความแจ้งเตือนและหยุดการทำงาน
            if (string.IsNullOrEmpty(selectedName))
            {
                MessageBox.Show("กรุณาเลือกรายการ");
                return;
            }

            // ดึงราคาจาก textBox3 และจำนวนสินค้าจาก textBox4
            string price = textBox3.Text;
            string quantity = textBox4.Text;

            // ประกาศตัวแปรสำหรับเก็บข้อมูลรูปภาพในรูปแบบ byte[]
            byte[] imageBytes = null;

            // ถ้ามีรูปภาพแสดงอยู่ใน pictureBox1 ให้นำไปแปลงเป็น byte[]
            if (pictureBox1.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // บันทึกรูปภาพลงใน MemoryStream ในรูปแบบ PNG
                    pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    // แปลง MemoryStream เป็นอาร์เรย์ของ byte
                    imageBytes = ms.ToArray();
                }
            }

            // ดึงชื่อของตาราง (menucha หรือ topping) จาก comboBox1
            string tableName = comboBox1.Tag?.ToString();

            // ตรวจสอบว่าแหล่งข้อมูลถูกต้องหรือไม่ (ต้องเป็น menucha หรือ topping เท่านั้น)
            if (tableName != "menucha" && tableName != "topping")
            {
                MessageBox.Show("ไม่สามารถระบุแหล่งข้อมูลได้");
                return;
            }

            // เชื่อมต่อกับฐานข้อมูล
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();

                // สร้างคำสั่ง SQL สำหรับอัปเดตข้อมูลสินค้าที่ชื่อ = selectedName ในตารางที่ระบุ
                string query = $"UPDATE `{tableName}` SET price = @price, quantity = @quantity, image = @image WHERE name = @name";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // เพิ่มพารามิเตอร์เข้าไปในคำสั่ง SQL
                    cmd.Parameters.AddWithValue("@price", price);         // ราคาสินค้าใหม่
                    cmd.Parameters.AddWithValue("@quantity", quantity);   // จำนวนสินค้าใหม่
                    cmd.Parameters.AddWithValue("@image", imageBytes);    // รูปภาพ (ในรูปแบบ byte[])
                    cmd.Parameters.AddWithValue("@name", selectedName);   // ชื่อสินค้าที่จะอัปเดต

                    // ดำเนินการอัปเดตข้อมูลในฐานข้อมูล
                    int rows = cmd.ExecuteNonQuery();

                    // ถ้ามีการอัปเดตข้อมูล (row มากกว่า 0) ให้แสดงข้อความสำเร็จ
                    if (rows > 0)
                    {
                        MessageBox.Show("อัปเดตข้อมูลเรียบร้อยแล้ว");
                    }
                    else
                    {
                        // ถ้าไม่พบข้อมูลในตารางให้แก้ไข ให้แสดงข้อความแจ้งเตือน
                        MessageBox.Show("ไม่พบรายการให้แก้ไข");
                    }
                }
            }
        }
        // ปุ่ม "ลบ" เพื่อทำการลบสินค้าที่เลือก
        private void button6_Click(object sender, EventArgs e)
        {
            // ดึงชื่อสินค้าที่ถูกเลือกจาก comboBox1
            string selectedName = comboBox1.SelectedItem?.ToString();

            // ถ้ายังไม่ได้เลือกรายการสินค้า ให้แสดงข้อความแจ้งเตือนและหยุดการทำงาน
            if (string.IsNullOrEmpty(selectedName))
            {
                MessageBox.Show("กรุณาเลือกรายการที่จะลบ");
                return;
            }

            // ดึงชื่อของตาราง (menucha หรือ topping) จาก comboBox1
            string tableName = comboBox1.Tag?.ToString();

            // ตรวจสอบว่าแหล่งข้อมูลถูกต้องหรือไม่
            if (tableName != "menucha" && tableName != "topping")
            {
                MessageBox.Show("ไม่สามารถระบุแหล่งข้อมูลได้");
                return;
            }

            // แสดงกล่องยืนยันการลบ ให้ผู้ใช้กดยืนยันก่อนลบจริง
            DialogResult result = MessageBox.Show("คุณแน่ใจหรือไม่ว่าต้องการลบรายการนี้?", "ยืนยันการลบ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            // ถ้าผู้ใช้กด "ไม่ใช่" (No) ให้หยุดการทำงาน ไม่ลบข้อมูล
            if (result == DialogResult.No) return;

            // เริ่มการเชื่อมต่อกับฐานข้อมูล
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();

                // สร้างคำสั่ง SQL สำหรับลบข้อมูลจากตารางที่ระบุ โดยอ้างอิงจากชื่อสินค้า
                string query = $"DELETE FROM `{tableName}` WHERE name = @name";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // กำหนดพารามิเตอร์ชื่อสินค้าเข้าไปในคำสั่ง SQL
                    cmd.Parameters.AddWithValue("@name", selectedName);

                    // ดำเนินการลบข้อมูล
                    int rows = cmd.ExecuteNonQuery();

                    // ถ้าลบข้อมูลสำเร็จ (มีแถวถูกลบ)
                    if (rows > 0)
                    {
                        MessageBox.Show("ลบข้อมูลสำเร็จ");

                        // ลบชื่อสินค้าที่ลบออกจาก comboBox1
                        comboBox1.Items.Remove(selectedName);

                        // เคลียร์ข้อมูลที่แสดงอยู่ใน textbox และรูปภาพ
                        textBox3.Clear();
                        textBox4.Clear();
                        pictureBox1.Image = null;
                    }
                    else
                    {
                        // ถ้าไม่พบรายการให้ลบ
                        MessageBox.Show("ไม่พบข้อมูลให้ลบ");
                    }
                }
            }
        }
        //ปุ่มเลือกรูปภาพ
        private void button7_Click(object sender, EventArgs e)
        {
            // เปิดหน้าต่างเลือกรูปภาพ
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp";
            openFileDialog1.Title = "เลือกรูปภาพ";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // แสดงรูปภาพที่เลือกใน pictureBox
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
            }
        }
        // เมื่อคลิกปุ่มเพิ่มเมนู เพื่อเพิ่มสินค้าลงในตาราง menucha
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // รับค่าชื่อ ราคา และจำนวนจาก TextBox
                string name = textBox2.Text;
                string price = textBox3.Text;
                string quantity = textBox4.Text;
                byte[] image = null; // ประกาศตัวแปรเก็บข้อมูลรูปภาพในรูปแบบ byte[]

                // ถ้ามีการเลือกรูปภาพ ให้แปลงรูปจาก PictureBox เป็น byte array
                if (pictureBox1.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // บันทึกรูปภาพจาก PictureBox ลงใน MemoryStream
                        pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                        image = ms.ToArray(); // แปลงรูปภาพเป็น byte array
                    }
                }

                // เชื่อมต่อฐานข้อมูล MySQL
                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open(); // เปิดการเชื่อมต่อฐานข้อมูล

                    // สร้างคำสั่ง SQL สำหรับเพิ่มข้อมูลใหม่ลงในตาราง menucha
                    string insertQuery = "INSERT INTO `menucha` (name, price, quantity, image) VALUES (@name, @price, @quantity, @image)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        // เพิ่มค่าพารามิเตอร์เพื่อป้องกัน SQL Injection
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@image", image);

                        // สั่งให้ทำงานคำสั่ง SQL
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // ถ้ามีการเพิ่มแถวใหม่ (rowsAffected > 0) ให้แสดงข้อความว่าทำสำเร็จ
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("เพิ่มข้อมูลสำเร็จในตาราง menucha สำเร็จ");
                        }
                    }
                }

                // หลังจากเพิ่มเสร็จแล้ว ให้เคลียร์ข้อมูลทั้งหมด
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                pictureBox1.Image = null;
            }
            catch (Exception ex)
            {
                // ถ้าเกิดข้อผิดพลาดใด ๆ ให้แสดงข้อความแจ้งผู้ใช้
                MessageBox.Show("กรุณากรอกข้อมูลก่อนค่ะ " + ex.Message);
            }
        }
        // ปุ่มเพิ่มท็อปปิ้ง
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // รับค่าชื่อ ราคา และจำนวนจาก TextBox
                string name = textBox2.Text;
                string price = textBox3.Text;
                string quantity = textBox4.Text;
                byte[] image = null; // ประกาศตัวแปรเก็บข้อมูลรูปภาพในรูปแบบ byte[]

                // ถ้ามีการเลือกรูปภาพ ให้แปลงรูปจาก PictureBox เป็น byte array
                if (pictureBox1.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // บันทึกรูปภาพจาก PictureBox ลงใน MemoryStream
                        pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                        image = ms.ToArray(); // แปลงรูปภาพเป็น byte array
                    }
                }

                // เชื่อมต่อฐานข้อมูล MySQL
                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open(); // เปิดการเชื่อมต่อฐานข้อมูล

                    // สร้างคำสั่ง SQL สำหรับเพิ่มข้อมูลใหม่ลงในตาราง topping
                    string insertQuery = "INSERT INTO `topping` (name, price, quantity, image) VALUES (@name, @price, @quantity, @image)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        // เพิ่มค่าพารามิเตอร์เพื่อป้องกัน SQL Injection
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@image", image);

                        // สั่งให้ทำงานคำสั่ง SQL
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // ถ้ามีการเพิ่มแถวใหม่ (rowsAffected > 0) ให้แสดงข้อความว่าทำสำเร็จ
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("เพิ่มข้อมูลสำเร็จในตาราง topping สำเร็จ");
                        }
                    }
                }

                // หลังจากเพิ่มเสร็จแล้ว ให้เคลียร์ข้อมูลทั้งหมด
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                pictureBox1.Image = null;
            }
            catch (Exception ex)
            {
                // ถ้าเกิดข้อผิดพลาดใด ๆ ให้แสดงข้อความแจ้งผู้ใช้
                MessageBox.Show("กรุณากรอกข้อมูลก่อนค่ะ " + ex.Message);
            }
        }
        //ปุ่มย้อนกลับ
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

