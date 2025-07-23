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

                    Program Commands : 
                    For More Information On A Specific Command, Type HELP command-name 
                        | exit         To Exit The Program.                               |
                        | dormitory    Show All Commands Related To Dormitories.          |
                        | block        Show All Commands Related To Blocks Of A Dormitory.|
                        | person       Show All Commands Related To People And Students.  |
                        | item         Show All Commands Related To Student And Room Items. |
                    Example : help item

                    """;
            }
            else if (parts.Length == 2)
            {
                switch (parts[1])
                {
                    default:
                        text = "Sorry, There Is No Such Subject To Show Help For !";
                        break;
                    case "dormitory":
                        text = """

                            Dormitory Commands : 
                                | exit        Back To Main Menu.                                     |
                                | list        Show List Of All Dormitories.                          |
                                | add         Add A New Dormitory To The Program.                    |
                                | edit [name] Edit A Dormitory's Address Or Supervisor.              |
                                | show [name] Show Detailed Information About A Dormitory.           |
                                | rem [name]  Remove A Dormitory And All Its Related Contents.       |

                            """;
                        break;
                    case "block":
                        text = """

                            Block Commands : 
                                | exit        Back To Main Menu.                                      |
                                | list        Show List Of All Blocks And Their Room Ranges.          |
                                | add         Add A New Block And Its Rooms Automatically.            |
                                | show [name] Show Detailed Information About A Block.                |
                                | rem [name]  Remove A Block And All Its Related Contents.            |

                            """;
                        break;
                    case "person":
                        text = """

                            Person Commands : 
                                | exit        Back To Main Menu.                                           |
                                | list        Show List Of All People Or Students.                         |
                                | add         Add A New Person Or Student.                                 |
                                | edit        Edit A Person's Details Or A Student's Room Assignment.      |
                                | show [NC]   Show Detailed Information About A Person By National Code.   |
                                | rem         Remove A Person (And Their Student Record) Or Just A Student.|

                            """;
                        break;
                    case "item":
                        text = """

                            Item Commands : 
                                | exit              Back To Main Menu.                                |
                                | list              Show List Of All Person Or Room Items.            |
                                | add               Add A New Item For A Student Or A Room.           |
                                | edit [partNum]    Edit An Existing Item By Its Part Number.         |
                                | show [partNum]    Show Detailed Information About An Item.          |
                                | roomitems         Show All Items Within A Specific Room.            |
                                | rem [partNum]     Remove An Item By Its Part Number.                |

                            """;
                        break;
                }
            }
            else
            {
                text = "Invalid Help Usage.";
            }

            Console.WriteLine(text);
        }

        static void Main(string[] args)
        {

            Database.GetConnection().Close();

            var personManager = new PersonManager();
            var studentManager = new StudentManager();
            var personItemManager = new PersonItemManager();
            var roomItemManager = new RoomItemManager();
            var roomManager = new RoomManager();
            var blockManager = new BlockManager();
            var dormManager = new DormitoryManager();

            personManager.LoadAll();
            studentManager.LoadAll();
            personItemManager.LoadAll();
            roomItemManager.LoadAll();
            roomManager.LoadAll();
            blockManager.LoadAll();
            dormManager.LoadAll();

            Console.WriteLine("---   Dormitory Management System  ------------");
            Console.WriteLine("----   Created By Group Of Cyberi Malakoot  ---");
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
                        HandlePersonMenu(personManager, studentManager, personItemManager, dormManager, blockManager, roomManager);
                        break;

                    case "dormitory":
                        HandleDormitoryMenu(dormManager, blockManager, roomManager, studentManager, personItemManager, roomItemManager, personManager);
                        break;

                    case "block":
                        HandleBlockMenu(blockManager, dormManager, roomManager, studentManager, personItemManager, roomItemManager);
                        break;

                    case "item":
                        HandleItemMenu(personItemManager, roomItemManager, studentManager, dormManager, blockManager, roomManager, personManager);
                        break;

                    case "help":
                        ShowHelp(input);
                        break;

                    default:
                        Console.WriteLine("Invalid Command. Type 'help' To See Available Commands.");
                        break;
                }
            }
        }

        static void HandleItemMenu(PersonItemManager personItemManager, RoomItemManager roomItemManager, StudentManager studentManager, DormitoryManager dormManager, BlockManager blockManager, RoomManager roomManager, PersonManager personManager)
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
                        Console.Write("Add An Item For A 'Person' Or A 'Room'? (Person/Room) : ");
                        var addType = Console.ReadLine()?.Trim().ToLower();
                        if (addType == "person")
                        {
                            AddPersonItem(personItemManager, studentManager);
                        }
                        else if (addType == "room")
                        {
                            AddRoomItem(roomItemManager, dormManager, blockManager, roomManager);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Type . Please Choose 'Person' Or 'Room' .");
                        }
                        break;

                    case "list":
                        Console.Write("List Items For 'Person' Or 'Room'? (Person/Room) : ");
                        var listType = Console.ReadLine()?.Trim().ToLower();
                        if (listType == "person")
                        {
                            personItemManager.List(studentManager);
                        }
                        else if (listType == "room")
                        {
                            roomItemManager.List();
                        }
                        else
                        {
                            Console.WriteLine("Invalid Type . Please Choose 'Person' Or 'Room' .");
                        }
                        break;

                    case "edit":
                        if (cmdParts.Length < 2) { Console.WriteLine("Usage: edit [partNumber]"); break; }
                        var partNumToEdit = cmdParts[1];
                        if (partNumToEdit.Length == 12)
                        {
                            personItemManager.Edit(partNumToEdit, studentManager);
                        }
                        else if (partNumToEdit.Length == 11)
                        {
                            roomItemManager.Edit(partNumToEdit, dormManager, blockManager, roomManager);
                        }
                        else
                        {
                            Console.WriteLine("Error: Invalid Part Number Length. Person Items Are 12 Digits, Room Items Are 11.");
                        }
                        break;

                    case "show":
                        if (cmdParts.Length < 2) { Console.WriteLine("Usage: show [partNumber]"); break; }
                        ShowItem(cmdParts[1], personItemManager, roomItemManager, studentManager, personManager);
                        break;

                    case "roomitems":
                        ShowItemsInRoom(dormManager, blockManager, roomManager, studentManager, personItemManager, roomItemManager, personManager);
                        break;

                    case "rem":
                        if (cmdParts.Length < 2) { Console.WriteLine("Usage: rem [partNumber]"); break; }
                        var partNumToRemove = cmdParts[1];
                        if (partNumToRemove.Length == 12)
                        {
                            personItemManager.Remove(partNumToRemove);
                        }
                        else if (partNumToRemove.Length == 11)
                        {
                            roomItemManager.Remove(partNumToRemove);
                        }
                        else
                        {
                            Console.WriteLine("Error: Invalid Part Number Length. Person Items Are 12 Digits, Room Items Are 11.");
                        }
                        break;

                    case "help":
                        ShowHelp("help item");
                        break;

                    default:
                        Console.WriteLine("Invalid Command. Use 'add', 'list', 'edit', 'show', 'roomitems', 'rem', 'help' Or 'exit'.");
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

                    case "edit":
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

                    case "list":
                        ListPeopleAndStudents(personManager, studentManager);
                        break;

                    case "show":
                        if (cmdParts.Length < 2)
                        {
                            Console.WriteLine("Usage: show [nationalCode]");
                            break;
                        }
                        personManager.Show(cmdParts[1], studentManager, itemManager);
                        break;

                    case "rem":
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
                        Console.WriteLine("Invalid command. Use 'add', 'list', 'edit', 'show', 'rem', 'help' or 'exit'.");
                        break;
                }
            }
        }

        static void HandleDormitoryMenu(DormitoryManager dormManager, BlockManager blockManager, RoomManager roomManager, StudentManager studentManager, PersonItemManager personItemManager, RoomItemManager roomItemManager, PersonManager personManager)
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

                    case "edit":
                        if (cmdParts.Length < 2)
                        {
                            Console.WriteLine("Usage: edit [dormitoryName]");
                            break;
                        }
                        EditDormitory(cmdParts[1], dormManager, personManager);
                        break;

                    case "list":
                        Console.WriteLine("--- List of All Dormitories ---");
                        dormManager.List();
                        break;

                    case "show":
                        if (cmdParts.Length < 2)
                        {
                            Console.WriteLine("Usage: show [dormitoryName]");
                            break;
                        }
                        dormManager.Show(cmdParts[1], blockManager, personManager, studentManager);
                        break;

                    case "rem":
                        if (cmdParts.Length < 2)
                        {
                            Console.WriteLine("Usage: rem [dormitoryName]");
                            break;
                        }
                        dormManager.Remove(cmdParts[1], blockManager, roomManager, studentManager, personItemManager, roomItemManager);
                        break;

                    case "help":
                        ShowHelp("help dormitory");
                        break;

                    default:
                        Console.WriteLine("Invalid command. Use 'add', 'list', 'edit', 'show', 'rem', 'help' or 'exit'.");
                        break;
                }
            }
        }

        static void HandleBlockMenu(BlockManager blockManager, DormitoryManager dormManager, RoomManager roomManager, StudentManager studentManager, PersonItemManager personItemManager, RoomItemManager roomItemManager)
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

                    case "show":
                        if (cmdParts.Length < 2)
                        {
                            Console.WriteLine("Usage: show [blockName]");
                            break;
                        }
                        blockManager.Show(cmdParts[1], roomManager, studentManager);
                        break;

                    case "rem":
                        if (cmdParts.Length < 2)
                        {
                            Console.WriteLine("Usage: rem [blockName]");
                            break;
                        }
                        blockManager.Remove(cmdParts[1], roomManager, studentManager, personItemManager, roomItemManager);
                        break;

                    case "help":
                        ShowHelp("help block");
                        break;

                    default:
                        Console.WriteLine("Invalid command. Use 'add', 'list', 'show', 'rem', 'help' or 'exit'.");
                        break;
                }
            }
        }

        static void ShowItemsInRoom(DormitoryManager dormManager, BlockManager blockManager, RoomManager roomManager, StudentManager studentManager, PersonItemManager personItemManager, RoomItemManager roomItemManager, PersonManager personManager)
        {
            if (!dormManager.Dorms.Any()) { Console.WriteLine("Error: No Dormitories Available. Please Add A Dormitory First."); return; }
            Console.WriteLine("--- Available Dormitories ---");
            dormManager.List();
            Console.Write("Enter Dormitory Name: ");
            string dormName = Console.ReadLine();
            if (!dormManager.Dorms.Exists(d => d.Name.Equals(dormName, StringComparison.OrdinalIgnoreCase))) { Console.WriteLine("Error: Dormitory Not Found."); return; }

            var blocksInDorm = blockManager.Blocks.Where(b => b.DormitoryName.Equals(dormName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!blocksInDorm.Any()) { Console.WriteLine("Error: No Blocks Available In This Dormitory."); return; }
            Console.WriteLine($"--- Available Blocks In {dormName} ---");
            foreach (var b in blocksInDorm) Console.WriteLine($"- Name:{b.Name} ,Floors:{b.Floors} ,Capacity:{b.Capacity} ");
            Console.Write("Enter Block Name : ");
            string blockName = Console.ReadLine();
            if (!blocksInDorm.Exists(b => b.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase))) { Console.WriteLine("Error: This Block Not Found In This Dormitory."); return; }

            var roomsInBlock = roomManager.Rooms.Where(r => r.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!roomsInBlock.Any()) { Console.WriteLine("Error: No Rooms Found In This Block."); return; }
            Console.WriteLine($"--- Available Rooms In {blockName} ---");
            var roomsByFloor = roomsInBlock.GroupBy(r => r.Floor).OrderBy(g => g.Key);
            foreach (var floorGroup in roomsByFloor)
            {
                var roomNumbers = floorGroup.Select(r => int.Parse(r.RoomNumber)).OrderBy(n => n).ToList();
                if (roomNumbers.Any())
                {
                    Console.WriteLine($"- Floor {floorGroup.Key}: Rooms From {roomNumbers.First()} To {roomNumbers.Last()}");
                }
            }

            Console.Write("Enter Room Number : ");
            string roomNumber = Console.ReadLine();
            if (!roomsInBlock.Exists(r => r.RoomNumber == roomNumber)) { Console.WriteLine("Error: This Room Number Not Found In This Block ."); return; }

            Console.WriteLine($"\n--- Showing All Items For Room {roomNumber} In Block {blockName} ---");

            var roomItems = roomItemManager.Items.Where(i => i.BlockName == blockName && i.RoomNumber == roomNumber).ToList();
            Console.WriteLine("\n--- Room Items ---");
            if (roomItems.Any())
            {
                foreach (var item in roomItems)
                {
                    Console.WriteLine($"- Part Number: {item.PartNumber}, Type: {item.GetItemType()}");
                }
            }
            else
            {
                Console.WriteLine("No General Items Found In This Room.");
            }

            var studentsInRoom = studentManager.Students.Where(s => s.BlockName == blockName && s.RoomNumber == roomNumber).ToList();
            Console.WriteLine("\n--- Students And Their Personal Items ---");
            if (studentsInRoom.Any())
            {
                foreach (var student in studentsInRoom)
                {
                    var person = personManager.People.Find(p => p.NationalCode == student.NationalCode);
                    string studentName = person != null ? $"{person.FirstName} {person.LastName}" : "Unknown";
                    Console.WriteLine($"\n- Student: {studentName} (Code: {student.StudentCode})");

                    var personItems = personItemManager.Items.Where(i => i.StudentCode == student.StudentCode).ToList();
                    if (personItems.Any())
                    {
                        foreach (var item in personItems)
                        {
                            Console.WriteLine($"  - Part Number: {item.PartNumber}, Type: {item.GetItemType()}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  - This Student Has No Personal Items.");
                    }
                }
            }
            else
            {
                Console.WriteLine("No Students Found In This Room.");
            }
        }

        static void ShowItem(string partNumber, PersonItemManager personItemManager, RoomItemManager roomItemManager, StudentManager studentManager, PersonManager personManager)
        {
            if (partNumber.Length == 12)
            {
                var personItem = personItemManager.Items.Find(i => i.PartNumber.Equals(partNumber, StringComparison.OrdinalIgnoreCase));
                if (personItem != null)
                {
                    Console.WriteLine($"--- Details For Person Item: {personItem.PartNumber} ---");
                    Console.WriteLine($"- Item Type: {personItem.GetItemType()}");
                    var student = studentManager.Students.Find(s => s.StudentCode == personItem.StudentCode);
                    if (student != null)
                    {
                        Console.WriteLine($"- Location: {student.DormitoryName}/{student.BlockName}/{student.RoomNumber}");
                        personManager.Show(student.NationalCode, studentManager, personItemManager);
                    }
                    else
                    {
                        Console.WriteLine($"- Owner Student Code: {personItem.StudentCode} (Student Not Found)");
                    }
                    return;
                }
            }
            else if (partNumber.Length == 11)
            {
                var roomItem = roomItemManager.Items.Find(i => i.PartNumber.Equals(partNumber, StringComparison.OrdinalIgnoreCase));
                if (roomItem != null)
                {
                    Console.WriteLine($"--- Details For Room Item: {roomItem.PartNumber} ---");
                    Console.WriteLine($"- Item Type: {roomItem.GetItemType()}");
                    Console.WriteLine($"- Location: {roomItem.DormitoryName}/{roomItem.BlockName}/{roomItem.RoomNumber}");
                    return;
                }
            }

            Console.WriteLine("Error: Item With This Part Number Not Found.");
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

            string newSupervisorCode;
            while (true)
            {
                Console.Write($"New Supervisor's National Code ({dorm.SupervisorCode}): ");
                newSupervisorCode = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newSupervisorCode))
                {
                    newSupervisorCode = dorm.SupervisorCode;
                    break;
                }
                if (newSupervisorCode.Length == 10 && newSupervisorCode.All(char.IsDigit))
                {
                    if (!personManager.People.Exists(p => p.NationalCode == newSupervisorCode))
                    {
                        Console.WriteLine("Error: No Person Found With This National Code.");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Error: National Code Must Be 10 Digits.");
                }
            }
            dorm.SupervisorCode = newSupervisorCode;
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

            string newPhone;
            while (true)
            {
                Console.Write($"New Phone ({person.Phone}) : ");
                newPhone = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newPhone))
                {
                    newPhone = person.Phone;
                    break;
                }
                if (newPhone.Length == 11 && newPhone.All(char.IsDigit))
                {
                    break;
                }
                Console.WriteLine("Error: Phone Number Must Be 11 Digits.");
            }
            person.Phone = newPhone;

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

        static void AddPersonItem(PersonItemManager itemManager, StudentManager studentManager)
        {
            string studentCode;
            while (true)
            {
                Console.Write("Enter Student Code To Assign An Item to : ");
                studentCode = Console.ReadLine();
                if (studentCode.Length == 9 && studentCode.All(char.IsDigit))
                {
                    if (studentManager.Students.Exists(s => s.StudentCode == studentCode))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error: Student With This Code Not Found .");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Student Code Must Be 9 Digits.");
                }
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
                Console.WriteLine("Error: Invalid Item Type Code.");
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

        static void AddRoomItem(RoomItemManager roomItemManager, DormitoryManager dormManager, BlockManager blockManager, RoomManager roomManager)
        {
            if (!dormManager.Dorms.Any()) { Console.WriteLine("Error: No Dormitories Available. Please Add A Dormitory First."); return; }
            Console.WriteLine("--- Available Dormitories ---");
            dormManager.List();
            Console.Write("Enter Dormitory Name: ");
            string dormName = Console.ReadLine();
            if (!dormManager.Dorms.Exists(d => d.Name.Equals(dormName, StringComparison.OrdinalIgnoreCase))) { Console.WriteLine("Error: Dormitory Not Found."); return; }

            var blocksInDorm = blockManager.Blocks.Where(b => b.DormitoryName.Equals(dormName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!blocksInDorm.Any()) { Console.WriteLine("Error: No Blocks Available In This Dormitory."); return; }
            Console.WriteLine($"--- Available Blocks In {dormName} ---");
            foreach (var b in blocksInDorm) Console.WriteLine($"- Name:{b.Name} ,Floors:{b.Floors} ,Capacity:{b.Capacity} ");
            Console.Write("Enter Block Name : ");
            string blockName = Console.ReadLine();
            if (!blocksInDorm.Exists(b => b.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase))) { Console.WriteLine("Error: This Block Not Found In This Dormitory."); return; }

            var roomsInBlock = roomManager.Rooms.Where(r => r.BlockName.Equals(blockName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!roomsInBlock.Any()) { Console.WriteLine("Error: No Rooms Found In This Block."); return; }
            Console.WriteLine($"--- Available Rooms In {blockName} ---");
            var roomsByFloor = roomsInBlock.GroupBy(r => r.Floor).OrderBy(g => g.Key);
            foreach (var floorGroup in roomsByFloor)
            {
                var roomNumbers = floorGroup.Select(r => int.Parse(r.RoomNumber)).OrderBy(n => n).ToList();
                if (roomNumbers.Any())
                {
                    Console.WriteLine($"- Floor {floorGroup.Key}: Rooms From {roomNumbers.First()} To {roomNumbers.Last()}");
                }
            }

            Console.Write("Enter Room Number : ");
            string roomNumber = Console.ReadLine();
            if (!roomsInBlock.Exists(r => r.RoomNumber == roomNumber)) { Console.WriteLine("Error: This Room Number Not Found In This Block ."); return; }

            Console.WriteLine("Select Item Type:");
            Console.WriteLine("- 100: (Carpet)");
            Console.WriteLine("- 200: (Refrigerator)");
            Console.WriteLine("- 300: (Television)");
            Console.Write("Enter Item Type Code : ");
            var itemTypeCode = Console.ReadLine();

            if (itemTypeCode != "100" && itemTypeCode != "200" && itemTypeCode != "300")
            {
                Console.WriteLine("Error: Invalid Item Type Code.");
                return;
            }

            string partNumber = roomItemManager.GenerateUniquePartNumber(itemTypeCode, roomNumber);
            var newItem = new RoomItem(partNumber, dormName, blockName, roomNumber);
            roomItemManager.Add(newItem);
            Console.WriteLine($"Room Item With PartNumber: '{partNumber}' Created And Assigned To Room '{roomNumber}'.");
        }

        static void AddPerson(PersonManager personManager)
        {
            string code;
            while (true)
            {
                Console.Write("Enter National Code : ");
                code = Console.ReadLine();
                if (code.Length == 10 && code.All(char.IsDigit))
                {
                    if (personManager.People.Exists(p => p.NationalCode == code))
                    {
                        Console.WriteLine("Error: A Person With This National Code Already Exists.");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Error: National Code Must Be 10 Digits.");
                }
            }

            Console.Write("Enter First Name : ");
            var fname = Console.ReadLine();
            Console.Write("Enter Last Name : ");
            var lname = Console.ReadLine();
            Console.Write("Enter Address : ");
            var addr = Console.ReadLine();

            string phone;
            while (true)
            {
                Console.Write("Enter Phone Number : ");
                phone = Console.ReadLine();
                if (phone.Length == 11 && phone.All(char.IsDigit))
                {
                    break;
                }
                Console.WriteLine("Error: Phone Number Must Be 11 Digits.");
            }

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

            string sc;
            while (true)
            {
                Console.Write("Enter Student Code : ");
                sc = Console.ReadLine();
                if (sc.Length == 9 && sc.All(char.IsDigit))
                {
                    if (studentManager.Students.Exists(s => s.StudentCode == sc))
                    {
                        Console.WriteLine("Error: A Student With This Student Code Already Exists.");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Error: Student Code Must Be 9 Digits.");
                }
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

            string sup;
            while (true)
            {
                Console.Write("Enter Supervisor's National Code : ");
                sup = Console.ReadLine();
                if (sup.Length == 10 && sup.All(char.IsDigit))
                {
                    if (!personManager.People.Exists(p => p.NationalCode == sup))
                    {
                        Console.WriteLine("Error: Supervisor With This National Code Not Found. Please Register The Person First .");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Error: National Code Must Be 10 Digits.");
                }
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
