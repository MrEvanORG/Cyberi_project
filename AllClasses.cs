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
                "PRAGMA foreign_keys = ON;",
                @"CREATE TABLE IF NOT EXISTS Persons (
                    FirstName TEXT NOT NULL, LastName TEXT NOT NULL, NationalCode TEXT PRIMARY KEY,
                    Address TEXT, Phone TEXT );",
                @"CREATE TABLE IF NOT EXISTS Dormitories ( Name TEXT PRIMARY KEY, Address TEXT, SupervisorCode TEXT );",
                @"CREATE TABLE IF NOT EXISTS Blocks (
                    Name TEXT PRIMARY KEY, Floors INTEGER NOT NULL, Capacity INTEGER NOT NULL,
                    DormitoryName TEXT NOT NULL,
                    FOREIGN KEY (DormitoryName) REFERENCES Dormitories(Name) ON DELETE CASCADE );",
                @"CREATE TABLE IF NOT EXISTS Rooms (
                    RoomNumber TEXT NOT NULL, Floor INTEGER NOT NULL, Capacity INTEGER NOT NULL, BlockName TEXT NOT NULL,
                    PRIMARY KEY (RoomNumber, BlockName),
                    FOREIGN KEY (BlockName) REFERENCES Blocks(Name) ON DELETE CASCADE );",
                @"CREATE TABLE IF NOT EXISTS Students (
                    StudentCode TEXT PRIMARY KEY, NationalCode TEXT NOT NULL UNIQUE, DormitoryName TEXT, BlockName TEXT, RoomNumber TEXT,
                    FOREIGN KEY (NationalCode) REFERENCES Persons(NationalCode) ON DELETE CASCADE,
                    FOREIGN KEY (BlockName) REFERENCES Blocks(Name) ON DELETE SET NULL,
                    FOREIGN KEY (DormitoryName) REFERENCES Dormitories(Name) ON DELETE SET NULL );",
                @"CREATE TABLE IF NOT EXISTS PersonItems (
                    PartNumber TEXT PRIMARY KEY, StudentCode TEXT NOT NULL,
                    FOREIGN KEY (StudentCode) REFERENCES Students(StudentCode) ON DELETE CASCADE );"
            };

            foreach (var query in queries)
            {
                using (var cmd = new SQLiteCommand(query, conn)) { cmd.ExecuteNonQuery(); }
            }
            Console.WriteLine("Database initialized successfully.");
        }
    }

    #region Base Deletable Class
    public abstract class Deletable
    {
        protected abstract string TableName { get; }
        protected abstract string PrimaryKeyColumn { get; }
        protected abstract object PrimaryKeyValue { get; }

        public void Delete()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand($"DELETE FROM {TableName} WHERE {PrimaryKeyColumn} = @id", conn);
                cmd.Parameters.AddWithValue("@id", PrimaryKeyValue);
                cmd.ExecuteNonQuery();
            }
        }
    }
    #endregion

    #region Item
    public class PersonItem : Deletable
    {
        public string PartNumber { get; set; }
        public string StudentCode { get; set; }
        protected override string TableName => "PersonItems";
        protected override string PrimaryKeyColumn => "PartNumber";
        protected override object PrimaryKeyValue => PartNumber;

        public PersonItem(string partNumber, string studentCode)
        {
            PartNumber = partNumber;
            StudentCode = studentCode;
        }
        public void Save()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("REPLACE INTO PersonItems (PartNumber, StudentCode) VALUES (@pn, @sc)", conn);
                cmd.Parameters.AddWithValue("@pn", PartNumber);
                cmd.Parameters.AddWithValue("@sc", StudentCode);
                cmd.ExecuteNonQuery();
            }
        }
        public string GetItemType()
        {
            if (PartNumber.StartsWith("100")) return " (Bed)";
            if (PartNumber.StartsWith("200")) return " (Closet)";
            if (PartNumber.StartsWith("300")) return " (Desk)";
            if (PartNumber.StartsWith("400")) return " (Chair)";
            return " (Unknown)";
        }
    }

    public class PersonItemManager
    {
        public List<PersonItem> Items { get; private set; } = new List<PersonItem>();
        public void LoadAll()
        {
            Items.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM PersonItems", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Items.Add(new PersonItem(reader["PartNumber"].ToString(), reader["StudentCode"].ToString()));
                    }
                }
            }
        }
        public void Add(PersonItem item) { item.Save(); Items.Add(item); }
        public void List(StudentManager studentManager)
        {
            Console.WriteLine("--- List of All Items ---");
            if (!Items.Any()) { Console.WriteLine("No items found."); return; }
            foreach (var item in Items)
            {
                var student = studentManager.Students.Find(s => s.StudentCode == item.StudentCode);
                string location = student != null ? $"{student.DormitoryName}/{student.BlockName}/{student.RoomNumber}" : "N/A";
                Console.WriteLine($"- Part Number: {item.PartNumber}, Type: {item.GetItemType()}, Owner Code: {item.StudentCode}, Location: {location}");
            }
            Console.WriteLine("--- End of List ---");
        }
        public void Remove(string partNumber)
        {
            var item = Items.Find(i => i.PartNumber.Equals(partNumber, StringComparison.OrdinalIgnoreCase));
            if (item == null) { Console.WriteLine("Error: Item not found."); return; }
            Console.Write($"Are you sure you want to delete item {item.PartNumber} ({item.GetItemType()})? (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y") { Console.WriteLine("Deletion cancelled."); return; }
            item.Delete();
            Items.Remove(item);
            Console.WriteLine("Item removed successfully.");
        }
        public void RemoveByStudent(string studentCode)
        {
            var itemsToRemove = Items.Where(i => i.StudentCode == studentCode).ToList();
            foreach (var item in itemsToRemove)
            {
                item.Delete();
                Items.Remove(item);
            }
            if (itemsToRemove.Any()) Console.WriteLine($"{itemsToRemove.Count} items removed for student {studentCode}.");
        }
    }
    #endregion

    #region Student
    public class Student : Deletable
    {
        public string StudentCode { get; set; }
        public string NationalCode { get; set; }
        public string DormitoryName { get; set; }
        public string BlockName { get; set; }
        public string RoomNumber { get; set; }
        protected override string TableName => "Students";
        protected override string PrimaryKeyColumn => "StudentCode";
        protected override object PrimaryKeyValue => StudentCode;
        public Student(string studentCode, string nationalCode, string dorm, string block, string room)
        {
            StudentCode = studentCode; NationalCode = nationalCode; DormitoryName = dorm; BlockName = block; RoomNumber = room;
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
                        Students.Add(new Student(reader["StudentCode"].ToString(), reader["NationalCode"].ToString(),
                            reader["DormitoryName"].ToString(), reader["BlockName"].ToString(), reader["RoomNumber"].ToString()));
                    }
                }
            }
        }
        public void Add(Student student)
        {
            student.Save();
            Students.Add(student);
        }
        public void Remove(string studentCode, PersonItemManager itemManager)
        {
            var student = Students.Find(s => s.StudentCode.Equals(studentCode, StringComparison.OrdinalIgnoreCase));
            if (student == null) { Console.WriteLine("Error: Student not found."); return; }
            Console.Write($"Are you sure you want to remove student {student.StudentCode}? This will also remove all their items. (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y") { Console.WriteLine("Deletion cancelled."); return; }
            itemManager.RemoveByStudent(student.StudentCode);
            student.Delete();
            Students.Remove(student);
            Console.WriteLine($"Student {studentCode} and their items removed successfully.");
        }
    }
    #endregion

    #region Person
    public class Person : Deletable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        protected override string TableName => "Persons";
        protected override string PrimaryKeyColumn => "NationalCode";
        protected override object PrimaryKeyValue => NationalCode;
        public Person(string firstName, string lastName, string nationalCode, string address, string phone)
        {
            FirstName = firstName; LastName = lastName; NationalCode = nationalCode; Address = address; Phone = phone;
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
                        People.Add(new Person(reader["FirstName"].ToString(), reader["LastName"].ToString(),
                            reader["NationalCode"].ToString(), reader["Address"].ToString(), reader["Phone"].ToString()));
                    }
                }
            }
        }
        public void Add(Person person)
        {
            person.Save();
            People.Add(person);
        }
        public void Remove(string nationalCode, StudentManager studentManager, PersonItemManager itemManager)
        {
            var person = People.Find(p => p.NationalCode.Equals(nationalCode, StringComparison.OrdinalIgnoreCase));
            if (person == null) { Console.WriteLine("Error: Person not found."); return; }
            Console.Write($"Are you sure you want to remove {person.FirstName} {person.LastName} ({person.NationalCode})? This will also remove their student record and items. (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y") { Console.WriteLine("Deletion cancelled."); return; }
            var student = studentManager.Students.Find(s => s.NationalCode == person.NationalCode);
            if (student != null)
            {
                studentManager.Remove(student.StudentCode, itemManager);
            }
            person.Delete();
            People.Remove(person);
            Console.WriteLine($"Person {person.NationalCode} removed successfully.");
        }
    }
    #endregion

    #region Room
    public class Room : Deletable
    {
        public string RoomNumber { get; set; }
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public string BlockName { get; set; }
        protected override string TableName => "Rooms";
        protected override string PrimaryKeyColumn => "RoomNumber";
        protected override object PrimaryKeyValue => RoomNumber;
        public Room(string roomNumber, int floor, int capacity, string blockName)
        {
            RoomNumber = roomNumber; Floor = floor; Capacity = capacity; BlockName = blockName;
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
                        Rooms.Add(new Room(reader["RoomNumber"].ToString(), Convert.ToInt32(reader["Floor"]),
                            Convert.ToInt32(reader["Capacity"]), reader["BlockName"].ToString()));
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
        public void RemoveByBlock(string blockName)
        {
            var roomsToRemove = Rooms.Where(r => r.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var room in roomsToRemove)
            {
                Rooms.Remove(room);
            }
            if (roomsToRemove.Any()) Console.WriteLine($"{roomsToRemove.Count} rooms removed from memory for block {blockName}.");
        }
    }
    #endregion

    #region Block
    public class Block : Deletable
    {
        public string Name { get; set; }
        public int Floors { get; set; }
        public int Capacity { get; set; }
        public string DormitoryName { get; set; }
        protected override string TableName => "Blocks";
        protected override string PrimaryKeyColumn => "Name";
        protected override object PrimaryKeyValue => Name;
        public Block(string name, int floors, int capacity, string dormitoryName)
        {
            Name = name; Floors = floors; Capacity = capacity; DormitoryName = dormitoryName;
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
                        Blocks.Add(new Block(reader["Name"].ToString(), Convert.ToInt32(reader["Floors"]),
                            Convert.ToInt32(reader["Capacity"]), reader["DormitoryName"].ToString()));
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
            if (!Blocks.Any()) { Console.WriteLine("No blocks found."); return; }
            foreach (var b in Blocks)
            {
                Console.WriteLine($"\n- Block Name: {b.Name}, Dormitory: {b.DormitoryName}, Floors: {b.Floors}, Capacity: {b.Capacity}");
                var roomsInBlock = roomManager.Rooms.Where(r => r.BlockName.Equals(b.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                if (!roomsInBlock.Any()) { Console.WriteLine("  (No rooms found for this block)"); continue; }
                var roomsByFloor = roomsInBlock.GroupBy(r => r.Floor).OrderBy(g => g.Key);
                Console.WriteLine("  Room Ranges:");
                foreach (var floorGroup in roomsByFloor)
                {
                    var roomNumbers = floorGroup.Select(r => int.Parse(r.RoomNumber)).OrderBy(n => n).ToList();
                    if (roomNumbers.Any()) { Console.WriteLine($"    Floor {floorGroup.Key}: From {roomNumbers.First()} to {roomNumbers.Last()}"); }
                }
            }
            Console.WriteLine("\n--- End of List ---");
        }
        public void Remove(string blockName, RoomManager roomManager, StudentManager studentManager, PersonItemManager itemManager)
        {
            var block = Blocks.Find(b => b.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase));
            if (block == null) { Console.WriteLine("Error: Block not found."); return; }
            Console.Write($"Are you sure you want to remove block {block.Name}? This will remove all its rooms, students, and their items. (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y") { Console.WriteLine("Deletion cancelled."); return; }

            block.Delete();
            var studentsInBlock = studentManager.Students.Where(s => s.BlockName.Equals(block.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var student in studentsInBlock)
            {
                itemManager.RemoveByStudent(student.StudentCode);
                studentManager.Students.Remove(student);
            }
            roomManager.RemoveByBlock(block.Name);
            Blocks.Remove(block);
            Console.WriteLine($"Block {blockName} and all its contents removed successfully.");
        }
    }
    #endregion

    #region Dormitory
    public class Dormitory : Deletable
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string SupervisorCode { get; set; }
        protected override string TableName => "Dormitories";
        protected override string PrimaryKeyColumn => "Name";
        protected override object PrimaryKeyValue => Name;
        public Dormitory(string name, string address, string supervisorCode)
        {
            Name = name; Address = address; SupervisorCode = supervisorCode;
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
                        Dorms.Add(new Dormitory(reader["Name"].ToString(), reader["Address"].ToString(), reader["SupervisorCode"].ToString()));
                    }
                }
            }
        }
        public void Add(Dormitory dorm)
        {
            dorm.Save();
            Dorms.Add(dorm);
        }
        public void List()
        {
            foreach (var d in Dorms) { Console.WriteLine($"- Name: {d.Name}, Address: {d.Address}, Supervisor Code: {d.SupervisorCode}"); }
        }
        public void Remove(string dormName, BlockManager blockManager, RoomManager roomManager, StudentManager studentManager, PersonItemManager itemManager)
        {
            var dorm = Dorms.Find(d => d.Name.Equals(dormName, StringComparison.OrdinalIgnoreCase));
            if (dorm == null) { Console.WriteLine("Error: Dormitory not found."); return; }
            Console.Write($"Are you sure you want to remove dormitory {dorm.Name}? This will remove ALL associated blocks, rooms, students, and items. (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y") { Console.WriteLine("Deletion cancelled."); return; }


            var blocksInDorm = blockManager.Blocks.Where(b => b.DormitoryName.Equals(dorm.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var block in blocksInDorm)
            {
                blockManager.Remove(block.Name, roomManager, studentManager, itemManager);
            }

            dorm.Delete();
            Dorms.Remove(dorm);
            Console.WriteLine($"Dormitory {dormName} and all its contents removed successfully.");
        }
    }
    #endregion
}