using System;
using System.IO;
using System.Collections.Generic;
using static System.Console;
using static Class_Generator.Toolkit;

namespace Class_Generator
{
    class Program
    {
        public static void Main()
        {
            GenerateClass();
        }

        public static void GenerateClass()
        {
            // Asking for the class properties
            string className = Input("Enter class name: ");
            string userNamespace = Input("Enter namespace: ");
            string newUserNamespace = userNamespace;

            // Remove spaces from the namespace for the file itself
            while (newUserNamespace.Contains(" "))
                newUserNamespace = newUserNamespace.Replace(" ", "_");

            // Create the file structure
            File.AppendAllText($"{className}.cs",
                "using System;" + Environment.NewLine +
                "" + Environment.NewLine +
                $"namespace {newUserNamespace}" + Environment.NewLine +
                "{" + Environment.NewLine +
                $"    public class {className}" + Environment.NewLine +
                "    {");

            string protectionLevel = "";
            List<string> attributeNameList = new List<string>();
            List<string> attributeTypeList = new List<string>();

            char input = Input("Add attributes? (y/n) ", "char");
            while (input != 'y' && input != 'n') input = Input("Invalid input. Add attributes? (y/n) ", "char");
            while (input == 'y') // Adding attrubutes
            {
                protectionLevelIndicator:
                byte protectionLevelIndicator = Input("Enter protection level " +
                    "(1 for public, 2 for private, " +
                    "3 for internal, 4 for protected internal, 5 for private protected): ", "byte");
                switch (protectionLevelIndicator)
                {
                    case 1:
                        protectionLevel = "public";
                        break;
                    case 2:
                        protectionLevel = "private";
                        break;
                    case 3:
                        protectionLevel = "internal";
                        break;
                    case 4:
                        protectionLevel = "protected internal";
                        break;
                    case 5:
                        protectionLevel = "private protected";
                        break;
                    default:
                        WriteLine("Invalid input. Try again");
                        goto protectionLevelIndicator;
                }

                string attributeType = Input("Enter attribute type: ");
                attributeTypeList.Add(attributeType);

                string attributeName = Input("Enter attribute name: ");
                attributeNameList.Add(attributeName);

                File.AppendAllText($"{className}.cs", Environment.NewLine +
                    $"		{protectionLevel} {attributeType} {attributeName};");

                input = Input("Continue? (y/n) ", "char");
                while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
            }

            if (attributeNameList.Count > 0) // Making constructors, gets and sets
            {
                input = Input("Add default constructor? (y/n) ", "char");
                while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
                // Default constructor
                if (input == 'y')
                {
                    File.AppendAllText($"{className}.cs",
                        Environment.NewLine + Environment.NewLine + $"        public {className}()" + " { }");
                }

                input = Input("Add values constructor? (y/n) ", "char");
                while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
                // Values constructor
                if (input == 'y')
                {
                    string signatureString = $"        public {className}(";
                    for (int i = 0; i < attributeNameList.Count; i++)
                    {
                        if (i != attributeNameList.Count - 1)
                            signatureString += $"{attributeTypeList[i]} {attributeNameList[i]}, ";
                        else
                            signatureString += $"{attributeTypeList[i]} {attributeNameList[i]})";
                    }
                    File.AppendAllText($"{className}.cs",
                        Environment.NewLine + Environment.NewLine + signatureString + Environment.NewLine + "        {");
                    foreach (string attributeName in attributeNameList)
                    {
                        File.AppendAllText($"{className}.cs",
                            Environment.NewLine + $"            this.{attributeName} = {attributeName};");
                    }
                    File.AppendAllText($"{className}.cs",
                        Environment.NewLine + "        }");
                }

                input = Input("Add copy constructor? (y/n) ", "char");
                while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
                // Copy constructor
                if (input == 'y')
                {
                    string signatureString = $"        public {className}({className} toCopy)";
                    File.AppendAllText($"{className}.cs",
                        Environment.NewLine + Environment.NewLine + signatureString + Environment.NewLine + "        {");
                    foreach (string attributeName in attributeNameList)
                    {
                        File.AppendAllText($"{className}.cs",
                            Environment.NewLine + $"            this.{attributeName} = toCopy.{attributeName};");
                    }
                    File.AppendAllText($"{className}.cs",
                        Environment.NewLine + "        }");
                }

                // Gets and sets
                for (int i = 0; i < attributeNameList.Count; i++)
                {
                    File.AppendAllText($"{className}.cs",
                            Environment.NewLine + Environment.NewLine + $"        public {attributeTypeList[i]} {attributeNameList[i].Substring(0, 1).ToUpper() + attributeNameList[i].Substring(1)}"
                            + Environment.NewLine + "        {"
                            + Environment.NewLine + "            get { return " + attributeNameList[i] + "; }"
                            + Environment.NewLine + "            set { " + attributeNameList[i] + " = value; }"
                            + Environment.NewLine + "        }");
                }
            }

            // Finishing the file
            File.AppendAllText($"{className}.cs", Environment.NewLine + "    }" + Environment.NewLine + "}");

            // Linking the class if the code runs from a VS project to the project if the user asks for it
            input = Input("Do you want to link the class to the VS project? (y/n) ", "char");
            while (input != 'y' && input != 'n') input = Input("Invalid input. Do you want the class to be linked? (y/n) ", "char");
            if (input == 'y')
            {
                List<string> newFile = new List<string>();
                bool added = false;
                string currentDirectory = Directory.GetCurrentDirectory();
                string newDirectory = Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\"));
                string[] txtLines = File.ReadAllLines($@"{newDirectory}\{userNamespace}.csproj");
                try { File.Move($"{className}.cs", $@"{newDirectory}\{className}.cs"); }
                catch (Exception)
                {
                    WriteLine("File already exists - copying failed.");
                    Environment.Exit(0);
                }
                File.WriteAllText($@"{newDirectory}\{userNamespace}.csproj", string.Empty);
                foreach (string line in txtLines)
                {
                    if (line.Contains("<Compile Include=") && !added)
                    {
                        newFile.Add($"    <Compile Include=\"{className}.cs\" />");
                        added = true;
                    }
                    newFile.Add(line);
                }
                File.AppendAllLines($@"{newDirectory}\{userNamespace}.csproj", newFile);
            }
        }
    }
}
