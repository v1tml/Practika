using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace AutoSchool1
{
    public partial class MainWindow : Window
    {
        // Строка подключения к базе данных
        private readonly string connectionString = "Server=DESKTOP-OPGRJAM\\SQLEXPRESS;Database=AutoSchool;Integrated Security=True;";

        public MainWindow()
        {
            InitializeComponent();
        }

        // Метод для отображения данных "Группы"
        private void ShowGroups_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = @"
                    SELECT 
                        [Group].GroupID, 
                        [Group].GroupName, 
                        [Group].StartDate, 
                        Instructors.FirstName AS InstructorName
                    FROM [Group]
                    LEFT JOIN Instructors ON [Group].InstructorID = Instructors.InstructorID";

                LoadData(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из таблицы 'Группы': {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для отображения данных "Студенты"
        private void ShowStudents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = @"
                    SELECT 
                        Student.StudentID, 
                        Student.FirstName, 
                        Student.LastName, 
                        Student.BirtchDate,
Student.status,
                        [Group].GroupName,
student.[CarID]
                    FROM Student
                    LEFT JOIN [Group] ON Student.GroupID = [Group].GroupID";

                LoadData(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из таблицы 'Студенты': {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для отображения данных "Машины"
        private void ShowCars_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = @"
                    SELECT 
                        Car.CarsID, 
                        Car.Model, 
                        Car.LicensePlate, 
                        Instructors.FirstName AS InstructorName
                    FROM Car
                    LEFT JOIN Instructors ON Car.InstructorID = Instructors.InstructorID";

                LoadData(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из таблицы 'Машины': {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для отображения данных "Инструкторы"
        private void ShowInstructors_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = @"
                    SELECT 
                        Instructors.InstructorID, 
                        Instructors.FirstName, 
                        Instructors.LastName, 
                        Instructors.ExperienceYears,
Instructors.[PhoneNumber]
                    FROM Instructors";

                LoadData(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из таблицы 'Инструкторы': {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ShowPayments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = @"
            SELECT 
                Student.FirstName, 
                Student.LastName, 
                Student.GroupID, 
                Student.Oplata 
            FROM Student";

                LoadDataForPayments(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных оплаты: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDataForPayments(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridPayments.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SavePayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string firstName = txtFirstName.Text;
                string lastName = txtLastName.Text;
                string paymentStatus = cmbPaymentStatus.Text;

                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(paymentStatus))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                UPDATE Student 
                SET Oplata = @PaymentStatus 
                WHERE FirstName = @FirstName AND LastName = @LastName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            MessageBox.Show("Данные оплаты успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                            MessageBox.Show("Студент не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Универсальный метод для загрузки данных
        private void LoadData(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridDisplay.ItemsSource = dataTable.DefaultView; // Привязка данных к DataGrid
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
