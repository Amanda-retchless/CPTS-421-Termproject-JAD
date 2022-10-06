using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql; /* for db connectivity */

namespace JAD_app
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CheckinWindow : Window
    {
        private string business_id = "";
        private string user_id = "";
        public class Checkin
        {
            public string month { get; set; }
            public string checkin { get; set; }
        };

        public CheckinWindow(string business_id, string user_id)
        {
            InitializeComponent();
            this.business_id = String.Copy(business_id);
            this.user_id = String.Copy(user_id);
            //addTips();
            addCheckins();
        }
        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = milestone2db; password = Kangaroo!19"; // necessary evil
        }

        private void addCheckins()
        {
            CheckinGrid.Items.Clear();
            string sqlstr = "SELECT month, COUNT(day_time) FROM Checkins WHERE business_id = \'" + business_id + "\' GROUP BY month;";
            executeQuery(sqlstr, addCheckinToGrid);

        }

        private void addCheckinToGrid(NpgsqlDataReader R)
        {
            CheckinGrid.Items.Add(new Checkin
            {
                month = R.GetString(0),
                checkin = R.GetInt32(1).ToString()
            });
        }
        private void executeQuery(string sqlstr, Action<NpgsqlDataReader> myf)
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sqlstr;
                    try
                    {
                        var reader = cmd.ExecuteReader(); // execute the cmd
                        while (reader.Read())             // read each row of returned list
                        {
                            myf(reader);                  // perform action on list row
                        }
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        System.Windows.MessageBox.Show($"SQL Error - {ex.Message.ToString()}");
                    }
                    finally // regardless of error or not
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void checkinButton_Click(object sender, RoutedEventArgs e)
        {
            if (DateTime.Today.Month < 10)
            {
                string sqlstr = "INSERT INTO Checkins VALUES (\'" + business_id + "\',\'" + DateTime.Today.Year + "\',\'0" + DateTime.Today.Month + "\',\'" + DateTime.Today.Day + "\',\'" + DateTime.Today.TimeOfDay + "\');";
                executeQuery(sqlstr, emptyFunct);
            }
            else
            {
                string sqlstr = "INSERT INTO Checkins VALUES (\'" + business_id + "\',\'" + DateTime.Today.Year + "\',\'" + DateTime.Today.Month + "\',\'" + DateTime.Today.Day + "\',\'" + DateTime.Today.TimeOfDay + "\');";
                executeQuery(sqlstr, emptyFunct);
            }

        }

        private void emptyFunct(NpgsqlDataReader R)
        {
            addCheckins();
        }
    }
};
