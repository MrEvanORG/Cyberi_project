using System;
namespace AllClass
{
    public class Person
    {
        public string Name;
        public string NationalCode;
        public string PhoneNumber;
        public string Address;
    }



    public class Dormitory
    {
        public string Name;
        public string Address;
        public string Capacity;
        public DormitoryManeger DormitoryManager;
        public Block Blocks;
    }



    public class Block
    {
        public string Name;
        public int FloorCount;
        public int RoomCount;
        public BlockManneger BlockSupervisor;
        public Room Rooms;
        public Dormitory Dormitory;
    }



    public class Room
    {
        public int RoomNumber;
        public int Floor;
        public int Capacity; // max 6 person 
        public List<Item> Items;
        public List<Student> Residents;
        public Block Block;
    }



    public class Item
    {
        public string Type;
        public string PartNumber;
        public string AssetNumber;
        public string Status;
        public Room AssignedRoom;
        public Student OwningStudent;
    }



    public class Student : Person
    {
        public string StudentId;
        public Room AssignedRoom;
        public Block AssignedBlock;
        public Dormitory AssignedDormitory;
        public Item PersonalItem;
    }



    public class DormitoryManeger : Person
    {
        public string Position;
        public Dormitory ManagedDormitory;
    }



    public class BlockManneger : Student
    {
        public string Position;
        public Block ManagedBlock;
    }




}