using System;

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

        public static void Main(string[] args)
        {
            ShowHelp("help");

            while (true)
            {
                Console.Write("$ main>");
                string input = Console.ReadLine();
                string command = input.Trim().ToLower();

                if (command.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                switch (command.Split()[0])
                {
                    default:
                        if (command == "") break;
                        Console.WriteLine("Command not found\nuse 'help' to see commands");
                        break;

                    case "help":
                        ShowHelp(command);
                        break;

                    case "dormitory":
                        while (true)
                        {
                            Console.Write("$ main/dormitory>");
                            string input1 = Console.ReadLine();
                            string command1 = input1.Trim().ToLower();

                            if (command1.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                            switch (command1.Split()[0])
                            {
                                default:
                                    if (command1 == "") break;
                                    Console.WriteLine("Command not found\nuse 'help dormitory' to see commands");
                                    break;

                                case "help":
                                    ShowHelp("help dormitory");
                                    break;
                            }
                        }
                        break; 

                    case "block":
                        while (true)
                        {
                            Console.Write("$ main/block>");
                            string input1 = Console.ReadLine();
                            string command1 = input1.Trim().ToLower();

                            if (command1.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                            switch (command1.Split()[0])
                            {
                                default:
                                    if (command1 == "") break;
                                    Console.WriteLine("Command not found\nuse 'help block' to see commands");
                                    break;

                                case "help":
                                    ShowHelp("help block");
                                    break;
                            }
                        }
                        break;

                    case "person":
                        while (true)
                        {
                            Console.Write("$ main/person>");
                            string input1 = Console.ReadLine();
                            string command1 = input1.Trim().ToLower();

                            if (command1.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                            switch (command1.Split()[0])
                            {
                                default:
                                    if (command1 == "") break;
                                    Console.WriteLine("Command not found\nuse 'help person' to see commands");
                                    break;

                                case "help":
                                    ShowHelp("help person");
                                    break;
                            }
                        }
                        break;

                    case "room":
                        while (true)
                        {
                            Console.Write("$ main/room>");
                            string input1 = Console.ReadLine();
                            string command1 = input1.Trim().ToLower();

                            if (command1.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                            switch (command1.Split()[0])
                            {
                                default:
                                    if (command1 == "") break;
                                    Console.WriteLine("Command not found\nuse 'help room' to see commands");
                                    break;

                                case "help":
                                    ShowHelp("help item");
                                    break;
                            }
                        }
                        break;

                    case "item":
                        while (true)
                        {
                            Console.Write("$ main/item>");
                            string input1 = Console.ReadLine();
                            string command1 = input1.Trim().ToLower();

                            if (command1.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                            switch (command1.Split()[0])
                            {
                                default:
                                    if (command1 == "") break;
                                    Console.WriteLine("Command not found\nuse 'help item' to see commands");
                                    break;

                                case "help":
                                    ShowHelp("help item");
                                    break;
                            }
                        }
                        break;

                    case "reporting":
                        while (true)
                        {
                            Console.Write("$ main/reporting>");
                            string input1 = Console.ReadLine();
                            string command1 = input1.Trim().ToLower();

                            if (command1.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                            switch (command1.Split()[0])
                            {
                                default:
                                    if (command1 == "") break;
                                    Console.WriteLine("Command not found\nuse 'help reporting' to see commands");
                                    break;

                                case "help":
                                    ShowHelp("help reporting");
                                    break;
                            }
                        }
                        break;
                }
            }
        }
    }
}
