using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4;

namespace WindowsFormsApp4
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent(); //เรียกใช้เมธอด (อินิเฉอะลายคัมโพเนิน) เพื่อกำหนดค่าคอนโทรลต่าง ๆ ที่ถูกออกแบบไว้ใน Form Designer
        }
        //ไปหน้าผู้พัฒนา
        private void button1_Click(object sender, EventArgs e)
        {
            Form8 F8 = new Form8();
            F8.Show();
            this.Hide();
        }
        //ไปหน้าจัดกาสินค้า
        private void button2_Click(object sender, EventArgs e)
        {
            Form9 F9 = new Form9();
            F9.Show();
            this.Hide();
        }
        //ไปหน้าจัดการข้อมูลลูกค้า
        private void button3_Click(object sender, EventArgs e)
        {
            Form10 F10 = new Form10();
            F10.Show();
            this.Hide();
        }
        //ไปหน้ายอดขาย
        private void button4_Click(object sender, EventArgs e)
        {
            SalesReportForm form12 = new SalesReportForm();
            form12.Show();
            this.Hide();
        }
        //ไปหน้าบัตรสะสม
        private void button8_Click(object sender, EventArgs e)
        {
            Form13 F13 = new Form13();
            F13.Show();
            this.Hide();
        }
        //ปุ่มกลับไปหน้าหลัก
        private void button6_Click(object sender, EventArgs e)
        {
            Form1 F1 = new Form1();
            F1.Show();
            this.Hide();
        }
        // ปุ่มปิดโปรแกรม
        private void button5_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }

    }
}
