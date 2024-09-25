using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace BT00
{
    public partial class Form1 : Form
    {
        string strcon = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=..\..\..\data\QLSV.mdb";
        DataSet ds = new DataSet();
        //Khai bao doi tuong DataAdapter de su dung cho cac bang du lieu
        OleDbDataAdapter adpSinhVien, adpKhoa, adpKetQua;
        //Khai bao doi tuong CommandBuilder SinhVien de cap nhat du lieu cho bang SinhVien
        OleDbCommandBuilder cmbSinhVien;
        BindingSource bs = new BindingSource();
        public Form1()
        {
            InitializeComponent();
            bs.CurrentChanged += Bs_CurrentChanged;
        }

        private void Bs_CurrentChanged(object sender, EventArgs e)
        {
            lblSTT.Text = bs.Position + 1 + "/" + bs.Count;
            txtTongDiem.Text = TongDiem(txtMaSV.Text).ToString();
        }

        private Double TongDiem(string msv)
        {
            double kq = 0;
            Object td = ds.Tables["KETQUA"].Compute("sum(Diem)", "MaSV='" + msv + "'");
            //Luu y: truong hop SV khong co diem thi phuong thuc Compute tra ve gia tri DBNull
            if (td == DBNull.Value)
                kq = 0;
            else
                kq = Convert.ToDouble(td);
            return kq;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //1. Khoi tao cac doi tuong
            KhoiTaoCacDoiTuong();
            //2. tao cau truc doc du lieu tu cac bang trong CSDL vao DataTable
            DocDuLieu();
            MocNoiQuanHe();
            //3. Lop BindingSource la lop trung gian: Doi tuong chua du lieu va cac dieu khien
            //Co cac phuong thuc: DataSource la DataSet , DataMember ten bang trong DataSet
            KhoiTaoBindingSource();
            //4.Khoi tao combobox
            KhoiTaoCboKH();
            //5. Lien ket cac dieu khien tren Form voi BindingSource
            LienKetDieuKHien();
            //Lien ket control BindingNavigator
            bdnSinhvien.BindingSource = bs;
        }

        private void KhoiTaoCacDoiTuong()
        {
            //1. Khoi tao cac doi tuong DataAdapter
            adpKhoa = new OleDbDataAdapter("select * from KHOA", strcon);
            adpSinhVien = new OleDbDataAdapter("select * from SINHVIEN", strcon);
            adpKetQua = new OleDbDataAdapter("select * from KETQUA", strcon);
            //2. Khoi tao doi tuong CommandBuilder
            cmbSinhVien = new OleDbCommandBuilder(adpSinhVien);
        }

        private void KhoiTaoBindingSource()
        {
            bs.DataSource = ds;
            bs.DataMember = "SINHVIEN";
        }

        private void KhoiTaoCboKH()
        {
            cboMaKH.DisplayMember = "TenKH";
            cboMaKH.ValueMember = "MaKH";
            cboMaKH.DataSource = ds.Tables["KHOA"];
        }

        private void LienKetDieuKHien()
        {
            //Chu y dieu khien du lieu va tinh toan
            foreach (Control ctl in this.Controls)
                if (ctl is TextBox && ctl.Name != "txtTongDiem" && ctl.Name != "txtPhai")
                    ctl.DataBindings.Add("text", bs, ctl.Name.Substring(3), true);
            else if(ctl is ComboBox)
                    ctl.DataBindings.Add("Selectedvalue", bs, ctl.Name.Substring(3), true);
            else if(ctl is DateTimePicker)
                    ctl.DataBindings.Add("value", bs, ctl.Name.Substring(3), true);
            //Binding cho dieu khien Phai
            Binding bdPhai = new Binding("text",bs,"Phai",true);
            //Su dung cac phuong thuc Khi hien thi va nhan lai gia tri
            bdPhai.Format += BdPhai_Format;
            bdPhai.Parse += BdPhai_Parse;
            txtPhai.DataBindings.Add(bdPhai);
        }

        private void BdPhai_Parse(object sender, ConvertEventArgs e)
        {
            if (e.Value == null) return;
            e.Value = e.Value.ToString().ToUpper() == "NAM" ? true : false;
        }

        private void BdPhai_Format(object sender, ConvertEventArgs e)
        {
            if (e.Value == DBNull.Value || e.Value == null) return;
            e.Value = (Boolean)e.Value ? "Nam" : "Nữ";
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            bs.MovePrevious();
        }

        private void btnSau_Click(object sender, EventArgs e)
        {
            bs.MoveNext();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaSV.ReadOnly = false;
            //Them moi
            bs.AddNew();
            cboMaKH.SelectedIndex = 0;
            dtpNgaySinh.Value = new DateTime(2005, 1, 1);
            txtMaSV.Focus();
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {
            //Su dung phuong thuc CancelEdit de huy su yhay doi tren BindingSource
            bs.CancelEdit();
            txtMaSV.ReadOnly = true;
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            if (txtMaSV.ReadOnly == false)//ghi khi them moi
            {
                //Kiem tra MaSV bi trung
                DataRow r = ds.Tables["SINHVIEN"].Rows.Find(txtMaSV.Text);
                if (r != null)
                {
                    MessageBox.Show("MaSV bi trung. Moi nhap lai", "Trung khoa chinh");
                    txtMaSV.Focus();
                    return;
                }
            }
            //Cap nhat lai viec them moi hay sua trong Datatable
            bs.EndEdit();
            //Cap nhat lai CSDL
            int n = adpSinhVien.Update(ds, "SINHVIEN");
            if (n > 0)
                MessageBox.Show("Cap nhat(Them/SUA) thah cong");
            txtMaSV.ReadOnly = true;
        }

        private void DocDuLieu()
        {
            //Sao chep cau truc va dua du lieu tu CSDL vao DataTable
            adpKhoa.FillSchema(ds, SchemaType.Source, "KHOA");
            adpKhoa.Fill(ds, "KHOA");

            adpSinhVien.FillSchema(ds, SchemaType.Source, "SINHVIEN");
            adpSinhVien.Fill(ds, "SINHVIEN");

            adpKetQua.FillSchema(ds, SchemaType.Source, "KETQUA");
            adpKetQua.Fill(ds, "KETQUA");
        }

        private void MocNoiQuanHe()
        {
            //Tao quan he giua tblKhoa vaf tblSinhVien
            ds.Relations.Add("FK_KHOA_SINHVIEN", ds.Tables["KHOA"].Columns["MaKH"], ds.Tables["SINHVIEN"].Columns["MaKH"], true);
            //Tao quan he giua tblSinhVien vaf tblKetQua
            ds.Relations.Add("FK_SINHVIEN_KETQUA", ds.Tables["SINHVIEN"].Columns["MaSV"], ds.Tables["KETQUA"].Columns["MaSV"], true);
            //Loai bo Cacase Delete trong cac quan he
            ds.Relations["FK_KHOA_SINHVIEN"].ChildKeyConstraint.DeleteRule = Rule.None;
            ds.Relations["FK_SINHVIEN_KETQUA"].ChildKeyConstraint.DeleteRule = Rule.None;
        }
    }
}
