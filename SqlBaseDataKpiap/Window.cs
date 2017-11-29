using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlBaseDataKpiap
{
    public partial class Window : Form
    {
        SqlConnection connection;
        SqlDataReader ReaderSql;
        SqlCommand command;
        public Window()
        {
            InitializeComponent();
        }

        private void Window_Load(object sender, EventArgs e)
        {
            connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\User\Documents\Visual Studio 2017\Projects\SqlBaseDataKpiap\SqlBaseDataKpiap\DataBase.mdf;Integrated Security=True");
            // типа присоеденились к базе данных через её строку подключения

            connection.Open();
            command = new SqlCommand("SELECT * FROM [Books]", connection);
            try
            {
                ReaderSql = command.ExecuteReader();
                while (ReaderSql.Read())
                    Data_Table.Rows.Add(ReaderSql["BookName"], ReaderSql["YearPubl"], ReaderSql["Genre"], ReaderSql["Mark"]);
            }
            catch (Exception exp)
            {
                MessageBox.Show("А че-то не так с БД, ща скажем почему... о, во причина: " + exp.Source.ToString());
            }
            if (ReaderSql != null)
                ReaderSql.Close();

        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.State != ConnectionState.Closed)
                connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошебка");
                return;
            }
            command = new SqlCommand("INSERT INTO [Books] (BookName, YearPubl, Genre, Mark)VALUES(@BookName, @YearPubl, @Genre, @Mark)", connection);

            command.Parameters.AddWithValue("BookName", textBox1.Text);
            command.Parameters.AddWithValue("YearPubl", textBox2.Text);
            command.Parameters.AddWithValue("Genre", textBox3.Text);
            command.Parameters.AddWithValue("Mark", textBox4.Text);
            command.ExecuteNonQuery();
            Data_Table.Rows.Add(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
        }

        private void Data_Table_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            command = new SqlCommand("DELETE FROM [Books] WHERE [ID] = @ID", connection);
            command.Parameters.AddWithValue("ID", e.Row.Index + 1);
            command.ExecuteNonQuery();
        }

        private void Data_Table_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string polySettings = default;
            string poly = default;
            switch (e.ColumnIndex)
            {
                case 0:
                    polySettings = "[BookName] = @BookName";
                    poly = "BookName";
                    break;
                case 1:
                    polySettings = "[YearPubl] = @YearPubl";
                    poly = "YearPubl";
                    break;
                case 2:
                    polySettings = "[Genre] = @Genre";
                    poly = "Genre";
                    break;
                case 3:
                    polySettings = "[Mark] = @Mark";
                    poly = "Mark";
                    break;
            }
            command = new SqlCommand("UPDATE [Books] SET " + polySettings + " WHERE [ID] = @ID", connection);
            command.Parameters.AddWithValue(poly, Data_Table.CurrentCell.Value.ToString());
            command.Parameters.AddWithValue("ID", Data_Table.CurrentRow.Index + 1);
            command.ExecuteNonQuery();
        }
    }
}
