using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BT07_MainSub_SV_KQ
{
    public partial class Form1 : Form
    {
        QLSVms ds = new QLSVms();
        QLSVmsTableAdapters.KHOATableAdapter adpKhoa = new QLSVmsTableAdapters.KHOATableAdapter();
        QLSVmsTableAdapters.SINHVIENTableAdapter adpSinhVien = new QLSVmsTableAdapters.SINHVIENTableAdapter();
        QLSVmsTableAdapters.KETQUATableAdapter adpKetQua = new QLSVmsTableAdapters.KETQUATableAdapter();
        QLSVmsTableAdapters.MONHOCTableAdapter adpMonHoc = new QLSVmsTableAdapters.MONHOCTableAdapter();

        BindingSource bsSV = new BindingSource();//Làm nguồn dữ liệu cho Main Form
        BindingSource bsKQ = new BindingSource();//Làm nguồn dữ liệu cho lưới

        int stt = -1;
        public Form1()
        {
            InitializeComponent();
            bsSV.CurrentChanged += bsSV_CurrentChanged;
        }

        private void bsSV_CurrentChanged(object sender, EventArgs e)
        {
            bdnSVKQ.BindingSource = bsSV;
            lblSTT.Text = bsSV.Position + 1 + "/" + bsSV.Count;
            txtTongDiem.Text = TongDiem(txtMaSV.Text).ToString();
            btnTruoc.Enabled = bsSV.Position > 0;
            btnSau.Enabled = bsSV.Position < bsSV.Count - 1;
        }

        private object TongDiem(string MSV)
        {
            double kq = 0;
            Object td = ds.Tables["KETQUA"].Compute("sum(Diem)", "MaSV='" + MSV + "'");
            //Luu y Truong hop SV khong co diem thi phuong thuc tra ve gia tri DBNull
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
            txtTongDiem.Text = TongDiem(txtMaSV.Text).ToString();
        }

        private void LienKetDieuKhien()
        {
            foreach (Control ctl in this.Controls)
                if (ctl is TextBox && ctl.Name != "txtTongDiem")
                    ctl.DataBindings.Add("text", bsSV, ctl.Name.Substring(3), true);
                else if (ctl is ComboBox)
                    ctl.DataBindings.Add("Selectedvalue", bsSV, ctl.Name.Substring(3), true);
                else if (ctl is DateTimePicker)
                    ctl.DataBindings.Add("value", bsSV, ctl.Name.Substring(3), true);
                else if (ctl is CheckBox)
                    ctl.DataBindings.Add("checked", bsSV, ctl.Name.Substring(3), true);
        }

        private void DocDuLieu()
        {
            //1. Nạp dữ liệu cho các DataTable
            adpKhoa.Fill(ds.KHOA);
            adpSinhVien.Fill(ds.SINHVIEN);
            adpMonHoc.Fill(ds.MONHOC);
            adpKetQua.Fill(ds.KETQUA);

            //2. Nạp nguồn cho Combobox
            cboMaKH.DisplayMember = "TenKH";
            cboMaKH.ValueMember = "MaKH";
            cboMaKH.DataSource = ds.KHOA;

            //3. Nạp nguồn cho BindingSource bsSV
            bsSV.DataSource = ds.SINHVIEN;

            //4. Nạp nguồn cho BindingSource bsKQ
            bsKQ.DataSource = bsSV;
            bsKQ.DataMember = "SINHVIENKETQUA";

            //5. Gán nguồn cho lưới
            dgvKetQua.DataSource = bsKQ;

            //6. Không hiển thị cột MaSV trong lưới
            dgvKetQua.Columns["MaSV"].Visible = false;
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            bsSV.MovePrevious();
        }

        private void btnSau_Click(object sender, EventArgs e)
        {
            bsSV.MoveNext();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaSV.ReadOnly = false;
            //Them moi
            stt = bsSV.Position;
            bsSV.AddNew();
            cboMaKH.SelectedIndex = 0;
            dtpNgaySinh.Value = new DateTime(2005, 1, 1);
            txtMaSV.Focus();
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {
            bsSV.CancelEdit();
            bsSV.Position = stt;
            txtMaSV.ReadOnly = true;
        }
    }
}
