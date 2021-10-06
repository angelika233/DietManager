using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace DietManager
{

    public partial class AddToDB : Window
    {
        public AddToDB()
        {
            InitializeComponent();
        }



        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd;
            string workingDirectory = Environment.CurrentDirectory;
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\DietManager\DietManagerDB.mdf;Integrated Security=True");

            con.Open();
            try
            {
                cmd = new SqlCommand(
                    "Insert into Products (Product,Fat,Carbs,Protein, Calories) VALUES (@Product, @Fat, @Carbs, @Protein, @Calories)",
                    con);
                cmd.Parameters.Add("@Product", TextBoxName.Text);
                cmd.Parameters.Add("@Fat", Math.Round(float.Parse(TextBoxFat.Text), 2));
                cmd.Parameters.Add("@Carbs", Math.Round(float.Parse(TextBoxCarbs.Text), 2));
                cmd.Parameters.Add("@Protein", Math.Round(float.Parse(TextBoxProtein.Text), 2));
                cmd.Parameters.Add("@Calories", Math.Round(float.Parse(TextBoxkcal.Text), 2));

                int i = cmd.ExecuteNonQuery();
                if (i != 0)
                {
                    MessageBox.Show("Product was added to database");
                }

                con.Close();
            }
            catch
            {
                MessageBox.Show("Enter correct data");
            }
        }

        private void Finishadding_Click(object sender, RoutedEventArgs e)
        {
            Close();
            MainWindow newWin = new MainWindow();
            newWin.Show();
            MessageBox.Show("Restart application after adding new products");
        }
    }
}
