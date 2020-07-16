using System;
using System.Collections.Generic;
using System.IO;
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
using MySql.Data.MySqlClient;

namespace phonebook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private string id;
        private MySqlConnection connect()
        {
            string host = "mysql11.hostland.ru";
            string database = "host1323541_itstep27";
            string port = "3306";
            string username = "host1323541_itstep";
            string pass = "269f43dc";

            string Connect = "Server=" + host + ";Database=" + database + ";port=" + port + ";User Id=" + username + ";password=" + pass;

            // Создаем соединение с базой данных
            MySqlConnection mysql_connection = new MySqlConnection(Connect);
            return mysql_connection;
        }

        private void create_btn_Click(object sender, RoutedEventArgs e)
        {
            search_border.Visibility = Visibility.Hidden;
            create_border.Visibility = Visibility.Visible;

        }

        private void search_btn_Click(object sender, RoutedEventArgs e)
        {
            search_border.Visibility = Visibility.Visible;
            create_border.Visibility = Visibility.Hidden;
            var connection = connect();
            connection.Open();
            var mysql_query = $"SELECT firstname, lastname, phone1, phone2 FROM phonebook WHERE firstname='{search_input.Text}' OR lastname='{search_input.Text}' OR phone1='{search_input.Text}' OR phone2='{search_input.Text}'";
            var query = new MySqlCommand { Connection = connection, CommandText = mysql_query };
            var result = query.ExecuteReader();
            if (result.Read())
            {
                input_name_search.Text = result.GetString(0);
                input_surename_search.Text = result.GetString(1);
                input_namber1_search.Text = result.GetString(2);
                input_namber2_search.Text = result.GetString(3);

                input_name_search.IsEnabled = false;
                input_surename_search.IsEnabled = false;
                input_namber1_search.IsEnabled = false;
                input_namber2_search.IsEnabled = false;

                search_input.Clear();
            }
            else
            {
                MessageBox.Show("Совпадений не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                search_input.Clear();
            }
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            var connection = connect();
            connection.Open();
            var mysql_query = "";
            if (input_surename.Text.Length == 0 && input_namber2.Text.Length != 0)
            {
                mysql_query = $"INSERT phonebook(firstname, lastname, phone1, phone2) VALUES('{input_name.Text}', '-', '{input_namber1.Text}', '{input_namber2.Text}');";
            }
            else
            {
                if (input_namber2.Text.Length == 0 && input_surename.Text.Length != 0)
                {
                    mysql_query = $"INSERT phonebook(firstname, lastname, phone1, phone2) VALUES('{input_name.Text}', '{input_surename.Text}', '{input_namber1.Text}', '-');";
                }
                else
                {
                    if (input_namber2.Text.Length == 0 && input_surename.Text.Length == 0)
                    {
                        mysql_query = $"INSERT phonebook(firstname, lastname, phone1, phone2) VALUES('{input_name.Text}', '-', '{input_namber1.Text}', '-');";
                    }
                    else
                    {
                        mysql_query = $"INSERT phonebook(firstname, lastname, phone1, phone2) VALUES('{input_name.Text}', '{input_surename.Text}', '{input_namber1.Text}', '{input_namber2.Text}');";
                    }
                }
            }
            var query = new MySqlCommand { Connection = connection, CommandText = mysql_query };
            var result = query.ExecuteNonQuery();
            if (result == 0)
            {
                MessageBox.Show("Запись не создана", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                input_name.Clear();
                input_surename.Clear();
                input_namber1.Clear();
                input_namber2.Clear();
            }
            else
            {
                MessageBox.Show("Запись создана", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                input_name.Clear();
                input_surename.Clear();
                input_namber1.Clear();
                input_namber2.Clear();
            }

        }

        private void input_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (input_name.Text.Length > 0)
            {
                name_marker.Foreground = Brushes.Green;
                name_marker.Text = "Поле заполнено";
            }
            else
            {
                name_marker.Foreground = Brushes.Red;
                name_marker.Text = "Обязательно для заполнения";
            }
        }

        private void input_namber1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (input_namber1.Text.Length > 0)
            {
                phone_marker.Foreground = Brushes.Green;
                phone_marker.Text = "Поле заполнено";
            }
            else
            {
                phone_marker.Foreground = Brushes.Red;
                phone_marker.Text = "Обязательно для заполнения";
            }
        }

        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            input_name_search.IsEnabled = true;
            input_surename_search.IsEnabled = true;
            input_namber1_search.IsEnabled = true;
            input_namber2_search.IsEnabled = true;
            edit_btn.IsEnabled = false;
            save_edit_btn.Visibility = Visibility.Visible;
        }

        private void save_edit_btn_Click(object sender, RoutedEventArgs e)
        {
            edit_btn.IsEnabled = true;
            save_edit_btn.Visibility = Visibility.Hidden;

            using (var connection = connect())
            {
                connection.Open();
                var mysql_query = $"SELECT id FROM phonebook WHERE firstname='{input_name_search.Text}' AND phone1='{input_namber1_search.Text}';";
                var query = new MySqlCommand { Connection = connection, CommandText = mysql_query };
                var result = query.ExecuteReader();
                if (result.Read())
                {
                    id = result.GetString(0);
                    Convert.ToInt32(id);
                }
                else
                {
                    MessageBox.Show("Совпадений не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    search_input.Clear();
                }
            }

            using (var connection = connect())
            {
                connection.Open();
                var mysql_query = $"UPDATE phonebook SET firstname='{input_name_search.Text}', lastname='{input_surename_search.Text}', phone1='{input_namber1_search.Text}', phone2='{input_namber2_search.Text}' WHERE id={id};";
                var query = new MySqlCommand { Connection = connection, CommandText = mysql_query };
                var result = query.ExecuteNonQuery();
                if (result == 0)
                    MessageBox.Show("Обновление данных не удалось", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    MessageBox.Show("Обновление данных успешно завершено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }



        }

        private void fromfile_btn_Click(object sender, RoutedEventArgs e)
        {
            using (StreamReader file = new StreamReader("load.txt"))
            {
                string tempLine;
                while ((tempLine = file.ReadLine()) != null)
                {
                    int tempIndex = tempLine.IndexOf('=');
                    if (tempIndex == -1) continue;
                    string tempVar = tempLine.Substring(0, tempIndex);
                    string tempSymbols = tempLine.Substring(tempIndex + 1);
                    switch (tempVar)
                    {
                        case "firstname":
                            input_name.Text = tempSymbols;
                            break;
                        case "lastname":
                            input_surename.Text = tempSymbols;
                            break;
                        case "phone1":
                            input_namber1.Text = tempSymbols;
                            break;
                        case "symbols3":
                            input_namber2.Text = tempSymbols;
                            break;
                    }
                }
            }
        }

        private void fail_btn_Click(object sender, RoutedEventArgs e)
        {
            string one = input_name_search.Text + Environment.NewLine;
            string two = input_surename_search.Text + Environment.NewLine;
            string three = input_namber1_search.Text + Environment.NewLine;
            string four = input_namber2_search.Text + Environment.NewLine;

            // запись в файл
            using (FileStream fstream = new FileStream($"save.txt", FileMode.Open))
            {
                // преобразуем строку в байты
                byte[] array1 = System.Text.Encoding.Default.GetBytes(one);
                // запись массива байтов в файл
                fstream.Write(array1, 0, array1.Length);
                byte[] array2 = System.Text.Encoding.Default.GetBytes(two);
                // запись массива байтов в файл
                fstream.Write(array2, 0, array2.Length);
                byte[] array3 = System.Text.Encoding.Default.GetBytes(three);
                // запись массива байтов в файл
                fstream.Write(array3, 0, array3.Length);
                byte[] array4 = System.Text.Encoding.Default.GetBytes(four);
                // запись массива байтов в файл
                fstream.Write(array4, 0, array4.Length);
            }
            MessageBox.Show("Данные успешно записаны в фаил", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

