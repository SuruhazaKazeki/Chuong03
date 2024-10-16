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
namespace BT05_DGV4__SQL
{
    public partial class Form1 : Form
    {
        string strcon = @"server=KAZEKI\SQLEXPRESS; Database = QLSV_PhamDucTrong;integrated security = true";
        DataSet ds = new DataSet();
        SqlDataAdapter adpMonHoc, adpKetQua;
        SqlCommandBuilder cmdMonHoc;
        BindingSource bs = new BindingSource();
        int stt = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Khoi_Tao_Doi_Tuong();
            Doc_Du_Lieu();
            Moc_Noi_Quan_He();
            Khoi_Tao_BindingSource();
            dgvMonHoc.DataSource = bs;
            dgvMonHoc.Columns[3].Visible = false;
            //Create_cbbKhoa();
            Control_Link();
            //Control_Link_Phai();
            //Liên kết control bindingNavigator
            bdnMonHoc.BindingSource = bs;
        }

        private void Control_Link()
        {
            txtMaMH.DataBindings.Add("Text", bs, "MaMH", true);
            txtTenMH.DataBindings.Add("Text", bs, "TenMH", true);
            txtSoTiet.DataBindings.Add("Text", bs, "SoTiet", true);
        }

        private void Moc_Noi_Quan_He()
        {
            ds.Relations.Add("FK_MH_KQ", ds.Tables["MONHOC"].Columns["MaMH"], ds.Tables["KETQUA"].Columns["MaMH"], true);
            ds.Relations["FK_MH_KQ"].ChildKeyConstraint.DeleteRule = Rule.None;
        }

        private void Khoi_Tao_BindingSource()
        {
            bs.DataSource = ds;
            bs.DataMember = "MONHOC";
        }

        private void Doc_Du_Lieu()
        {

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

        private void Khoi_Tao_Doi_Tuong()
        {
            adpMonHoc = new SqlDataAdapter("select * from MonHoc", strcon);
            adpKetQua = new SqlDataAdapter("select * from KetQua", strcon);
            cmdMonHoc = new SqlCommandBuilder(adpMonHoc);
        }
    }
}
