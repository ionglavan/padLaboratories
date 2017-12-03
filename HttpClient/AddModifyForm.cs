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
    public partial class AddModifyForm : Form
    {
        bool action = false;//false for new, true for modify
        int modifyId;
        public AddModifyForm()
        {
            InitializeComponent();
            labelAction.Text = "Add new Employee";
            action = false;
        }

        public AddModifyForm(Employee employee)
        {
            InitializeComponent();
            labelAction.Text = "Modify employee with id " + employee.id;
            textBoxFirstName.Text = employee.firstName;
            textBoxLastName.Text = employee.lastName;
            textBoxDepartment.Text = employee.department;
            textBoxSalary.Text = employee.salary + "";
            action = true;
            modifyId = employee.id;
        }

        private void AddModifyForm_Load(object sender, EventArgs e)
        {

        }

        private void buttonAddModify_Click(object sender, EventArgs e)
        {
            HttpService httpService = new HttpService();
            Employee employee = new Employee();
            employee.firstName = textBoxFirstName.Text;
            employee.lastName = textBoxLastName.Text;
            employee.department = textBoxDepartment.Text;
            employee.salary = float.Parse(textBoxSalary.Text);
            if (action)
            {
                employee.id = modifyId;
                httpService.ModifyEmployee(employee);
            }
            else
            {
                employee.id = -1;
                httpService.AddEmployee(employee);
            }
            this.Close();
        }
    }
}
