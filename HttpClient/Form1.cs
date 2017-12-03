using MyLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HttpClient
{
    public partial class Form1 : Form
    {
        HttpService httpService;
        List<Employee> employees;
        public Form1()
        {
            InitializeComponent();
            httpService = new HttpService();
        }

        private void buttonGetById_Click(object sender, EventArgs e)
        {
            employees = httpService.GetById(Int32.Parse(textBoxId.Text));
            SetDatagridView();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void buttonAll_Click(object sender, EventArgs e)
        {
            employees = httpService.GetAll();
            SetDatagridView();
        }

        public void SetDatagridView()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("First name");
            dt.Columns.Add("Last name");
            dt.Columns.Add("Department");
            dt.Columns.Add("Salary");
            foreach (var emp in employees)
            {
                dt.Rows.Add(emp.id, emp.firstName, emp.lastName, emp.department, emp.salary);
            }
            dataGridView.DataSource = null;
            dataGridView.DataSource = dt;
        }

        private void buttonNewEmployee_Click(object sender, EventArgs e)
        {
            AddModifyForm addForm = new AddModifyForm();
            addForm.Show();
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            AddModifyForm addForm = new AddModifyForm(employees[e.RowIndex]);
            addForm.Show();
        }
    }
}
