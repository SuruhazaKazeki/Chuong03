using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BT04
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            themDongVaoLuoi();
            DataGridViewRow r = dgvMonHoc.Rows[2];
            r.Selected = true;
            GanDuLieu(r);
        }

        private void GanDuLieu(DataGridViewRow r)
        {
            txtMaMH.Text = r.Cells[0].Value.ToString();
            txtTenMH.Text = r.Cells[1].Value.ToString();
            txtSoTiet.Text = r.Cells[2].Value.ToString();
        }

        private void themDongVaoLuoi()
        {
            dgvMonHoc.Rows.Add("01", "Cơ sở dữ liệu", 90, "BB");

            dgvMonHoc.Rows.Add("82", "Tin Học", 75, "C");
            dgvMonHoc.Rows.Add("83", "Lập trình Windows", 105, "BB");

            dgvMonHoc.Rows.Add("84", "Lập trình CSDL cơ bản", 75, "88");

            dgvMonHoc.Rows.Add("85", "Java cơ bản", 75, "тс");
        }

        private void dgvMonHoc_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //sự kiện sảy ra khi có 1 ô trên dòng nào đó được click
            // để lấy thông tin của dòng chứa ô được click => e.RowIndex
            DataGridViewRow r = dgvMonHoc.Rows[e.RowIndex];
            GanDuLieu(r);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaMH.ReadOnly = false;
            foreach (Control ctl in this.Controls)
                if (ctl is TextBox)
                    (ctl as TextBox).Clear();
            txtMaMH.Focus();
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {
            GanDuLieu(dgvMonHoc.SelectedRows[0]);
            txtMaMH.ReadOnly = true;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult dl;
            dl = MessageBox.Show("Bạn có muốn hủy môn học này không(Y/N)?", "Hủy môn học", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dl == DialogResult.Yes)
            {
                //lấy dòng cần hủy từ dòng thứ 0 trong tập hợp các dòng được selectedRows[0]
                DataGridViewRow rhuy = dgvMonHoc.SelectedRows[0];
                // hủy dòng trong lưới
                dgvMonHoc.Rows.Remove(rhuy);
                // chọn lại dòng đầu tiên
                dgvMonHoc.Rows[0].Selected = true;
                GanDuLieu(dgvMonHoc.Rows[0]);
            }
        }

        private void btnGhi_Click(object sender, EventArgs e)
        {
            if(txtMaMH.ReadOnly == true)// ghi khi sửa
            {
                DataGridViewRow rsua = dgvMonHoc.SelectedRows[0];
                //sửa thông tin của dòng theo thông tin của các điều khiển
                rsua.Cells[1].Value = txtMaMH.Text;
                rsua.Cells[2].Value = txtSoTiet.Text;
            }else// ghi khi  thêm mới
            {
                int stt = dgvMonHoc.Rows.Add(txtMaMH.Text, txtTenMH.Text, txtSoTiet.Text);
                //stt trả về chỉ số của dòng mới thêm vào
                dgvMonHoc.Rows[stt].Selected = true;
                txtMaMH.ReadOnly = true;
            }
        }
    }
}
