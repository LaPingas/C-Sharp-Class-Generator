using System;
using System.IO;
using System.Collections.Generic;
using static System.Console;
using static Class_Generator.Toolkit;

namespace Class_Generator
{
    public class Program
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
            string inheritance = Input("Enter inheritance (default: no inheritance): ");
            if (inheritance != string.Empty)
                inheritance = $" : {inheritance}";
            string newUserNamespace = userNamespace;

            // Remove spaces from the namespace for the file itself
            while (newUserNamespace.Contains(" "))
                newUserNamespace = newUserNamespace.Replace(" ", "_");
            // Create the file structure
            File.AppendAllText($"{className}.cs",
                "using System;" + NewLine +
                "" + NewLine +
                $"namespace {newUserNamespace}" + NewLine +
                "{" + NewLine +
                $"    public class {className}{inheritance}" + NewLine +
                "    {");

            string protectionLevel = "";
            List<string> attributeNameList = new List<string>();
            List<string> inheritanceAttributeNameList = new List<string>();
            List<string> attributeTypeList = new List<string>();
            List<string> inheritanceAttributeTypeList = new List<string>();

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

                File.AppendAllText($"{className}.cs", NewLine +
                    $"		{protectionLevel} {attributeType} {attributeName};");

                input = Input("Continue? (y/n) ", "char");
                while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
            }

            if (inheritance != string.Empty)
            {
                input = Input("Add inheritance attributes? (y/n) ", "char");
                while (input != 'y' && input != 'n') input = Input("Invalid input. Add inheritance attributes? (y/n) ", "char");
                while (input == 'y') // Adding inheritance attrubutes
                {
                    string attributeType = Input("Enter inheritance attribute type: ");
                    inheritanceAttributeTypeList.Add(attributeType);

                    string attributeName = Input("Enter inheritance attribute name: ");
                    inheritanceAttributeNameList.Add(attributeName);

                    input = Input("Continue? (y/n) ", "char");
                    while (input != 'y' && input != 'n')
                        input = Input("Invalid input. Continue? (y/n) ", "char");
                }
            }

            if (attributeNameList.Count > 0 || inheritanceAttributeNameList.Count > 0) // Making constructors, gets and sets
            {
                input = Input("Add default constructor? (y/n) ", "char");
                while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
                // Default constructor
                if (input == 'y')
                {
                    File.AppendAllText($"{className}.cs",
                        NewLine + NewLine + $"        public {className}()" + " { }");
                }

                input = Input("Add values constructor? (y/n) ", "char");
                while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
                // Values constructor
                if (input == 'y')
                {
                    string signatureString = $"        public {className}(";
                    for (int i = 0; i < attributeNameList.Count; i++)
                    {
                        if (i != attributeNameList.Count - 1 || inheritanceAttributeNameList.Count != 0)
                            signatureString += $"{attributeTypeList[i]} {attributeNameList[i]}, ";
                        else
                            signatureString += $"{attributeTypeList[i]} {attributeNameList[i]})";
                    }
                    for (int i = 0; i < inheritanceAttributeNameList.Count; i++)
                    {
                        if (i != inheritanceAttributeNameList.Count - 1)
                            signatureString += $"{inheritanceAttributeTypeList[i]} {inheritanceAttributeNameList[i]}, ";
                        else
                            signatureString += $"{inheritanceAttributeTypeList[i]} {inheritanceAttributeNameList[i]})";
                    }
                    for (int i = 0; i < inheritanceAttributeNameList.Count; i++)
                    {
                        if (i == 0)
                            signatureString += " : base(";
                        if (i != inheritanceAttributeNameList.Count - 1)
                            signatureString += $"{inheritanceAttributeNameList[i]}, ";
                        else
                            signatureString += $"{inheritanceAttributeNameList[i]})";
                    }
                    File.AppendAllText($"{className}.cs",
                        NewLine + NewLine + signatureString + NewLine + "        {");
                    foreach (string attributeName in attributeNameList)
                    {
                        File.AppendAllText($"{className}.cs",
                            NewLine + $"            this.{attributeName} = {attributeName};");
                    }
                    File.AppendAllText($"{className}.cs",
                        NewLine + "        }");
                }

                input = Input("Add copy constructor? (y/n) ", "char");
                while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
                // Copy constructor
                if (input == 'y')
                {
                    string signatureString = $"        public {className}({className} toCopy)";
                    File.AppendAllText($"{className}.cs",
                        NewLine + NewLine + signatureString + NewLine + "        {");
                    foreach (string attributeName in attributeNameList)
                    {
                        File.AppendAllText($"{className}.cs",
                            NewLine + $"            this.{attributeName} = toCopy.{attributeName};");
                    }
                    foreach (string inheritanceAttributeName in inheritanceAttributeNameList)
                    {
                        File.AppendAllText($"{className}.cs",
                            NewLine + $"            this.{inheritanceAttributeName.Substring(0, 1).ToUpper() + inheritanceAttributeName.Substring(1)} = toCopy.{inheritanceAttributeName.Substring(0, 1).ToUpper() + inheritanceAttributeName.Substring(1)};");
                    }
                    File.AppendAllText($"{className}.cs",
                        NewLine + "        }");
                }

                // Gets and sets
                for (int i = 0; i < attributeNameList.Count; i++)
                {
                    File.AppendAllText($"{className}.cs",
                            NewLine + NewLine + $"        public {attributeTypeList[i]} {attributeNameList[i].Substring(0, 1).ToUpper() + attributeNameList[i].Substring(1)}"
                            + NewLine + "        {"
                            + NewLine + "            get { return " + attributeNameList[i] + "; }"
                            + NewLine + "            set { " + attributeNameList[i] + " = value; }"
                            + NewLine + "        }");
                }
            }

            // Finishing the file
            File.AppendAllText($"{className}.cs", NewLine + "    }" + NewLine + "}");

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
                    Exit(0);
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

    public class Toolkit
    {
        public static dynamic ReturnParsedValue(dynamic x, string type)
        {
            switch (type)
            {
                case "byte":
                    x = byte.TryParse($"{x}", out byte resultByte) ? (byte?)byte.Parse($"{x}") : null;
                    break;
                case "sbyte":
                    x = sbyte.TryParse($"{x}", out sbyte resultSByte) ? (sbyte?)sbyte.Parse($"{x}") : null;
                    break;
                case "ushort":
                    x = ushort.TryParse($"{x}", out ushort resultUShort) ? (ushort?)ushort.Parse($"{x}") : null;
                    break;
                case "short":
                    x = short.TryParse($"{x}", out short resultShort) ? (short?)short.Parse($"{x}") : null;
                    break;
                case "uint":
                    x = uint.TryParse($"{x}", out uint resultUInt) ? (uint?)uint.Parse($"{x}") : null;
                    break;
                case "int":
                    x = int.TryParse($"{x}", out int resultInt) ? (int?)int.Parse($"{x}") : null;
                    break;
                case "ulong":
                    x = ulong.TryParse($"{x}", out ulong resultULong) ? (ulong?)ulong.Parse($"{x}") : null;
                    break;
                case "long":
                    x = long.TryParse($"{x}", out long resultLong) ? (long?)long.Parse($"{x}") : null;
                    break;
                case "float":
                    x = float.TryParse($"{x}", out float resultFloat) ? (float?)float.Parse($"{x}") : null;
                    break;
                case "double":
                    x = double.TryParse($"{x}", out double resultDouble) ? (double?)double.Parse($"{x}") : null;
                    break;
                case "decimal":
                    x = decimal.TryParse($"{x}", out decimal resultDecimal) ? (decimal?)decimal.Parse($"{x}") : null;
                    break;
                case "bool":
                    x = bool.TryParse($"{x}", out bool resultBool) ? (bool?)bool.Parse($"{x}") : null;
                    break;
                case "char":
                    x = char.TryParse($"{x}", out char resultChar) ? (char?)char.Parse($"{x}") : null;
                    break;
                case null:
                    x = $"{x}";
                    break;
                default:
                    WriteLine("ERROR - VARIABLE TYPE DOESN'T EXIST. EXITING APPLICATION.");
                    Environment.Exit(0);
                    break;
            }
            return x;
        }

        public static dynamic Input(string message, string type = null)
        {
            Write(message);
            dynamic x = ReadLine();
            x = ReturnParsedValue(x, type);
            while (x == null)
            {
                Write("Invalid value. Please try again: ");
                x = ReadLine();
                x = ReturnParsedValue(x, type);
            }
            return x;
        }
    }
}
