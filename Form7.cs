using MySql.Data.MySqlClient;
using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;

namespace WindowsFormsApp4
{
    public partial class Form7 : Form

    {
        private MySqlConnection databaseConnection()
        {
            // สร้างการเชื่อมต่อกับฐานข้อมูล
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=mmigichanew;charset=utf8;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        public Form7()
        {
            InitializeComponent(); //เรียกใช้เมธอด (อินิเฉอะลายคัมโพเนิน) เพื่อกำหนดค่าคอนโทรลต่าง ๆ ที่ถูกออกแบบไว้ใน Form Designer
        }
        private void Form7_Load(object sender, EventArgs e)
        {
            // ซ่อนปุ่ม ใบเสร็จpdf เมื่อโหลด Form
            button3.Visible = false;

            // ดึงข้อมูลจากตาราง order และแสดงใน dataGridView1
            LoadOrderData();

            // คำนวณราคารวมและแสดงใน textBox1
            CalculateTotalPrice();

            // ดึงข้อมูลล่าสุดจากตาราง orderlogin และแสดงใน TextBoxes
            LoadLatestOrderloginData();
        }
        //ฟังก์ชันที่ใช้ในการดึงข้อมูลจากฐานข้อมูลและแสดงผลใน DataGridView (หรือที่เรียกว่า ตารางข้อมูลใน Windows Forms)
        //โดยการดึงข้อมูลจากตาราง order ในฐานข้อมูล MySQL มาแสดงในโปรแกรม
        private void LoadOrderData()
        {
            // เชื่อมต่อกับฐานข้อมูล MySQL
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();  // เปิดการเชื่อมต่อกับฐานข้อมูล

                // ดึงข้อมูลจากตาราง order
                string query = "SELECT namecha, topping, sweet, quantity, price, allprice, image FROM `order`";

                // สร้าง MySqlCommand เพื่อดำเนินการกับคำสั่ง SQL
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // ใช้ MySqlDataReader เพื่ออ่านข้อมูลจากฐานข้อมูล
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // สร้าง DataTable ที่จะเก็บข้อมูลที่อ่านจากฐานข้อมูล
                        DataTable dt = new DataTable();

                        // โหลดข้อมูลที่อ่านได้จาก MySqlDataReader ลงใน DataTable
                        dt.Load(reader);

                        // กำหนดให้ DataGridView1 แสดงข้อมูลจาก DataTable
                        dataGridView1.DataSource = dt;
                        dataGridView1.DataError += dataGridView1_DataError;
                    }
                }
            }
        }
        //ฟังก์ชันแคลคูลเลส ใช้ในการคำนวณราคารวมของสินค้าทั้งหมดจากตาราง order โชว์ในTextBox
        private void CalculateTotalPrice()
        {
            // เชื่อมต่อกับฐานข้อมูล
            MySqlConnection conn = databaseConnection();
            conn.Open();

            // สร้างคำสั่ง SQL คำนวณผลรวมของ allprice 
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT SUM(allprice) AS TotalPrice FROM `order`";

            object result = cmd.ExecuteScalar(); // ดึงผลลัพธ์จากฐานข้อมูล
            conn.Close(); // ปิดการเชื่อมต่อฐานข้อมูล

            //ตรวจสอบค่าที่ดึงมาว่าไม่ใช่ NULL หรือ DBNull ก่อนแปลงเป็นตัวเลข
            double allprice = (result != DBNull.Value && result != null) ? Convert.ToDouble(result) : 0.00; // ถ้า รีเซาว์ เป็น NULL ให้ใช้ค่าเริ่มต้นเป็น 0.00
                                                                                                           // ใช้ ternary operator (?:) เพื่อตรวจสอบและกำหนดค่าอย่างรวดเร็ว

            // คำนวณ VAT 7%
            double vat = allprice * 0.07;
            double totalWithVAT = allprice + vat; // ราคาสุทธิรวม VAT

            // แสดงผลใน TextBox1
            textBox1.Text = totalWithVAT.ToString("N2");  // แสดงราคารวมทั้งหมด + VAT
        }
        //ฟังก์ชันนี้จะดึงข้อมูลล่าสุดจากตาราง login ในฐานข้อมูลและแสดงใน TextBox
                          //เล'ทิสทฺ
        private void LoadLatestOrderloginData()
        {
            // ดึงข้อมูลล่าสุดจากตาราง login และแสดงใน TextBoxes
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();

               //ดึงข้อมูลลูกค้าล่าสุดจากตาราง login 
                string query = "SELECT username, number, email, date FROM login ORDER BY id DESC LIMIT 1"; ////ORDER BY id DESC คือ เรียงลำดับข้อมูลตามคอลัมน์ id จากมากไปน้อย(ข้อมูลล่าสุดอยู่บนสุด) แค่ 1 แถว
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Execute คำสั่ง SQL เพื่อดึงข้อมูล
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // อ่านข้อมูลและใส่ลงใน TextBox
                            textBox2.Text = reader["username"].ToString();
                            textBox4.Text = reader["email"].ToString();
                            textBox5.Text = reader["number"].ToString();
                            // แปลงวันที่เป็นรูปแบบที่แสดงเฉพาะวัน
                            DateTime date = Convert.ToDateTime(reader["date"]);
                            textBox3.Text = date.ToShortDateString();
                        }
                    }
                }
            }
        }
        // ปุ่มจ่ายเงินเสร็จสิ้น
        private void button2_Click(object sender, EventArgs e)
        {
            // แสดงปุ่ม button3 เมื่อคลิกที่ปุ่ม button2
            button3.Visible = true;

            // กำหนดตัวแปรเพื่อเก็บข้อมูลจากฐานข้อมูล
            string username = "";
            string number = "";
            string namecha = "";
            string email = "";
            int quantity = 0;
            double sum = 0; // ใช้ double เพื่อรองรับการคำนวณ VAT

            // ดึงข้อมูลล่าสุดจากตาราง login (เช่น ชื่อ, เบอร์โทร, อีเมล)
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();        //เพื่อดึงจากแถวล่าสุดในตาราง login
                string loginQuery = "SELECT username, number, email FROM login ORDER BY id DESC LIMIT 1";
                using (MySqlCommand loginCmd = new MySqlCommand(loginQuery, conn))
                using (MySqlDataReader reader = loginCmd.ExecuteReader())
                {
                    if (reader.Read()) // ถ้ามีข้อมูล (อย่างน้อย 1 แถว)
                    {
                        // อ่านค่าจากคอลัมน์แล้วเก็บไว้ในตัวแปร
                        username = reader.GetString("username"); 
                        number = reader.GetString("number");
                        email = reader.GetString("email");
                    }
                }
            }

            // ดึงข้อมูลจากตาราง order (เช่น ชื่อชา, จำนวน, ราคารวม)
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open(); // เลือก namecha(ชื่อชา), quantity(จำนวน), allprice(ราคารวม) จากตาราง order
                string orderQuery = "SELECT namecha, quantity, allprice FROM `order`";
                using (MySqlCommand orderCmd = new MySqlCommand(orderQuery, conn))
                using (MySqlDataReader reader = orderCmd.ExecuteReader())
                {
                    while (reader.Read()) // วนลูปอ่านแต่ละแถวที่ได้จากผลลัพธ์
                    {
                        namecha += reader.GetString("namecha") + ","; // รวมชื่อชาโดยคั่นด้วย ,
                        quantity += reader.GetInt32("quantity"); // รวมจำนวน
                        sum += reader.GetDouble("allprice"); // รวมราคาทั้งหมดก่อน VAT
                        //+= คือการเอาค่าปัจจุบันของตัวแปร มาบวกกับค่าที่อ่านได้ แล้วเก็บไว้ในตัวแปรเดิม
                    }
                }
            }

            // ลบเครื่องหมายคอมมา (,) ที่ท้ายสตริง namecha
            namecha = namecha.TrimEnd(',');

            // คำนวณ VAT 7%
            double vat = sum * 0.07; // 7% ของราคาสินค้า
            sum += vat; // รวม VAT เข้าไปในราคาสุทธิ

            // กำหนดที่อยู่ไฟล์ PDF สำหรับบันทึกใบเสร็จ
            string pdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ใบเสร็จ.pdf");

            // บันทึกข้อมูลลงตาราง history
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();
                string insertQuery = "INSERT INTO history (username, number, namecha, email, quantity, sum, date, pdf) " +
                                     "VALUES (@username, @number, @namecha, @email, @quantity, @sum, @date, @pdf)";
                using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                {
                    // ผูกค่าพารามิเตอร์ก่อนบันทึกลงฐานข้อมูล
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@number", number);
                    cmd.Parameters.AddWithValue("@namecha", namecha);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@sum", sum); 
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@pdf", pdfPath);
                    cmd.ExecuteNonQuery(); // บันทึกลงฐานข้อมูล
                }
            }
        }
        //ปุ่มบิลpdf
        private void button3_Click(object sender, EventArgs e)
        {
            // สร้าง PDF
            CreatePDF();
            // เชื่อมต่อฐานข้อมูล MySQL
            using (MySqlConnection conn = databaseConnection()) 
            {
                conn.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล

                // ลบข้อมูลในตาราง 'order'
                string deleteQuery = "DELETE FROM `order`"; // ทำการลบข้อมูลทั้งหมดในตาราง 'order'

                using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn)) //ใช้การเชื่อมต่อที่เปิดแล้ว
                {
                    deleteCmd.ExecuteNonQuery(); // เรียกใช้งานคำสั่ง SQL และทำการลบข้อมูลทั้งหมดในตาราง 'order'
                }

            }
        }
        //สร้างpdf
        private void CreatePDF()
        {
            try
            {
                using (MySqlConnection conn = databaseConnection())  // เชื่อมต่อฐานข้อมูล MySQL โดยใช้การเชื่อมต่อที่ได้จากฟังก์ชัน 'databaseConnection'
                {
                    conn.Open();  // เปิดการเชื่อมต่อกับฐานข้อมูล

                    // ดึงข้อมูลล่าสุดจากตาราง history
                    string selectCustomerQuery = "SELECT username, number, email FROM login ORDER BY id DESC LIMIT 1"; //ORDER BY id DESC คือ เรียงลำดับข้อมูลตามคอลัมน์ id จากมากไปน้อย(ข้อมูลล่าสุดอยู่บนสุด) แค่ 1 แถว
                    // คำสั่ง SQL เพื่อดึงข้อมูลลูกค้า
                    MySqlCommand selectCustomerCmd = new MySqlCommand(selectCustomerQuery, conn);  
                    // สร้างคำสั่ง SQL
                    MySqlDataReader customerReader = selectCustomerCmd.ExecuteReader();  
                    // รันคำสั่ง SQL และดึงข้อมูลจากฐานข้อมูล

                    if (customerReader.Read())  // ถ้ามีข้อมูลลูกค้า
                    {
                        // อ่านข้อมูลลูกค้าจากฐานข้อมูล
                        string customerName = customerReader.GetString("username");
                        string customerPhone = customerReader.GetString("number");
                        string customerEmail = customerReader.GetString("email");

                        customerReader.Close();  // ปิดการอ่านข้อมูลจากฐานข้อมูล

                        // ตรวจสอบและสร้างโฟลเดอร์ "migicha" บนเดสก์ท็อป
                        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "migicha");
                        if (!Directory.Exists(folderPath))  // หากโฟลเดอร์ไม่อยู่
                        {
                            Directory.CreateDirectory(folderPath);  // สร้างโฟลเดอร์ใหม่
                        }

                        // สร้างชื่อไฟล์ PDF ที่ไม่ซ้ำกัน
                        string fileName = $"ใบเสร็จ_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                        string pdfPath = Path.Combine(folderPath, fileName);  // รวมเส้นทางโฟลเดอร์และชื่อไฟล์

                        // สร้างไฟล์ PDF
                        using (FileStream fs = new FileStream(pdfPath, FileMode.Create))  // เปิดไฟล์ PDF ในโหมดสร้าง
                        using (Document doc = new Document(PageSize.A4))  // สร้างเอกสาร PDF ขนาด A4
                        {
                            PdfWriter.GetInstance(doc, fs);  // สร้าง PDF writer
                            doc.Open();  // เปิดเอกสาร PDF

                            // ฟอนต์สำหรับข้อความภาษาไทย // ใช้ฟอนต์ TH Sarabun
                            BaseFont thaiFont = BaseFont.CreateFont(@"C:\Users\WINDOWS\OneDrive\Progamming\WindowsFormsApp4\THSarabunNew Bold.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);  
                            iTextSharp.text.Font font = new iTextSharp.text.Font(thaiFont, 16);  // กำหนดขนาดฟอนต์ 16

                            // เพิ่มภาพโลโก้
                            string imagePath = @"C:\Users\WINDOWS\OneDrive\Progamming\WindowsFormsApp4\Logo\เมนู (6).png";  
                            // เส้นทางไปยังไฟล์รูปภาพโลโก้
                            if (File.Exists(imagePath))  // ตรวจสอบว่ามีไฟล์ภาพหรือไม่
                            {
                                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);  // โหลดภาพ
                                image.ScaleToFit(175f, 175f);  // ปรับขนาดให้พอดีกับพื้นที่
                                image.Alignment = iTextSharp.text.Image.ALIGN_CENTER;  // จัดแนวกลาง
                                doc.Add(image);  // เพิ่มภาพลงในเอกสาร PDF
                            }

                            // ส่วนหัวของเอกสาร
                            Paragraph header = new Paragraph("Tel : 0941111111\n---------------------------------------------------------------------------------------------------------------------------------------\n", font)
                            {
                                Alignment = Element.ALIGN_CENTER  // จัดกลาง
                            };
                            doc.Add(header);  // เพิ่มส่วนหัว

                            // ข้อมูลลูกค้า
                            Paragraph customerInfo = new Paragraph($"ชื่อลูกค้า: {customerName}\nเบอร์โทร: {customerPhone}\nอีเมล: {customerEmail}\nวันที่: {DateTime.Now.ToShortDateString()}\n\n", font)
                            {
                                Alignment = Element.ALIGN_RIGHT,  // จัดชิดขวา
                                Leading = 30f  // ระยะห่างระหว่างบรรทัด
                            };
                            doc.Add(customerInfo);  // เพิ่มข้อมูลลูกค้า

                            Paragraph separator = new Paragraph("\n===========================================================================\n", font)
                            {
                                Alignment = Element.ALIGN_CENTER,  // จัดกลาง
                                Leading = 10f
                            };
                            doc.Add(separator);  // เพิ่มเส้นแบ่ง

                            // ดึงรายละเอียดการสั่งซื้อ
                            string selectOrderQuery = "SELECT namecha, quantity, allprice FROM `order`";  // คำสั่ง SQL เพื่อดึงข้อมูลการสั่งซื้อ
                            MySqlCommand selectOrderCmd = new MySqlCommand(selectOrderQuery, conn);  // สร้างคำสั่ง SQL
                            MySqlDataReader orderReader = selectOrderCmd.ExecuteReader();  // รันคำสั่ง SQL และดึงข้อมูลการสั่งซื้อ

                            if (orderReader.HasRows)  // ถ้ามีข้อมูลการสั่งซื้อ
                            {
                                double totalVAT = 0, allprice = 0;  // ตัวแปรเก็บยอดรวม VAT และราคา

                                // สร้างตารางสำหรับข้อมูลการสั่งซื้อ
                                PdfPTable table = new PdfPTable(3);  // สร้างตาราง 3 คอลัมน์ (ชื่อสินค้า, จำนวน, ราคา)
                                table.WidthPercentage = 100;  // กำหนดความกว้างของตาราง
                                table.SetWidths(new float[] { 50f, 20f, 30f });  // กำหนดความกว้างของแต่ละคอลัมน์

                                // เพิ่มหัวข้อในตาราง
                                PdfPCell cellHeader1 = new PdfPCell(new Phrase("ชื่อสินค้า", font)) { HorizontalAlignment = Element.ALIGN_LEFT };
                                PdfPCell cellHeader2 = new PdfPCell(new Phrase("จำนวน", font)) { HorizontalAlignment = Element.ALIGN_CENTER };
                                PdfPCell cellHeader3 = new PdfPCell(new Phrase("ราคา", font)) { HorizontalAlignment = Element.ALIGN_RIGHT };
                                table.AddCell(cellHeader1);  // เพิ่มคอลัมน์ "ชื่อสินค้า"
                                table.AddCell(cellHeader2);  // เพิ่มคอลัมน์ "จำนวน"
                                table.AddCell(cellHeader3);  // เพิ่มคอลัมน์ "ราคา"
                                //วนลูป
                                while (orderReader.Read())  // อ่านข้อมูลการสั่งซื้อ
                                {
                                    string namecha = orderReader.GetString("namecha");
                                    int quantity = orderReader.GetInt32("quantity");
                                    double price = orderReader.GetDouble("allprice");

                                    double vat = price * 0.07;  // คำนวณ VAT 7%
                                    totalVAT += vat;  // รวม VAT
                                    allprice += price;  // รวมราคาทั้งหมด

                                    // เพิ่มข้อมูลลงในตาราง
                                    PdfPCell cellName = new PdfPCell(new Phrase(namecha, font)) { HorizontalAlignment = Element.ALIGN_LEFT };
                                    PdfPCell cellQuantity = new PdfPCell(new Phrase(quantity.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER };
                                    PdfPCell cellPrice = new PdfPCell(new Phrase(price.ToString("N2") + " บาท", font)) { HorizontalAlignment = Element.ALIGN_RIGHT };
                                    table.AddCell(cellName);
                                    table.AddCell(cellQuantity);
                                    table.AddCell(cellPrice);
                                }

                                // เพิ่มตารางลงในเอกสาร
                                doc.Add(table);

                                // สรุป VAT และราคาทั้งหมด
                                Paragraph vatSummary = new Paragraph($"VAT (7%): {totalVAT.ToString("N2")} บาท", font)
                                {
                                    Alignment = Element.ALIGN_RIGHT,  // จัดชิดขวา
                                    Leading = 25f
                                };
                                doc.Add(vatSummary);

                                Paragraph totalSummary = new Paragraph($"ราคาสุทธิ: {(allprice + totalVAT).ToString("N2")} บาท", font)
                                {
                                    Alignment = Element.ALIGN_RIGHT,  // จัดชิดขวา
                                    Leading = 25f
                                };
                                doc.Add(totalSummary);
                            }
                            else  // หากไม่มีข้อมูลการสั่งซื้อ
                            {
                                MessageBox.Show("ไม่มีข้อมูลในตาราง 'order'", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                            // ส่วนท้ายของเอกสาร
                            Paragraph footer = new Paragraph("ขอบพระคุณค่ะ อย่าลืมเก็บแต้มสะสมกันด้วยนะคะ ", font)
                            {
                                Alignment = Element.ALIGN_CENTER,  // จัดกลาง
                                Leading = 60f
                            };
                            doc.Add(footer);  // เพิ่มข้อความส่วนท้าย

                            doc.Close();  // ปิดเอกสาร PDF
                        }

                        // ตรวจสอบว่าไฟล์ PDF ถูกสร้างขึ้นหรือไม่
                        if (File.Exists(pdfPath))
                        {
                            MessageBox.Show("PDF ได้ถูกสร้างเรียบร้อยแล้ว", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Process.Start(pdfPath);  // เปิดไฟล์ PDF
                        }
                        else
                        {
                            MessageBox.Show("ไม่พบไฟล์ PDF ที่สร้างขึ้น", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else  // หากไม่พบข้อมูลลูกค้า
                    {
                        MessageBox.Show("ไม่มีข้อมูลลูกค้า", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)  // หากเกิดข้อผิดพลาด
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //ปุ่มกลับหน้าหลัก
        private void button1_Click(object sender, EventArgs e)
        {
                // แสดงข้อความ "ขอบพระคุณค่ะ"
                MessageBox.Show("ขอบคุณที่ให้ดูแลนะคะ ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // แสดงข้อความใน MessageBox แจ้งผู้ใช้งานว่า "ขอบคุณที่ให้ดูแลนะคะ"

                // เปลี่ยนไปยังหน้า Form1
                Form1 form1 = new Form1(); // สร้างหน้า Form1 ขึ้นมาใหม่
                form1.Show(); // แสดง Form1
                this.Hide(); // ซ่อนหน้าปัจจุบัน (Form ที่มีปุ่มกดอยู่) หลังจากที่ Form1 ถูกเปิด
        }
        // ปุ่มปิดโปรแกรม
        private void button5_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false; // ป้องกันการแสดง error popup
        }

    }
}