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



    public class Dormintory { }



    public class Block { }



    public class Room { }



    public class Item { }



    public class Student : Person
    {
        public string StudentId;
    }



    public class BlochManeger : Student { }


    
    public class DormintoryManeger : Person { }
    
}