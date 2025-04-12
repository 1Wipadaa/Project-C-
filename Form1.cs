using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        //Constructor (คอนสตัคเจอร์) ของฟอร์ม Form1
        public Form1()
        {
            InitializeComponent(); //เรียกใช้เมธอด (อินิเฉอะลายคัมโพเนิน) เพื่อกำหนดค่าคอนโทรลต่าง ๆ ที่ถูกออกแบบไว้ใน Form Designer
        }
        //ปุ่มสมัครสมาชิก
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 F2 = new Form2();
            F2.Show();
            this.Hide();
        }
        //ปุ่มเข้าสู่ระบบ
        private void button1_Click(object sender, EventArgs e)
        {
            Form16 F16 = new Form16();
            F16.Show();
            this.Hide();
        }
        //ปุ่มแอดมิน
        private void button3_Click(object sender, EventArgs e)
        {
            Form4 F4 = new Form4();
            F4.Show();
            this.Hide();
        }
        //ปุ่มออกจากโปรแกรม
        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit(); // ปิดโปรแกรม
        }
    }
}
