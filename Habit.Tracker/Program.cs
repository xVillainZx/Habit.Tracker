using Microsoft.Data.Sqlite;
using System.Globalization;

class Program
{
    static string connectionString = @"Data Source=habit-tracker.db";
    static void Main(string[] args)
    { 

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText =
               @"CREATE TABLE IF NOT EXISTS drinking_water (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Date TEXT,
            Quantity INTEGER
            )";

            tableCmd.ExecuteNonQuery();

            connection.Close();
        }
        
        GetUserInput();
    }

    static void GetUserInput()
    {
        Console.Clear();
        bool closeApp = false;
        while (closeApp == false)
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("\nSelect on of the following options to continue.");
            Console.WriteLine("\n0 - Close Application");
            Console.WriteLine("1 - View All Records");
            Console.WriteLine("2 - Insert Record");
            Console.WriteLine("3 - Delete Record");
            Console.WriteLine("4 - Update Record");
            Console.WriteLine("\n-------------------------------");

            string command = Console.ReadLine();

            switch (command)
            {
                case "0":
                    Console.WriteLine("\ngoodbye!\n");
                    closeApp = true;
                    Environment.Exit(0);
                    break;
                case "1":
                    GetAllRecords();
                    break;
                case "2":
                    Insert();
                    break;
                case "3":
                    Delete();
                    break;
                case "4":
                    Update();
                    break;
                default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
                    break;
            }
        }
    }

    private static void GetAllRecords()
    {
        Console.Clear();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"SELECT * FROM drinking_water ";
           
            List<DrinkingWater> tableData = new();

            SqliteDataReader reader = tableCmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    tableData.Add(
                    new DrinkingWater
                    {
                        Id = reader.GetInt32(0),
                        Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                        Quantity = reader.GetInt32(2)
                    });
                }
            }

            else
            {
                Console.WriteLine("No rows found.");
            }

            connection.Close();

            Console.WriteLine("--------------------------------------------");
            foreach (var dw in tableData)
            {
                Console.WriteLine($"{dw.Id} - {dw.Date.ToString("dd-MMM-yyyy")} - Quantity: {dw.Quantity}");
            }

            Console.WriteLine("--------------------------------------------");
        }
    }

    private static void Insert()
    {
        string date = GetDateInput();

        int quantity = GetNumberInput("\n\nPlease inser number of glasses, or other measurement of your choice (no decimals allowed).\n\n");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText =
                $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";

            tableCmd.ExecuteNonQuery();

            connection.Close();
        }
    }

    internal static int GetNumberInput(string message)
    {
        Console.WriteLine(message);

        string numberInput = Console.ReadLine();

        if (numberInput == "0") GetUserInput();

        while (!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
        {
            Console.WriteLine("\n\nInvalid number. Try again.\n\n");
            numberInput = Console.ReadLine();
        }

        int finalInput = Convert.ToInt32(numberInput);

        return finalInput;
    }
    private static void Delete()
    {
        Console.Clear();
        GetAllRecords();

        var recordId = GetNumberInput("\n\nPlease type the Id of the record you want to delete or type 0 to go back to the Main Menu\n\n");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"DELETE from drinking_water WHERE Id = '{recordId}'";

            int rowCount = tableCmd.ExecuteNonQuery();
            
            if (rowCount == 0)
            {
                Console.WriteLine($"\n\nRecord with Id {recordId} doesnt exist. \n\n");
                Console.ReadLine();
                Delete();
            }
        }
        Console.WriteLine($"\n\nRecord with Id {recordId} was deleted. \n\n");

        GetUserInput();
    }
    internal static void Update()
    {
        Console.Clear();
        GetAllRecords();

        var recordId = GetNumberInput("\n\nPlease type Id of the record you would like to update. Type 0 to return to Main Menu.\n\n");

        using (var connection = new SqliteConnection( connectionString))
        {
            connection.Open();

            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {recordId})";
            int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

            if (checkQuery == 0)
            {
                Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
                connection.Close();
                Console.ReadLine();
                Update();
            }

            string date = GetDateInput();

            int quantity = GetNumberInput("\n\nPlease insert number of glasses, or other measurement of your choice (no decimals allowed).\n\n");

            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"UPDATE drinking_water SET date = '{date}', quantity = {quantity} WHERE Id = {recordId}";

            tableCmd.ExecuteNonQuery();

            connection.Close();
        }
    }
    internal static string GetDateInput()
    {
        Console.WriteLine("\n\nPlease insert the date: (Format: DD-MM-YY). Type 0 to return ot main menu\n\n");

        string dateInput = Console.ReadLine();

        if (dateInput == "0") GetUserInput();

        while(!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
        {
            Console.WriteLine("\n\nInvalid date. (Format: dd-MM-yy). Please try again.\n\n");
            dateInput = Console.ReadLine();
        }

        return dateInput;
    }
}

public class DrinkingWater
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
}
