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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql; /* for db connectivity */
using System.Diagnostics; /*DEBUG*/

namespace JAD_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Business
        {
            public string business_id { get; set; }
            public string business_name { get; set;}
            public string business_address { get; set;}
            public string business_city { get; set;}
            public string business_state { get; set;}
            public int zipcode { get; set;}
            public double latitude { get; set;}
            public double longitude { get; set;}
            public double distance { get; set; }
            public int numtips { get; set;}
            public int numcheckins { get; set;}
            public bool isopen { get; set;}
            public double stars { get; set;}
        };

        public class User
        {
            public string user_id { get; set; }
            public string user_name { get; set; }
            public double average_stars { get; set; }
            public int fans { get; set; }
            public int cool { get; set; }
            public int tipCount { get; set; }
            public int funny { get; set; }
            public int total_likes { get; set; }
            public int useful { get; set; }
            public string yelping_since { get; set; }
            public float lat { get; set; }
            public float longitude { get; set; }
        }


        public class Tip
        {
            public string user_name { get; set; }
            public string business_name { get; set; }
            public string city { get; set; }
            public string tipText { get; set; }
            public string date { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            addStates();
            addColumnsToBusinessGrid();
            addSortValues();
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

        private void addColumnsToBusinessGrid()
        {
            addColumnToGrid(businessGrid, "business_name", "BusinessName");
            addColumnToGrid(businessGrid, "business_address", "Address");
            addColumnToGrid(businessGrid, "business_city", "City");
            addColumnToGrid(businessGrid, "business_state", "State");
            addColumnToGrid(businessGrid, "distance", "Distance (miles)");
            addColumnToGrid(businessGrid, "stars", "Stars");
            addColumnToGrid(businessGrid, "numtips", "# of Tips");
            addColumnToGrid(businessGrid, "numcheckins", "Total Checkins");
            //DataGridTextColumn col = addColumnToGrid(businessGrid, "business_id", "");
            //col.Width = 0;
        }

        private void addSortValues()
        {
            sortList.Items.Add("Name (default)");
            sortList.Items.Add("Address");
            sortList.Items.Add("City");
            sortList.Items.Add("State");
            sortList.Items.Add("Rating");
            sortList.Items.Add("Number of Tips");
            sortList.Items.Add("Check-ins");
            sortList.Items.Add("Nearest");
            sortList.SelectedIndex = 0; // set to default value
        }

        private void addState(NpgsqlDataReader R)
        {
            stateList.Items.Add(R.GetString(0)); // add state to state combobox
        }

        private void addStates()
        {
            /* adds distinct states from database to the combo box */
            string sqlstr = "SELECT distinct business_state FROM business ORDER BY business_state";
            executeQuery(sqlstr, addState);
        }

        private void addCity(NpgsqlDataReader R)
        {
            cityList.Items.Add(R.GetString(0)); // add to city list
        }

        private void stateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* when a state is selected, populate cities */
            cityList.Items.Clear();
            if (!(stateList.SelectedIndex > -1))
                return;
            /* adds distinct cities from database to the combobox */
            string sqlstr = $"SELECT distinct business_city FROM business WHERE business_state = '{stateList.SelectedItem.ToString()}' ORDER BY business_city";
            executeQuery(sqlstr, addCity);
        }

        private void addZipcode(NpgsqlDataReader R)
        {
            zipcodeList.Items.Add(R.GetInt32(0)); // add to zipcode list
        }

        private void cityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* when a city is selected, populate zip codes */
            zipcodeList.Items.Clear();
            if (!(cityList.SelectedIndex > -1))
                return;
            /* adds distinct zipcodes from database to the combobox */
            string sqlstr = $"SELECT distinct zipcode FROM business WHERE business_state = '{stateList.SelectedItem.ToString()}' AND business_city = '{cityList.SelectedItem.ToString()}' ORDER BY zipcode";
            executeQuery(sqlstr, addZipcode);
        }

        private void addCategory(NpgsqlDataReader R)
        {
            categoryList.Items.Add(R.GetString(0)); // add to category list
        }

        private void zipcodeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* when a zipcode is selected retrieve the business categories for the businesses that appear in that zipcode */
            categoryList.Items.Clear();
            if (!(zipcodeList.SelectedIndex > -1))
                return;
            /* adds distinct categories from database to the combobox */
            string sqlstr = $"SELECT distinct catagory_name FROM business, catagories WHERE business.business_id = catagories.business_id AND zipcode = '{zipcodeList.SelectedItem.ToString()}' ORDER BY catagory_name";
            executeQuery(sqlstr, addCategory);
        }

        private void addBusinessGridRow(NpgsqlDataReader R)
        {
            if (UserInputTextBox.Text != null)
            {
                User userSelection = userGrid.SelectedItem as User;
                string sqlstr = "SELECT U.user_lat,U.user_long,B.latitude,B.longitude,B.business_id,B.business_name,B.business_address,B.business_city,B.business_state,B.zipcode,B.numtips,B.numcheckins,B.isopen,B.stars FROM Users as U,business as B WHERE user_id = \'" + userSelection.user_id + "\' AND business_id = \'" + R.GetString(0) + "\';";
                executeQuery(sqlstr, calcDistance);
            }
            else
            {
                businessGrid.Items.Add(new Business()
                {
                    business_id = R.GetString(0),
                    business_name = R.GetString(1),
                    business_address = R.GetString(2),
                    business_city = R.GetString(3),
                    business_state = R.GetString(4),
                    zipcode = R.GetInt32(5),
                    distance = 0.0,
                    numtips = R.GetInt32(8),
                    numcheckins = R.GetInt32(9),
                    isopen = R.GetBoolean(10),
                    stars = R.GetDouble(11)
                });
            }
        }

        private void calcDistance(NpgsqlDataReader R)
        {
            double r = 3958.8;
            double p = Math.PI / 180;
            double latUser = R.GetDouble(0);
            double longUser = R.GetDouble(1);
            double latBus = R.GetDouble(2);
            double longBus = R.GetDouble(3);
            double ans = 2 * r * Math.Asin(Math.Sqrt(0.5 - (Math.Cos((latUser - latBus) * p) / 2) + (Math.Cos(latBus * p) * Math.Cos(latUser * p) * (1 - Math.Cos((longUser - longBus) * p)) / 2)));

            businessGrid.Items.Add(new Business()
            {
                business_id = R.GetString(4),
                business_name = R.GetString(5),
                business_address = R.GetString(6),
                business_city = R.GetString(7),
                business_state = R.GetString(8),
                zipcode = R.GetInt32(9),
                distance = ans,
                numtips = R.GetInt32(10),
                numcheckins = R.GetInt32(11),
                isopen = R.GetBoolean(12),
                stars = R.GetDouble(13)
            });
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            /* when the user searches for businesses, all the businesses in the selected zipcode will be displayed */
            businessGrid.Items.Clear();
            if (!(zipcodeList.SelectedIndex > -1))
                return;

            string attr = "business_id, business_name, business_address, business_city, business_state, zipcode, latitude, longitude, numtips, numcheckins, isopen, stars";

            string sqlstr = $"SELECT {attr} FROM business WHERE zipcode = '{zipcodeList.SelectedItem.ToString()}'";

            // filter by category
            foreach (var category in categoryFilterList.Items)
            {
                sqlstr += $" AND business_id IN (SELECT business_id FROM catagories WHERE catagory_name = '{category.ToString()}')";
            }
            // filter by price range attribute
            {
                string attr_name = "RestaurantsPriceRange2";
                bool[] filter_prices = new bool[] { (bool)filterPrice1.IsChecked, (bool)filterPrice2.IsChecked, (bool)filterPrice3.IsChecked, (bool)filterPrice4.IsChecked };
                int i = 1;
                int count = 0;
                foreach (var price in filter_prices)
                {
                    if (price)
                    {
                        sqlstr += count == 0 ? $" AND (business_id IN (SELECT business_id FROM attributes WHERE attr_name = '{attr_name}' AND (" : " OR";
                        sqlstr += $" attr_value = '{i}'";
                        ++count;
                    }
                    ++i;
                }
                if (count > 0)
                {
                    sqlstr += ")))";
                }
            }
            // filter by meal
            {
                Tuple<string, CheckBox>[] attrs = new Tuple<string, CheckBox>[] {
                    new Tuple<string, CheckBox>("breakfast", filterMealBreakfast),
                    new Tuple<string, CheckBox>("lunch", filterMealLunch),
                    new Tuple<string, CheckBox>("brunch", filterMealBrunch),
                    new Tuple<string, CheckBox>("dinner", filterMealDinner),
                    new Tuple<string, CheckBox>("dessert", filterMealDessert),
                    new Tuple<string, CheckBox>("latenight", filterMealLatenight)
                };
                int count = 0;
                foreach (Tuple<string, CheckBox> attr_tuple in attrs)
                {
                    if ((bool)attr_tuple.Item2.IsChecked)
                    {
                        sqlstr += count == 0 ? $" AND (business_id IN (SELECT business_id FROM attributes WHERE " : " OR";
                        sqlstr += $" attr_name = '{attr_tuple.Item1}' AND attr_value = 'True'";
                        ++count;
                    }
                }
                if (count > 0)
                {
                    sqlstr += "))";
                }
            }
            // filter by attribute
            {
                Tuple<string, CheckBox>[] attrs = new Tuple<string, CheckBox>[] {
                    new Tuple<string, CheckBox>("BusinessAcceptsCreditCards", filterAttrCards),
                    new Tuple<string, CheckBox>("RestaurantsReservations", filterAttrReservation),
                    new Tuple<string, CheckBox>("WheelchairAccessible", filterAttrWheelchair),
                    new Tuple<string, CheckBox>("OutdoorSeating", filterAttrOutdoor),
                    new Tuple<string, CheckBox>("GoodForKids", filterAttrKids),
                    new Tuple<string, CheckBox>("RestaurantsGoodForGroups", filterAttrGroups),
                    new Tuple<string, CheckBox>("RestaurantsDelivery", filterAttrDelivery),
                    new Tuple<string, CheckBox>("RestaurantsTakeOut", filterAttrTakeout),
                    new Tuple<string, CheckBox>("WiFi", filterAttrWifi),
                    new Tuple<string, CheckBox>("BikeParking", filterAttrBike)
                };
                foreach (Tuple<string, CheckBox> attr_tuple in attrs)
                {
                    if ((bool)attr_tuple.Item2.IsChecked)
                    {
                        sqlstr += $" AND business_id IN (SELECT business_id FROM attributes WHERE attr_name = '{attr_tuple.Item1}' AND attr_value = 'True')";
                    }
                }
            }

            // sorting
            string[] sorting = new string[]{"business_name", "business_address", "business_city", "business_state", "stars", "numTips", "numCheckins", "longitude" };
            sqlstr += " ORDER BY " + sorting[sortList.SelectedIndex] + ";";

            Trace.WriteLine(sqlstr);
            executeQuery(sqlstr, addBusinessGridRow);
        }

        private void businessGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /**/
            if (!(businessGrid.SelectedIndex > -1))
                return;
            Business business = businessGrid.SelectedItem as Business;

            if ((business.business_id != null) && (business.business_id.ToString().CompareTo("") != 0))
            {
                onSelectBusinessSetData(business);
                if (userSelection == null)
                {
                    TipsWindow window = new TipsWindow(business.business_id.ToString(), "");
                    window.Show();
                    CheckinWindow checkwindow = new CheckinWindow(business.business_id.ToString(), "");
                    checkwindow.Show();
                }
                else
                {
                    TipsWindow window = new TipsWindow(business.business_id.ToString(), userSelection.user_id.ToString());
                    window.Show();
                    CheckinWindow checkwindow = new CheckinWindow(business.business_id.ToString(), userSelection.user_id.ToString());
                    checkwindow.Show();
                }
            }
        }

        private void dispUserBtn_Click(object sender, RoutedEventArgs e)
        {
            FriendsGrid.Items.Clear();
            userGrid.Items.Clear();
            
            string userNameInput = UserInputTextBox.Text;

            string sqlstr = "SELECT user_id,user_name FROM Users WHERE user_name = \'" + userNameInput + "\';";
            executeQuery(sqlstr, addUsersGridRow);

        }

        private void addUsersGridRow(NpgsqlDataReader R)
        {
            userGrid.Items.Add(new User() { 
                user_id = R.GetString(0),
                user_name = R.GetString(1)
            });

        }
        
        private void addFriendsTipRow(NpgsqlDataReader R)
        {
            FriendsTipGrid.Items.Add(new Tip()
            {

                user_name = R.GetString(0),
                business_name = R.GetString(1),
                city = R.GetString(2),
                tipText = R.GetString(3),
                date = R.GetDate(4).ToString() + " " + R.GetTimeSpan(5).ToString()
            }) ;
        }

        private void addFriendsGridRow(NpgsqlDataReader R)
        {
            FriendsGrid.Items.Add(new User()
            {
                user_name = R.GetString(1),
                average_stars = R.GetDouble(2),
                yelping_since = R.GetDate(3).ToString()
            }) ;
            string sqlstr = "SELECT Users.user_name, Business.business_name, business_city, tipText, tipDate, tipTime FROM Tips,Users,Business WHERE Tips.user_id = \'" + R.GetString(0) + "\' AND tipDate <= (SELECT CURRENT_DATE) AND Users.user_id = Tips.user_id AND Business.business_id = Tips.business_id ORDER BY tipDate LIMIT 1";
            executeQuery(sqlstr, addFriendsTipRow);
        }

        private void onSelectBusinessSetData(Business business)
        {
            BusinessNameSelected.Content = business.business_name;
            BusinessAddressSelected.Content = business.business_address;
            string sqlstr = "SELECT open_state,close_state FROM store_hours WHERE business_id = \'" + business.business_id + "\' AND daysofweek = \'" + DateTime.Today.DayOfWeek + "\';";
            executeQuery(sqlstr, getHours);
        }

        private void getHours(NpgsqlDataReader R)
        {
            TimesSelected.Content = DateTime.Today.DayOfWeek + ": " + R.GetString(0) + " - " + R.GetString(1);
        }
        
        private void addCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(categoryList.SelectedIndex > -1))
                return;
            // don't allow duplicates
            foreach (var category in categoryFilterList.Items)
            {
                if (category.ToString().Equals(categoryList.SelectedItem.ToString()))
                    return;
            }
            categoryFilterList.Items.Add(categoryList.SelectedItem);
        }

        private void removeCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(categoryFilterList.SelectedIndex > -1))
                return;
            categoryFilterList.Items.Remove(categoryFilterList.SelectedItem);
        }

        public User userSelection;
        private void userGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FriendsTipGrid.Items.Clear();
            FriendsGrid.Items.Clear();
            userSelection = userGrid.SelectedItem as User;
            if (userSelection != null)
            {
                string userId = userSelection.user_id;
                string sqlstr = "SELECT user_name,average_stars,yelping_since,fans,Cool,funny,useful,tipCount,total_likes,user_lat,user_long FROM Users WHERE user_id = \'" + userId + "\';";
                executeQuery(sqlstr, updateUserGrid);

                sqlstr = "SELECT user_id,user_name,average_stars,yelping_since FROM (SELECT friend_id FROM Friends WHERE user_id = \'" + userId + "\') as F, Users WHERE Users.user_id = F.friend_id;";
                executeQuery(sqlstr, addFriendsGridRow);


            }

        }

        private void updateUserGrid(NpgsqlDataReader R)
        {
            userNameLabel.Content = R.GetString(0);
            userAvgStarLabel.Content = R.GetDouble(1);
            userYelpingSinceLabel.Content = R.GetDate(2);
            userFansLabel.Content = R.GetInt32(3);
            userVotesLabel.Content = R.GetInt32(4) + R.GetInt32(5) + R.GetInt32(6);
            userTipCountLabel.Content = R.GetInt32(7);
            userLikesLabel.Content = R.GetInt32(8);
            UserLocationLabel.Content = "Lat: " + R.GetDouble(9).ToString() + " Long: " + R.GetDouble(10).ToString();
        }


        private void userLatLongButton_Click(object sender, RoutedEventArgs e)
        {
            User userSelection = userGrid.SelectedItem as User;
            string lat = userInputLatBox.Text;
            string userLong = userInputLongBox.Text;

            userInputLatBox.Text = "";
            userInputLongBox.Text = "";

            string sqlStr = "UPDATE users SET user_lat = \'" + lat + "\', user_long = \'" + userLong + "\' WHERE user_id = \'" + userSelection.user_id + "\';";
            executeQuery(sqlStr, emptyFunct);
        }

        private void emptyFunct(NpgsqlDataReader R)
        {

        }

        

        private void ShowCheckinsBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button is clicked!");
            // User UserSelection = userGrid.SelectedItem as User;
            // string sqlStr = "SELECT day, sum(morning + afternoon + evening + night) FROM checkins WHERE business_id = '" + UserSelection + "' GROUP BY day ORDER BY day";
            // executeQuery(sqlStr, emptyFunct);

        }

    }
}
