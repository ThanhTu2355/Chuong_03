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

        private void btnHuy_Click(object sender, EventArgs e)
        {
            //Kiểm tra có tồn tại các mẫu có liên quan trong KETQUA hay không
            QLSVms.SINHVIENRow rSV = (bsSV.Current as DataRowView).Row as QLSVms.SINHVIENRow;
            if (rSV.GetKETQUARows().Length > 0)
            {
                MessageBox.Show("SINH VIÊN này đã dự thi, không huỷ được", "Thông báo xoá",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult tl;
            tl = MessageBox.Show("Bạn có muốn xoá SINH VIÊN:" + "\r\n" +
                "   + MaSV: " + txtMaSV.Text + "\r\n" +
                "   + Họ tên: " + txtHoSV.Text + ' ' + txtTenSV.Text + "\r\n" +
                " này không?", "Xoá SINH VIÊN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (tl == DialogResult.Yes)
            {
                //Xoa trong DataTable
                bsSV.RemoveCurrent();
                //Xoa trong CSDL
                int n = adpMonHoc.Update(ds.MONHOC);
                if (n > 0)
                    MessageBox.Show("Xoá SINH VIÊN thành công", "Xoá SINH VIÊN", MessageBoxButtons.OK);
            }
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            if (txtMaSV.ReadOnly == false)//Them moi
            {
                QLSVms.SINHVIENRow rSV = ds.SINHVIEN.FindByMaSV(txtMaSV.Text);
                if (rSV != null)
                {
                    MessageBox.Show("Mã sinh viên: " + txtMaSV.Text + "vừa nhập bị trùng, mời nhập lại", "Thông báo trùng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaSV.Clear();
                    txtMaSV.Focus();
                    return;
                }
                txtMaSV.ReadOnly = true;
                //Cập nhật lại việc thêm mới hay sửa vào trong Data Table
                bsSV.EndEdit();
                int n = adpSinhVien.Update(ds.SINHVIEN);
                if (n > 0)
                    MessageBox.Show("Cập nhật (THÊM/XOÁ) cho SINH VIÊN:" + "\r\n" +
                "   + MaSV: " + txtMaSV.Text + "\r\n" +
                "   + Họ tên: " + txtHoSV.Text + ' ' + txtTenSV.Text + "\r\n" +
                " thành công", "Cập nhật SINH VIÊN", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvKetQua_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            //Xử lý cập nhật trên DataGridView
            //1. Khi là dòng trống, thì không làm gì hết nếu click chọn dòng khác
            if (dgvKetQua.CurrentRow.IsNewRow == true) return;
            //2. Dòng có chỉnh sửa: Thêm mới hay dòng đang chỉnh sửa
            if (dgvKetQua.IsCurrentRowDirty == true)
            {
                if ((dgvKetQua.CurrentRow.DataBoundItem as DataRowView).IsNew == true)
                {
                    //Kiểm tra khoá chính có bị trùng hay không
                    if (ds.KETQUA.FindByMaSVMaMH(dgvKetQua.CurrentRow.Cells["MaSV"].Value.ToString(),
                        dgvKetQua.CurrentRow.Cells["colMaMH"].Value.ToString())!=null)
                    {
                        MessageBox.Show("Môn học này sinh viên đã thi , vui lòng chọn môn học khác",
                            "Thông báo lỗi bị trùng Mã môn học", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        //Cho Ô maxMH là hiện hành
                        dgvKetQua.CurrentCell = dgvKetQua.CurrentRow.Cells["colMaMH"];
                        return;
                    }
                }
                //Kết thúc quá trình chỉnh sửa
                (dgvKetQua.CurrentRow.DataBoundItem as DataRowView).EndEdit();
                //Cập nhật về CSDL
                int n = adpKetQua.Update(ds.KETQUA);
                if(n>0)
                    MessageBox.Show("Cập nhật điểm thi cho sinh viên thành công",
                            "Cập nhật kết quả thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvKetQua_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            //Xảy ra khi người dùng chọn 1 dòng và bấm phím Delete
            //Cập nhật về CSDL
            int n = adpKetQua.Update(ds.KETQUA);
            if (n > 0)
                MessageBox.Show("Huỷ kết quả điểm thành công",
                        "Huỷ kết quả điểm", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
