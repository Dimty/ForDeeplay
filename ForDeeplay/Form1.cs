using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ForDeeplay
{
    public partial class Form1 : Form
    {
        private static string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Дмитрий\source\repos\ForDeeplay\ForDeeplay\Database.mdf;Integrated Security=True";
        private static SqlConnection sqlConnection;
        private static SqlCommandBuilder sqlCommand;
        private SqlDataAdapter sqladapter;
        private DataSet dataSet = new DataSet();
        private bool IsRowNew = false;
        private bool IsRowChange = false;
        string choiсe = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(connStr);
            sqlConnection.Open();

            sqladapter = new SqlDataAdapter("SELECT *, 'Delete' AS [Action] FROM [Table]", connStr);
            dataSet = new DataSet();
            sqladapter.Fill(dataSet,"Table");
            dataGridView1.DataSource=dataSet.Tables["Table"];

            sqlCommand = new SqlCommandBuilder(sqladapter);
            sqlCommand.GetDeleteCommand();
            sqlCommand.GetInsertCommand();
            sqlCommand.GetUpdateCommand();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!IsRowNew)
            {
                if (!IsRowChange)
                {
                    dataGridView1[6, e.RowIndex].Value="Update";
                    IsRowChange=true;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==6)
            {
                string command = dataGridView1[6,e.RowIndex].Value.ToString();
                switch (command)
                {
                    #region Delete

                    
                    case "Delete":
                        if (MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.Yes)
                        {
                            int indexDel = e.RowIndex;
                            dataGridView1.Rows.RemoveAt(indexDel);
                            dataSet.Tables["Table"].Rows[indexDel].Delete();
                            sqladapter.Update(dataSet,"Table");
                        }
                        break;
                    #endregion
                    #region Update
                    case "Update":
                        
                        sqladapter.Update(dataSet, "Table");

                        dataGridView1[6, e.RowIndex].Value="Delete";
                        IsRowChange=false;
                        break;
                    #endregion
                    #region Insert

                    case "Insert":
                        int indexIns = dataGridView1.Rows.Count-2;
                        DataRow newRowIns = dataSet.Tables["Table"].NewRow();
                        for (int i = 1; i < dataGridView1.Columns.Count-1; i++)
                        {
                            newRowIns[i] = dataGridView1.Rows[indexIns].Cells[i].Value;
                        }
                        dataSet.Tables["Table"].Rows.Add(newRowIns);
                        dataSet.Tables["Table"].Rows.RemoveAt(dataSet.Tables["Table"].Rows.Count-1);

                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count-2);
                        dataGridView1[6, e.RowIndex].Value="Delete";

                        sqladapter.Update(dataSet, "Table");

                        IsRowNew=false;
                        break;
                    #endregion
                    default:
                        break;
                }
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            if(!IsRowNew)
            {
                IsRowNew=true;
                dataGridView1[6, dataGridView1.Rows.Count-2].Value="Insert";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            choiсe =  comboBox1.SelectedItem.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (choiсe!=null)
            {
                string sort = $"{choiсe} LIKE '%{textBox1.Text}%'";
                (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = sort;
            }
        }
    }
}
