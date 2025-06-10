using ConsoleProject;
using System;
using System.Data.SQLite;
using DormitorySystem;

namespace ConsoleProject
{
    class Program
    {
        static void ShowHelp(string command)
        {
            string text = "";
            var parts = command.Split();

            if (parts.Length == 1)
            {
                text = """

                    Program commands : 
                    For more information on a specific command, type HELP command-name 
                        | exit         to exit the program .                            |
                        | Dormitory    show all command related to dormitories .        |
                        | Block        show all command related to blocks of dormitory .|
                        | Room         show all command related to rooms of block .     |     
                        | Person       show all command related to persons .            |
                        | Item         show all command related to items .              |
                        | Reporting    show all command related to reporting .          |
                    example : help block

                    """;
            }
            else if (parts.Length == 2)
            {
                switch (parts[1])
                {
                    default:
                        text = "Sorry there is not such subject to show help!";
                        break;
                    case "dormitory":
                        text = """

                            Dormitory commands : 
                                | exit        back to main menu .                     |
                                | List        show list of all dormitories .          |
                                | Add         add new dormitory to program .          |
                                | Edit [Name] editing dormitory informations .        |
                                | Rem [Name]  remove a dormitory with all belonging   |
                                |             this will be remove all blocks and      |
                                |             other items .                           |
                            example : rem KhabgahPesaran

                            """;
                        break;
                    case "block":
                        text = """

                            Block commands : 
                                | exit        back to main menu .                    |
                                | List        show list of all block .               |
                                | Add         add new block to selected dormitory .  |
                                | Edit [Name] editing block informations .           |
                                | Rem [Name]  remove a block with all belongin       |
                                |             this will be remove all rooms and      |
                                |             students .                             |
                            example : edit BlockAfzal

                            """;
                        break;
                    case "room":
                        text = """

                            Room commands : 
                                | exit        back to main menu .                    |
                                | List        show list of all rooms .               |
                                | Add         add new room to selected block .       |
                                | Edit [Name] editing room informations .            |
                                | Rem [Name]  remove a room with all belongin        |
                                |             this will be remove all students and   |
                                |             items .                                |
                            example : rem OtaghGolha

                            """;
                        break;
                    case "person":
                        text = """

                            Person commands : 
                                | exit        back to main menu .                    |
                                | List        show list of all persons .             |
                                | Add         add new person as supervisor/student . |
                                | Edit [Code] editing person informations .          |
                                | Rem [Code]  remove a person with all belonging     |
                                |             this will be remove all items of       |
                                |             person .                               |
                            example : edit 403405048

                            """;
                        break;
                    case "item":
                        text = """

                            Item commands : 
                                | exit        back to main menu .                    |
                                | List        show list of all items .               |
                                | Add         add new item for a person that         |
                                |             belong to the room also .              |
                                | Edit [Code] editing item informations like type.   |
                                | Rem [Code]  remove an item                         |
                            example : Rem 01002003

                            """;
                        break;
                    case "reporting":
                        text = """

                            Reporting commands : 
                                | exit        back to main menu .                          |
                                | items       show list of all items any where .           |
                                | bitems      show list of all broken items .              |
                                | ritems      show list of all repairing items .           |      
                                | roomitems    list of all items of specific room .        |
                                | studentitems list of all items that belong to an student |

                            """;
                        break;
                }
            }
            else
            {
                text = "Invalid help usage.";
            }

            Console.WriteLine(text);
        }

