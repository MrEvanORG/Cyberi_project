using AllClass;
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

                    For more information on a specific command, type HELP command-name 

                        | Dormitory    show all command related to dormitories          |
                        | Block        show all command related to buildings' block     |
                        | Person       show all command related to persons              |
                        | Item         show all command related to items                |
                        | Reporting    show all command related to reporting            |
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
                        text = "show all command related to dormitories";
                        break;
                    case "block":
                        text = "show all command related to blocks";
                        break;
                    case "person":
                        text = "show all command related to persons";
                        break;
                    case "item":
                        text = "show all command related to items";
                        break;
                    case "reporting":
                        text = "show all command related to reporting";
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
                        break; // این break مربوط به case "dormitory" است
                }
            }
        }
    }
}
