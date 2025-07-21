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
                        | exit         to exit the program.                               |
                        | dormitory    show all commands related to dormitories.          |
                        | block        show all commands related to blocks of a dormitory.|
                        | person       show all commands related to people and students.  |
                        | item         show all commands related to student items.        |
                    example : help item

                    """;
            }
            else if (parts.Length == 2)
            {
                switch (parts[1])
                {
                    default:
                        text = "Sorry, there is no such subject to show help for !";
                        break;
                    case "dormitory":
                        text = """

                            Dormitory commands : 
                                | exit        back to main menu.                                     |
                                | list        show list of all dormitories.                          |
                                | add         add a new dormitory to the program.                    |
                                | edit [name] edit a dormitory's address or supervisor.              |
                                | rem [name]  remove a dormitory and all its related blocks/students.|

                            """;
                        break;
                    case "block":
                        text = """

                            Block commands : 
                                | exit        back to main menu.                                      |
                                | list        show list of all blocks and their room ranges.          |
                                | add         add a new block and its rooms automatically.            |
                                | rem [name]  remove a block and all its related rooms/students.      |

                            """;
                        break;
                    case "person":
                        text = """

                            Person commands : 
                                | exit        back to main menu.                                           |
                                | list        show list of all people or students.                         |
                                | add         add a new person or student.                                 |
                                | edit        edit a person's details or a student's room assignment.      |
                                | rem         remove a person (and their student record) or just a student.|

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

            Console.WriteLine("---   Dormitory Management System  ------------");
            Console.WriteLine("----   Created by group of cyberi malakoot  ---");
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
                    case "add"://ok
                        AddItem(itemManager, studentManager);
                        break;

                    case "list"://ok
                        itemManager.List(studentManager);
                        break;

                    case "rem"://ok
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
                    case "add": //OK
                        Console.Write("Add A New Person Or Student? (Person/Student) : ");
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
                            Console.WriteLine("Invalid Type . Please Choose 'Person' Or 'Student' .");
                        }
                        break;

                    case "edit": //ok
                        Console.Write("Edit 'Person' Details Or 'Student' Assignment? (Person/Student) : ");
                        var editType = Console.ReadLine()?.Trim().ToLower();
                        if (editType == "person")
                        {
                            EditPerson(personManager);
                        }
                        else if (editType == "student")
                        {
                            EditStudent(studentManager, dormManager, blockManager, roomManager);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Type .");
                        }
                        break;

                    case "list": //ok
                        ListPeopleAndStudents(personManager, studentManager);
                        break;

                    case "rem"://ok
                        Console.Write("Remove A 'Person' (Including Student Object Too) Or Just A 'Student'? (Person/Student) : ");
                        var remType = Console.ReadLine()?.Trim().ToLower();
                        if (remType == "person")
                        {
                            Console.Write("Enter National Code Of The Person To Remove : ");
                            var nationalCode = Console.ReadLine();
                            personManager.Remove(nationalCode, studentManager, itemManager, dormManager);
                        }
                        else if (remType == "student")
                        {
                            Console.Write("Enter Student Code Of The Student To Remove : ");
                            var studentCode = Console.ReadLine();
                            studentManager.Remove(studentCode, itemManager);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Type .");
                        }
                        break;

                    case "help":
                        ShowHelp("help person");
                        break;

                    default:
                        Console.WriteLine("Invalid command. Use 'add', 'list', 'edit', 'rem', 'help' or 'exit'.");
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
                    case "add"://OK
                        AddDormitory(dormManager, personManager);
                        break;

                    case "edit"://ok
                        if (cmdParts.Length < 2)
                        {
                            Console.WriteLine("Usage: edit [dormitoryName]");
                            break;
                        }
                        EditDormitory(cmdParts[1], dormManager, personManager);
                        break;

                    case "list"://ok
                        Console.WriteLine("--- List of All Dormitories ---");
                        dormManager.List();
                        break;

                    case "rem"://ok
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
                        Console.WriteLine("Invalid command. Use 'add', 'list', 'edit', 'rem', 'help' or 'exit'.");
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
                    case "add"://ok
                        AddBlock(blockManager, dormManager, roomManager);
                        break;

                    case "list"://ok
                        blockManager.List(roomManager);
                        break;

                    case "rem"://ok
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

        static void EditDormitory(string dormName, DormitoryManager dormManager, PersonManager personManager)
        {
            var dorm = dormManager.Dorms.Find(d => d.Name.Equals(dormName, StringComparison.OrdinalIgnoreCase));
            if (dorm == null)
            {
                Console.WriteLine("Error: Dormitory Not Found.");
                return;
            }

            Console.WriteLine($"Editing ... Dormitory '{dorm.Name}'. Press Enter To Keep Current Value .");
            Console.WriteLine("Note : You Can't Edit Dorm Name .");

            Console.Write($"New Address ({dorm.Address}) : ");
            var newAddress = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newAddress))
            {
                dorm.Address = newAddress;
            }

            Console.Write($"New Supervisor's National Code ({dorm.SupervisorCode}): ");
            var newSupervisorCode = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newSupervisorCode))
            {
                if (!personManager.People.Exists(p => p.NationalCode == newSupervisorCode))
                {
                    Console.WriteLine("Error: No Person Found With This National Code. Update Failed .");
                    dormManager.LoadAll();
                    return;
                }
                dorm.SupervisorCode = newSupervisorCode;
            }

            dorm.Save();
            Console.WriteLine("Dormitory Updated Successfully.");
        }

        static void EditPerson(PersonManager personManager)
        {
            Console.Write("Enter National Code Of The Person To Edit : ");
            var nationalCode = Console.ReadLine();
            var person = personManager.People.Find(p => p.NationalCode.Equals(nationalCode, StringComparison.OrdinalIgnoreCase));

            if (person == null)
            {
                Console.WriteLine("Error: Person Not Found .");
                return;
            }

            Console.WriteLine($"Editing ... Person '{person.FirstName} {person.LastName}'. Press Enter And Skip To Keep Current Value.");
            Console.WriteLine("Note: National Code Cannot Be Changed .");

            Console.Write($"New First Name ({person.FirstName}) : ");
            var newFirstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newFirstName))
            {
                person.FirstName = newFirstName;
            }

            Console.Write($"New Last Name ({person.LastName}) : ");
            var newLastName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newLastName))
            {
                person.LastName = newLastName;
            }

            Console.Write($"New Address ({person.Address}) : ");
            var newAddress = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newAddress))
            {
                person.Address = newAddress;
            }

            Console.Write($"New Phone ({person.Phone}) : ");
            var newPhone = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newPhone))
            {
                person.Phone = newPhone;
            }

            person.Save();
            Console.WriteLine("Person Details Updated Successfully.");
        }

        static void EditStudent(StudentManager studentManager, DormitoryManager dormManager, BlockManager blockManager, RoomManager roomManager)
        {
            Console.Write("Enter Student Code of The Student To Edit : ");
            var studentCode = Console.ReadLine();
            var student = studentManager.Students.Find(s => s.StudentCode.Equals(studentCode, StringComparison.OrdinalIgnoreCase));

            if (student == null)
            {
                Console.WriteLine("Error: Student Not Found.");
                return;
            }

            Console.WriteLine($"Current Location For Student {student.StudentCode}: {student.DormitoryName}/{student.BlockName}/{student.RoomNumber}");
            Console.WriteLine("Enter New Location Details To Reassign The Student. Press Enter To Keep Current Assignment .");
            Console.WriteLine("Note: Student Code And National Code Cannot Be Changed .");

            if (!dormManager.Dorms.Any()) { Console.WriteLine("Error: No Dormitories Available. May Program Have Some Issue"); return; }
            Console.WriteLine("--- Available Dormitories ---");
            dormManager.List();
            Console.Write($"Enter New Dormitory Name ({student.DormitoryName}) : ");
            string dormName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dormName)) dormName = student.DormitoryName;
            if (!dormManager.Dorms.Exists(d => d.Name.Equals(dormName, StringComparison.OrdinalIgnoreCase))) { Console.WriteLine("Error: Dormitory Not Found."); return; }

            var blocksInDorm = blockManager.Blocks.Where(b => b.DormitoryName.Equals(dormName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!blocksInDorm.Any()) { Console.WriteLine("Error: No Blocks Available In This Dormitory ."); return; }
            Console.WriteLine($"--- Available Blocks in {dormName} ---");
            foreach (var b in blocksInDorm) Console.WriteLine($"- Name:{b.Name} ,Floors:{b.Floors} ,Capacity:{b.Capacity} ");
            Console.Write($"Enter New Block Name ({student.BlockName}) : ");
            string blockName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(blockName)) blockName = student.BlockName;
            if (!blocksInDorm.Exists(b => b.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase))) { Console.WriteLine("Error: Block Not Found In This Dormitory."); return; }

            var roomsInBlock = roomManager.Rooms.Where(r => r.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!roomsInBlock.Any()) { Console.WriteLine("Error: No Rooms Found In This Block ."); return; }
            Console.WriteLine($"--- Available Rooms in {blockName} ---");
            var roomsByFloor = roomsInBlock.GroupBy(r => r.Floor).OrderBy(g => g.Key);
            foreach (var floorGroup in roomsByFloor)
            {
                var roomNumbers = floorGroup.Select(r => int.Parse(r.RoomNumber)).OrderBy(n => n).ToList();
                if (roomNumbers.Any()) Console.WriteLine($"- Floor {floorGroup.Key}: Rooms from {roomNumbers.First()} to {roomNumbers.Last()}");
            }

            Console.Write($"Enter New Room Number ({student.RoomNumber}) : ");
            string roomNumber = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(roomNumber)) roomNumber = student.RoomNumber;
            var selectedRoom = roomManager.Rooms.Find(r => r.RoomNumber == roomNumber && r.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase));
            if (selectedRoom == null) { Console.WriteLine("Error: This RoomNumber Not Found In This Block. "); return; }

            if (roomNumber != student.RoomNumber || blockName != student.BlockName)
            {
                var studentsInRoomCount = studentManager.Students.Count(s => s.RoomNumber == roomNumber && s.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase));
                if (studentsInRoomCount >= selectedRoom.Capacity)
                {
                    Console.WriteLine($"Error: Room {roomNumber} Is Full ({studentsInRoomCount}/{selectedRoom.Capacity}). Update Failed.");
                    Console.WriteLine("Please Choose Another Room .");
                    studentManager.LoadAll();
                    return;
                }
            }

            student.DormitoryName = dormName;
            student.BlockName = blockName;
            student.RoomNumber = roomNumber;
            student.Save();

            Console.WriteLine($"Student Asignmented And Updated Successfully.");
        }

        static void AddItem(PersonItemManager itemManager, StudentManager studentManager)
        {
            Console.Write("Enter Student Code To Assign An Item to : ");
            var studentCode = Console.ReadLine();

            if (!studentManager.Students.Exists(s => s.StudentCode == studentCode))
            {
                Console.WriteLine("Error: Student With This Code Not Found .");
                return;
            }

            Console.WriteLine("Select Item Type:");
            Console.WriteLine("- 100: (Bed)");
            Console.WriteLine("- 200: (Closet)");
            Console.WriteLine("- 300: (Desk)");
            Console.WriteLine("- 400: (Chair)");
            Console.Write("Enter Item Type Code : ");
            var itemTypeCode = Console.ReadLine();

            if (itemTypeCode != "100" && itemTypeCode != "200" && itemTypeCode != "300" && itemTypeCode != "400")
            {
                Console.WriteLine("Error: Invalid item type code.");
                return;
            }

            string partNumber = itemTypeCode + studentCode;

            if (itemManager.Items.Exists(i => i.PartNumber == partNumber))
            {
                Console.WriteLine("Error: This Exact Item (Same Type For Same Student) Already Exists.");
                Console.WriteLine("You Must To Choose Another Item Or Student");
                return;
            }

            var newItem = new PersonItem(partNumber, studentCode);
            itemManager.Add(newItem);
            Console.WriteLine($"Item With PartNumber: '{partNumber}' Created And Assigned To Student ('{studentCode})'.");
        }

        static void AddPerson(PersonManager personManager)
        {
            Console.Write("Enter National Code : ");
            var code = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(code) || personManager.People.Exists(p => p.NationalCode == code))
            {
                Console.WriteLine("Error: A Person With This National Code Already Exists Or The Code Is Invalid .");
                return;
            }
            Console.Write("Enter First Name : ");
            var fname = Console.ReadLine();
            Console.Write("Enter Last Name : ");
            var lname = Console.ReadLine();
            Console.Write("Enter Address : ");
            var addr = Console.ReadLine();
            Console.Write("Enter Phone Number : ");
            var phone = Console.ReadLine();

            var p = new Person(fname, lname, code, addr, phone);
            personManager.Add(p);
            Console.WriteLine("Person Added Successfully .");
        }

        static void AddStudent(PersonManager personManager, StudentManager studentManager, DormitoryManager dormManager, BlockManager blockManager, RoomManager roomManager)
        {
            Console.Write("Enter National Code : ");
            var nc = Console.ReadLine();

            if (studentManager.Students.Exists(s => s.NationalCode == nc))
            {
                Console.WriteLine("Error: A Student With This National Code Already Exists .");
                return;
            }

            var person = personManager.People.Find(x => x.NationalCode == nc);
            if (person == null)
            {
                Console.WriteLine("This Person Is Not Registered. Please Enter their Details For Register First.");
                AddPerson(personManager);
                person = personManager.People.Find(x => x.NationalCode == nc);
                if (person == null) return;
            }

            Console.Write("Enter Student Code : ");
            var sc = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(sc) || studentManager.Students.Exists(s => s.StudentCode == sc))
            {
                Console.WriteLine("Error: A Student With This Student Code Already Exists Or The Code Is Invalid.");
                return;
            }

            if (!dormManager.Dorms.Any())
            {
                Console.WriteLine("Error: No Dormitories available yet. Please Add a Dormitory and Block First.");
                return;
            }
            Console.WriteLine("--- Available Dormitories ---");
            dormManager.List();
            Console.Write("Enter Dormitory Name: ");
            string dormName = Console.ReadLine();
            if (!dormManager.Dorms.Exists(d => d.Name.Equals(dormName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: Dormitory Not Found.");
                return;
            }

            var blocksInDorm = blockManager.Blocks.Where(b => b.DormitoryName.Equals(dormName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!blocksInDorm.Any())
            {
                Console.WriteLine("Error: No Blocks Available In This Dorm. I Told You Please Add A Block First.");
                return;
            }
            Console.WriteLine($"--- Available Blocks in {dormName} ---");
            foreach (var b in blocksInDorm) Console.WriteLine($"- Name:{b.Name} ,Floors:{b.Floors} ,Capacity:{b.Capacity} ");
            Console.Write("Enter Block Name : ");
            string blockName = Console.ReadLine();
            if (!blocksInDorm.Exists(b => b.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: This Block Not Found In This Dormitory.");
                return;
            }

            var roomsInBlock = roomManager.Rooms.Where(r => r.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!roomsInBlock.Any())
            {
                Console.WriteLine("Error: No Rooms Found In This Block. The Block Might Not Have Been Created Correctly.");
                return;
            }

            Console.WriteLine($"--- Available Rooms In {blockName} ---");
            var roomsByFloor = roomsInBlock.GroupBy(r => r.Floor).OrderBy(g => g.Key);

            foreach (var floorGroup in roomsByFloor)
            {
                var roomNumbers = floorGroup.Select(r => int.Parse(r.RoomNumber)).OrderBy(n => n).ToList();
                if (roomNumbers.Any())
                {
                    Console.WriteLine($"- Floor {floorGroup.Key}: Rooms from {roomNumbers.First()} to {roomNumbers.Last()}");
                }
            }

            Console.Write("Enter Room Number : ");
            string roomNumber = Console.ReadLine();
            var selectedRoom = roomManager.Rooms.Find(r => r.RoomNumber == roomNumber && r.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase));

            if (selectedRoom == null)
            {
                Console.WriteLine("Error: This Room Number Not Found In This Block .");
                return;
            }

            var studentsInRoomCount = studentManager.Students.Count(s => s.RoomNumber == roomNumber && s.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase));
            if (studentsInRoomCount >= selectedRoom.Capacity)
            {
                Console.WriteLine($"Error: Room {roomNumber} Is Full ({studentsInRoomCount}/{selectedRoom.Capacity}).");
                return;
            }

            var s = new Student(sc, nc, dormName, blockName, roomNumber);
            studentManager.Add(s);
            Console.WriteLine($"Student Successfully Added To Room {roomNumber}. Current Occupancy Is: {studentsInRoomCount + 1}/{selectedRoom.Capacity}");
        }

        static void ListPeopleAndStudents(PersonManager personManager, StudentManager studentManager)
        {
            Console.Write("List Person Or Students? (Person/Student) : ");
            var listKind = Console.ReadLine()?.Trim().ToLower();
            if (listKind == "person")
            {
                Console.WriteLine("--- List of All Persons ---");
                foreach (var p in personManager.People)
                {
                    Console.WriteLine($"- Name: {p.FirstName} {p.LastName}, National Code: {p.NationalCode}, Phone: {p.Phone}, Address: {p.Address}");
                }
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
            }
            else
            {
                Console.WriteLine("Invalid Type .");
            }
        }

        static void AddDormitory(DormitoryManager dormManager, PersonManager personManager)
        {
            Console.Write("Enter Dormitory Name : ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name) || dormManager.Dorms.Exists(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: A Dormitory With This Name Already Exists Or The Name Is Invalid .");
                return;
            }
            Console.Write("Enter Address : ");
            string addr = Console.ReadLine();
            Console.Write("Enter Supervisor's National Code : ");
            string sup = Console.ReadLine();
            if (!personManager.People.Exists(p => p.NationalCode == sup))
            {
                Console.WriteLine("Error: Supervisor With This National Code Not Found. Please Register The Person First .");
                return;
            }

            dormManager.Add(new Dormitory(name, addr, sup));
            Console.WriteLine("Dormitory Added successfully .");
        }

        static void AddBlock(BlockManager blockManager, DormitoryManager dormManager, RoomManager roomManager)
        {
            if (!dormManager.Dorms.Any())
            {
                Console.WriteLine("Error: No Dormitories Exist. Please Add A Dormitory First .");
                return;
            }
            Console.WriteLine("--- Available Dormitories ---");
            dormManager.List();
            Console.Write("Enter The Name Of The Dormitory For This Block : ");
            string dormName = Console.ReadLine();
            if (!dormManager.Dorms.Exists(d => d.Name.Equals(dormName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: Dormitory Not Found .");
                return;
            }

            Console.Write("Enter Block Name : ");
            string blockName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(blockName) || blockManager.Blocks.Exists(b => b.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Error: A Block With This Name Already Exists Or The Name Is Invalid .");
                return;
            }

            Console.Write("Enter Total Capacity For The Block : ");
            if (!int.TryParse(Console.ReadLine(), out int capacity) || capacity <= 0)
            {
                Console.WriteLine("Invalid Capacity. Please Enter A Positive Number .");
                return;
            }

            Console.Write("Enter Number of Floors: ");
            if (!int.TryParse(Console.ReadLine(), out int floors) || floors <= 0)
            {
                Console.WriteLine("Invalid Number Of Floors. Please Enter A Positive Number .");
                return;
            }

            if (capacity % (floors * 6) != 0)
            {
                Console.WriteLine($"Warning: The Capacity {capacity} Is Not Perfectly Divisible By {floors} Floors With 6-Person Rooms .");
                Console.WriteLine("This May Result In Unused Space. Continue? (y/n)");
                if (Console.ReadLine()?.ToLower() != "y") return;
            }

            var block = new Block(blockName, floors, capacity, dormName);
            blockManager.Add(block, roomManager);
            Console.WriteLine("Block And Its Rooms Created Successfully.");
        }
    }
}