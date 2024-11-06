using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnTap_MainSub
{
    public partial class Form1 : Form
    {
        QLKH ds = new QLKH();
        QLKHTableAdapters.KHOATableAdapter adpKhoa = new QLKHTableAdapters.KHOATableAdapter();
        QLKHTableAdapters.SINHVIENTableAdapter adpSinhVien = new QLKHTableAdapters.SINHVIENTableAdapter();


        BindingSource bsKH = new BindingSource();//Làm nguồn dữ liệu cho Main Form
        BindingSource bsSV = new BindingSource();//Làm nguồn dữ liệu cho lưới

        int stt = -1;
        public Form1()
        {
            InitializeComponent();
            bsKH.CurrentChanged += Bs_CurrentChanged;
        }

        private void Bs_CurrentChanged(object sender, EventArgs e)
        {
            txtTSSV.Text = TongSoSinhVien(txtMaKH.Text).ToString();
            btnTruoc.Enabled = bsKH.Position > 0;
            btnSau.Enabled = bsKH.Position < bsKH.Count - 1;
            lblSTT.Text = bsKH.Position + 1 + "/" + bsKH.Count;
        }

        private object TongSoSinhVien(string mkh)
        {
            double kq = 0;
            Object td = ds.Tables["SINHVIEN"].Compute("count(MaSV)", "MaKH='" + mkh + "'");
            //Luu y: truong hop SV khong co diem thi phuong thuc Compute tra ve gia tri DBNull
            if (td == DBNull.Value)
                kq = 0;
            else
                kq = Convert.ToDouble(td);
            return kq;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DocDuLieu();
            LienKetDieuKhien();
            txtTSSV.Text = TongSoSinhVien(txtMaKH.Text).ToString();
            bdnKhoa.BindingSource = bsKH;
        }

        private void LienKetDieuKhien()
        {
            foreach (Control ctl in this.Controls)
                if (ctl is TextBox && ctl.Name != "txtTSSV")
                    ctl.DataBindings.Add("text", bsKH, ctl.Name.Substring(3), true);
        }

        private void DocDuLieu()
        {
            //1. Nạp dữ liệu cho các DataTable
            adpKhoa.Fill(ds.KHOA);
            adpSinhVien.Fill(ds.SINHVIEN);


            //2. Nạp nguồn cho BindingSource bsKH
            bsKH.DataSource = ds.KHOA;

            //3. Nạp nguồn cho BindingSource bsSV
            bsSV.DataSource = bsKH;
            bsSV.DataMember = "KHOASINHVIEN";

            //4. Gán nguồn cho lưới
            dgvKhoa.DataSource = bsSV;

            //6. Không hiển thị cột MaSV trong lưới
            //dgvKetQua.Columns["MaSV"].Visible = false;
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            bsKH.MovePrevious();
        }

        private void btnSau_Click(object sender, EventArgs e)
        {
            bsKH.MoveNext();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaKH.ReadOnly = false;
            //Them moi
            stt = bsKH.Position;
            bsKH.AddNew();
            txtMaKH.Focus();
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {
            bsKH.CancelEdit();
            bsKH.Position = stt;
            txtMaKH.ReadOnly = true;
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            if (txtMaKH.ReadOnly == false)//Them moi
            {
                QLKH.KHOARow rSV = ds.KHOA.FindByMaKH(txtMaKH.Text);
                if (rSV != null)
                {
                    MessageBox.Show("Mã khoa: " + txtMaKH.Text + "vừa nhập bị trùng, mời nhập lại", "Thông báo trùng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaKH.Clear();
                    txtMaKH.Focus();
                    return;
                }
                txtMaKH.ReadOnly = true;
                //Cập nhật lại việc thêm mới hay sửa vào trong Data Table
                bsKH.EndEdit();
                int n = adpKhoa.Update(ds.KHOA);
                if (n > 0)
                    MessageBox.Show("Cập nhật (THÊM/XOÁ) cho Khoa:" + "\r\n" +
                "   + MaSV: " + txtMaKH.Text + "\r\n" +
                "   + Họ tên: " + txtTenKH.Text + 
                " thành công", "Cập nhật Khoa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            //Kiểm tra có tồn tại các mẫu có liên quan trong KETQUA hay không
            QLKH.KHOARow rSV = (bsKH.Current as DataRowView).Row as QLKH.KHOARow;
            if (rSV.GetSINHVIENRows().Length > 0)
            {
                MessageBox.Show("KHOA này đã dự thi, không huỷ được", "Thông báo xoá",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult tl;
            tl = MessageBox.Show("Bạn có muốn xoá KHOA:" + "\r\n" +
                "   + MaKH: " + txtMaKH.Text + "\r\n" +
                "   + Họ tên: " + txtTenKH.Text + 
                " này không?", "Xoá KHOA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (tl == DialogResult.Yes)
            {
                //Xoa trong DataTable
                bsKH.RemoveCurrent();
                //Xoa trong CSDL
                int n = adpKhoa.Update(ds.KHOA);
                if (n > 0)
                    MessageBox.Show("Xoá KHOA thành công", "Xoá KHOA", MessageBoxButtons.OK);
            }
        }
    }
}
