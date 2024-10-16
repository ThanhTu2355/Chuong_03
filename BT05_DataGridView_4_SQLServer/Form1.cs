using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BT05_DataGridView_4_SQLServer
{
    public partial class Form1 : Form
    {
        string strcon = @"server=.; Database=QLSV_SV_L1; integrated security=true";
        DataSet ds = new DataSet();
        //Khai bao doi tuong DataAdapter de su dung cho cac bang du lieu
        SqlDataAdapter adpMonHoc, adpKetQua;
        //Khai bao doi tuong CommandBuilder MonHoc de cap nhat du lieu cho bang MonHoc
        SqlCommandBuilder cmbMonHoc;
        BindingSource bs = new BindingSource();
        public Form1()
        {
            InitializeComponent();
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
            dgvMonHoc.DataSource = bs;
            dgvMonHoc.Columns[3].Visible = false;
            //5. Lien ket cac dieu khien tren Form voi BindingSource
            LienKetDieuKHien();
            //Lien ket control BindingNavigator
            bdnMonHoc.BindingSource = bs;
        }

        private void LienKetDieuKHien()
        {
            txtMaMH.DataBindings.Add("Text", bs, "MaMH", true);
            txtTenMH.DataBindings.Add("Text", bs, "TenMH", true);
            txtSoTiet.DataBindings.Add("Text", bs, "SoTiet", true);
        }

        private void KhoiTaoBindingSource()
        {
            bs.DataSource = ds;
            bs.DataMember = "MONHOC";
        }

        private void MocNoiQuanHe()
        {
            //Tao quan he giua tblMonHoc va tblKetQua
            ds.Relations.Add("FK_MONHOC_KETQUA", ds.Tables["MONHOC"].Columns["MaMH"], ds.Tables["KETQUA"].Columns["MaMH"], true);
            //Loai bo Cacase Delete trong cac quan he
            ds.Relations["FK_MONHOC_KETQUA"].ChildKeyConstraint.DeleteRule = Rule.None;
        }

        private void DocDuLieu()
        {
            //Sao chep cau truc va dua du lieu tu CSDL vao DataTable
            adpMonHoc.FillSchema(ds, SchemaType.Source, "MONHOC");
            adpMonHoc.Fill(ds, "MONHOC");

            adpKetQua.FillSchema(ds, SchemaType.Source, "KETQUA");
            adpKetQua.Fill(ds, "KETQUA");
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaMH.ReadOnly = false;
            bs.AddNew();
            txtMaMH.Focus();
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {
            bs.CancelEdit();
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            if (txtMaMH.ReadOnly == false)//ghi khi them moi
            {
                //Kiem tra MaSV bi trung
                DataRow r = ds.Tables["MONHOC"].Rows.Find(txtMaMH.Text);
                if (r != null)
                {
                    MessageBox.Show("MonHoc bi trung. Moi nhap lai", "Trung khoa chinh");
                    txtMaMH.Focus();
                    return;
                }
            }
            //Cap nhat lai viec them moi hay sua trong Database
            bs.EndEdit();
            //Cap nhat trong CSDL
            int n = adpMonHoc.Update(ds, "MONHOC");
            if (n > 0)
                MessageBox.Show("Cap nhat (THEM/SUA) thanh cong");
            txtMaMH.ReadOnly = true;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            //Xac dinh dong can huy Su dung ham Find
            DataRow rmh = (bs.Current as DataRowView).Row;
            //Can kien tra Neu rmh ton tai trong tblKetQua thi khong xoa. Nguoc lai thi cho xoa
            //Su dung ham getChilRow de kiem tra nhung dong lien quan co ton tai hay khong. Gia tri tra ve la mang
            DataRow[] mangDongLienQuan = rmh.GetChildRows("FK_MONHOC_KETQUA");
            if (mangDongLienQuan.Length > 0)//co ton tai nhung dong lien quan trong tblKetQua
                MessageBox.Show("Khong xoa duoc MonHoc vi da co SinhVien thi","Thong bao loi xoa Mon Hoc",MessageBoxButtons.OK,MessageBoxIcon.Error);
            else
            {
                DialogResult tl;
                tl = MessageBox.Show("Xoa MonHoc nay khong?", "Can than", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (tl == DialogResult.Yes)
                {
                    //Xoa trong DataTable
                    bs.RemoveCurrent();
                    //Xoa trong CSDL
                    int n = adpMonHoc.Update(ds, "MONHOC");
                    if (n > 0)
                        MessageBox.Show("Xoa MonHoc thanh cong");
                }
            }
        }

        private void KhoiTaoCacDoiTuong()
        {
            //1. Khoi tao cac doi tuong DataAdapter
            adpMonHoc = new SqlDataAdapter("select * from MONHOC", strcon);
            adpKetQua = new SqlDataAdapter("select * from KETQUA", strcon);
            //2. Khoi tao doi tuong CommandBuilder
            cmbMonHoc = new SqlCommandBuilder(adpMonHoc);
        }
    }
}
