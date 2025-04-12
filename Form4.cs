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
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent(); //ฟังก์เรียกใช้เมธอด (อินิเฉอะลายคัมโพเนิน) เพื่อกำหนดค่าคอนโทรลต่าง ๆ ที่ถูกออกแบบไว้ใน Form Designer
            textBox1.PasswordChar = '*'; // กำหนดให้คอนโทรล textBox1 แสดงเป็นตัวอักษร "*" เมื่อพิมพ์
        }
        //ปุ่มเข้าสู่ระบบ
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "1552")
            {
                //ไปหน้าหลักแอดมิน
                Form5 F5 = new Form5();
                F5.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("รหัสไม่ถูกต้อง", "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //ปุ่มกลับหน้าหลัก
        private void button3_Click(object sender, EventArgs e)
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
