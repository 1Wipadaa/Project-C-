using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form6 : Form
    {
        private MySqlConnection databaseConnection()
        {
            // สร้างการเชื่อมต่อกับฐานข้อมูล
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=mmigichanew;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        public Form6()  // ฟังก์ชันคอนสตรัคเตอร์สำหรับสร้างและเริ่มต้น Form6
        {
            InitializeComponent();  // เรียกใช้งานเมธอดที่สร้าง UI ของ Form6
            comboBox3.Items.AddRange(new string[]  // เพิ่มรายการใน ComboBox3
            {
                "0% ไม่หวาน",  // ตัวเลือกที่ 1
                "25% หวานน้อย",  // ตัวเลือกที่ 2
                "50% หวาน",  // ตัวเลือกที่ 3
                "75% หวานปานกลาง",  // ตัวเลือกที่ 4
                "100% หวานมาก"  // ตัวเลือกที่ 5
            });
        }
        // ฟังก์ชันที่จะทำงานเมื่อ Form6 โหลด
        private void Form6_Load(object sender, EventArgs e)  
        {
            LoadComboBoxData();  // เรียกใช้เมธอด LoadComboBoxData() เพื่อดึงข้อมูลจากฐานข้อมูลและแสดงใน ComboBox
            LoadToppingData();  // เรียกใช้เมธอด LoadToppingData() เพื่อดึงข้อมูลจากฐานข้อมูลสำหรับข้อมูล topping
            using (MySqlConnection conn = databaseConnection()) //เชื่อมต่อกับฐานข้อมูล
            {
                conn.Open();  // เปิดการเชื่อมต่อกับฐานข้อมูล

                string query = "SELECT namecha, topping, sweet, quantity, price, allprice, image FROM `order`"; 
                using (MySqlCommand cmd = new MySqlCommand(query, conn))  // สร้างคำสั่ง SQL จากการเชื่อมต่อฐานข้อมูล
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())  // ดึงข้อมูลจากฐานข้อมูล
                    {
                        DataTable dt = new DataTable();  // สร้าง DataTable เพื่อเก็บข้อมูลที่ดึงมา
                        dt.Load(reader);  // โหลดข้อมูลจาก reader ไปยัง DataTable
                        dataGridView1.DataSource = dt;  // กำหนด Dataซอร์ส ของ DataGridView1 ให้เป็น DataTable ที่ได้
                    }
                }
            }
        }
        // โหลดข้อมูลจากตาราง 'order' มาแสดงใน DataGridView
        private void LoadOrderData()
        {
            try
            {
                // เชื่อมต่อกับฐานข้อมูล
                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open();  // เปิดการเชื่อมต่อกับฐานข้อมูล

                    // สร้างคำสั่ง SQL เพื่อดึงข้อมูลจากตาราง 'order'
                    string query = "SELECT namecha, topping, sweet, quantity, price, allprice, image FROM `order`";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // ดึงข้อมูลจากฐานข้อมูลและเก็บไว้ใน DataReader
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();  // สร้าง DataTable เพื่อเก็บข้อมูลที่ดึงมา
                            dt.Load(reader);  // โหลดข้อมูลจาก DataReader เข้า DataTable

                            // สร้าง Dictionary เพื่อเก็บรูปภาพจากข้อมูล
                            Dictionary<int, Image> imageDict = new Dictionary<int, Image>();

                            // ลูปผ่านแต่ละแถวของ DataTable
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                // ตรวจสอบว่ามีข้อมูลรูปภาพในแถวนี้หรือไม่
                                if (!(dt.Rows[i]["image"] is DBNull) && dt.Rows[i]["image"] is byte[])
                                {
                                    byte[] imgBytes = (byte[])dt.Rows[i]["image"];  // ดึงข้อมูลรูปภาพจากฐานข้อมูล

                                    if (imgBytes.Length > 0)  // ตรวจสอบว่ามีข้อมูลจริง
                                    {
                                        try
                                        {
                                            // แปลง byte[] ที่ได้เป็นรูปภาพ (Image) และเก็บใน Dictionary
                                            using (MemoryStream ms = new MemoryStream(imgBytes))
                                            {
                                                imageDict[i] = Image.FromStream(ms);  // เก็บรูปภาพใน Dictionary โดยใช้ index ของแถว
                                            }
                                        }
                                        catch (Exception imgEx)
                                        {
                                            // ถ้าเกิดข้อผิดพลาดในการแปลงรูปภาพ จะแสดงข้อความเตือนและป้องกันการใส่รูปภาพผิดพลาด
                                            MessageBox.Show("Error in Image Processing: " + imgEx.Message, "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            imageDict[i] = null;  // ป้องกันการใส่รูปภาพที่ผิดพลาด
                                        }
                                    }
                                }
                            }

                            // ตั้งค่า DataGridView ให้แสดงข้อมูลจาก DataTable
                            dataGridView1.DataSource = dt;

                            // ตรวจสอบว่า DataGridView มีคอลัมน์รูปภาพอยู่หรือยัง
                            if (!(dataGridView1.Columns["image"] is DataGridViewImageColumn))
                            {
                                // ถ้าไม่มีคอลัมน์รูปภาพ ให้สร้างคอลัมน์ใหม่
                                DataGridViewImageColumn newImgColumn = new DataGridViewImageColumn
                                {
                                    Name = "image",  // ตั้งชื่อคอลัมน์เป็น "image"
                                    HeaderText = "Image",  // ตั้งหัวข้อของคอลัมน์
                                    ImageLayout = DataGridViewImageCellLayout.Zoom  // ตั้งการแสดงผลของรูปภาพเป็นแบบซูม
                                };
                                dataGridView1.Columns.Add(newImgColumn);  // เพิ่มคอลัมน์ใหม่เข้าไปใน DataGridView
                            }

                            // ใส่รูปภาพที่ได้จาก Dictionary ลงใน DataGridView
                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                if (imageDict.ContainsKey(i) && imageDict[i] != null)
                                {
                                    // ใส่รูปภาพลงในเซลล์ของ DataGridView ที่ตรงกับ index
                                    dataGridView1.Rows[i].Cells["image"].Value = imageDict[i];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ถ้าเกิดข้อผิดพลาดในการโหลดข้อมูลจากฐานข้อมูลจะแสดงข้อความเตือน
                MessageBox.Show("เกิดข้อผิดพลาดขณะโหลดข้อมูล: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //  โหลดข้อมูลลงcombobox ดึงชื่อจากmenucha
        private void LoadComboBoxData()
        {
            try
            {
                // เชื่อมต่อกับฐานข้อมูล
                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open();  // เปิดการเชื่อมต่อกับฐานข้อมูล

                    // สร้างคำสั่ง SQL เพื่อดึงข้อมูลชื่อจากตาราง 'menucha'
                    string query = "SELECT name FROM menucha";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // ดึงข้อมูลจากฐานข้อมูล
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            comboBox1.Items.Clear();  // ล้างข้อมูลที่มีใน ComboBox ก่อน เพื่อป้องกันข้อมูลซ้ำ

                            // ลูปผ่านผลลัพธ์ที่ได้จากฐานข้อมูล
                            while (reader.Read())
                            {
                                // ดึงชื่อจากฐานข้อมูลที่ได้
                                string name = reader["name"].ToString();
                                comboBox1.Items.Add(name);  // เพิ่มชื่อที่ได้ลงใน ComboBox
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // ถ้าเกิดข้อผิดพลาดเกี่ยวกับฐานข้อมูลจะแสดงข้อความนี้
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // ถ้าเกิดข้อผิดพลาดทั่วไปจะแสดงข้อความนี้
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ดึงชื่อจากตาราง topping ลง checkedListBox1
        private void LoadToppingData()
        {
            try
            {
                // เชื่อมต่อกับฐานข้อมูล
                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล

                    // สร้างคำสั่ง SQL เพื่อดึงชื่อจากตาราง topping
                    string query = "SELECT name FROM topping";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // ดึงข้อมูลจากฐานข้อมูล
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            checkedListBox1.Items.Clear(); // ล้างข้อมูลเก่าใน CheckedListBox ก่อนเพื่อไม่ให้ข้อมูลซ้ำ

                            // ลูปเพื่ออ่านข้อมูลทีละแถว
                            while (reader.Read())
                            {
                                // ดึงค่าชื่อท็อปปิ้งจากฐานข้อมูลและแปลงเป็น string
                                string name = reader["name"].ToString();
                                // เพิ่มชื่อท็อปปิ้งลงใน CheckedListBox
                                checkedListBox1.Items.Add(name);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // ถ้าเกิดข้อผิดพลาดเกี่ยวกับฐานข้อมูลจะจับข้อผิดพลาดและแสดงข้อความ
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // ถ้าเกิดข้อผิดพลาดทั่วไปจะจับข้อผิดพลาดและแสดงข้อความ
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //โชว์ตารางออเดอร์(ตระกร้าสินค้า)
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // ตรวจสอบว่า DataGridView มีข้อมูลหรือไม่
            if (dataGridView1.Rows.Count == 0 || e.RowIndex < 0) return; // ถ้า DataGridView ไม่มีข้อมูลหรือคลิกที่แถวไม่ถูกต้อง (RowIndex < 0) ให้หยุดการทำงาน

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex]; // กำหนดตัวแปร 'row' ให้เป็นแถวที่ถูกคลิก

            // ตรวจสอบและตั้งค่าข้อมูลใน ComboBox และ TextBox
            comboBox1.SelectedItem = row.Cells["namecha"]?.Value?.ToString();  // ตั้งค่า ComboBox1 ด้วยข้อมูลในคอลัมน์ 'namecha' ของแถวที่คลิก
            checkedListBox1.ClearSelected();  // เคลียร์การเลือกทั้งหมดใน CheckedListBox1 ก่อน
            string selectedToppings = row.Cells["topping"]?.Value?.ToString()?.Trim() ?? "";  // ดึงข้อมูลจากคอลัมน์ 'topping' และตัดช่องว่างออก

            if (!string.IsNullOrEmpty(selectedToppings))  // ถ้ามีท็อปปิ้งที่เลือก
            {
                foreach (var topping in selectedToppings.Split(','))  // แยกรายการท็อปปิ้งออกเป็นแต่ละรายการ
                {
                    int index = checkedListBox1.Items.IndexOf(topping.Trim());  // ค้นหาดัชนีของท็อปปิ้งใน CheckedListBox1
                    if (index >= 0)
                        checkedListBox1.SetItemChecked(index, true);  // ถ้าพบให้เลือกรายการใน CheckedListBox1
                }
                comboBox3.SelectedItem = row.Cells["sweet"]?.Value?.ToString();  // ตั้งค่า ComboBox3 ด้วยข้อมูลในคอลัมน์ 'sweet'
                textBox1.Text = row.Cells["quantity"]?.Value?.ToString();  // ตั้งค่า TextBox1 ด้วยข้อมูลในคอลัมน์ 'quantity'

                // แสดงรูปภาพใน PictureBox (ถ้ามี)
                string columnName = "image"; // ชื่อคอลัมน์ที่เก็บรูปภาพ
                if (dataGridView1.Columns.Contains(columnName))  // ถ้า DataGridView มีคอลัมน์ 'image'
                {
                    if (row.Cells["image"].Value != DBNull.Value)
                    {
                        byte[] imageData = (byte[])row.Cells["image"].Value;
                        if (imageData.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                pictureBox1.Image = Image.FromStream(ms);
                            }
                        }
                    }
                    else
                    {
                        pictureBox1.Image = null;
                    }
                }
            }
        }
        //ชื่อเมนู
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ตรวจสอบว่าเลือกค่าใน comboBox1 แล้วหรือยัง
            if (comboBox1.SelectedItem != null)
            {
                string selectedName = comboBox1.SelectedItem.ToString();  // ดึงค่าที่เลือกจาก comboBox

                try
                {
                    // เชื่อมต่อกับฐานข้อมูล
                    using (MySqlConnection conn = databaseConnection())
                    {
                        conn.Open();  // เปิดการเชื่อมต่อกับฐานข้อมูล

                        // เพื่อดึงข้อมูลรูปภาพจากตาราง menucha โดยใช้ชื่อที่เลือก
                        string query = @"SELECT image FROM menucha WHERE name = @name";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            // เพิ่มพารามิเตอร์ @name เพื่อป้องกัน SQL Injection
                            cmd.Parameters.AddWithValue("@name", selectedName);

                            // ดึงค่าผลลัพธ์จากฐานข้อมูล (รูปภาพ) สเกลเลอร์
                            object result = cmd.ExecuteScalar();

                            // ตรวจสอบว่าผลลัพธ์มีข้อมูลรูปภาพหรือไม่
                            if (result != null && result is byte[] imageData)
                            {
                                // ถ้ามีรูปภาพ ให้แปลง byte[] เป็นรูปภาพและแสดงใน PictureBox
                                using (MemoryStream ms = new MemoryStream(imageData))
                                {
                                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;  // ปรับการแสดงผลรูปภาพให้พอดีกับ PictureBox
                                    pictureBox1.Image = Image.FromStream(ms);  // ตั้งค่ารูปภาพใน PictureBox
                                }
                            }
                            else
                            {
                                // ถ้าไม่มีรูปภาพให้แสดงข้อความเตือน
                                pictureBox1.Image = null;  // ลบรูปภาพใน PictureBox
                                MessageBox.Show("No image found for the selected item.");  // แสดงข้อความแจ้งเตือน
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // ถ้ามีข้อผิดพลาดในกระบวนการโหลดรูปภาพ จะมีการแสดงข้อความข้อผิดพลาด
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }
        //ปุ่มนำใส่ในตระกร้า
        private void button1_Click(object sender, EventArgs e)
        {
            // ดึงข้อมูลที่ผู้ใช้เลือกจาก comboBox1 (ชื่อสินค้า) และ comboBox3 (ความหวาน) 
            string selectedName = comboBox1.SelectedItem?.ToString()?.Trim(); //ตัวอักษร์ /ตัดแต่ง
            string sweet = comboBox3.SelectedItem?.ToString()?.Trim();
            int quantityToAdd;

            // เช็คว่าผู้ใช้กรอกข้อมูลครบหรือไม่
            if (string.IsNullOrEmpty(selectedName) || string.IsNullOrEmpty(sweet))
            {
                // ถ้าผู้ใช้ไม่ได้เลือกข้อมูลครบถ้วน จะหยุดการทำงาน
                MessageBox.Show("กรุณาเลือกข้อมูลให้ครบถ้วน", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            // ตรวจสอบว่า quantity ที่ผู้ใช้กรอกเป็นตัวเลขและมากกว่า 0 หรือที่ไม่ติดลบ
            if (!int.TryParse(textBox1.Text.Trim(), out quantityToAdd) || quantityToAdd <= 0)
            {
                // ถ้าจำนวนสินค้าไม่ถูกต้อง จะหยุดการทำงาน
                MessageBox.Show("กรุณากรอกจำนวนสินค้าเป็นตัวเลขที่มากกว่า 0", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;  
            }

            // เชื่อมต่อฐานข้อมูลเพื่อดึงข้อมูลราคาสินค้า
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();  // เปิดการเชื่อมต่อกับฐานข้อมูล

                // ตรวจสอบราคาสินค้า
                //เดซ-อิแม็ล ทศนิยม กำหนดค่าเริ่มต้นเป็น 0 ใช้สำหรับเก็บราคาสินค้า/เครื่องดื่มที่มีทศนิยม เช่น 25.50, 19.75
                decimal price = 0; 
                string priceQuery = "SELECT price FROM menucha WHERE name = @namecha";
                using (MySqlCommand priceCmd = new MySqlCommand(priceQuery, conn))
                {
                    priceCmd.Parameters.AddWithValue("@namecha", selectedName);
                           //รีเซาว
                    object result = priceCmd.ExecuteScalar();  // ดึงข้อมูลราคาจากตาราง namecha
                                                               // ใช้ Exeคิวทสเกเลอร์() เพื่อดึงค่าราคาออกมาเป็นผลลัพธ์จากคอลัมน์ price.
                    price = result != null ? Convert.ToDecimal(result) : 0;
                }

                // ตรวจสอบราคาท็อปปิ้งที่เลือก
                decimal toppingPrice = 0;
                decimal totalToppingPrice = 0;
                List<Image> toppingImages = new List<Image>();  // สร้างลิสต์เก็บรูปภาพท็อปปิ้งที่เลือก
                List<string> selectedToppings = new List<string>();  // สร้างลิสต์เก็บชื่อท็อปปิ้งที่เลือก

                // ทำการดึงข้อมูลท็อปปิ้งที่ผู้ใช้เลือกจาก CheckedListBox
                foreach (var item in checkedListBox1.CheckedItems) //จะเก็บรายการที่ผู้ใช้เลือกใน CheckedListBox.
                {
                    string topping = item.ToString().Trim(); //จะดึงชื่อท็อปปิ้งที่ผู้ใช้เลือกมาเก็บไว้ในตัวแปร topping โดยใช้ Trim(ทริม)
                    selectedToppings.Add(topping);  // เพิ่มท็อปปิ้งที่เลือกลงในลิสต์ selectedToppings
                    //ดึงข้อมูลจากฐานข้อมูล topping
                    string toppingPriceQuery = "SELECT price, image FROM topping WHERE name = @topping";
                    using (MySqlCommand toppingCmd = new MySqlCommand(toppingPriceQuery, conn))
                    {
                        toppingCmd.Parameters.AddWithValue("@topping", topping); //การส่งค่าชื่อท็อปปิ้ง(topping) ไปเป็นพารามิเตอร์
                        using (MySqlDataReader reader = toppingCmd.ExecuteReader()) //ใช้ MySqlDataReader เพื่ออ่านผลลัพธ์จากฐานข้อมูล.
                        {
                            if (reader.Read())
                            {
                                toppingPrice = reader.GetDecimal("price"); //ถ้ามีข้อมูล จะดึงค่าราคาท็อปปิ้งจากฐานข้อมูล ซึ่งจะเก็บในตัวแปร toppingPrice
                                totalToppingPrice += toppingPrice;  //คำนวณราคาท็อปปิ้งรวม โดยการเพิ่มราคาของท็อปปิ้งที่เลือกในแต่ละรอบเข้าไปใน totalToppingPrice.

                                // ดึงรูปภาพท็อปปิ้งจากฐานข้อมูลและแปลงเป็น Image
                                byte[] imgBytes = reader["image"] as byte[];
                                if (imgBytes != null) //ถ้ามีรูปภาพจะทำการสร้าง MemoryStream จาก imgBytes.
                                {
                                    using (MemoryStream ms = new MemoryStream(imgBytes))
                                    {
                                        Image toppingImage = Image.FromStream(ms);
                                        toppingImages.Add(toppingImage);  // เก็บรูปภาพท็อปปิ้งลงในลิสต์
                                    }
                                }
                            }
                        }
                    }
                }

                // แสดงรูปภาพของท็อปปิ้งที่เลือกใน PictureBox
                if (toppingImages.Count > 0) //ตรวจสอบว่าในลิสต์ toppingImages มีรูปภาพท็อปปิ้งที่ถูกเลือกหรือไม่ (เช็คจากจำนวนของลิสต์).
                {
                    pictureBox1.Image = toppingImages[0];  // แสดงรูปภาพท็อปปิ้งแรกที่เลือก
                }

                // ตรวจสอบจำนวนสินค้าในสต็อก
                string stockQuery = "SELECT quantity FROM menucha WHERE name = @namecha";
                int availableStock = 0;

                using (MySqlCommand stockCmd = new MySqlCommand(stockQuery, conn))
                {
                    stockCmd.Parameters.AddWithValue("@namecha", selectedName);
                    object result = stockCmd.ExecuteScalar();  // ดึงข้อมูลจำนวนสินค้าจากฐานข้อมูล
                    availableStock = result != null ? Convert.ToInt32(result) : 0;
                }

                // เช็คว่าสินค้าในสต็อกเพียงพอหรือไม่
                if (availableStock <= 0) //หากจำนวนสินค้าในสต็อก(อะแวเลโบ) น้อยกว่าหรือเท่ากับ 0
                {
                    MessageBox.Show("ขออภัยสินค้าหมดในสต็อกค่ะ", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;  // ถ้าไม่มีสินค้าในสต็อก จะหยุดการทำงาน
                }
                //ถ้าจำนวนสินค้าในสต็อกมีน้อยกว่าจำนวนที่ผู้ใช้ต้องการซื้อ(quantityToAdd)
                if (availableStock < quantityToAdd)
                {   
                    // ถ้าจำนวนในสต็อกไม่พอ จะมีMessageBox 
                    MessageBox.Show($"จำนวนสินค้าไม่พอ มีสินค้าเพียง {availableStock} แก้วค่ะ", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;  // และจะหยุดการทำงาน
                }

                // คำนวณราคาทั้งหมด (รวมราคาของท็อปปิ้งและสินค้า)
                string toppingList = string.Join(", ", selectedToppings);  // รวมชื่อท็อปปิ้งที่เลือกเป็นข้อความ โดยคั่นด้วย ,
                decimal newPrice = (price + totalToppingPrice) * quantityToAdd;  // คำนวณราคาทั้งหมดของสินค้าที่เลือก และคูณราคาของสินค้าและท็อปปิ้งกับจำนวนที่ลูกค้าต้องการ

                // เช็คว่ามีคำสั่งซื้อสินค้านี้ซ้ำไหม 
                string orderQuery = "SELECT id, quantity FROM `order` WHERE namecha = @namecha AND sweet = @sweet AND topping = @topping";
                int existingOrderId = 0;
                int existingQuantity = 0;

                using (MySqlCommand orderCmd = new MySqlCommand(orderQuery, conn))
                {
                    orderCmd.Parameters.AddWithValue("@namecha", selectedName); //โดยเทียบจาก (namecha)
                    orderCmd.Parameters.AddWithValue("@sweet", sweet);
                    orderCmd.Parameters.AddWithValue("@topping", toppingList);

                    using (MySqlDataReader reader = orderCmd.ExecuteReader())
                    {
                        //ถ้ามีคำสั่งซื้อเดิม จะดึงข้อมูล id และ quantity ของคำสั่งซื้อเดิมมาใช้.
                        if (reader.Read())
                        {
                            existingOrderId = reader.GetInt32("id");
                            existingQuantity = reader.GetInt32("quantity");
                        }
                    }
                }

                // ถ้ามีคำสั่งซื้อเดิม อัปเดตข้อมูลที่มีอยู่
                if (existingOrderId > 0)
                {
                    //โดยเพิ่มจำนวนสินค้า(quantity) และราคา(allprice).
                    string updateorderQuery = "UPDATE `order` SET quantity = quantity + @quantityToAdd, allprice = allprice + @allprice WHERE id = @id";
                    using (MySqlCommand updateorderCmd = new MySqlCommand(updateorderQuery, conn))
                    {
                        updateorderCmd.Parameters.AddWithValue("@quantityToAdd", quantityToAdd);
                        updateorderCmd.Parameters.AddWithValue("@allprice", newPrice);
                        updateorderCmd.Parameters.AddWithValue("@id", existingOrderId);
                        updateorderCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // ถ้าไม่เคยมีการสั่งซื้อนี้มาก่อน ให้เพิ่มคำสั่งซื้อใหม่
                    string insertorderQuery = @"
                        INSERT INTO `order` (namecha, sweet, quantity, price, allprice, topping)
                        VALUES (@namecha, @sweet, @quantity, @price, @allprice, @topping)";
                    using (MySqlCommand insertorderCmd = new MySqlCommand(insertorderQuery, conn))
                    {
                        insertorderCmd.Parameters.AddWithValue("@namecha", selectedName);
                        insertorderCmd.Parameters.AddWithValue("@sweet", sweet);
                        insertorderCmd.Parameters.AddWithValue("@quantity", quantityToAdd);
                        insertorderCmd.Parameters.AddWithValue("@price", price + totalToppingPrice);
                        insertorderCmd.Parameters.AddWithValue("@allprice", newPrice);
                        insertorderCmd.Parameters.AddWithValue("@topping", toppingList);
                        insertorderCmd.ExecuteNonQuery();
                    }
                }

                // ลดสต็อกสินค้าในฐานข้อมูล
                string updateStockQueryMenucha = "UPDATE menucha SET quantity = quantity - @quantityToAdd WHERE name = @namecha;";
                string updateStockQueryTopping = "UPDATE topping SET quantity = quantity - @quantityToAdd WHERE name = @topping;";

                // ลดสต็อกของสินค้าหลัก
                using (MySqlCommand updateStockCmd = new MySqlCommand(updateStockQueryMenucha, conn))
                {
                    updateStockCmd.Parameters.AddWithValue("@quantityToAdd", quantityToAdd);
                    updateStockCmd.Parameters.AddWithValue("@namecha", selectedName);
                    updateStockCmd.ExecuteNonQuery();
                }

                // ลดสต็อกท็อปปิ้ง
                foreach (var item in checkedListBox1.CheckedItems)
                {
                    string topping = item.ToString().Trim();
                    using (MySqlCommand updateToppingCmd = new MySqlCommand(updateStockQueryTopping, conn))
                    {
                        updateToppingCmd.Parameters.AddWithValue("@quantityToAdd", quantityToAdd);
                        updateToppingCmd.Parameters.AddWithValue("@topping", topping);
                        updateToppingCmd.ExecuteNonQuery();
                    }
                }

                // แสดงข้อความสำเร็จ
                MessageBox.Show("เพิ่มข้อมูลสำเร็จ!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // โหลดข้อมูลใหม่ใน DataGridView
                LoadOrderData();

                // รีเซ็ตฟอร์มให้พร้อมใช้งานใหม่
                comboBox1.SelectedIndex = -1;
                comboBox3.SelectedIndex = -1;
                textBox1.Clear();
                lastClickedButton = "button1"; //เมื่อเพิ่มสินค้าลงในตระกร้า
                checkedListBox1_SelectedIndexChanged_1(null, null); //เรียกฟังก์ชันเพื่อจะล้างcheckedListBox1ตามการทำงานนี้ 
            }
        }
        //ปุ่มออกจากตระกร้า
        private void button2_Click(object sender, EventArgs e)
        {
            string selectedName = comboBox1.SelectedItem?.ToString()?.Trim();  // ดึงชื่อเมนูสินค้าที่เลือกจาก ComboBox1 และตัดช่องว่าง
            string sweet = comboBox3.SelectedItem?.ToString()?.Trim();  // ดึงระดับความหวานจาก ComboBox3 และตัดช่องว่าง
            string selectedQuantityStr = textBox1.Text.Trim();  // ดึงค่าจำนวนที่ใส่ใน TextBox1 และตัดช่องว่าง

            // ตรวจสอบว่าผู้ใช้เลือกชื่อสินค้าใน ComboBox1 หรือไม่
            if (string.IsNullOrEmpty(selectedName))
            {
                MessageBox.Show("กรุณาเลือกเมนูสินค้าค่ะ ");  // แจ้งเตือนหากไม่ได้เลือกสินค้า
                return;  // หยุดการทำงานของฟังก์ชัน
            }

            // ตรวจสอบว่าจำนวนที่ใส่ใน TextBox1 เป็นตัวเลขที่มากกว่า 0 หรือไม่
            if (!int.TryParse(selectedQuantityStr, out int selectedQuantity) || selectedQuantity <= 0)
            {
                MessageBox.Show("กรุณาใส่จำนวนแก้วเป็นตัวเลขที่มากกว่า 0 ค่ะ ");  // แจ้งเตือนหากจำนวนไม่ถูกต้อง
                return;  // หยุดการทำงานของฟังก์ชัน
            }

            // ตรวจสอบรายการท็อปปิ้งที่เลือกจาก CheckedListBox1
            List<string> selectedToppings = new List<string>();  // สร้าง List สำหรับเก็บชื่อท็อปปิ้งที่เลือก
            foreach (var item in checkedListBox1.CheckedItems)  // วนลูปในรายการที่ถูกเลือกใน CheckedListBox1
            {
                selectedToppings.Add(item.ToString().Trim());  // เพิ่มท็อปปิ้งที่เลือกลงใน List
            }

            // ตรวจสอบว่าผู้ใช้เลือกท็อปปิ้งหรือไม่
            if (selectedToppings.Count == 0)
            {
                MessageBox.Show("กรุณาเลือกท็อปปิ้งค่ะ");  // แจ้งเตือนหากไม่เลือกท็อปปิ้ง
                return;  // หยุดการทำงานของฟังก์ชัน
            }

            using (MySqlConnection conn = databaseConnection())  // เปิดการเชื่อมต่อกับฐานข้อมูล
            {
                conn.Open();  // เปิดการเชื่อมต่อ

                // ตรวจสอบข้อมูลในตาราง `order` เกี่ยวกับสินค้าที่เลือก
                string orderQuery = "SELECT id, quantity FROM `order` WHERE namecha = @namecha AND sweet = @sweet";
                int orderQuantity = 0;  // ตัวแปรเก็บจำนวนสินค้าที่อยู่ในตะกร้า
                int orderId = 0;  // ตัวแปรเก็บ ID ของแถวในตาราง `order`

                using (MySqlCommand checkCmd = new MySqlCommand(orderQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@namecha", selectedName);  // กำหนดชื่อสินค้าที่เลือก
                    checkCmd.Parameters.AddWithValue("@sweet", sweet);  // กำหนดระดับความหวานที่เลือก
                    using (MySqlDataReader reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())  // ถ้ามีข้อมูลในฐานข้อมูล
                        {
                            orderQuantity = reader.GetInt32("quantity");  // ดึงจำนวนสินค้าที่มีในตะกร้า
                            orderId = reader.GetInt32("id");  // ดึง ID ของแถวที่ตรงกับเงื่อนไข
                        }
                    }
                }

                // ตรวจสอบหากจำนวนสินค้าที่ในตะกร้าน้อยกว่าหรือเท่ากับ 0
                if (orderQuantity <= 0)
                {
                    MessageBox.Show("สินค้าไม่มีอยู่ในตะกร้า");  // แจ้งเตือนหากสินค้าไม่มีในตะกร้า
                    return;  // หยุดการทำงานของฟังก์ชัน
                }

                // ถ้าจำนวนที่เลือกมากกว่าหรือเท่ากับจำนวนในตะกร้า
                if (selectedQuantity >= orderQuantity)
                {
                    // ลบสินค้าที่มีในตะกร้า
                    string deleteOrderQuery = "DELETE FROM `order` WHERE id = @id";
                    using (MySqlCommand deleteOrderCmd = new MySqlCommand(deleteOrderQuery, conn))
                    {
                        deleteOrderCmd.Parameters.AddWithValue("@id", orderId);  // กำหนด ID ของแถวที่ต้องการลบ
                        deleteOrderCmd.ExecuteNonQuery();  // ลบแถวจากตาราง `order`
                    }

                    // คืนสินค้าเข้าสู่สต็อก
                    RemoveOrderAndRestoreStock(conn, selectedName, orderQuantity);  // เรียกฟังก์ชันเพื่อคืนสินค้าเข้าสู่สต็อก
                }
                else
                {
                    // อัพเดตข้อมูลในตะกร้าและสต็อก
                    UpdateOrderAndStock(conn, selectedName, selectedQuantity, orderQuantity, selectedToppings);  // เรียกฟังก์ชันเพื่ออัพเดตข้อมูล
                }

                // โหลดข้อมูลใหม่ลงใน DataGridView
                LoadOrderData();  // โหลดข้อมูลที่อัพเดตลงใน DataGridView

                // รีเซ็ตฟอร์ม
                comboBox1.SelectedIndex = -1;  // เคลียร์การเลือกใน ComboBox1
                comboBox3.SelectedIndex = -1;  // เคลียร์การเลือกใน ComboBox3
                textBox1.Clear();  // เคลียร์ TextBox1
                lastClickedButton = "button2";  // เก็บข้อมูลปุ่มที่ถูกคลิก
                checkedListBox1_SelectedIndexChanged_1(null, null);  //เรียกฟังก์ชันเพื่อจะล้างcheckedListBox1ตามการทำงานนี้ (ท้อปปิ้ง)
            }
        }
        //ตรวจสอบการเลือกท้อปปิ้ง
        private string lastClickedButton = ""; // ตัวแปรเก็บชื่อของปุ่มล่าสุด
        //ตรวจสอบListBox1
        private void checkedListBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            try
            {
                // ล้างภาพใน PictureBox
                pictureBox1.Image = null;  // เริ่มต้นโดยการล้างภาพใน PictureBox เพื่อเตรียมที่จะโหลดภาพใหม่

                // ตรวจสอบว่ามีไอเท็มถูกเลือกหรือไม่
                if (checkedListBox1.CheckedItems.Count == 0)
                {
                    MessageBox.Show("กรุณาเลือกท็อปปิ้ง.");  // หากไม่มีท็อปปิ้งถูกเลือก จะมีการแสดงข้อความแจ้งเตือน
                    return;  // หากไม่มีการเลือก ท็อปปิ้งจะไม่ทำงานต่อ
                }

                // ตรวจสอบว่ามีการกดปุ่ม BT1, BT2, BT3, หรือ BT6 หรือไม่
                if (lastClickedButton == "button1" || lastClickedButton == "button2" ||
                    lastClickedButton == "button3" || lastClickedButton == "button6")
                {
                    // วนลูปเพื่อเข้าถึงรายการทั้งหมดใน checkedListBox1 ทีละรายการ
                    // โดย i คือดัชนีของแต่ละรายการ เริ่มตั้งแต่ 0 จนถึงจำนวนรายการทั้งหมด - 1 (i++คือi = i + 1;(ใช้ก่อนค่อยเพิ่ม)เพิ่มเรื่อยๆ)
                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        checkedListBox1.SetItemChecked(i, false);  // ล้างการเลือกใน checkedListBox1 ทั้งหมด
                    }

                    return;  // จบการทำงานตรงนี้และไม่ให้โหลดภาพ
                }

                // วนลูปโหลดภาพจากฐานข้อมูล
                foreach (var item in checkedListBox1.CheckedItems)
                {
                    string selectedName = item.ToString();  // ดึงชื่อของท็อปปิ้งที่ถูกเลือก

                    using (MySqlConnection conn = databaseConnection())  // เปิดการเชื่อมต่อฐานข้อมูล
                    {
                        conn.Open();
                        string query = @"SELECT image FROM topping WHERE name = @name";  // คำสั่ง SQL เพื่อดึงข้อมูลภาพจากฐานข้อมูล

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", selectedName);  // ส่งชื่อท็อปปิ้งที่เลือกไปในพารามิเตอร์
                            object result = cmd.ExecuteScalar();  // ใช้ ExecuteScalar เพื่อดึงผลลัพธ์กลับมาจากฐานข้อมูล

                            // ตรวจสอบผลลัพธ์ และแสดงภาพ
                            if (result != null && result is byte[] imageData)
                            {
                                // หากมีข้อมูลภาพ (ประเภท byte[]), แสดงภาพใน PictureBox
                                using (MemoryStream ms = new MemoryStream(imageData))
                                {
                                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;  // ตั้งโหมดการแสดงภาพเป็น Zoom เพื่อให้ภาพพอดีกับ PictureBox
                                    pictureBox1.Image = Image.FromStream(ms);  // แสดงภาพจาก MemoryStream
                                }
                            }
                            else
                            {
                                // หากไม่พบภาพในฐานข้อมูล จะแสดงข้อความแจ้งเตือน
                                MessageBox.Show($"No image found for the selected item: {selectedName}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // หากเกิดข้อผิดพลาดในการโหลดภาพ จะแสดงข้อความแสดงข้อผิดพลาด
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }
        // ฟังก์ชันคืนสินค้าในสต๊อก
        private void RemoveOrderAndRestoreStock(MySqlConnection conn, string name, int orderQuantity)
        {
            try
            {
                // Restore stock (คืนสต๊อก)
                string restoreStockQuery = @"
                    UPDATE menucha SET quantity = quantity + @orderQuantity WHERE name = @name;
                    UPDATE topping SET quantity = quantity + @orderQuantity WHERE name = @name;";
                using (MySqlCommand restoreStockCmd = new MySqlCommand(restoreStockQuery, conn))
                {
                    restoreStockCmd.Parameters.AddWithValue("@orderQuantity", orderQuantity);  // จำนวนที่ต้องการคืนสต๊อก
                    restoreStockCmd.Parameters.AddWithValue("@name", name);  // ชื่อสินค้า
                    restoreStockCmd.ExecuteNonQuery();  // รันคำสั่ง SQL เพื่อคืนสต๊อก
                }

                // ตรวจสอบสต๊อกสินค้า (เช็คว่าสินค้ายังมีอยู่ในสต๊อกหรือไม่)
                string checkStockQuery = @"
                    SELECT COALESCE(SUM(quantity), 0) 
                    FROM menucha WHERE name = @name 
                    UNION ALL
                    SELECT COALESCE(SUM(quantity), 0)
                    FROM topping WHERE name = @name"; //จะใช้ โคเลส เพื่อเปลี่ยน NULL ให้กลายเป็น 0 แทน เพื่อป้องกันค่าว่าง

                int totalStock = 0;
                using (MySqlCommand checkStockCmd = new MySqlCommand(checkStockQuery, conn))
                {
                    checkStockCmd.Parameters.AddWithValue("@name", name);  // ชื่อสินค้า
                    totalStock = Convert.ToInt32(checkStockCmd.ExecuteScalar());  // ตรวจสอบค่าสต๊อกทั้งหมด
                }

                // ถ้าสต๊อกสินค้าหมดหรือเป็นลบ (totalStock <= 0)
                if (totalStock <= 0)
                {
                    // ลบคำสั่งซื้อหากสินค้าหมด
                    string deleteOrderQuery = "DELETE FROM `order` WHERE namecha = @name";
                    using (MySqlCommand deleteOrderCmd = new MySqlCommand(deleteOrderQuery, conn))
                    {
                        deleteOrderCmd.Parameters.AddWithValue("@name", name);  // ใช้ชื่อสินค้าที่เลือก
                        deleteOrderCmd.ExecuteNonQuery();  // ลบคำสั่งซื้อ
                    }
                }

                // ตรวจสอบและอัปเดตราคาของคำสั่งซื้อ
                string priceQuery = "SELECT price FROM `order` WHERE namecha = @name";
                decimal price = 0;
                using (MySqlCommand priceCmd = new MySqlCommand(priceQuery, conn))
                {
                    priceCmd.Parameters.AddWithValue("@name", name);  // ชื่อสินค้า
                    object priceResult = priceCmd.ExecuteScalar();  // ดึงราคาของคำสั่งซื้อจากฐานข้อมูล
                    if (priceResult != null)
                    {
                        price = Convert.ToDecimal(priceResult);  // แปลงข้อมูลราคา
                    }
                }

                decimal updatedPrice = price * orderQuantity;  // คำนวณราคาที่อัปเดตจากจำนวนสินค้า
                if (updatedPrice <= 0)
                {
                    MessageBox.Show("ราคาจะไม่สามารถติดลบได้ กรุณาตรวจสอบจำนวนสินค้า", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;  // ถ้าราคาน้อยกว่าหรือเท่ากับ 0 แสดงข้อความเตือนและไม่ทำการอัปเดต
                }

                // อัปเดตราคาของคำสั่งซื้อถ้าราคาใหม่ถูกต้อง
                string updateOrderQuery = @"
                    UPDATE `order` SET price = @updatedPrice WHERE namecha = @name";
                using (MySqlCommand updateOrderCmd = new MySqlCommand(updateOrderQuery, conn))
                {
                    updateOrderCmd.Parameters.AddWithValue("@updatedPrice", updatedPrice);  // ราคาใหม่ที่อัปเดต
                    updateOrderCmd.Parameters.AddWithValue("@name", name);  // ชื่อสินค้า
                    updateOrderCmd.ExecuteNonQuery();  // รันคำสั่ง SQL เพื่ออัปเดตราคา
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);  // หากเกิดข้อผิดพลาดจากฐานข้อมูล
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);  // หากเกิดข้อผิดพลาดที่ไม่คาดคิด
            }
        }
        //ฟังก์ชันอัปเดตออเดอร์กับสต็อก
        private void UpdateOrderAndStock(MySqlConnection conn, string namecha, int selectedQuantity, int orderQuantity, List<string> selectedToppings)
        {
            // คำนวณการเปลี่ยนแปลงจำนวนสินค้าในตาราง order
                       //เซง
            int quantityChange = orderQuantity - selectedQuantity;  // คำนวณการเปลี่ยนแปลงจำนวนสินค้าในตะกร้า

            // คำนวณการเปลี่ยนแปลงราคา
            //แดสแม็ล                                                   
            decimal priceChange = (orderQuantity - selectedQuantity) * GetPriceFromDatabase(conn, namecha);  

            // แปลง List<string> เป็น string สำหรับท็อปปิ้งที่เลือก
            string toppings = string.Join(", ", selectedToppings);  // รวมชื่อท็อปปิ้งที่เลือกเป็นสตริง เช่น 'ท็อปปิ้ง1, ท็อปปิ้ง2'

            // อัปเดตข้อมูลในตาราง order
            string updateOrderQuery = @"
                UPDATE `order` 
                   SET quantity = @quantityChange, 
                   allprice = @priceChange,
                   topping = @toppings
                   WHERE namecha = @namecha";  // คำสั่ง SQL สำหรับอัปเดตจำนวนสินค้า, ราคา, และท็อปปิ้งในตาราง `order`

            using (MySqlCommand updateOrderCmd = new MySqlCommand(updateOrderQuery, conn))  // ใช้ MySqlCommand เพื่ออัปเดตข้อมูล
            {
                updateOrderCmd.Parameters.AddWithValue("@quantityChange", quantityChange);  // เพิ่มพารามิเตอร์จำนวนสินค้า
                updateOrderCmd.Parameters.AddWithValue("@priceChange", priceChange);  // เพิ่มพารามิเตอร์ราคา
                updateOrderCmd.Parameters.AddWithValue("@toppings", toppings);  // เพิ่มพารามิเตอร์ท็อปปิ้งที่เลือก
                updateOrderCmd.Parameters.AddWithValue("@namecha", namecha);  // เพิ่มพารามิเตอร์ชื่อสินค้า
                updateOrderCmd.ExecuteNonQuery();  // ดำเนินการคำสั่ง SQL เพื่ออัปเดตข้อมูล
            }

            // คืนสินค้าเข้าสู่สต็อก
            string updateStockQuery = @"
                UPDATE `menucha` SET quantity = quantity + @selectedQuantity WHERE name = @name;
                UPDATE `topping` SET quantity = quantity + @selectedQuantity WHERE name = @name;";  // คำสั่ง SQL สำหรับอัปเดตสต็อกสินค้าและท็อปปิ้ง

            using (MySqlCommand updateStockCmd = new MySqlCommand(updateStockQuery, conn))  // ใช้ MySqlCommand เพื่ออัปเดตข้อมูลสต็อก
            {
                updateStockCmd.Parameters.AddWithValue("@selectedQuantity", selectedQuantity);  // เพิ่มพารามิเตอร์จำนวนสินค้าที่คืน
                updateStockCmd.Parameters.AddWithValue("@name", namecha);  // เพิ่มพารามิเตอร์ชื่อสินค้า
                updateStockCmd.ExecuteNonQuery();  // ดำเนินการคำสั่ง SQL เพื่ออัปเดตสต็อก
            }
        }
        // ฟังก์ชันสำหรับดึงราคาจากฐานข้อมูล
        private decimal GetPriceFromDatabase(MySqlConnection conn, string namecha)
        {
            decimal price = 0.0m;  // กำหนดตัวแปรราคาเริ่มต้นเป็น 0.0m (ประเภทข้อมูล ทศนิยม)

            // สร้างคำสั่ง SQL เพื่อดึงข้อมูลราคาจากตาราง 'menucha' ตามชื่อของสินค้า
            string query = "SELECT price FROM `menucha` WHERE name = @namecha";

            // สร้าง MySqlCommand เพื่อดำเนินการคำสั่ง SQL
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                // เพิ่มพารามิเตอร์ @namecha เพื่อป้องกัน SQL injection (คนจะมาทำร้ายระบบ) และใส่ค่าชื่อสินค้าลงในพารามิเตอร์
                cmd.Parameters.AddWithValue("@namecha", namecha);

                // ใช้ MySqlDataReader เพื่ออ่านผลลัพธ์จากการค้นหาข้อมูลในฐานข้อมูล
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    // ถ้ามีข้อมูล (อ่านได้อย่างน้อย 1 แถว)
                    if (reader.Read())
                    {
                        // ดึงค่าราคาจากคอลัมน์ "price" และแปลงเป็น decimal
                        price = reader.GetDecimal("price");
                    }
                }
            }

            // คืนค่าราคาที่ได้จากฐานข้อมูล
            return price;
        }
        // ปุ่มคลิกเพื่ออัปเดตข้อมูลที่แก้ไข
        private void button6_Click(object sender, EventArgs e)
        {
            string selectedName = comboBox1.SelectedItem?.ToString()?.Trim();  // ดึงชื่อสินค้าที่เลือกจาก ComboBox1 และลบช่องว่าง
            string sweet = comboBox3.SelectedItem?.ToString()?.Trim();  // ดึงระดับความหวานจาก ComboBox3 และลบช่องว่าง
            int quantityToUpdate;  // กำหนดตัวแปรสำหรับเก็บจำนวนสินค้าที่ต้องการอัปเดต

            // ตรวจสอบว่าเลือกข้อมูลใน ComboBox เมนู,ระดับความหวาน ครบหรือไม่
            if (string.IsNullOrEmpty(selectedName) || string.IsNullOrEmpty(sweet))
            {
                MessageBox.Show("กรุณาเลือกข้อมูลให้ครบถ้วน", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ตรวจสอบจำนวนสินค้าที่ต้องการอัปเดตให้เป็นตัวเลขที่มากกว่า 0
            if (!int.TryParse(textBox1.Text.Trim(), out quantityToUpdate) || quantityToUpdate <= 0)
            {
                MessageBox.Show("กรุณากรอกจำนวนสินค้าเป็นตัวเลขที่มากกว่า 0", "ข้อมูลไม่ถูกต้อง", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // เชื่อมต่อฐานข้อมูล
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();  // เปิดการเชื่อมต่อกับฐานข้อมูล

                // ดึงราคาสินค้าหลักจากฐานข้อมูล
                decimal price = 0;
                string priceQuery = @"
                    SELECT price FROM menucha WHERE name = @namecha";

                using (MySqlCommand priceCmd = new MySqlCommand(priceQuery, conn))
                {
                    priceCmd.Parameters.AddWithValue("@namecha", selectedName);  // ส่งชื่อสินค้าลงในพารามิเตอร์
                    object result = priceCmd.ExecuteScalar();  // ดึงค่าราคา (ใช้ Exeคิวสเกเลอร์ เพื่อดึงค่าหนึ่งค่า)
                    price = result != null ? Convert.ToDecimal(result) : 0; // ถ้ามีค่า (result != null) ให้แปลง result เป็นชนิด decimal และเก็บไว้ในตัวแปร price
                                                                            // ถ้า result เป็น null ให้กำหนดค่าเริ่มต้นให้ price เท่ากับ 0 
                }

                // ดึงราคาท็อปปิ้งที่เลือก
                decimal toppingPrice = 0;
                decimal totalToppingPrice = 0;
                List<string> selectedToppings = new List<string>();

                // วนลูปเพื่อดึงราคาของท็อปปิ้งที่เลือกจาก CheckedListBox
                foreach (var item in checkedListBox1.CheckedItems)
                {
                    string topping = item.ToString().Trim();  // ดึงชื่อท็อปปิ้ง
                    selectedToppings.Add(topping);  // เพิ่มชื่อท็อปปิ้งที่เลือกลงใน List

                    string toppingPriceQuery = "SELECT price FROM topping WHERE name = @topping";
                    using (MySqlCommand toppingCmd = new MySqlCommand(toppingPriceQuery, conn))
                    {
                        toppingCmd.Parameters.AddWithValue("@topping", topping);  // ส่งชื่อท็อปปิ้งลงในพารามิเตอร์
                        using (MySqlDataReader reader = toppingCmd.ExecuteReader())  // อ่านข้อมูลราคาท็อปปิ้ง
                        {
                            if (reader.Read())
                            {
                                toppingPrice = reader.GetDecimal("price");  // ดึงราคาท็อปปิ้ง
                                totalToppingPrice += toppingPrice;  // คำนวณราคาท็อปปิ้งรวม คือ totalToppingPrice = totalToppingPrice + toppingPrice;
                            }
                        }
                    }
                }

                // คำนวณราคาสินค้ารวม (ราคาสินค้าหลัก + ราคาท็อปปิ้ง) คูณด้วยจำนวนสินค้าที่อัปเดต
                decimal newPrice = (price + totalToppingPrice) * quantityToUpdate;

                // สร้าง string ที่แสดงรายชื่อท็อปปิ้งทั้งหมดที่เลือก
                string toppingList = string.Join(", ", selectedToppings);

                // ดึงค่า namecha จาก DataGridView (ชื่อสินค้าที่อยู่ในแถวที่ถูกเลือก)
                string selectedNamecha = dataGridView1.CurrentRow.Cells["namecha"].Value.ToString();

                // อัปเดตข้อมูลในตาราง `order` ตามที่เลือก
                string updateOrderQuery = @"
                    UPDATE `order`
                    SET quantity = @quantity, sweet = @sweet, allprice = @newPrice, topping = @topping
                    WHERE namecha = @namecha";

                using (MySqlCommand updateOrderCmd = new MySqlCommand(updateOrderQuery, conn))
                {
                    // เพิ่มพารามิเตอร์ต่างๆ เพื่ออัปเดตข้อมูล
                    updateOrderCmd.Parameters.AddWithValue("@quantity", quantityToUpdate);
                    updateOrderCmd.Parameters.AddWithValue("@sweet", sweet);
                    updateOrderCmd.Parameters.AddWithValue("@newPrice", newPrice);
                    updateOrderCmd.Parameters.AddWithValue("@topping", toppingList);
                    updateOrderCmd.Parameters.AddWithValue("@namecha", selectedNamecha);
                    updateOrderCmd.ExecuteNonQuery();  // ดำเนินการอัปเดตข้อมูล
                }

                MessageBox.Show("ข้อมูลได้รับการอัปเดตแล้ว!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // รีเฟรชข้อมูลใน DataGridView
                LoadOrderData();

                // รีเซ็ตฟอร์ม (เคลียร์ค่าต่างๆ หลังการอัปเดต)
                comboBox1.SelectedIndex = -1;
                comboBox3.SelectedIndex = -1;
                textBox1.Clear();
                lastClickedButton = "button6";
                checkedListBox1_SelectedIndexChanged_1(null, null);  // เคลียร์การเลือกท็อปปิ้ง
            }
        }
        //ตรวจสอบว่าเซลล์ในแถวและคอลัมน์ที่เกิดข้อผิดพลาด
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // ตรวจสอบว่าเซลล์ในแถวและคอลัมน์ที่เกิดข้อผิดพลาดมีค่าเป็น DBNull หรือไม่
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == DBNull.Value)
            {
                e.Cancel = true;  // หากมีค่าเป็น DBNull จะยกเลิกข้อผิดพลาด
            }
        }
        //ปุ่มเสร็จสิ้นไปหน้าจ่าย
        private void button3_Click(object sender, EventArgs e)
        {
            //หน้าใบเสร็จ
            Form7 form7 = new Form7();
            form7.Show();
            this.Hide();

        }
        //ปุ่มย้อนกลับ
        private void button4_Click(object sender, EventArgs e)
        {
            //ย้อนกลับ
            Form3 form3 = new Form3();
            form3.Show();
            this.Hide();
        }
        // ปุ่มปิดโปรแกรม
        private void button5_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }

    }
}
