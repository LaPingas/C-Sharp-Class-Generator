using System;
using System.IO;
using System.Collections.Generic;
using static System.Console;
using static Class_Generator.Toolkit;

namespace Class_Generator
{
    class Program
    {
        public static void Main(string[] args)
        {
            string className = Input("Enter class name: ");
            string userNamespace = Input("Enter namespace: ");
            File.AppendAllText($"{className}.cs",
                "using System" + Environment.NewLine +
                "" + Environment.NewLine +
                $"namespace {userNamespace}" + Environment.NewLine +
                "{" + Environment.NewLine +
                $"    public class {className}" + Environment.NewLine +
                "    {" + Environment.NewLine);
            string protectionLevel = "";
            List<string> attributeNameList = new List<string>();
            List<string> attributeTypeList = new List<string>();
            char input = Input("Add attributes? (y/n) ", "char");
            while (input != 'y' && input != 'n') input = Input("Invalid input. Add attributes? (Y/N) ", "char");
            while (input == 'y')
            {
                protectionLevelIndicator:
                int protectionLevelIndicator = Input("Enter protection level " +
                    "(1 for public, 2 for private, " +
                    "3 for internal, 4 for protected internal, 5 for private protected): ", "int");
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
                File.AppendAllText($"{className}.cs",
                    $"		{protectionLevel} {attributeType} {attributeName};" + Environment.NewLine);
                input = Input("Continue? (y/n) ", "char");
                while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
            }
            input = Input("Add default constructor? (y/n) ", "char");
            while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
            if (input == 'y')
            {
                File.AppendAllText($"{className}.cs",
                    Environment.NewLine + $"        public {className}()" + " { }");
            }
            input = Input("Add values constructor? (y/n) ", "char");
            while (input != 'y' && input != 'n') input = Input("Invalid input. Continue? (y/n) ", "char");
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
            for (int i = 0; i < attributeNameList.Count; i++)
            {
                File.AppendAllText($"{className}.cs",
                        Environment.NewLine + Environment.NewLine + $"        public {attributeTypeList[i]} {attributeNameList[i].Substring(0, 1).ToUpper() + attributeNameList[i].Substring(1)}"
                        + Environment.NewLine + "        {"
                        + Environment.NewLine + "            get { return " + attributeNameList[i] + "; }"
                        + Environment.NewLine + "            set { " + attributeNameList[i] + " = value; }"
                        + Environment.NewLine + "        }");
            }
            File.AppendAllText($"{className}.cs", Environment.NewLine + "    }" + Environment.NewLine + "}");
        }
    }
}
