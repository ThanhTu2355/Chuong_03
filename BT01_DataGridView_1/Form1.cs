using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BT01_DataGridView_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ThemCotVaoLuoi();
            ThemDongVaoLuoi();
            DinhDangLuoi();
        }

        private void ThemCotVaoLuoi()
        {
            dgvMonHoc.Columns.Add("colMaMH", "Mã môn");
            dgvMonHoc.Columns.Add("colTenMH", "Tên môn học");
            dgvMonHoc.Columns.Add("colSoTiet", "Số tiết");
            dgvMonHoc.Columns.Add("colLoaiMH", "Loại MH");
        }

        private void ThemDongVaoLuoi()
        {
            dgvMonHoc.Rows.Add("01","Cơ sở dữ liệu",90,"Bắt buộc");
            dgvMonHoc.Rows.Add("02", "Lập trình cơ sở dữ liệu", 75, "Bắt buộc");
            dgvMonHoc.Rows.Add("03", "Pháp luật", 60, "Tuỳ chọn");
            dgvMonHoc.Rows.Add("04", "Nhập môn lập trình", 90, "Bắt buộc");
            dgvMonHoc.Rows.Add("05", "Lập trình Java cơ bản", 90, "Bắt buộc");
        }

        private void DinhDangLuoi()
        {
            dgvMonHoc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMonHoc.MultiSelect = false;
            dgvMonHoc.AllowUserToAddRows = false;

            dgvMonHoc.Columns[0].Width = 100;
            dgvMonHoc.Columns[1].Width = 300;
            dgvMonHoc.Columns[2].Width = 100;
            dgvMonHoc.Columns[3].Width = 100;
        }
    }
}
