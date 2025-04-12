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
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent(); //เรียกใช้เมธอด (อินิเฉอะลายคัมโพเนิน) เพื่อกำหนดค่าคอนโทรลต่าง ๆ ที่ถูกออกแบบไว้ใน Form Designer
        }
        //ย้อนกลับหน้าหลัก
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
