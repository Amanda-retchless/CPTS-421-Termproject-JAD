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
    public partial class TipsWindow : Window
    {
        private string business_id = "";
        private string user_id = "";
        public class Tip
        {
            public string user_id { get; set; }
            public string user_name { get; set; }
            public string business_id { get; set; }
            public string date { get; set; }
            public string tiptext { get; set; }
            public int likes { get; set; }
        };

        public class Friend
        {
            public string user_name { get; set; }

            public string date { get; set; }

            public string tiptext { get; set; }
        };

        public TipsWindow(string business_id, string user_id)
        {
            InitializeComponent();
            this.business_id = String.Copy(business_id);
            this.user_id = String.Copy(user_id);
            addTips();
        }
        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = milestone2db; password = Kangaroo!19"; // necessary evil
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


        private DataGridTextColumn addColumnToGrid(DataGrid grid, string bind_name, string header = "")
        {
            // each column is an object
            DataGridTextColumn col = new DataGridTextColumn();
            col.Binding = new Binding(bind_name);  // bind to name field of the business
            // set all properties
            col.Header = header;
            col.Width = DataGridLength.Auto;
            // add column to grid
            grid.Columns.Add(col);
            return col; // in case user wants to change the default settings
        }

        private void addTipGridRow(NpgsqlDataReader R)
        {
            string date = R.GetDate(3).ToString();
            string time = R.GetTimeSpan(4).ToString();
            tipGrid.Items.Add(new Tip()
            {
                user_id = R.GetString(0),
                business_id = R.GetString(2),
                date = date + " " + time,
                tiptext = R.GetString(5),
                likes = R.GetInt32(6),
                user_name = R.GetString(1)
            });
        }

        private void addFriendsGridRow(NpgsqlDataReader R)
        {
            //string date = R.GetDate(2).ToString();

            //string time = R.GetTimeSpan(4).ToString();
            Friend f = new Friend();
            f.user_name = R.GetString(0);
            f.date = R.GetDate(1).ToString();
            f.tiptext = R.GetString(2);
            friendTipGrid.Items.Add(f);
        }

        private void addTips()
        {
            /* initialize tip grid */
            addColumnToGrid(tipGrid, "date", "Date");
            addColumnToGrid(tipGrid, "user_name", "User Name");
            addColumnToGrid(tipGrid, "likes", "Likes");
            addColumnToGrid(tipGrid, "tiptext", "Text");

            /* initialize friend tip grid */
            addColumnToGrid(friendTipGrid, "user_name", "User Name");
            addColumnToGrid(friendTipGrid, "date", "Date");
            addColumnToGrid(friendTipGrid, "tiptext", "Text");

            /* populate tip grid */

            // query: get every tip
            string attr = "tips.user_id as user_id, user_name, business.business_id as business_id, tipdate, tiptime, tiptext, likes";
            string str = $"SELECT {attr} FROM tips INNER JOIN business ON tips.business_id = business.business_id INNER JOIN users ON tips.user_id = users.user_id WHERE business.business_id = '{this.business_id}';";
            executeQuery(str, addTipGridRow);

            /* populate friend tip grid */
            string str1 = $"SELECT {attr} FROM tips INNER JOIN business ON tips.business_id = business.business_id INNER JOIN users ON tips.user_id = users.user_id WHERE business.business_id = '{this.business_id}'";
            string str2 = "SELECT user_id,user_name,average_stars,yelping_since FROM (SELECT friend_id FROM Friends WHERE user_id = \'" + user_id + "\') as F, Users WHERE Users.user_id = F.friend_id";
            string sqlstr = $"Select A.user_name, A.tipDate, A.tipText From (" + str1 + ") as A, (" + str2 + ") as T WHERE T.user_id = A.user_id;";
            executeQuery(sqlstr, addFriendsGridRow);


            // query: get every tip of users that are this user's friend
            str = "";
            //executeQuery(str, addTipGridRow);
        }

        private void addTipBtn_Click(object sender, RoutedEventArgs e)
        {
            /* user adds a new tip for a selected business. the trigger implemented in task milestone2.5.a. should update numTips value for the business and tipCount value for the user after the new tip is added */

            // check that there's actually text in the textbox
            if (tipTextBox.Text.CompareTo("") == 0)
            {
                return;
            }
            // save the tip
            DateTime now = DateTime.Now;
            // TODO: replace postgres username placeholder with actual values
            string str = $"INSERT INTO tips VALUES ('{this.user_id}', '{this.business_id}', '{now.ToLongDateString()}', '{now.ToLongTimeString()}','{tipTextBox.Text}', 0);"; 
            executeQuery(str, null);
            // clean up for next tip
            tipTextBox.Clear();

            // add new tip
            tipGrid.Items.Clear();
            friendTipGrid.Items.Clear();
            addTips();
        }

        private void likeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tip = (Tip)tipGrid.SelectedItem;
                Console.WriteLine(tip.business_id);
                tipGrid.Items.Clear();
                string str = $"UPDATE Tips SET likes = likes + 1 WHERE business_id = '{tip.business_id}' and user_id = '{tip.user_id}' and tipDate = '{tip.date}';";
                executeQuery(str, null);
                addTips();
            }
            catch
            {
                Console.WriteLine("Error");
            }
        }
    }
}

