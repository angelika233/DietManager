using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DietManager
{
    public partial class MainWindow : Window
    {
        private double cal;
        private float sumkcal = 0;
        private float sumprotein = 0;
        private float sumcarbs = 0;
        private float sumfat = 0;

        public double BMI(double w, double h)
        {
            return w / ((h / 100) * (h / 100));
        }

        public double BMR(double w, double h, int a)
        {
            double bmr_res;
            if (RadioF.IsChecked == true)
            {
                bmr_res = 655 + (9.6 * w) + (1.8 * h) - (4.7 * a);
            }
            else
            {
                bmr_res = 66 + (13.7 * w) + (5 * h) - (6.8 * a);
            }

            if (RadioSed.IsChecked == true)
            {
                return bmr_res * 1.2;
            }
            else if (RadioMod.IsChecked == true)
            {
                return bmr_res * 1.55;
            }
            else if (RadioVery.IsChecked == true)
            {
                return bmr_res * 1.725;
            }
            else if (RadioLight.IsChecked == true)
            {
                return bmr_res * 1.375;
            }
            else
            {
                return bmr_res * 1.9;
            }

        }

        public double Calories(double w, double h, int a)
        {
            if (RadioGain.IsChecked == true)
            {
                return BMR(w, h, a) + 200;
            }
            else if (RadioLoss.IsChecked == true)
            {
                return BMR(w, h, a) - 200;
            }
            else
            {
                return BMR(w, h, a);
            }
        }

        public string BmiRanges(double bmi)
        {
            if (bmi < 18.5)
            {
                return "you are underweight. ";
            }
            if (bmi < 24.9)
            {
                return "you are normal weight. ";
            }
            if (bmi < 29.9)
            {
                return "you are overweight. ";
            }
            else
            {
                return " you are obese. ";
            }
        }

        public string Remaining(float eaten, double bmr)
        {

            if (bmr == 0)
            {
                return " Calculate your bmr to see more info.";
            }
            else if (eaten > bmr)
            {
                return " It's " + (eaten - bmr).ToString("0,00") + " Calories over your daily limit.";
            }
            else
            {
                return " You have " + (bmr - eaten).ToString("0,00") + " Calories remaining to reach your daily goal.";
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            DietManager.ProductsDB productsDB = ((DietManager.ProductsDB)(this.FindResource("productsDB")));
            DietManager.ProductsDBTableAdapters.ProductsTableAdapter productsDBProductsTableAdapter = new DietManager.ProductsDBTableAdapters.ProductsTableAdapter();
            productsDBProductsTableAdapter.Fill(productsDB.Products);
            System.Windows.Data.CollectionViewSource productsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productsViewSource")));
            productsViewSource.View.MoveCurrentToFirst();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double bmi_res = BMI(Double.Parse(Textbox_weight.Text), Double.Parse(Textbox_height.Text));
                cal = Calories(Double.Parse(Textbox_weight.Text), Double.Parse(Textbox_height.Text),
                    int.Parse(Textbox_Age.Text));
                Results.Text = "Your BMI: " + String.Format("{0:f}", bmi_res) + " This means that " +
                               BmiRanges(bmi_res) + " To reach your goal you should consume about " +
                               String.Format("{0:f}", cal) + " Calories. ";
                if (Double.Parse(Textbox_weight.Text) < 35 || Double.Parse(Textbox_weight.Text) > 200)
                {
                    MessageBox.Show("Make sure you entered correct weight");
                }
                if (Double.Parse(Textbox_height.Text) < 145 || Double.Parse(Textbox_weight.Text) > 210)
                {
                    MessageBox.Show("Make sure you entered correct height");
                }
                if (Double.Parse(Textbox_Age.Text) < 11 || Double.Parse(Textbox_Age.Text) > 100)
                {
                    MessageBox.Show("Age should be between 12 and 99");
                }
            }
            catch
            {
                Results.Text = "Enter correct data";
            }
        }


        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            AddToDB window = new AddToDB();
            window.Show();
            Hide();
        }

        private void AddDiary_OnClick(object sender, RoutedEventArgs e)
        {
            List<object> row = new List<object>();
            SqlCommand cmd;
            string workingDirectory = Environment.CurrentDirectory;
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\DietManager\DietManagerDB.mdf;Integrated Security=True");

            con.Open();
            cmd = new SqlCommand("Select * FROM products", con);
            int i = cmd.ExecuteNonQuery();
            try
            {
                using (SqlDataReader dataRead = cmd.ExecuteReader())
                {
                    if (dataRead != null)
                    {
                        while (dataRead.Read())
                        {
                            if (productComboBox.Text == dataRead["Product"].ToString())
                            {
                                float kcalForGrams = int.Parse(TextBoxGrams.Text) *
                                    float.Parse(dataRead["Calories"].ToString()) / 100;
                                float fatForGrams = int.Parse(TextBoxGrams.Text) *
                                    float.Parse(dataRead["Fat"].ToString()) / 100;
                                float carbsForGrams = int.Parse(TextBoxGrams.Text) *
                                    float.Parse(dataRead["Carbs"].ToString()) / 100;
                                float proteinForGrams = int.Parse(TextBoxGrams.Text) *
                                    float.Parse(dataRead["Protein"].ToString()) / 100;
                                row.Add(dataRead["Product"].ToString());
                                row.Add(int.Parse(TextBoxGrams.Text));
                                row.Add(proteinForGrams);
                                row.Add(fatForGrams);
                                row.Add(carbsForGrams);
                                row.Add(kcalForGrams);

                                sumkcal = sumkcal + kcalForGrams;
                                sumcarbs = sumcarbs + carbsForGrams;
                                sumfat = sumfat + fatForGrams;
                                sumprotein = sumprotein + proteinForGrams;
                                DailyFoodListView.Items.Add(row);
                            }

                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Enter correct data");
            }
        }

        private void Final_Click(object sender, RoutedEventArgs e)
        {
            SummaryTextBox.Text = "You have consumed " + sumkcal.ToString("") + " Calories. " + Remaining(sumkcal, cal) + "Your Macros: " + sumcarbs + " grams of carbs, " + sumfat + " grams of fat and " + sumprotein + " grams of protein.";
        }

    }
}
