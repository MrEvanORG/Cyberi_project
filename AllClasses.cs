using System;
namespace AllClass
{
    public class Person
    {
        public string Name { get; set; };
        public string NationalCode { get; set; };
        public string PhoneNumber { get; set; };
        public string Address { get; set; };

        public Person(string name, string nationalCode, string phoneNumber, string address)
        {
            Name = name;
            NationalCode = nationalCode;
            PhoneNumber = phoneNumber;
            Address = address;
        }
    }



    public class Dormitory
    {
        public string Name { get; set; };
        public string Address { get; set; };
        public string Capacity { get; set; };
        public DormitoryManeger DormitoryManager { get; set; };
        public Block Blocks { get; set; };

        public Dormitory(string name, string address, int capacity)
        {
            Name = name;
            Address = address;
            Capacity = capacity;
            Blocks = new List<Block>(); // Initialize the list
        }
    }



    public class Block
    {
        public string Name { get; set; };
        public int FloorCount { get; set; };
        public int RoomCount { get; set; };
        public BlockManneger BlockSupervisor { get; set; };
        public Room Rooms { get; set; };
        public Dormitory Dormitory { get; set; };

        public Block(string name, int floorCount, int roomCount, Dormitory dormitory)
        {
            Name = name;
            FloorCount = floorCount;
            RoomCount = roomCount;
            Dormitory = dormitory;
            Rooms = new List<Room>(); // Initialize the list
        }
    }



    public class Room
    {
        public int RoomNumber { get; set; };
        public int Floor { get; set; };
        public int Capacity { get; set; }; // max 6 person 
        public List<Item> Items { get; set; };
        public List<Student> Residents { get; set; };
        public Block Block { get; set; };

        public Room(int roomNumber, int floor, Block block)
        {
            RoomNumber = roomNumber;
            Floor = floor;
            Block = block;
            Capacity = 6; 
            Items = new List<Item>(); 
            Residents = new List<Student>(); 
        }
    }



    public class Item
    {
        public string Type { get; set; };
        public string PartNumber { get; set; };
        public string AssetNumber { get; set; };
        public string Status { get; set; };
        public Room AssignedRoom { get; set; };
        public Student OwningStudent { get; set; };

        public Item(ItemType type, string partNumber, string assetNumber, ItemStatus status)
        {
            Type = type;
            PartNumber = partNumber;
            AssetNumber = assetNumber; // This will likely be generated elsewhere
            Status = status;
        }
    }



    public class Student : Person
    {
        public string StudentId { get; set; };
        public Room AssignedRoom { get; set; };
        public Block AssignedBlock { get; set; };
        public Dormitory AssignedDormitory { get; set; };
        public Item PersonalItem { get; set; };

        public Student(string name, string nationalCode, string phoneNumber, string address, string studentId)
            : base(name, nationalCode, phoneNumber, address)
        {
            StudentId = studentId;
            PersonalItems = new List<Item>(); // Initialize the list
        }
    }



    public class DormitoryManeger : Person
    {
        public string Position { get; set; };
        public Dormitory ManagedDormitory { get; set; };

        public DormitoryManager(string name, string nationalCode, string phoneNumber, string address, string position)
            : base(name, nationalCode, phoneNumber, address)
        {
            Position = position;
        }
    }



    public class BlockManneger : Student
    {
        public string Position { get; set; };
        public Block ManagedBlock { get; set; };

        public BlockManager(string name, string nationalCode, string phoneNumber, string address, string studentId, string position)
            : base(name, nationalCode, phoneNumber, address, studentId)
        {
            Position = position; // Typically "مسئول بلوک"
        }
    }




}