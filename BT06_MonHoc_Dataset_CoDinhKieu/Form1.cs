using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BT06_MonHoc_Dataset_CoDinhKieu
{
    public partial class Form1 : Form
    {
        //Khai bao Dataset co dinh kieu
        DsQLSV ds = new DsQLSV();
        //Khai bao cac doi tuong DataAdapter
        DsQLSVTableAdapters.MONHOCTableAdapter adpMonHoc = new DsQLSVTableAdapters.MONHOCTableAdapter();
        DsQLSVTableAdapters.KETQUATableAdapter adpKetQua = new DsQLSVTableAdapters.KETQUATableAdapter();
        BindingSource bs = new BindingSource();
        int stt = -1;
        public Form1()
        {
            InitializeComponent();
            bs.CurrentChanged += Bs_CurrentChanged;
        }

        private void Bs_CurrentChanged(object sender, EventArgs e)
        {
            lblSTT.Text = bs.Position + 1 + "/" + bs.Count;
            txtTSSV.Text = TongSoSinhVien(txtMaMH.Text).ToString();
            txtMaxDiem.Text = MaxDiem(txtMaMH.Text).ToString();
            btnDau.Enabled = bs.Position > 0;
            btnTruoc.Enabled = btnDau.Enabled;
            btnSau.Enabled = bs.Position < bs.Count - 1;
            btnCuoi.Enabled = btnSau.Enabled;
        }

        private object TongSoSinhVien(string mmh)
        {
            double kq = 0;
            Object td = ds.Tables["KETQUA"].Compute("count(MaSV)", "MaMH='" + mmh + "'");
            //Luu y: truong hop SV khong co diem thi phuong thuc Compute tra ve gia tri DBNull
            if (td == DBNull.Value)
                kq = 0;
            else
                kq = Convert.ToDouble(td);
            return kq;
        }

        private object MaxDiem(string mmh)
        {
            double kq = 0;
            Object td = ds.Tables["KETQUA"].Compute("max(diem)", "MaMH='" + mmh + "'");
            //Luu y: truong hop SV khong co diem thi phuong thuc Compute tra ve gia tri DBNull
            if (td == DBNull.Value)
                kq = 0;
            else
                kq = Convert.ToDouble(td);
            return kq;
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DocDuLieu();
            KhoiTaoBindingSource();
            LienKetDieuKhien();
            bdnLoaiMonHoc.BindingSource = bs;
        }

        private void LienKetDieuKhien()
        {
            //Chu y dieu khien du lieu va tinh toan
            foreach (Control ctl in this.Controls)
                if (ctl is TextBox && ctl.Name != "txtTSSV" && ctl.Name != "txtMaxDiem" && ctl.Name != "txtLoaiMon")
                    ctl.DataBindings.Add("text", bs, ctl.Name.Substring(3), true);
            //Binding cho dieu khien Phai
            Binding bdLoaiMH = new Binding("text", bs, "LoaiMH", true);
            //Su dung cac phuong thuc Khi hien thi va nhan lai gia tri
            bdLoaiMH.Format += BdLoaiMH_Format;
            bdLoaiMH.Parse += BdLoaiMH_Parse;
            txtLoaiMon.DataBindings.Add(bdLoaiMH);
        }

        private void BdLoaiMH_Format(object sender, ConvertEventArgs e)
        {
            if (e.Value == DBNull.Value || e.Value == null) return;
            e.Value = (Boolean)e.Value ? "Bắt buộc" : "Tuỳ chọn";
        }

        private void BdLoaiMH_Parse(object sender, ConvertEventArgs e)
        {
            if (e.Value == null) return;
            e.Value = e.Value.ToString().ToUpper() == "Bắt buộc" ? true : false;
        }

        private void KhoiTaoBindingSource()
        {
            bs.DataSource = ds;
            bs.DataMember = ds.MONHOC.TableName;
        }

        private void DocDuLieu()
        {
            adpMonHoc.Fill(ds.MONHOC);
            adpKetQua.Fill(ds.KETQUA);
        }

        private void btnDau_Click(object sender, EventArgs e)
        {
            bs.MoveFirst();
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            bs.MovePrevious();
        }

        private void btnSau_Click(object sender, EventArgs e)
        {
            bs.MoveNext();
        }

        private void btnCuoi_Click(object sender, EventArgs e)
        {
            bs.MoveLast();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaMH.ReadOnly = false;
            stt = bs.Position;
            bs.AddNew();
            txtMaMH.Focus();
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {
            bs.CancelEdit();
            bs.Position = stt;
            txtMaMH.ReadOnly = true;
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            if(txtMaMH.ReadOnly==false)//Them moi
            {
                DsQLSV.MONHOCRow rMH = ds.MONHOC.FindByMaMH(txtMaMH.Text);
                if (rMH != null)
                {
                    MessageBox.Show("MaMH bị trùng, mời nhập lại", "Thông báo trùng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaMH.Clear();
                    txtMaMH.Focus();
                    return;
                }
                txtMaMH.ReadOnly = true;
                //Cập nhật lại việc thêm mới hay sửa vào trong Data Table
                bs.EndEdit();
                int n = adpMonHoc.Update(ds.MONHOC);
                if (n > 0)
                    MessageBox.Show("Cập nhật (Thêm/Sửa) thành công");
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            //Kiểm tra có tồn tại các mẫu có liên quan trong KETQUA hay không
            DsQLSV.MONHOCRow rMH = (bs.Current as DataRowView).Row as DsQLSV.MONHOCRow;
            if (rMH.GetKETQUARows().Length > 0)
            {
                MessageBox.Show("Môn học này đã có sinh viên dự thi, không huỷ được", "Thông báo xoá",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult tl;
            tl = MessageBox.Show("Xoá MonHoc này không?", "Xoá Mon học", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (tl == DialogResult.Yes)
            {
                //Xoa trong DataTable
                bs.RemoveCurrent();
                //Xoa trong CSDL
                int n = adpMonHoc.Update(ds.MONHOC);
                if (n > 0)
                    MessageBox.Show("Xoá Môn học thành công","Xoá môn học",MessageBoxButtons.OK);
            }
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
