using Microsoft.Interflow.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace CheckSign
{
    internal class Program
    {
        public static ProgramArgs Args { get; private set; }
        public static String ResultsPath { get; private set; }
        public static String DropPath { get; private set; }
        public static String ExpandPath { get; private set; }
        public static bool CheckPowerShellFiles { get; private set; }
        public static bool VerboseOutput { get; private set; }
        public static bool SkipPackages { get; private set; }
        public static bool UseRecursion { get; private set; }
        public static int MaxDepth { get; private set; }

        public static String UnsignedPath { get; private set; }
        public static String WrongCertPath { get; private set; }

        private static int depth = 0;

        private static CertListInfo unsigned = new CertListInfo() { Name = "Unsigned" };
        private static CertListInfo signedWith143360005 = new CertListInfo() { Name = "143360005", ValidForProd = true };
        private static CertListInfo signedWith143360006 = new CertListInfo() { Name = "143360006", ValidForProd = true };
        private static CertListInfo signedWith143360007 = new CertListInfo() { Name = "143360007", ValidForProd = true };
        private static CertListInfo signedWith143360008 = new CertListInfo() { Name = "143360008", ValidForProd = true };
        private static CertListInfo signedWithMSCorpCert = new CertListInfo() { Name = "MS Corp", ValidForProd = true };
        private static CertListInfo signedWithMSWindowsCert = new CertListInfo() { Name = "MS Windows", ValidForProd = true };
        private static CertListInfo signedWithAzureEngBuildCert = new CertListInfo() { Name = "Azure Build", ValidForProd = false };
        private static CertListInfo signedWithTestCert = new CertListInfo() { Name = "Test Cert", ValidForProd = false };
        private static CertListInfo signedWithOtherCert = new CertListInfo() { Name = "Other Cert", ValidForProd = false };

        private static Stack<String> pathStack = new Stack<string>();

        static Dictionary<string, CertListInfo> findList = new Dictionary<string, CertListInfo>()
            {
                { "CN=Microsoft Azure Code Sign", signedWith143360005},
                { "CN=Microsoft Azure 3rd Party Code Sign", signedWith143360006},
                { "CN=Microsoft Azure Dependency Code Sign", signedWith143360007},
                { "CN=Microsoft Azure Catalog CodeSign", signedWith143360008},
                { "CN=Microsoft Corporation, O=Microsoft Corporation, L=Redmond, S=Washington, C=US", signedWithMSCorpCert},
                { "CN=Microsoft Corporation, OU=MOPR, O=Microsoft Corporation, L=Redmond, S=Washington, C=US", signedWithMSCorpCert},
                { "CN=Microsoft Corporation, OU=AOC, O=Microsoft Corporation, L=Redmond, S=Washington, C=US", signedWithMSCorpCert},
                { "CN=Microsoft Windows Hardware Compatibility Publisher, O=Microsoft Corporation, L=Redmond, S=Washington, C=US", signedWithMSWindowsCert},
                { "CN=Microsoft Windows, O=Microsoft Corporation, L=Redmond, S=Washington, C=US", signedWithMSWindowsCert},
                { "CN=AzureEngBuildCodeSign", signedWithAzureEngBuildCert},
                { "CN=TestAzureEngBuildCodeSign", signedWithTestCert},
                { "UNSIGNED", unsigned},
                { "OTHER", signedWithOtherCert},
            };

        static int Main(string[] args)
        {
            ConsoleHelper.Write($"Check Sign : {Assembly.GetExecutingAssembly().GetName().Version}");
            Args = new ProgramArgs(args);
            if (Args.HasErrors)
            {
                Args.DisplayErrors();
                return ErrorValue.CommandLineErrors;
            }

            if (!ValiateArgs())
            {
                return ErrorValue.ArgumentValidation;
            }

            PathStack.Clear();

            CheckDirectory(DropPath);

            return ErrorValue.Success;
        }

        private static void CheckDirectory(string directory)
        {
            try
            {
                depth++;
                PathStack.Push(directory);
                try
                {
                    if (depth < MaxDepth)
                    {
                        if (Directory.Exists(directory))
                        {

                            string[] dllFiles = Directory.GetFiles(directory, "*.dll");
                            string[] exeFiles = Directory.GetFiles(directory, "*.exe");
                            string[] ps1Files = Directory.GetFiles(directory, "*.ps*");

                            List<string> allFiles = new List<string>();
                            allFiles.AddRange(dllFiles);
                            allFiles.AddRange(exeFiles);
                            if (CheckPowerShellFiles)
                            {
                                allFiles.AddRange(ps1Files);
                            }

                            if (!SkipPackages)
                            {
                                string[] cspkgFiles = Directory.GetFiles(directory, "*.cspkg"); // compressed directories
                                string[] cssxFiles = Directory.GetFiles(directory, "*.cssx"); // compressed directories
                                allFiles.AddRange(cspkgFiles);
                                allFiles.AddRange(cssxFiles);
                            }

                            string path = directory.Substring(directory.LastIndexOf("\\") + 1);

                            if (allFiles.Count > 0)
                            {
                                ConsoleHelper.Write($"Checking {allFiles.Count} files in {directory}.");
                            }

                            foreach (string file in allFiles)
                            {
                                if (file.EndsWith(".cspkg") || file.EndsWith(".cssx"))
                                {
                                    if (!file.Contains(".NotSigned."))
                                    {
                                        string expandDir = $"{ExpandPath}\\{Path.GetFileName(file)}.X";
                                        if (Directory.Exists(expandDir))
                                        {
                                            if (VerboseOutput)
                                            {
                                                ConsoleHelper.Write($"Found previous expanded file. Removing : {Path.GetFileName(file)}");
                                            }
                                            // clean up from previous run if necessary...
                                            Directory.Delete(expandDir, true);
                                        }

                                        ConsoleHelper.Write($"Expanding file : {Path.GetFileName(file)}");
                                        bool isPacked = file.StartsWith(ExpandPath);
                                        string rootToUse = isPacked ? ExpandPath : DropPath;
                                        string rootless = file.Substring(rootToUse.Length + 1);
                                        System.IO.Compression.ZipFile.ExtractToDirectory(file, expandDir);
                                        if (VerboseOutput)
                                        {
                                            Console.WriteLine($"Expanded file : {Path.GetFileName(file)}");
                                        }

                                        CheckDirectory(expandDir);

                                        if (VerboseOutput)
                                        {
                                            Console.WriteLine($"Finished compressed file. Removing expanded files : {Path.GetFileName(file)}");
                                        }

                                        Directory.Delete(expandDir, true);

                                    }
                                    else
                                    {
                                        if (VerboseOutput)
                                        {
                                            ConsoleHelper.Write($"Skipping unsigned Package File : {Path.GetFileName(file)}");
                                        }
                                    }
                                }
                                else
                                {
                                    X509Certificate2 signedassembly = null;
                                    CertListInfo actOnCert = findList["OTHER"];

                                    try
                                    {
                                        string fileName = Path.GetFileName(file);
                                        signedassembly = new X509Certificate2(X509Certificate.CreateFromSignedFile(file));

                                        // check for correct signature
                                        foreach (string key in findList.Keys) // Okay this is a hacky way of doing this.
                                        {
                                            if (signedassembly.Subject.StartsWith(key))
                                            {
                                                actOnCert = findList[key];
                                                break;
                                            }
                                        }

                                        if (actOnCert != null)
                                        {
                                            bool isPacked = directory.StartsWith(ExpandPath);
                                            string rootToUse = isPacked ? ExpandPath : DropPath;
                                            if (!actOnCert.ValidForProd)
                                            {
                                                ConsoleHelper.WriteError($"File : {file} signed with non-production cert : {actOnCert.Name}");
                                                File.AppendAllText(WrongCertPath, $"NonProd Cert used : {file} : {actOnCert.Name}{Environment.NewLine}");
                                            }
                                            else
                                            {
                                                if (VerboseOutput)
                                                {
                                                    ConsoleHelper.Write($"File : {file} signed with production cert : {actOnCert.Name}");
                                                }
                                            }

                                        }


                                        }
                                    catch (Exception)
                                    {
                                        string notes = GetNotes(file);
                                        if (notes.StartsWith("Warning"))
                                        {
                                            ConsoleHelper.WriteWarning($"{notes}Unsigned file : {file}");
                                        }
                                        else
                                        {
                                            ConsoleHelper.WriteError($"Unsigned file : {file}");
                                        }

                                        File.AppendAllText(UnsignedPath, $"{notes}Unsigned file : {file}{Environment.NewLine}");
                                    }
                                }
                            }

                            if (UseRecursion)
                            {
                                string[] dirs = Directory.GetDirectories(directory);
                                foreach (string d in dirs)
                                {
                                    CheckDirectory(d);
                                }
                            }
                        }
                    }
                    else
                    {
                        ConsoleHelper.WriteWarning($"Reached max recursion depth of {MaxDepth}. ");
                    }
                }
                catch (Exception exception)
                {
                    ConsoleHelper.WriteError($"Unexpected Error processing {directory} : {exception.Message}");
                    ConsoleHelper.Indent(true);
                    ConsoleHelper.WriteError($"Exception : {exception.Message}");
                    ConsoleHelper.Indent(false);
                    return;
                }
            }
            finally
            {
                PathStack.Pop();
                depth--;
            }
        }

        private static string GetNotes(string file)
        {
            if (file.Contains(@"plugins\Diagnostics\Newtonsoft.Json.dll"))
            {
                return "Warning : File added by packaging system and we are not able to sign it. : ";
            }

            if (file.Contains(@"\bin\App_Web_") && file.EndsWith(".dll") && file.Contains(@"\_PublishedWebsites\")) 
            {
                return "Warning : File dynamically built with random names are not signed because wildcards don't work correctly. : ";
            }

            return "Error : ";
        }

        private static bool ValiateArgs()
        {
            DropPath = (string) Args["drop"];
            ResultsPath = (string)Args["results"];
            ExpandPath = (string)Args["expand"];
            SkipPackages = (bool)Args["skippackages"];
            VerboseOutput = (bool)Args["verbose"];
            CheckPowerShellFiles = (bool)Args["powershell"];
            UseRecursion = !(bool)Args["norecurse"];

            MaxDepth = (int)Args["maxdepth"];
            try
            {
                if (!Directory.Exists(DropPath))
                {
                    ConsoleHelper.WriteError($"Error: Drop Path not found : {DropPath}");
                    return false;
                }

                if (!Directory.Exists(ResultsPath))
                {
                    ConsoleHelper.Write($"Result Path not found. Creating directory : {ResultsPath}");
                    Directory.CreateDirectory(ResultsPath);
                }

                if (!Directory.Exists(ExpandPath))
                {
                    ConsoleHelper.Write($"Expand Path not found. Creating directory : {ExpandPath}");
                    Directory.CreateDirectory(ExpandPath);
                }

            }
            catch (Exception exception)
            {
                ConsoleHelper.WriteError($"Error: Exception encountered validating arguments : {exception.Message}.");
                return false;
            }

            UnsignedPath = Path.Combine(ResultsPath, "Unsigned.log");
            WrongCertPath = Path.Combine(ResultsPath, "NonProduction.log");

            if (File.Exists(UnsignedPath))
            {
                ConsoleHelper.Write($"Warning {UnsignedPath} already exists. New data will be appended to the end of this file.");
            }
            if (File.Exists(WrongCertPath))
            {
                ConsoleHelper.Write($"Warning {WrongCertPath} already exists. New data will be appended to the end of this file.");
            }

            File.AppendAllText(UnsignedPath, $"----- Processing {DropPath} at UTC {DateTime.UtcNow} -----{Environment.NewLine}");
            File.AppendAllText(WrongCertPath, $"----- Processing {DropPath} at UTC {DateTime.UtcNow} -----{Environment.NewLine}");

            return true;
        }
    }

}
