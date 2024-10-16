using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BT02_DataGirdView_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            themCotVaoLuoi();
            themDongVaoLuoi();
            themDoDaiVaoLuoi();
        }

        private void themDongVaoLuoi()
        {
            dgvMonHoc.Rows.Add("01", "Cơ sở dữ liệu", 90, "BB");

            dgvMonHoc.Rows.Add("82", "Tin Học", 75, "C"); 
            dgvMonHoc.Rows.Add("83", "Lập trình Windows", 105, "BB");

            dgvMonHoc.Rows.Add("84", "Lập trình CSDL cơ bản", 75, "88");

            dgvMonHoc.Rows.Add("85", "Java cơ bản", 75, "тс");
        }

        private void themCotVaoLuoi()
        {
            dgvMonHoc.Columns.Add("colMaMH", "Mã MH");
            dgvMonHoc.Columns.Add("colTenMH", "Tên MH");
            dgvMonHoc.Columns.Add("colSoTiet", "Số Tiết");
            dgvMonHoc.Columns.Add("colLoaiMH", "Loại MH");
        }

        private void dgvMonHoc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