        static void Main(string[] args)
        {
            ShowHelp("help");
            Database.GetConnection().Close();

            var personManager = new PersonManager();
            personManager.LoadAll();

            var studentManager = new StudentManager();
            var dormManager = new DormitoryManager(personManager);
            var blockManager = new BlockManager();
            var roomManager = new RoomManager();

            studentManager.LoadAll();
            dormManager.LoadAll();
            blockManager.LoadAll();
            roomManager.LoadAll();

            while (true)
            {
                Console.Write("$ main> ");
                var input = Console.ReadLine().Trim().ToLower();
                if (input == "exit") break;
                if (input.Split()[0] == "help") ShowHelp(input);

                switch (input)
                {
                    case "person":
                        while (true)
                        {
                            Console.Write("$ main/person> ");
                            var subcmd = Console.ReadLine().Trim().ToLower();
                            if (subcmd == "exit") break;

                            string[] cmdParts = subcmd.Split(' ');
                            string command = cmdParts[0];

                            switch (command)
                            {
                                case "add":
                                    Console.Write("Add person or student? (person/student): ");
                                    var kind = Console.ReadLine().Trim().ToLower();

                                    if (kind == "person")
                                    {
                                        Console.Write("National code: ");
                                        var code = Console.ReadLine();
                                        if (personManager.People.Exists(p => p.NationalCode == code))
                                        {
                                            Console.WriteLine("Person with this national code already exists.");
                                            break;
                                        }
                                        Console.Write("First name: ");
                                        var fname = Console.ReadLine();
                                        Console.Write("Last name: ");
                                        var lname = Console.ReadLine();
                                        Console.Write("Address: ");
                                        var addr = Console.ReadLine();
                                        Console.Write("Phone: ");
                                        var phone = Console.ReadLine();
                                        var p = new Person(fname, lname, code, addr, phone);
                                        p.Save();
                                        personManager.People.Add(p);
                                        Console.WriteLine("Person added.");
                                    }
                                    else if (kind == "student")
                                    {
                                        Console.Write("Enter national code: ");
                                        var nc = Console.ReadLine();

                                        if (studentManager.Students.Exists(s => s.NationalCode == nc))
                                        {
                                            Console.WriteLine("Student with this national code already exists.");
                                            break;
                                        }

                                        var person = personManager.People.Find(x => x.NationalCode == nc);
                                        if (person == null)
                                        {
                                            Console.WriteLine("Person not found. Enter personal details.");
                                            Console.Write("First name: "); var fname2 = Console.ReadLine();
                                            Console.Write("Last name: "); var lname2 = Console.ReadLine();
                                            Console.Write("Address: "); var addr2 = Console.ReadLine();
                                            Console.Write("Phone: "); var phone2 = Console.ReadLine();
                                            person = new Person(fname2, lname2, nc, addr2, phone2);
                                            person.Save();
                                            personManager.People.Add(person);
                                        }

                                        Console.Write("Student code: ");
                                        var sc = Console.ReadLine();

                                        dormManager.LoadAll();
                                        Console.WriteLine("Available dormitories:");
                                        foreach (var dorm in dormManager.Dorms)
                                            Console.WriteLine("- " + dorm.Name);

                                        Console.Write("Enter dormitory name: ");
                                        string dormName = Console.ReadLine();
                                        if (!dormManager.Dorms.Exists(d => d.Name == dormName))
                                        {
                                            Console.WriteLine("Dormitory not found.");
                                            continue;
                                        }

                                        blockManager.LoadAll();
                                        Console.WriteLine("Available blocks in dormitory:");
                                        foreach (var b in blockManager.Blocks)
                                            if (b.DormitoryName == dormName)
                                                Console.WriteLine("- " + b.Name);

                                        Console.Write("Enter block name: ");
                                        string blockName = Console.ReadLine();
                                        if (!blockManager.Blocks.Exists(b => b.Name == blockName && b.DormitoryName == dormName))
                                        {
                                            Console.WriteLine("Block not found in dormitory.");
                                            continue;
                                        }

                                        roomManager.LoadAll();
                                        Console.WriteLine("Available rooms in block:");
                                        foreach (var r in roomManager.Rooms)
                                            if (r.BlockName == blockName)
                                                Console.WriteLine("- " + r.RoomNumber);

                                        Console.Write("Enter room number: ");
                                        string roomNumber = Console.ReadLine();
                                        if (!roomManager.Rooms.Exists(r => r.RoomNumber == roomNumber && r.BlockName == blockName))
                                        {
                                            Console.WriteLine("Room not found in block.");
                                            continue;
                                        }

                                        var s = new Student(sc, nc, dormName, blockName, roomNumber);
                                        studentManager.Add(s);
                                        Console.WriteLine("Student added.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid type. Use person or student.");
                                    }
                                    break;

                                case "list":
                                    Console.Write("List persons or students? (person/student): ");
                                    var listKind = Console.ReadLine().Trim().ToLower();
                                    if (listKind == "person")
                                    {
                                        Console.WriteLine("--- List of All Persons ---");
                                        foreach (var p in personManager.People)
                                        {
                                            Console.WriteLine($"Name: {p.FirstName} {p.LastName}, National Code: {p.NationalCode}, Phone: {p.Phone}, Address: {p.Address}");
                                        }
                                        Console.WriteLine("--- - ---");
                                    }
                                    else if (listKind == "student")
                                    {
                                        Console.WriteLine("--- List of All Students ---");
                                        foreach (var s in studentManager.Students)
                                        {
                                            var p = personManager.People.Find(x => x.NationalCode == s.NationalCode);
                                            string personInfo = (p != null) ? $"{p.FirstName} {p.LastName}" : "N/A";
                                            Console.WriteLine($"Student Code: {s.StudentCode}, Name: {personInfo}, National Code: {s.NationalCode}, Location: {s.DormitoryName}/{s.BlockName}/{s.RoomNumber}");
                                        }
                                        Console.WriteLine("--- - ---");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid type.");
                                    }
                                    break;

                                case "rem":
                                    Console.Write("Not ready yet! ");
                                    break;

                                case "edit":
                                    Console.Write("Not ready yet! ");
                                    break;
                                default:
                                    Console.WriteLine("Invalid command. Use 'add', 'list', 'rem', 'edit' or 'exit'.");
                                    break;
                            }
                        }
                        break;

                    case "dormitory":
                        while (true)
                        {
                            Console.Write("$ main/dormitory> ");
                            var cmd = Console.ReadLine().Trim();
                            if (cmd == "exit") break;

                            switch (cmd)
                            {
                                case "add":
                                    Console.Write("Name: ");
                                    string name = Console.ReadLine();
                                    Console.Write("Capacity: ");
                                    int cap = int.Parse(Console.ReadLine());
                                    Console.Write("Address: ");
                                    string addr = Console.ReadLine();
                                    Console.Write("Supervisor National Code: ");
                                    string sup = Console.ReadLine();
                                    dormManager.Add(new Dormitory(name, cap, addr, sup));
                                    break;

                                case "rem":
                                    Console.Write("Not ready yet! ");
                                    break;
                                case "edit":
                                    Console.Write("Not ready yet! ");
                                    break;

                                case "list":
                                    dormManager.List();
                                    break;
                            }
                        }
                        break;

                    case "block":
                        while (true)
                        {
                            Console.Write("$ main/block> ");
                            var cmd = Console.ReadLine().Trim();
                            if (cmd == "exit") break;

                            switch (cmd)
                            {
                                case "add":
                                    Console.Write("Block name: ");
                                    string blockName = Console.ReadLine();

                                    Console.Write("Number of floors: ");
                                    if (!int.TryParse(Console.ReadLine(), out int floors))
                                    {
                                        Console.WriteLine("Invalid number.");
                                        break;
                                    }

                                    dormManager.LoadAll();
                                    Console.WriteLine("Available dormitories:");
                                    foreach (var d in dormManager.Dorms)
                                        Console.WriteLine("- " + d.Name);

                                    Console.Write("Dormitory name for this block: ");
                                    string dormName = Console.ReadLine();
                                    if (!dormManager.Dorms.Exists(d => d.Name == dormName))
                                    {
                                        Console.WriteLine("Dormitory not found.");
                                        break;
                                    }

                                    var block = new Block(blockName, floors, dormName);
                                    blockManager.Add(block);
                                    Console.WriteLine("Block added.");
                                    break;

                                case "rem":
                                    Console.Write("Not ready yet!");
                                    break;

                                case "edit":
                                    Console.Write("Not ready yet! ");
                                    break;

                                case "list":
                                    blockManager.List();
                                    break;
                            }
                        }
                        break;

                    case "room":
                        while (true)
                        {
                            Console.Write("$ main/room> ");
                            var cmd = Console.ReadLine().Trim();
                            if (cmd == "exit") break;

                            switch (cmd)
                            {
                                case "add":
                                    Console.Write("Room Number: ");
                                    string rn = Console.ReadLine();
                                    Console.Write("Floor: ");
                                    int rf = int.Parse(Console.ReadLine());
                                    Console.Write("Capacity: ");
                                    int rc = int.Parse(Console.ReadLine());

                                    Console.WriteLine("\n--- Available Blocks ---");
                                    if (blockManager.Blocks.Count == 0)
                                    {
                                        Console.WriteLine("No blocks exist. Please add a block first.");
                                        break;
                                    }
                                    blockManager.List();
                                    Console.WriteLine("------------------------");

                                    Console.Write("Enter the name of the block for this room: ");
                                    string rb = Console.ReadLine();

                                    if (!blockManager.Blocks.Exists(b => b.Name == rb))
                                    {
                                        Console.WriteLine("Error: Block not found. Room cannot be added.");
                                        break;
                                    }

                                    roomManager.Add(new Room(rn, rf, rc, rb));
                                    Console.WriteLine("Room added successfully.");
                                    break;

                                case "rem":
                                    Console.Write("Not ready yet! ");
                                    break;

                                case "edit":
                                    Console.Write("Not ready yet! ");
                                    break;

                                case "list":
                                    roomManager.List();
                                    break;
                            }
                        }
                        break;
                }
            }
        }
    }
}