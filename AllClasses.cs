using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace DormitorySystem
{
    public static class Database
    {
        private static bool initialized = false;
        private const string DbName = "DormitorySystem.db";

        public static SQLiteConnection GetConnection()
        {
            bool dbExists = File.Exists(DbName);
            var conn = new SQLiteConnection($"Data Source={DbName};Version=3;");
            conn.Open();

            if (!dbExists && !initialized)
            {
                InitializeDatabase(conn);
                initialized = true;
            }

            return conn;
        }

        private static void InitializeDatabase(SQLiteConnection conn)
        {
            string[] queries = {
                @"CREATE TABLE IF NOT EXISTS Persons (
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    NationalCode TEXT PRIMARY KEY,
                    Address TEXT,
                    Phone TEXT
                );",

                @"CREATE TABLE IF NOT EXISTS Students (
                    StudentCode TEXT PRIMARY KEY,
                    NationalCode TEXT NOT NULL UNIQUE,
                    DormitoryName TEXT NOT NULL,
                    BlockName TEXT NOT NULL,
                    RoomNumber TEXT NOT NULL
                );",

                @"CREATE TABLE IF NOT EXISTS Dormitories (
                    Name TEXT PRIMARY KEY,
                    Address TEXT,
                    SupervisorCode TEXT
                );",

                @"CREATE TABLE IF NOT EXISTS Blocks (
                    Name TEXT PRIMARY KEY,
                    Floors INTEGER NOT NULL,
                    Capacity INTEGER NOT NULL,
                    DormitoryName TEXT NOT NULL
                );",

                @"CREATE TABLE IF NOT EXISTS Rooms (
                    RoomNumber TEXT NOT NULL,
                    Floor INTEGER NOT NULL,
                    Capacity INTEGER NOT NULL,
                    BlockName TEXT NOT NULL,
                    PRIMARY KEY (RoomNumber, BlockName)
                );"
            };

            foreach (var query in queries)
            {
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Database initialized successfully.");
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public Person(string firstName, string lastName, string nationalCode, string address, string phone)
        {
            FirstName = firstName;
            LastName = lastName;
            NationalCode = nationalCode;
            Address = address;
            Phone = phone;
        }

        public void Save()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("REPLACE INTO Persons (FirstName, LastName, NationalCode, Address, Phone) VALUES (@f, @l, @n, @a, @p)", conn);
                cmd.Parameters.AddWithValue("@f", FirstName);
                cmd.Parameters.AddWithValue("@l", LastName);
                cmd.Parameters.AddWithValue("@n", NationalCode);
                cmd.Parameters.AddWithValue("@a", Address);
                cmd.Parameters.AddWithValue("@p", Phone);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class PersonManager
    {
        public List<Person> People { get; private set; } = new List<Person>();

        public void LoadAll()
        {
            People.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Persons", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        People.Add(new Person(
                            reader["FirstName"].ToString(),
                            reader["LastName"].ToString(),
                            reader["NationalCode"].ToString(),
                            reader["Address"].ToString(),
                            reader["Phone"].ToString()
                        ));
                    }
                }
            }
        }

        public void Add(Person person)
        {
            person.Save();
            People.Add(person);
        }
    }

    public class Student
    {
        public string StudentCode { get; set; }
        public string NationalCode { get; set; }
        public string DormitoryName { get; set; }
        public string BlockName { get; set; }
        public string RoomNumber { get; set; }

        public Student(string studentCode, string nationalCode, string dorm, string block, string room)
        {
            StudentCode = studentCode;
            NationalCode = nationalCode;
            DormitoryName = dorm;
            BlockName = block;
            RoomNumber = room;
        }

        public void Save()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("REPLACE INTO Students (StudentCode, NationalCode, DormitoryName, BlockName, RoomNumber) VALUES (@sc, @nc, @d, @b, @r)", conn);
                cmd.Parameters.AddWithValue("@sc", StudentCode);
                cmd.Parameters.AddWithValue("@nc", NationalCode);
                cmd.Parameters.AddWithValue("@d", DormitoryName);
                cmd.Parameters.AddWithValue("@b", BlockName);
                cmd.Parameters.AddWithValue("@r", RoomNumber);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class StudentManager
    {
        public List<Student> Students { get; private set; } = new List<Student>();

        public void LoadAll()
        {
            Students.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Students", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Students.Add(new Student(
                            reader["StudentCode"].ToString(),
                            reader["NationalCode"].ToString(),
                            reader["DormitoryName"].ToString(),
                            reader["BlockName"].ToString(),
                            reader["RoomNumber"].ToString()
                        ));
                    }
                }
            }
        }

        public void Add(Student student)
        {
            student.Save();
            Students.Add(student);
        }
    }

    public class Dormitory
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string SupervisorCode { get; set; }

        public Dormitory(string name, int capacity, string address, string supervisorCode)
        {
            Name = name;
            Address = address;
            SupervisorCode = supervisorCode;
        }

        public void Save()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("REPLACE INTO Dormitories (Name, Address, SupervisorCode) VALUES (@n, @a, @s)", conn);
                cmd.Parameters.AddWithValue("@n", Name);
                cmd.Parameters.AddWithValue("@a", Address);
                cmd.Parameters.AddWithValue("@s", SupervisorCode);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class DormitoryManager
    {
        public List<Dormitory> Dorms { get; private set; } = new List<Dormitory>();
        private readonly PersonManager _personManager;

        public DormitoryManager(PersonManager pm)
        {
            _personManager = pm;
        }

        public void LoadAll()
        {
            Dorms.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Dormitories", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dorms.Add(new Dormitory(
                            reader["Name"].ToString(),
                            0, 
                            reader["Address"].ToString(),
                            reader["SupervisorCode"].ToString()
                        ));
                    }
                }
            }
        }

        public void Add(Dormitory dorm)
        {
            if (!_personManager.People.Exists(p => p.NationalCode == dorm.SupervisorCode))
            {
                Console.WriteLine("Error: Supervisor must be a registered person.");
                return;
            }
            dorm.Save();
            Dorms.Add(dorm);
        }

        public void List()
        {
            foreach (var d in Dorms)
            {
                Console.WriteLine($"- Name: {d.Name}, Address: {d.Address}, Supervisor Code: {d.SupervisorCode}");
            }
        }
    }

    public class Block
    {
        public string Name { get; set; }
        public int Floors { get; set; }
        public int Capacity { get; set; }
        public string DormitoryName { get; set; }

        public Block(string name, int floors, int capacity, string dormitoryName)
        {
            Name = name;
            Floors = floors;
            Capacity = capacity;
            DormitoryName = dormitoryName;
        }

        public void Save()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("REPLACE INTO Blocks (Name, Floors, Capacity, DormitoryName) VALUES (@n, @f, @c, @d)", conn);
                cmd.Parameters.AddWithValue("@n", Name);
                cmd.Parameters.AddWithValue("@f", Floors);
                cmd.Parameters.AddWithValue("@c", Capacity);
                cmd.Parameters.AddWithValue("@d", DormitoryName);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class BlockManager
    {
        public List<Block> Blocks { get; private set; } = new List<Block>();

        public void LoadAll()
        {
            Blocks.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Blocks", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Blocks.Add(new Block(
                            reader["Name"].ToString(),
                            Convert.ToInt32(reader["Floors"]),
                            Convert.ToInt32(reader["Capacity"]),
                            reader["DormitoryName"].ToString()
                        ));
                    }
                }
            }
        }

        public void Add(Block block, RoomManager roomManager)
        {
            block.Save();
            Blocks.Add(block);

            const int roomCapacity = 6;
            int capacityPerFloor = block.Capacity / block.Floors;
            int roomsPerFloor = capacityPerFloor / roomCapacity;

            if (roomsPerFloor == 0)
            {
                Console.WriteLine("Warning: Block capacity is too low to create any rooms on each floor.");
                return;
            }

            for (int floor = 1; floor <= block.Floors; floor++)
            {
                for (int roomIndex = 1; roomIndex <= roomsPerFloor; roomIndex++)
                {
                    string roomNumber = ((floor * 100) + roomIndex).ToString();
                    Room newRoom = new Room(roomNumber, floor, roomCapacity, block.Name);
                    roomManager.Add(newRoom);
                }
            }
            Console.WriteLine($"{roomsPerFloor * block.Floors} rooms were created for block '{block.Name}'.");
        }

        public void List(RoomManager roomManager)
        {
            Console.WriteLine("--- List of All Blocks ---");
            if (!Blocks.Any())
            {
                Console.WriteLine("No blocks found.");
                return;
            }

            foreach (var b in Blocks)
            {
                Console.WriteLine($"\n- Block Name: {b.Name}, Dormitory: {b.DormitoryName}, Floors: {b.Floors}, Capacity: {b.Capacity}");

                var roomsInBlock = roomManager.Rooms
                    .Where(r => r.BlockName.Equals(b.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!roomsInBlock.Any())
                {
                    Console.WriteLine("  (No rooms found for this block)");
                    continue;
                }

                var roomsByFloor = roomsInBlock
                    .GroupBy(r => r.Floor)
                    .OrderBy(g => g.Key);

                Console.WriteLine("  Room Ranges:");
                foreach (var floorGroup in roomsByFloor)
                {
                    var roomNumbers = floorGroup.Select(r => int.Parse(r.RoomNumber)).OrderBy(n => n).ToList();
                    if (roomNumbers.Any())
                    {
                        Console.WriteLine($"    Floor {floorGroup.Key}: From {roomNumbers.First()} to {roomNumbers.Last()}");
                    }
                }
            }
            Console.WriteLine("\n--- End of List ---");
        }
    }

    public class Room
    {
        public string RoomNumber { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public string BlockName { get; set; }

        public Room(string roomNumber, int floor, int capacity, string blockName)
        {
            RoomNumber = roomNumber;
            Floor = floor;
            Capacity = capacity;
            BlockName = blockName;
        }

        public void Save()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("REPLACE INTO Rooms (RoomNumber, Floor, Capacity, BlockName) VALUES (@r, @f, @c, @b)", conn);
                cmd.Parameters.AddWithValue("@r", RoomNumber);
                cmd.Parameters.AddWithValue("@f", Floor);
                cmd.Parameters.AddWithValue("@c", Capacity);
                cmd.Parameters.AddWithValue("@b", BlockName);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class RoomManager
    {
        public List<Room> Rooms { get; private set; } = new List<Room>();

        public void LoadAll()
        {
            Rooms.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Rooms", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Rooms.Add(new Room(
                            reader["RoomNumber"].ToString(),
                            Convert.ToInt32(reader["Floor"]),
                            Convert.ToInt32(reader["Capacity"]),
                            reader["BlockName"].ToString()
                        ));
                    }
                }
            }
        }

        public void Add(Room room)
        {
            if (Rooms.Exists(r => r.RoomNumber == room.RoomNumber && r.BlockName == room.BlockName))
            {
                return; 
            }
            room.Save();
            Rooms.Add(room);
        }

        public void List()
        {
            foreach (var r in Rooms)
            {
                Console.WriteLine($"Room: {r.RoomNumber}, Floor: {r.Floor}, Capacity: {r.Capacity}, Block: {r.BlockName}");
            }
        }
    }
}