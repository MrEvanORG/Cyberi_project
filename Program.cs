using System;
using System.Linq;
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
                        | exit         to exit the program.                             |
                        | dormitory    show all commands related to dormitories.        |
                        | block        show all commands related to blocks of a dormitory.|
                        | person       show all commands related to people and students.  |
                        | item         show all commands related to student items.      |
                    example : help item

                    """;
            }
            else if (parts.Length == 2)
            {
                switch (parts[1])
                {
                    default:
                        text = "Sorry, there is no such subject to show help for!";
                        break;
                    case "dormitory":
                        text = """

                            Dormitory commands : 
                                | exit        back to main menu.                                     |
                                | list        show list of all dormitories.                          |
                                | add         add a new dormitory to the program.                      |
                                | rem [name]  remove a dormitory and all its related blocks/students.|

                            """;
                        break;
                    case "block":
                        text = """

                            Block commands : 
                                | exit        back to main menu.                                       |
                                | list        show list of all blocks and their room ranges.           |
                                | add         add a new block and its rooms automatically.             |
                                | rem [name]  remove a block and all its related rooms/students.       |

                            """;
                        break;
                    case "person":
                        text = """

                            Person commands : 
                                | exit        back to main menu.                                         |
                                | list        show list of all people or students.                     |
                                | add         add a new person or student.                             |
                                | rem         remove a person (and their student record) or just a student. |

                            """;
                        break;
                    case "item":
                        text = """

                            Item commands : 
                                | exit              back to main menu.                  |
                                | list              show list of all items.             |
                                | add               add a new item for a student.       |
                                | rem [partNumber]  remove an item by its part number.  |

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
            Database.GetConnection().Close();

            var personManager = new PersonManager();
            var studentManager = new StudentManager();
            var itemManager = new PersonItemManager();
            var roomManager = new RoomManager();
            var blockManager = new BlockManager();
            var dormManager = new DormitoryManager();

            personManager.LoadAll();
            studentManager.LoadAll();
            itemManager.LoadAll();
            roomManager.LoadAll();
            blockManager.LoadAll();
            dormManager.LoadAll();

            Console.WriteLine("--- Dormitory Management System ---");
            ShowHelp("help");

            while (true)
            {
                Console.Write("$ main> ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (input == "exit") break;

                var cmdParts = input.Split(' ');
                var command = cmdParts[0];

                switch (command)
                {
                    case "person":
                        HandlePersonMenu(personManager, studentManager, itemManager, dormManager, blockManager, roomManager);
                        break;

                    case "dormitory":
                        HandleDormitoryMenu(dormManager, blockManager, roomManager, studentManager, itemManager, personManager);
                        break;

                    case "block":
                        HandleBlockMenu(blockManager, dormManager, roomManager, studentManager, itemManager);
                        break;

                    case "item":
                        HandleItemMenu(itemManager, studentManager);
                        break;

                    case "help":
                        ShowHelp(input);
                        break;

                    default:
                        Console.WriteLine("Invalid command. Type 'help' to see available commands.");
                        break;
                }
            }
        }

        #region Menu Handlers

        static void HandleItemMenu(PersonItemManager itemManager, StudentManager studentManager)
        {
            while (true)
            {
                Console.Write("$ main/item> ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (input == "exit") break;

                var cmdParts = input.Split(' ');
                var command = cmdParts[0];

                switch (command)
                {
                    case "add":
                        AddItem(itemManager, studentManager);
                        break;

                    case "list":
                        itemManager.List(studentManager);
                        break;

                    case "rem":
                        if (cmdParts.Length < 2)
                        {
                            Console.WriteLine("Usage: rem [partNumber]");
                            break;
                        }
                        itemManager.Remove(cmdParts[1]);
                        break;

                    case "help":
                        ShowHelp("help item");
                        break;

                    default:
                        Console.WriteLine("Invalid command. Use 'add', 'list', 'rem', 'help' or 'exit'.");
                        break;
                }
            }
        }

        static void HandlePersonMenu(PersonManager personManager, StudentManager studentManager, PersonItemManager itemManager, DormitoryManager dormManager, BlockManager blockManager, RoomManager roomManager)
        {
            while (true)
            {
                Console.Write("$ main/person> ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (input == "exit") break;

                var cmdParts = input.Split(' ');
                var command = cmdParts[0];

                switch (command)
                {
                    case "add":
                        Console.Write("Add a new person or student? (person/student): ");
                        var kind = Console.ReadLine()?.Trim().ToLower();

                        if (kind == "person")
                        {
                            AddPerson(personManager);
                        }
                        else if (kind == "student")
                        {
                            AddStudent(personManager, studentManager, dormManager, blockManager, roomManager);
                        }
                        else
                        {
                            Console.WriteLine("Invalid type. Please choose 'person' or 'student'.");
                        }
                        break;

                    case "list":
                        ListPeopleAndStudents(personManager, studentManager);
                        break;

                    case "rem":
                        Console.Write("Remove a 'person' (including student record) or just a 'student'? (person/student): ");
                        var remType = Console.ReadLine()?.Trim().ToLower();
                        if (remType == "person")
                        {
                            Console.Write("Enter National Code of the person to remove: ");
                            var nationalCode = Console.ReadLine();
                            personManager.Remove(nationalCode, studentManager, itemManager);
                        }
                        else if (remType == "student")
                        {
                            Console.Write("Enter Student Code of the student to remove: ");
                            var studentCode = Console.ReadLine();
                            studentManager.Remove(studentCode, itemManager);
                        }
                        else
                        {
                            Console.WriteLine("Invalid type.");
                        }
                        break;

                    case "help":
                        ShowHelp("help person");
                        break;

                    default:
                        Console.WriteLine("Invalid command. Use 'add', 'list', 'rem', 'help' or 'exit'.");
                        break;
                }
            }
        }

        static void HandleDormitoryMenu(DormitoryManager dormManager, BlockManager blockManager, RoomManager roomManager, StudentManager studentManager, PersonItemManager itemManager, PersonManager personManager)
        {
            while (true)
            {
                Console.Write("$ main/dormitory> ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (input == "exit") break;

                var cmdParts = input.Split(' ');
                var command = cmdParts[0];

                switch (command)
                {
                    case "add":
                        AddDormitory(dormManager, personManager);
                        break;

                    case "list":
                        Console.WriteLine("--- List of All Dormitories ---");
                        dormManager.List();
                        Console.WriteLine("--- End of List ---");
                        break;

                    case "rem":
                        if (cmdParts.Length < 2)
                        {
                            Console.WriteLine("Usage: rem [dormitoryName]");
                            break;
                        }
                        dormManager.Remove(cmdParts[1], blockManager, roomManager, studentManager, itemManager);
                        break;

                    case "help":
                        ShowHelp("help dormitory");
                        break;

                    default:
                        Console.WriteLine("Invalid command. Use 'add', 'list', 'rem', 'help' or 'exit'.");
                        break;
                }
            }
        }

        static void HandleBlockMenu(BlockManager blockManager, DormitoryManager dormManager, RoomManager roomManager, StudentManager studentManager, PersonItemManager itemManager)
        {
            while (true)
            {
                Console.Write("$ main/block> ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (input == "exit") break;

                var cmdParts = input.Split(' ');
                var command = cmdParts[0];

                switch (command)
                {
                    case "add":
                        AddBlock(blockManager, dormManager, roomManager);
                        break;

                    case "list":
                        blockManager.List(roomManager);
                        break;

                    case "rem":
                        if (cmdParts.Length < 2)
                        {
                            Console.WriteLine("Usage: rem [blockName]");
                            break;
                        }
                        blockManager.Remove(cmdParts[1], roomManager, studentManager, itemManager);
                        break;

                    case "help":
                        ShowHelp("help block");
                        break;

                    default:
                        Console.WriteLine("Invalid command. Use 'add', 'list', 'rem', 'help' or 'exit'.");
                        break;
                }
            }
        }
        #endregion

        #region Add/List Methods

        static void AddItem(PersonItemManager itemManager, StudentManager studentManager)
        {
            Console.Write("Enter Student Code to assign an item to: ");
            var studentCode = Console.ReadLine();

            if (!studentManager.Students.Exists(s => s.StudentCode == studentCode))
            {
                Console.WriteLine("Error: Student with this code not found.");
                return;
            }

            Console.WriteLine("Select Item Type:");
            Console.WriteLine("  100:  (Bed)");
            Console.WriteLine("  200:  (Closet)");
            Console.WriteLine("  300:  (Desk)");
            Console.WriteLine("  400:  (Chair)");
            Console.Write("Enter item type code: ");
            var itemTypeCode = Console.ReadLine();

            if (itemTypeCode != "100" && itemTypeCode != "200" && itemTypeCode != "300" && itemTypeCode != "400")
            {
                Console.WriteLine("Error: Invalid item type code.");
                return;
            }

            string partNumber = itemTypeCode + studentCode;

            if (itemManager.Items.Exists(i => i.PartNumber == partNumber))
            {
                Console.WriteLine("Error: This exact item (same type for same student) already exists.");
                return;
            }

            var newItem = new PersonItem(partNumber, studentCode);
            itemManager.Add(newItem);
            Console.WriteLine($"Item with PartNumber '{partNumber}' created and assigned to student '{studentCode}'.");
        }

        static void AddPerson(PersonManager personManager)
        {
            Console.Write("Enter National Code: ");
            var code = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(code) || personManager.People.Exists(p => p.NationalCode == code))
            {
                Console.WriteLine("Error: A person with this national code already exists or the code is invalid.");
                return;
            }
            Console.Write("Enter First Name: ");
            var fname = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            var lname = Console.ReadLine();
            Console.Write("Enter Address: ");
            var addr = Console.ReadLine();
            Console.Write("Enter Phone Number: ");
            var phone = Console.ReadLine();

            var p = new Person(fname, lname, code, addr, phone);
            personManager.Add(p);
            Console.WriteLine("Person added successfully.");
        }

        static void AddStudent(PersonManager personManager, StudentManager studentManager, DormitoryManager dormManager, BlockManager blockManager, RoomManager roomManager)
        {
            Console.Write("Enter National Code: ");
            var nc = Console.ReadLine();

            if (studentManager.Students.Exists(s => s.NationalCode == nc))
            {
                Console.WriteLine("Error: A student with this national code already exists.");
                return;
            }

            var person = personManager.People.Find(x => x.NationalCode == nc);
            if (person == null)
            {
                Console.WriteLine("This person is not registered. Please enter their details first.");
                AddPerson(personManager);
                person = personManager.People.Find(x => x.NationalCode == nc);
                if (person == null) return; // Exit if person creation was cancelled or failed
            }

            Console.Write("Enter Student Code: ");
            var sc = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(sc) || studentManager.Students.Exists(s => s.StudentCode == sc))
            {
                Console.WriteLine("Error: A student with this student code already exists or the code is invalid.");
                return;
            }

            if (!dormManager.Dorms.Any())
            {
                Console.WriteLine("Error: No dormitories available. Please add a dormitory first.");
                return;
            }
            Console.WriteLine("--- Available Dormitories ---");
            dormManager.List();
            Console.Write("Enter dormitory name: ");
            string dormName = Console.ReadLine();
            if (!dormManager.Dorms.Exists(d => d.Name.Equals(dormName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: Dormitory not found.");
                return;
            }

            var blocksInDorm = blockManager.Blocks.Where(b => b.DormitoryName.Equals(dormName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!blocksInDorm.Any())
            {
                Console.WriteLine("Error: No blocks available in this dormitory. Please add a block first.");
                return;
            }
            Console.WriteLine($"--- Available Blocks in {dormName} ---");
            foreach (var b in blocksInDorm) Console.WriteLine($"- {b.Name}");
            Console.Write("Enter block name: ");
            string blockName = Console.ReadLine();
            if (!blocksInDorm.Exists(b => b.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: Block not found in this dormitory.");
                return;
            }

            var roomsInBlock = roomManager.Rooms.Where(r => r.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!roomsInBlock.Any())
            {
                Console.WriteLine("Error: No rooms found in this block. The block might not have been created correctly.");
                return;
            }

            Console.WriteLine($"--- Available Rooms in {blockName} ---");
            var roomsByFloor = roomsInBlock.GroupBy(r => r.Floor).OrderBy(g => g.Key);

            foreach (var floorGroup in roomsByFloor)
            {
                var roomNumbers = floorGroup.Select(r => int.Parse(r.RoomNumber)).OrderBy(n => n).ToList();
                if (roomNumbers.Any())
                {
                    Console.WriteLine($"Floor {floorGroup.Key}: Rooms from {roomNumbers.First()} to {roomNumbers.Last()}");
                }
            }

            Console.Write("Enter room number: ");
            string roomNumber = Console.ReadLine();
            var selectedRoom = roomManager.Rooms.Find(r => r.RoomNumber == roomNumber && r.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase));

            if (selectedRoom == null)
            {
                Console.WriteLine("Error: Room not found in this block.");
                return;
            }

            var studentsInRoomCount = studentManager.Students.Count(s => s.RoomNumber == roomNumber && s.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase));
            if (studentsInRoomCount >= selectedRoom.Capacity)
            {
                Console.WriteLine($"Error: Room {roomNumber} is full ({studentsInRoomCount}/{selectedRoom.Capacity}).");
                return;
            }

            var s = new Student(sc, nc, dormName, blockName, roomNumber);
            studentManager.Add(s);
            Console.WriteLine($"Student successfully added to Room {roomNumber}. Current occupancy: {studentsInRoomCount + 1}/{selectedRoom.Capacity}");
        }

        static void ListPeopleAndStudents(PersonManager personManager, StudentManager studentManager)
        {
            Console.Write("List persons or students? (person/student): ");
            var listKind = Console.ReadLine()?.Trim().ToLower();
            if (listKind == "person")
            {
                Console.WriteLine("--- List of All Persons ---");
                foreach (var p in personManager.People)
                {
                    Console.WriteLine($"Name: {p.FirstName} {p.LastName}, National Code: {p.NationalCode}, Phone: {p.Phone}, Address: {p.Address}");
                }
                Console.WriteLine("--- End of List ---");
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
                Console.WriteLine("--- End of List ---");
            }
            else
            {
                Console.WriteLine("Invalid type.");
            }
        }

        static void AddDormitory(DormitoryManager dormManager, PersonManager personManager)
        {
            Console.Write("Enter Dormitory Name: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name) || dormManager.Dorms.Exists(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: A dormitory with this name already exists or the name is invalid.");
                return;
            }
            Console.Write("Enter Address: ");
            string addr = Console.ReadLine();
            Console.Write("Enter Supervisor's National Code: ");
            string sup = Console.ReadLine();
            if (!personManager.People.Exists(p => p.NationalCode == sup))
            {
                Console.WriteLine("Error: Supervisor with this national code not found. Please register the person first.");
                return;
            }

            dormManager.Add(new Dormitory(name, addr, sup));
            Console.WriteLine("Dormitory added successfully.");
        }

        static void AddBlock(BlockManager blockManager, DormitoryManager dormManager, RoomManager roomManager)
        {
            if (!dormManager.Dorms.Any())
            {
                Console.WriteLine("Error: No dormitories exist. Please add a dormitory first.");
                return;
            }
            Console.WriteLine("--- Available Dormitories ---");
            dormManager.List();
            Console.Write("Enter the name of the dormitory for this block: ");
            string dormName = Console.ReadLine();
            if (!dormManager.Dorms.Exists(d => d.Name.Equals(dormName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: Dormitory not found.");
                return;
            }

            Console.Write("Enter Block Name: ");
            string blockName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(blockName) || blockManager.Blocks.Exists(b => b.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: A block with this name already exists or the name is invalid.");
                return;
            }

            Console.Write("Enter Total Capacity for the block: ");
            if (!int.TryParse(Console.ReadLine(), out int capacity) || capacity <= 0)
            {
                Console.WriteLine("Invalid capacity. Please enter a positive number.");
                return;
            }

            Console.Write("Enter Number of Floors: ");
            if (!int.TryParse(Console.ReadLine(), out int floors) || floors <= 0)
            {
                Console.WriteLine("Invalid number of floors. Please enter a positive number.");
                return;
            }

            if (capacity % (floors * 6) != 0)
            {
                Console.WriteLine($"Warning: The capacity {capacity} is not perfectly divisible by {floors} floors with 6-person rooms.");
                Console.WriteLine("This may result in unused space. Continue? (y/n)");
                if (Console.ReadLine()?.ToLower() != "y") return;
            }

            var block = new Block(blockName, floors, capacity, dormName);
            blockManager.Add(block, roomManager);
            Console.WriteLine("Block and its rooms created successfully.");
        }
        #endregion
    }
}