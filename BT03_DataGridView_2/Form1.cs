using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BT03_DataGridView_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvMonHoc.Rows.Add("01", "Cơ sở dữ liệu", 90, "Bắt buộc");
            dgvMonHoc.Rows.Add("02", "Lập trình cơ sở dữ liệu", 75, "Bắt buộc");
            dgvMonHoc.Rows.Add("03", "Pháp luật", 60, "Tuỳ chọn");
            dgvMonHoc.Rows.Add("04", "Nhập môn lập trình", 90, "Bắt buộc");
            dgvMonHoc.Rows.Add("05", "Lập trình Java cơ bản", 90, "Bắt buộc");
        }
    }
}
