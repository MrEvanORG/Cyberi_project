using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace DormitorySystem
{
    public static class Database
    {
        private static bool initialized = false;

        public static SQLiteConnection GetConnection()
        {
            bool needInit = !File.Exists("DormSystem.db");

            var conn = new SQLiteConnection("Data Source=DormSystem.db;Version=3;");
            conn.Open();

            if (needInit && !initialized)
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
                    FirstName TEXT,
                    LastName TEXT,
                    NationalCode TEXT PRIMARY KEY,
                    Address TEXT,
                    Phone TEXT
                );",

                @"CREATE TABLE IF NOT EXISTS Students (
                    StudentCode TEXT PRIMARY KEY,
                    NationalCode TEXT,
                    DormitoryName TEXT,
                    BlockName TEXT,
                    RoomNumber TEXT
                );",

                @"CREATE TABLE IF NOT EXISTS Dormitories (
                    Name TEXT PRIMARY KEY,
                    Capacity INTEGER,
                    Address TEXT,
                    SupervisorCode TEXT
                );",

                @"CREATE TABLE IF NOT EXISTS Blocks (
                    Name TEXT PRIMARY KEY,
                    Floors INTEGER,
                    DormitoryName TEXT,
                    SupervisorStudentCode TEXT
                );",

                @"CREATE TABLE IF NOT EXISTS Rooms (
                    RoomNumber TEXT PRIMARY KEY,
                    Floor INTEGER,
                    Capacity INTEGER,
                    BlockName TEXT
                );"
            };

            foreach (var query in queries)
            {
                var cmd = new SQLiteCommand(query, conn);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Database initialized successfully.");
        }
    }

    public class Person
    {
        public string FirstName;
        public string LastName;
        public string NationalCode;
        public string Address;
        public string Phone;

        public Person(string nationalCode)
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Persons WHERE NationalCode = @code", conn);
                cmd.Parameters.AddWithValue("@code", nationalCode);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    FirstName = reader["FirstName"].ToString();
                    LastName = reader["LastName"].ToString();
                    NationalCode = reader["NationalCode"].ToString();
                    Address = reader["Address"].ToString();
                    Phone = reader["Phone"].ToString();
                }
            }
        }

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
        public List<Person> People = new();

        public void LoadAll()
        {
            People.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Persons", conn);
                var reader = cmd.ExecuteReader();
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




        public Person GetOrCreatePersonInteractive(string nationalCode)
        {
            var found = People.Find(p => p.NationalCode == nationalCode);
            if (found != null)
            {
                Console.WriteLine("Person found.");
                return found;
            }

            Console.WriteLine("Person not found. Please enter details.");
            Console.Write("First Name: ");
            string fname = Console.ReadLine();
            Console.Write("Last Name: ");
            string lname = Console.ReadLine();
            Console.Write("Address: ");
            string addr = Console.ReadLine();
            Console.Write("Phone: ");
            string phone = Console.ReadLine();

            var newPerson = new Person(fname, lname, nationalCode, addr, phone);
            newPerson.Save();
            People.Add(newPerson);
            return newPerson;
        }
    }

    public class Student
    {
        public string StudentCode;
        public string NationalCode;
        public string DormitoryName;
        public string BlockName;
        public string RoomNumber;

        public Student(string code)
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Students WHERE StudentCode = @code", conn);
                cmd.Parameters.AddWithValue("@code", code);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    StudentCode = code;
                    NationalCode = reader["NationalCode"].ToString();
                    DormitoryName = reader["DormitoryName"].ToString();
                    BlockName = reader["BlockName"].ToString();
                    RoomNumber = reader["RoomNumber"].ToString();
                }
            }
        }

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
        public List<Student> Students = new();

        public void LoadAll()
        {
            Students.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Students", conn);
                var reader = cmd.ExecuteReader();
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


        public void Add(Student student)
        {
            student.Save();
            Students.Add(student);
        }


        public void List()
        {
            foreach (var s in Students)
            {
                Console.WriteLine($"StudentCode: {s.StudentCode}, NationalCode: {s.NationalCode}, Dorm: {s.DormitoryName}, Block: {s.BlockName}, Room: {s.RoomNumber}");
            }
        }
    }

    public class Dormitory
    {
        public string Name;
        public int Capacity;
        public string Address;
        public string SupervisorCode;

        public Dormitory(string name, int capacity, string address, string supervisorCode)
        {
            Name = name;
            Capacity = capacity;
            Address = address;
            SupervisorCode = supervisorCode;
        }

        public void Save()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("REPLACE INTO Dormitories (Name, Capacity, Address, SupervisorCode) VALUES (@n, @c, @a, @s)", conn);
                cmd.Parameters.AddWithValue("@n", Name);
                cmd.Parameters.AddWithValue("@c", Capacity);
                cmd.Parameters.AddWithValue("@a", Address);
                cmd.Parameters.AddWithValue("@s", SupervisorCode);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class DormitoryManager
    {
        public List<Dormitory> Dorms = new();
        private PersonManager personManager;

        public DormitoryManager(PersonManager pm)
        {
            personManager = pm;
        }

        public void LoadAll()
        {
            Dorms.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Dormitories", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dorms.Add(new Dormitory(
                        reader["Name"].ToString(),
                        int.Parse(reader["Capacity"].ToString()),
                        reader["Address"].ToString(),
                        reader["SupervisorCode"].ToString()
                    ));
                }
            }
        }

        public void Add(Dormitory dorm)
        {
            if (!personManager.People.Exists(p => p.NationalCode == dorm.SupervisorCode))
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
                Console.WriteLine($"Dorm: {d.Name}, Capacity: {d.Capacity}, Address: {d.Address}, Supervisor: {d.SupervisorCode}");
            }
        }
    }
    public class Block
    {
        public string Name;
        public int Floors;
        public string DormitoryName;
        public string SupervisorStudentCode;

        public Block(string name, int floors, string dormitoryName)
        {
            Name = name;
            Floors = floors;
            DormitoryName = dormitoryName;
        }

        public void Save()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("REPLACE INTO Blocks (Name, Floors, DormitoryName, SupervisorStudentCode) VALUES (@n, @f, @d, @s)", conn);
                cmd.Parameters.AddWithValue("@n", Name);
                cmd.Parameters.AddWithValue("@f", Floors);
                cmd.Parameters.AddWithValue("@d", DormitoryName);
                cmd.Parameters.AddWithValue("@s", SupervisorStudentCode);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class BlockManager
    {
        public List<Block> Blocks = new();

        public void LoadAll()
        {
            Blocks.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Blocks", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Blocks.Add(new Block(
                        reader["Name"].ToString(),
                        int.Parse(reader["Floors"].ToString()),
                        reader["DormitoryName"].ToString()
                    )
                    {
                        SupervisorStudentCode = reader["SupervisorStudentCode"].ToString()
                    });
                }
            }
        }

        public void Add(Block block)
        {
            block.Save();
            Blocks.Add(block);
        }



        public void List()
        {
            foreach (var b in Blocks)
            {
                Console.WriteLine($"Block: {b.Name}, Floors: {b.Floors}, Dormitory: {b.DormitoryName}, Supervisor: {b.SupervisorStudentCode}");
            }
        }
    }

    public class Room
    {
        public string RoomNumber;
        public int Floor;
        public int Capacity;
        public string BlockName;

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
        public List<Room> Rooms = new();

        public void LoadAll()
        {
            Rooms.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Rooms", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Rooms.Add(new Room(
                        reader["RoomNumber"].ToString(),
                        int.Parse(reader["Floor"].ToString()),
                        int.Parse(reader["Capacity"].ToString()),
                        reader["BlockName"].ToString()
                    ));
                }
            }
        }

        public void Add(Room room)
        {
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