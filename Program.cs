using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace BillingSystem
{
    class DatabaseConnection
    {
        private string connectionString = $"server=127.0.0.1;user=root;database=billingsystem;port=3306;password={Environment.GetEnvironmentVariable("DB_PASSWORD")};SslMode=Preferred";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public MySqlConnection OpenConnection()
        {
            var connection = GetConnection();
            connection.Open();
            return connection;
        }
    }

    class MenuItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    class RestaurantBillingRepo
    {
        private DatabaseConnection db = new DatabaseConnection();

        public void AddMenuItem(string name, decimal price)
        {
            using var connection = db.OpenConnection();
            string query = "INSERT INTO MenuItems (Name, Price) VALUES (@Name, @Price)";
            connection.Execute(query, new { Name = name, Price = price });
            Console.WriteLine("\nMenu item added!");
        }

        public int AddCustomer(string name, string contact)
        {
            using var connection = db.OpenConnection();
            string query = "INSERT INTO Customers (Name, Contact) VALUES (@Name, @Contact)";
            connection.Execute(query, new { Name = name, Contact = contact });
            int customerId = connection.QuerySingle<int>("SELECT LAST_INSERT_ID();");
            Console.WriteLine($"\nCustomer added with ID: {customerId}");
            return customerId;
        }

        public int CreateOrder(int customerId)
        {
            using var connection = db.OpenConnection();
            string query = "INSERT INTO Orders (CustomerID) VALUES (@CustomerID)";
            connection.Execute(query, new { CustomerID = customerId });
            int orderId = connection.QuerySingle<int>("SELECT LAST_INSERT_ID();");
            Console.WriteLine($"\nOrder created with ID: {orderId}");
            return orderId;
        }

        public void AddOrderItem(int orderId, int menuItemId, int quantity)
        {
            using var connection = db.OpenConnection();
            string query = "INSERT INTO OrderItems (OrderID, MenuItemID, Quantity) VALUES (@OrderID, @MenuItemID, @Quantity)";
            connection.Execute(query, new { OrderID = orderId, MenuItemID = menuItemId, Quantity = quantity });
            Console.WriteLine("🍽Order item added!");
        }

        public List<MenuItem> GetMenuItems()
        {
            using var connection = db.OpenConnection();
            string query = "SELECT * FROM MenuItems";
            return connection.Query<MenuItem>(query).ToList();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var repo = new RestaurantBillingRepo();

            Console.WriteLine("Welcome to Star Restaurant!");

            while (true)
            {
                var menuItems = repo.GetMenuItems();

                Console.WriteLine("\n--- MENU ---");
                foreach (var item in menuItems)
                {
                    Console.WriteLine($"{item.ID}. {item.Name} - Rs. {item.Price}");
                }

                Console.Write("\nDo you want to place an order? (yes/no): ");
                string answer = Console.ReadLine().Trim().ToLower();

                if (answer == "yes")
                {
                    Console.Write("\nYour name: ");
                    string name = Console.ReadLine();

                    Console.Write("Your contact: ");
                    string contact = Console.ReadLine();

                    int customerId = repo.AddCustomer(name, contact);
                    int orderId = repo.CreateOrder(customerId);

                    Console.Write("\nHow many items do you want to order? ");
                    if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
                    {
                        Console.WriteLine("❌ Invalid number. Cancelling order.");
                        continue;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        Console.Write($"\n#{i + 1} Enter the Menu Item ID: ");
                        int menuItemId;
                        while (!int.TryParse(Console.ReadLine(), out menuItemId))
                        {
                            Console.Write("Invalid input. Enter a valid Menu Item ID: ");
                        }

                        Console.Write("Enter quantity: ");
                        int qty;
                        while (!int.TryParse(Console.ReadLine(), out qty) || qty <= 0)
                        {
                            Console.Write("Invalid quantity. Enter again: ");
                        }

                        repo.AddOrderItem(orderId, menuItemId, qty);
                    }

                    Console.WriteLine("\nThank you! Your order has been placed.\n");
                }
                else if (answer == "no")
                {
                    Console.WriteLine("\nThanks for visiting Star Restaurant. Goodbye!");
                    break;
                }
                else
                {
                    Console.WriteLine("\nInvalid input. Please type 'yes' or 'no'.");
                }
            }
        }
    }
}
