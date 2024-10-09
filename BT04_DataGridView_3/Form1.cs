using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BT04_DataGridView_3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dgvMonHoc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void ThemDongVaoLuoi()
        {
            dgvMonHoc.Rows.Add("01", "Cơ sở dữ liệu", 90);
            dgvMonHoc.Rows.Add("02", "Lập trình cơ sở dữ liệu", 75);
            dgvMonHoc.Rows.Add("03", "Pháp luật", 60);
            dgvMonHoc.Rows.Add("04", "Nhập môn lập trình", 90);
            dgvMonHoc.Rows.Add("05", "Lập trình Java cơ bản", 90);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Them du lieu vao luoi
            ThemDongVaoLuoi();
            //Chon 1 dong tren luoi
            DataGridViewRow r = dgvMonHoc.Rows[0];
            r.Selected = true;
            GanDuLieu(r);
        }

        private void GanDuLieu(DataGridViewRow r)
        {
            txtMaMH.Text = r.Cells["colMaMH"].Value.ToString();
            txtTenMH.Text = r.Cells[1].Value.ToString();
            txtSoTiet.Text = r.Cells[2].Value.ToString();
        }

        private void dgvMonHoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Su kien nay xay ra khi co mot o tren dong nao do duoc Click
            //De lay thong tin cua dong chua o duoc Click => e.RowIndex
            DataGridViewRow r = dgvMonHoc.Rows[e.RowIndex];
            GanDuLieu(r);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaMH.ReadOnly = false;
            foreach (Control ctl in this.Controls)
                if (ctl is TextBox)
                    (ctl as TextBox).Clear();
            txtMaMH.Focus();
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {
            GanDuLieu(dgvMonHoc.SelectedRows[0]);
            txtMaMH.ReadOnly = true;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult tl;
            tl = MessageBox.Show("ban co muon huy mon hoc nay khong (Y?N)?", "Huy mon hoc",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (tl == DialogResult.Yes)
            {
                //Lay dong can huy tu dong thu 0 trong tap hop cac dong duoc chon SelectedRows[0]
                DataGridViewRow rhuy = dgvMonHoc.SelectedRows[0];
                //Huy dong trong luoi
                dgvMonHoc.Rows.Remove(rhuy);
                //Chon lai dong dau tien
                dgvMonHoc.Rows[0].Selected = true;
                GanDuLieu(dgvMonHoc.Rows[0]);
                //Khong huy trong csdl
            }
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            if(txtMaMH.ReadOnly==true)//ghi khi sua
            {
                DataGridViewRow rsua = dgvMonHoc.SelectedRows[0];
                //Sua thong tin cua dong theo thong tin cua cac dieu khien
                rsua.Cells[1].Value = txtTenMH.Text;
                rsua.Cells[2].Value = txtSoTiet.Text;
            }
            else//ghi khi them moi
            {
                int stt = dgvMonHoc.Rows.Add(txtMaMH.Text, txtTenMH.Text, txtSoTiet.Text);
                //stt tra ve chi so cua dong moi them vao
                dgvMonHoc.Rows[stt].Selected = true;
                txtMaMH.ReadOnly = true;
            }
        }
    }
}
