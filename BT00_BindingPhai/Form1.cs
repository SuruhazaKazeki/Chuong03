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

namespace BT00_BindingPhai
{
    public partial class Form1 : Form
    {
        string strcon = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\..\DATA\QLSV.mdb";
        DataSet ds = new DataSet();
        OleDbDataAdapter adpSinhVien, adpKhoa, adpKetQua;
        OleDbCommandBuilder cmdSinhVien;
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

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Khoi_Tao_Doi_Tuong();
            Doc_Du_Lieu();
            Moc_Noi_Quan_He();
            Khoi_Tao_BindingSource();
            Create_cbbKhoa();
            Control_Link();
            Control_Link_Phai();
        }

        private void Control_Link_Phai()
        {
            Binding bdPhai = new Binding("text", bs, "Phai", true);
            bdPhai.Format += bdPhai_Format;
            bdPhai.Parse += bdPhai_Parse;
            txtPhai.DataBindings.Add(bdPhai);
        }

        private void Control_Link()
        {
            //chú ý các điều kiện và tính toán
            foreach (Control ctl in this.Controls)
                if (ctl is TextBox && ctl.Name != "txtTongDiem" && ctl.Name != "txtPhai")
                    ctl.DataBindings.Add("Text", bs, ctl.Name.Substring(3), true);
                else if (ctl is ComboBox)
                    ctl.DataBindings.Add("Selectedvalue", bs, ctl.Name.Substring(3), true);
                else if (ctl is DateTimePicker)
                    ctl.DataBindings.Add("value", bs, ctl.Name.Substring(3), true);
            
        }

        private void bdPhai_Parse(object sender, ConvertEventArgs e)
        {
            if (e.Value == null) return;
            e.Value = e.Value.ToString().ToUpper() == "Nam" ? true : false;
        }

        private void bdPhai_Format(object sender, ConvertEventArgs e)
        {
            if (e.Value == DBNull.Value || e.Value == null) return;
            e.Value = (Boolean)e.Value ? "Nam" : "Nữ";
        }

        private void Create_cbbKhoa()
        {
            cboMaKH.DisplayMember = "TenKH";
            cboMaKH.ValueMember = "MaKH";
            cboMaKH.DataSource = ds.Tables["KHOA"];
        }

        private void Khoi_Tao_BindingSource()
        {
            bs.DataSource = ds;
            bs.DataMember = "SINHVIEN";
        }
        private double TongDiem(string MSV)
        {
            double kq = 0;
            object td = ds.Tables["KETQUA"].Compute("sum(Diem)", "MaSV='" + MSV + "'");
            if (td == DBNull.Value) kq = 0;
            else kq = Convert.ToDouble(td);
            return kq;
        }
        private void Moc_Noi_Quan_He()
        {
            //Tạo quan hệ giữa tblKhoa và tblSinhVien
            ds.Relations.Add("FK_KH_SV", ds.Tables["KHOA"].Columns["MaKH"], ds.Tables["SINHVIEN"].Columns["MaKH"], true);
            //Tạo quan hệ giữa tblSinhVien và tblKetQua
            ds.Relations.Add("FK_SV_KQ", ds.Tables["SINHVIEN"].Columns["MaSV"], ds.Tables["KETQUA"].Columns["MaSV"], true);
            //Loại bỏ Cacase Delete trong các quan hệ
            ds.Relations["FK_KH_SV"].ChildKeyConstraint.DeleteRule = Rule.None;
            ds.Relations["FK_SV_KQ"].ChildKeyConstraint.DeleteRule = Rule.None;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaSV.ReadOnly = false;
            //new 
            bs.AddNew();
            cboMaKH.SelectedIndex = 0;
            dtpNgaySinh.Value = new DateTime(2006, 1, 1);
            txtMaSV.Focus();
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            bs.MovePrevious();
        }

        private void btnSau_Click(object sender, EventArgs e)
        {
            bs.MoveNext();
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

        private void btnHuy_Click(object sender, EventArgs e)
        {
            //Xac dinh dong can huy Su dung ham Find
            DataRow rsv = (bs.Current as DataRowView).Row;
            //Can kien tra Neu rsv ton tai trong tblKetQua thi khong xoa. Nguoc lai thi cho xoa
            //Su dung ham getChilRow de kiem tra nhung dong lien quan co ton tai hay khong. Gia tri tra ve la mang
            DataRow[] mangDongLienQuan = rsv.GetChildRows("FK_SINHVIEN_KETQUA");
            if (mangDongLienQuan.Length > 0)//co ton tai nhung dong lien quan trong tblKetQua
                MessageBox.Show("Khong xoa duoc Sv vi da co ket qua thi");
            else
            {
                DialogResult tl;
                tl = MessageBox.Show("Xoa sinh vien nay khong?", "Can than", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (tl == DialogResult.Yes)
                {
                    //Xoa trong DataTable
                    bs.RemoveCurrent();
                    //Xoa trong CSDL
                    int n = adpSinhVien.Update(ds, "SINHVIEN");
                    if (n > 0)
                        MessageBox.Show("Xoa sinh vien thanh cong");
                }
            }
        }

        private void Doc_Du_Lieu()
        {
            adpKhoa.FillSchema(ds, SchemaType.Source, "KHOA");
            adpKhoa.Fill(ds, "KHOA");
            adpSinhVien.FillSchema(ds, SchemaType.Source, "SINHVIEN");
            adpSinhVien.Fill(ds, "SINHVIEN");
            adpKetQua.FillSchema(ds, SchemaType.Source, "KETQUA");
            adpKetQua.Fill(ds, "KETQUA");
        }

        private void Khoi_Tao_Doi_Tuong()
        {
            adpKhoa = new OleDbDataAdapter("select * from Khoa", strcon);
            adpSinhVien = new OleDbDataAdapter("select * from SinhVien", strcon);
            adpKetQua = new OleDbDataAdapter("select * from KetQua", strcon);

            cmdSinhVien = new OleDbCommandBuilder(adpSinhVien);
        }
    }
}
