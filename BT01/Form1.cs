using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BT01
{
    public partial class Form1 : Form
    {
        string strcon = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\..\..\DATA\QLSV.mdb";
        DataSet ds = new DataSet();
        OleDbDataAdapter adpSinhVien, adpKhoa, adpMonHoc, adpKetQua;
        OleDbCommandBuilder cmdMonHoc;
        BindingSource bs = new BindingSource();
        public Form1()
        {
            InitializeComponent();
            bs.CurrentChanged += Bs_CurrentChanged;
        }
        private void Bs_CurrentChanged(object sender, EventArgs e)
        {
            lblSTT.Text = bs.Position + 1 + "/" + bs.Count;
            txtMaxDiem.Text = MaxDiem(txtMaMH.Text).ToString();
            txtTSSV.Text = TongSV(txtMaMH.Text).ToString();
        }

        private double TongSV(string mmh)
        {
            double kq = 0;
            object td = ds.Tables["KETQUA"].Compute("count(MaSV)", "MaMH='" + mmh + "'");
            if (td == DBNull.Value)
            {
                kq = 0;
            }
            else 
            { 
                kq = Convert.ToDouble(td); 
            }
            return kq;
        }
        private double MaxDiem(string mmh)
        {
            double kq = 0;
            object td = ds.Tables["KETQUA"].Compute("max(Diem)", "MaMH='" + mmh + "'");
            if (td == DBNull.Value)
            {
                kq = 0;
            }
            else
            {
                kq = Convert.ToDouble(td);
            }
            return kq;
        }
        private void btnSau_Click(object sender, EventArgs e)
        {
            bs.MoveNext();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Khoi_Tao_Doi_Tuong();
            Doc_Du_Lieu();
            Moc_Noi_Quan_He();
            Khoi_Tao_BindingSource();
            Control_Link();
        }
        private void Control_Link()
        {
            //chú ý các điều kiện và tính toán
            foreach (Control ctl in this.Controls)
                if (ctl is TextBox && ctl.Name != "txtMaxDiem" && ctl.Name != "txtTSSV" && ctl.Name!="txtLoaiMH")
                    ctl.DataBindings.Add("Text", bs, ctl.Name.Substring(3), true);
            Binding bdLoaiMH = new Binding("text", bs, "LoaiMH", true);
            bdLoaiMH.Format += BdLoaiMH_Format; ;
            bdLoaiMH.Parse += BdLoaiMH_Parse; ;
            txtLoaiMH.DataBindings.Add(bdLoaiMH);

        }

        private void BdLoaiMH_Parse(object sender, ConvertEventArgs e)
        {
            if (e.Value == null) return;
            e.Value = e.Value.ToString().ToUpper() == "BatBuoc" ? true : false;
        }

        private void BdLoaiMH_Format(object sender, ConvertEventArgs e)
        {
            if (e.Value == DBNull.Value || e.Value == null) return;
            e.Value = (Boolean)e.Value ? "BatBuoc" : "Tuy chon";
        }

        private void Khoi_Tao_BindingSource()
        {
            bs.DataSource = ds;
            bs.DataMember = "MONHOC";
        }

        private void Moc_Noi_Quan_He()
        {
            
            ds.Relations.Add("FK_SV_KQ", ds.Tables["SINHVIEN"].Columns["MaSV"], ds.Tables["KETQUA"].Columns["MaSV"], true);
            ds.Relations.Add("FK_MH_KQ", ds.Tables["MONHOC"].Columns["MaMH"], ds.Tables["KETQUA"].Columns["MaMH"], true);
            ds.Relations["FK_MH_KQ"].ChildKeyConstraint.DeleteRule = Rule.None;
            ds.Relations["FK_SV_KQ"].ChildKeyConstraint.DeleteRule = Rule.None;
        }

        private void btnTruoc_Click(object sender, EventArgs e)
        {
            bs.MovePrevious();
        }

        private void btnDau_Click(object sender, EventArgs e)
        {
            bs.MoveFirst();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaMH.ReadOnly = false;
            //new 
            bs.AddNew();
            txtMaMH.Focus();
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            if (txtMaMH.ReadOnly == false)
            {
                
                DataRow r = ds.Tables["MONHOC"].Rows.Find(txtMaMH.Text);
                if (r != null)
                {
                    MessageBox.Show("MaMH bị trùng", "Trùng khóa chính");
                    txtMaMH.Focus();
                    return;
                }
            }
            
            bs.EndEdit();
            int n = adpMonHoc.Update(ds, "MONHOC");
            if (n > 0)
                MessageBox.Show("Update Thành Công");
            txtMaMH.ReadOnly = true;
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {
            bs.CancelEdit();
            txtMaMH.ReadOnly = true;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
                
            DataRow rmh = (bs.Current as DataRowView).Row;
            DataRow[] mangDongLienQuan = rmh.GetChildRows("FK_MH_KQ");
            if (mangDongLienQuan.Length > 0)
                MessageBox.Show("Không xóa được vì MaMH có trong kết quả thi.","Thông báo",MessageBoxButtons.OK);
            else
            {
                DialogResult tl;
                tl = MessageBox.Show("Xoa mon hoc nay khong?", "Can than", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (tl == DialogResult.Yes)
                {
                    
                    bs.RemoveCurrent();
                    
                    int n = adpMonHoc.Update(ds, "MONHOC");
                    if (n > 0)
                        MessageBox.Show("Xóa Môn Học thành công.");
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult tl = MessageBox.Show("Bạn muốn thoát chương trình này không?", "Thông báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(tl == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnCuoi_Click(object sender, EventArgs e)
        {
            bs.MoveLast();
        }

        private void Doc_Du_Lieu()
        {
            adpKhoa.FillSchema(ds, SchemaType.Source, "KHOA");
            adpKhoa.Fill(ds, "KHOA");
            adpMonHoc.FillSchema(ds, SchemaType.Source, "MONHOC");
            adpMonHoc.Fill(ds, "MONHOC");
            adpSinhVien.FillSchema(ds, SchemaType.Source, "SINHVIEN");
            adpSinhVien.Fill(ds, "SINHVIEN");
            adpKetQua.FillSchema(ds, SchemaType.Source, "KETQUA");
            adpKetQua.Fill(ds, "KETQUA");
        }
        private void Khoi_Tao_Doi_Tuong()
        {
            adpKhoa = new OleDbDataAdapter("select * from Khoa", strcon);
            adpMonHoc = new OleDbDataAdapter("select * from MonHoc", strcon);
            adpSinhVien = new OleDbDataAdapter("select * from SinhVien", strcon);
            adpKetQua = new OleDbDataAdapter("select * from KetQua", strcon);

            cmdMonHoc = new OleDbCommandBuilder(adpMonHoc);
        }
    }
}
