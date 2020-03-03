using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CodeGen
{
    public partial class Form1 : Form
    {
        public string[] config { get; private set; }
        private string serviceName;
        private string servideDefinitionPath;
        private string serviceConfigurationPath1;
        private string serviceConfigurationPath2;
        private string serviceConfigurationPath3;
        private string scopeBindingsPath;
        private bool writeFiles = false;
        private Dictionary<string, ItemData> configValues = new Dictionary<string, ItemData>();
        public Form1()
        {
            config = File.ReadAllLines(@"configsettings.xml");
            InitializeComponent();
            csdefDir.Text = @"E:\cbt\Interflow\src\ThreatIntel.Azure";
            Symbol.Text = "CosmosDbProcessingSettings.IndicatorsByUrlDocumentDbEndpointHost";
        }

        private void Symbol_TextChanged(object sender, EventArgs e)
        {
            string className;
            string SymbolName;
            string ServiceDefinionPath = Path.Combine(csdefDir.Text, "ServiceDefinition.csdef");

            Apply.Enabled = File.Exists(ServiceDefinionPath);

            SplitName(this.Symbol.Text, out className, out SymbolName);
            ServiceDefinitionTextBox.Text = BuildServiceDef(SymbolName);
            ServiceConfigurationTextBox.Text = BuildServiceConfig(SymbolName);
            ScopeBindingTextBox.Text = BuildScope(className, SymbolName);
            textBox4.Text = BuildCode(className, SymbolName);
            UpdatePrivates();
        }

        private string BuildCode(string className, string symbolName)
        {
//            CosmosDbProcessingSettings.IndicatorsByUrlDocumentDbEndpointHost = CloudConfigurationManager.GetSetting(nameof(CosmosDbProcessingSettings.IndicatorsByUrlDocumentDbEndpointHost));
//            CosmosDbProcessingSettings.IndicatorsByUrlDocumentDbAuthKey = KeyVaultHelper.GetKeyVaultSecretByUri(CloudConfigurationManager.GetSetting(nameof(CosmosDbProcessingSettings.IndicatorsByUrlDocumentDbAuthKeyUri)));

            StringBuilder sb = new StringBuilder();
            if(symbolName.EndsWith("Key"))
            {
                sb.AppendLine($"{className}.{symbolName} = KeyVaultHelper.GetKeyVaultSecretByUri(CloudConfigurationManager.GetSetting(nameof({className}.{symbolName}Uri)));");
            }
            else
            {
                sb.AppendLine($"{className}.{symbolName} = CloudConfigurationManager.GetSetting(nameof({className}.{symbolName}));");
            }

            return sb.ToString();
        }

        private void SplitName(string text, out string className, out string symbolName)
        {
            className = "unexpected input"; ;
            symbolName = "unexpected input";
            string[] splits = text.Split(new char[] { '.' });
            if (splits.Length == 1)
            {
                className = "";
                symbolName = splits[0];
            }

            if (splits.Length == 2)
            {
                className = splits[0]; ;
                symbolName = splits[1];
            }
        }

        private string BuildScope(string className, string text)
        {
            StringBuilder sb = new StringBuilder();
            string _text;
            if (text.EndsWith("Key"))
            {
                _text = CamelToUnderScore($"{text}Uri");
            }
            else
            {
                _text = CamelToUnderScore($"{text}");
            }

            string value = FindValue(className, text);

//            sb.AppendLine($",");
            sb.AppendLine($"        {{");
            sb.AppendLine($"          \"find\": \"{_text}\",");
            sb.AppendLine($"          \"replaceWith\": \"{value}\"");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        private string FindValue(string className, string text)
        {
            string currentClass = "None";
            string retVal = "XXX";
            bool getValue = false;
            bool getKeyValue = false;
            for (int index = 0; index < config.Length; index++)
            {
                if (config[index].Contains("<SettingClass "))
                {
                    int indexOfLastDot = config[index].LastIndexOf('.');
                    int indexOfLastQuote = config[index].LastIndexOf('\"');
                    currentClass = config[index].Substring(indexOfLastDot + 1, indexOfLastQuote - indexOfLastDot - 1);
                }

                if (currentClass.Equals(className))
                {
                    if (config[index].Contains($"<SettingValue PropertyName=\"{text}\""))
                    {
                        getValue = true;
                    }
                    if (config[index].Contains($"<SettingValue PropertyName=\"{text}Name\""))
                    {
                        getKeyValue = true;
                    }
                    if (getValue && config[index].Contains($"<PropertyValue"))
                    {
                        int indexOfValueStart = config[index].IndexOf('>');
                        string sub = config[index].Substring(indexOfValueStart + 1);
                        int indexOfValueEnd = sub.IndexOf('<');
                        if (indexOfValueEnd == -1)
                        {
                            // multi line
                            bool foundEnd = false;
                            StringBuilder multiline = new StringBuilder();
                            multiline.AppendLine(sub);
                            while (!foundEnd)
                            {
                                index++;
                                string sub2 = config[index];
                                int indexOfValueEnd2 = sub2.IndexOf('<');
                                if (indexOfValueEnd2 > 0)
                                {
                                    foundEnd = true;
                                    multiline.AppendLine(sub2.Substring(0, indexOfValueEnd2));
                                }
                                else
                                {
                                    multiline.AppendLine(sub2);
                                }
                            }
                            retVal = multiline.ToString();
                        }
                        else
                        {
                            retVal = sub.Substring(0, indexOfValueEnd);
                        }
                        getValue = false;

                    }

                    if (getKeyValue && config[index].Contains($"<PropertyValue"))
                    {
                        int indexOfValueStart = config[index].IndexOf('>');
                        string sub = config[index].Substring(indexOfValueStart + 1);
                        int indexOfValueEnd = sub.IndexOf('<');
                        retVal = $"Get From KeyVault Named {sub.Substring(0, indexOfValueEnd)}";
                        getKeyValue = false;

                    }


                }

            }
            retVal = retVal.Replace("\"", "&quot;");
            return retVal;
        }

        private string BuildServiceConfig(string text)
        {
            string _text;
            string _name = text;
            if (text.EndsWith("Key"))
            {
                _text = CamelToUnderScore($"{text}Uri");
                _name = $"{text}Uri";
            }
            else
            {
                _text = CamelToUnderScore($"{text}");
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"<Setting name=\"{_name}\" value=\"{_text}\"/>");

            return sb.ToString();
        }

        private string BuildServiceDef(string text)
        {
            StringBuilder sb = new StringBuilder();
            if (text.EndsWith("Key"))
            {
                sb.AppendLine($"<Setting name=\"{text}Uri\"/>");
            }
            else
            {
                sb.AppendLine($"<Setting name=\"{text}\"/>");
            }
            return sb.ToString();
        }

        private string CamelToUnderScore(string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("_");
            for (int index = 0; index < text.Length; index++)
            {
                char ch = text[index];
                if (char.IsUpper(ch))
                {
                    sb.Append("_");
                }
                sb.Append(ch.ToString().ToUpper());
            }
            sb.Append("__");
            return sb.ToString().Replace("_DOCUMENT_DB_","_DOCUMENTDB_");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(ServiceDefinitionTextBox.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(ServiceConfigurationTextBox.Text);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(ScopeBindingTextBox.Text);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(textBox4.Text);

        }

        private void OnBrowseDir_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = @"E:\cbt\Interflow\src\";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                csdefDir.Text = dialog.FileName; // E:\cbt\Interflow\src\ThreatIntel.Azure\ServiceDefinition.csdef
                UpdatePrivates();
            }
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            writeFiles = true;
            ApplyServiceDefinition(servideDefinitionPath, ServiceDefinitionTextBox.Text);
            ApplyServiceConfiguration(serviceConfigurationPath1, ServiceConfigurationTextBox.Text);
            ApplyServiceConfiguration(serviceConfigurationPath2, ServiceConfigurationTextBox.Text);
            ApplyServiceConfiguration(serviceConfigurationPath3, ServiceConfigurationTextBox.Text);
            ApplyScope(scopeBindingsPath, ScopeBindingTextBox.Text);
            writeFiles = false;

            ApplyServiceDefinition(servideDefinitionPath, ServiceDefinitionTextBox.Text);
            ApplyServiceConfiguration(serviceConfigurationPath1, ServiceConfigurationTextBox.Text);
            ApplyServiceConfiguration(serviceConfigurationPath2, ServiceConfigurationTextBox.Text);
            ApplyServiceConfiguration(serviceConfigurationPath3, ServiceConfigurationTextBox.Text);
            ApplyScope(scopeBindingsPath, ScopeBindingTextBox.Text);
        }

        private void ApplyScope(string scopeBindingsPath, string text)
        {
            // ReadFile 
            bool inScope = false;
            bool inBindings = false;
            bool inObject = false;
            bool found = false;
            string postfix = "";

            StringBuilder sb = new StringBuilder();
            StringBuilder obj = new StringBuilder();
            string lastFind = "";
            string[] replaceLines = text.Split(new char[] { '\r', '\n' },StringSplitOptions.RemoveEmptyEntries);
            string tryFind = replaceLines[1].Replace(" ", "");
            Encoding e = GetFileEncoding(scopeBindingsPath);

            string[] lines = File.ReadAllLines(scopeBindingsPath);
            for (int index = 0; index < lines.Length; index++)
            {
                if (lines[index].Contains("\"scopeTagName\": \"ServiceConfiguration\"")) { inScope = true; }
                if (lines[index].Contains("\"bindings\": [")) { inBindings = true; inObject = false; }
                if (inBindings /* && inScope */)
                {
                    if (lines[index].Contains("]"))
                    {
                        // Append the data
                        if (inScope && inBindings && !found)
                        {
                            //                            sb.AppendLine(text);
                            // Write the data.
                            checkBox3.Checked = false;
                            sb.Append(text);

                        }
                        inBindings = false;
                        inScope = false;
                        
                    }
                    if (lines[index].Trim().StartsWith("{"))
                    {
                        inObject = true;
                        obj = new StringBuilder();
                    }
                    if (lines[index].Trim().StartsWith("}"))
                    {
                        inObject = false;
                        sb.Append(obj.ToString());
                        if (inScope && lines[index+1].Trim().StartsWith("]") && !found && inBindings)
                        {
                            postfix = ",";
                        }
                    }
                    if (inObject)
                    {
                        string normal = lines[index].Replace(" ", "");
                        if(normal.Contains(tryFind))
                        {
                            found = true;
                        }
                        obj.AppendLine(lines[index]);
                    }
                    // findreplace
                    // read full object
                }
                if (!inObject)
                {
                    sb.AppendLine(lines[index] + postfix);
                    postfix = "";
                }
            }
            sb.AppendLine();
            string toWrite = sb.ToString().TrimEnd();
            //toWrite = toWrite.Substring(0, toWrite.Length - 2);
            if (writeFiles)
            {
                File.WriteAllText(scopeBindingsPath, toWrite, e);
            }
        }

        private Encoding GetFileEncoding(string scopeBindingsPath)
        {
            using (var reader = new StreamReader(scopeBindingsPath, Encoding.Default, true))
            {
                if (reader.Peek() >= 0) // you need this!
                    reader.Read();

                return reader.CurrentEncoding;
            }

        }

        private void ApplyServiceConfiguration(string serviceConfigurationPath, string text)
        {
            // ReadFile 
            bool inSettings = false;
            bool found = false;
            StringBuilder sb = new StringBuilder();

            Encoding e = GetFileEncoding(serviceConfigurationPath);

            string[] lines = File.ReadAllLines(serviceConfigurationPath);
            for (int index = 0; index < lines.Length; index++)
            {
                if (lines[index].Contains("<ConfigurationSettings>")) { inSettings = true; }
                if (inSettings)
                {
                    if (lines[index].Contains("</ConfigurationSettings>") && !found)
                    {
                        // Append the line
                        sb.AppendLine($"      {text.Trim()}");
                        inSettings = false;
                        checkBox2.Checked = false;
                    }
                    // normalize line for compare
                    string normal = lines[index].Replace(" ", "").Trim();
                    string normal2 = text.Replace(" ", "").Trim();
                    if (normal.Contains(normal2))
                    {
                        // already there ignore
                        inSettings = false;
                        found = true;
                    }
                }
                sb.AppendLine(lines[index]);
            }

            string toWrite = sb.ToString().TrimEnd();
            //toWrite = toWrite.Substring(0, toWrite.Length - 2);
            if (writeFiles)
            {
                File.WriteAllText(serviceConfigurationPath, toWrite, e);
            }
        }

        private void ApplyServiceDefinition(string servideDefinitionPath, string text)
        {
            // ReadFile 
            bool inSettings = false;
            StringBuilder sb = new StringBuilder();

            Encoding e = GetFileEncoding(servideDefinitionPath);
            checkBox1.Checked = true;
            checkBox2.Checked = true;
            checkBox3.Checked = true;

            string[] lines = File.ReadAllLines(servideDefinitionPath);
            for(int index = 0; index < lines.Length; index++)
            {
                if (lines[index].Contains("<ConfigurationSettings>")){ inSettings = true; }
                if (inSettings)
                {
                    if (lines[index].Contains("</ConfigurationSettings>"))
                    {
                        // Append the line
                        sb.AppendLine($"      {text.Trim()}");
                        inSettings = false;
                        checkBox1.Checked = false;
                    }

                    if (lines[index].Contains(text.Trim()))
                    {
                        // already there ignore
                        inSettings = false;
                        checkBox1.Checked = true;
                    }
                }
                
                sb.AppendLine(lines[index]);
            }
            sb.AppendLine();
            string toWrite = sb.ToString().TrimEnd();
            //            toWrite = toWrite.Substring(0, toWrite.Length - 2);
            if (writeFiles)
            {
                File.WriteAllText(servideDefinitionPath, toWrite, e);
            }
        }

        private void csdefDir_TextChanged(object sender, EventArgs e)
        {
            UpdatePrivates();
        }

        private void UpdatePrivates()
        {
            string srcPath = csdefDir.Text.Substring(0, csdefDir.Text.LastIndexOf('\\'));

            // E:\cbt\Interflow\src\ThreatIntel.Azure
            serviceName = csdefDir.Text.Substring(csdefDir.Text.LastIndexOf('\\') + 1);
            servideDefinitionPath = Path.Combine(csdefDir.Text, "ServiceDefinition.csdef");
            serviceConfigurationPath1 = Path.Combine(csdefDir.Text, "ServiceConfiguration.Cloud.cscfg");
            serviceConfigurationPath2 = Path.Combine(csdefDir.Text, "ServiceConfiguration.Local.cscfg");
            serviceConfigurationPath3 = Path.Combine(srcPath, $"Deployment\\{serviceName}\\Configurations\\ServiceConfiguration.Cloud.cscfg");
            scopeBindingsPath = Path.Combine(srcPath, $"Deployment\\{serviceName}\\ScopeBindings_INTVA.json");

            bool b1 = File.Exists(servideDefinitionPath);
            bool b2 = File.Exists(serviceConfigurationPath1);
            bool b3 = File.Exists(serviceConfigurationPath2);
            bool b4 = File.Exists(serviceConfigurationPath3);
            bool b5 = File.Exists(scopeBindingsPath); 

            Apply.Enabled = b1 & b2 & b3 & b4 & b5;

            writeFiles = false;
            if (!string.IsNullOrEmpty(ServiceDefinitionTextBox.Text))
            {
                ApplyServiceDefinition(servideDefinitionPath, ServiceDefinitionTextBox.Text);
            }
            if (!string.IsNullOrEmpty(ServiceConfigurationTextBox.Text))
            {
                ApplyServiceConfiguration(serviceConfigurationPath1, ServiceConfigurationTextBox.Text);
                ApplyServiceConfiguration(serviceConfigurationPath2, ServiceConfigurationTextBox.Text);
                ApplyServiceConfiguration(serviceConfigurationPath3, ServiceConfigurationTextBox.Text);
            }
            if (!string.IsNullOrEmpty(ScopeBindingTextBox.Text))
            {

                ApplyScope(scopeBindingsPath, ScopeBindingTextBox.Text);
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            bool inSettings = false;
            string pattern = "<Setting\\s+name\\s*=\\s*\"(?<name>\\w+)\"\\s*/>";
            Regex regex = new Regex(pattern);
            string[] source = File.ReadAllLines(@"E:\cbt\Interflow\src\ThreatIntel\App_Start\WebRole.cs");

            string pattern1 = "\\.GetSetting\\(nameof\\(\\w+\\.(?<name>\\w+)\\)\\)";
            Regex regex2 = new Regex(pattern1);

            // read settings in webrole.cs
            HashSet<string> inFile = new HashSet<string>();

            for (int index = 0; index < source.Length; index++)
            {
                string line = source[index];
                MatchCollection m = regex2.Matches(line);
                if (m.Count == 1)
                {
                    GroupCollection groups = m[0].Groups;
                    string name = groups["name"].Value;
                    inFile.Add(name);
                }
            }
            StringBuilder sb = new StringBuilder();

            string[] serviceDefinitionLines = File.ReadAllLines(servideDefinitionPath);
            for (int index = 0; index < serviceDefinitionLines.Length; index++)
            {
                string line = serviceDefinitionLines[index];
                if (line.Contains("<ConfigurationSettings>")) { inSettings = true; }
                if (inSettings)
                {
                    if (line.Contains("</ConfigurationSettings>"))
                    {
                        inSettings = false;
                    }
                    
                    MatchCollection matches = regex.Matches(line);

                    if (matches.Count == 1)
                    {
                        GroupCollection groups = matches[0].Groups;
                        string name = groups["name"].Value;
                        if (!inFile.Contains(name))
                        {
                            sb.AppendLine(name);
                        }
                    }
                }
                textBox4.Text = sb.ToString();
            }



        }

        private void GetConfigButtonClicked(object sender, EventArgs e)
        {
            int count = 0;
            string currentClass = "Unknown";
            string currentPropertyName = "unknown";
            string currentType = "unknown";
            bool MultiLine = false;
            StringBuilder sbMl = new StringBuilder();

            string[] config = File.ReadAllLines(@"configsettings.xml");
            
            Regex findClassRegex = new Regex("<SettingClass FullClassName=\"(?<class>.+)\">");
            Regex findPropertyName = new Regex("<SettingValue PropertyName=\"(?<name>.+)\">");
            Regex findPropertyValue =   new Regex("<PropertyValue xsi:type=\"xsd:(?<type>\\w+)\">(?<value>.*)</PropertyValue>");
            Regex findPropertyValueML = new Regex("<PropertyValue xsi:type=\"xsd:(?<type>\\w+)\">(?<value>[{\\[])\\s*");
            Regex findEndML = new Regex("^\\s*(?<value>[}\\]])</PropertyValue>");

            configValues.Clear();
            for(int index = 0; index < config.Length; index++)
            {
                string line = config[index];
                MatchCollection matchCollection = findClassRegex.Matches(line);
                if (matchCollection.Count == 1)
                {
                    GroupCollection groups = matchCollection[0].Groups;
                    currentClass = groups["class"].Value;
                }
                matchCollection = findPropertyName.Matches(line);
                if (matchCollection.Count == 1)
                {
                    GroupCollection groups = matchCollection[0].Groups;
                    currentPropertyName = groups["name"].Value;
                }
                matchCollection = findPropertyValue.Matches(line);
                if (matchCollection.Count == 1)
                {
                    GroupCollection groups = matchCollection[0].Groups;
                    string type = groups["type"].Value;
                    string value  = groups["value"].Value;
                    configValues[$"{count++:0000}"] = new ItemData { Class = currentClass, Property = currentPropertyName, Type = type, Value = value, MultiLine=false };
                }
                matchCollection = findPropertyValueML.Matches(line);
                if (matchCollection.Count == 1)
                {
                    sbMl.Clear();

                    GroupCollection groups = matchCollection[0].Groups;
                    string type = groups["type"].Value;
                    string value = groups["value"].Value;
                    line = value;
                    MultiLine = true;
                }
                matchCollection = findEndML.Matches(line);
                if (matchCollection.Count == 1 && MultiLine)
                {
                    GroupCollection groups = matchCollection[0].Groups;
                    string value = groups["value"].Value;
                    sbMl.AppendLine(value);
                    configValues[$"{count++:0000}"] = new ItemData { Class = currentClass, Property = currentPropertyName, Type = currentType, Value = sbMl.ToString(),MultiLine=true };
                    MultiLine = false;
                    sbMl.Clear();
                }
                if (MultiLine)
                {
                    sbMl.AppendLine(line);
                }
            }

            sbMl.Clear();
            sbMl.AppendLine($"      <!-- ServiceConfiguration.Cloud.cscfg  -->");
            string className = string.Empty;

            foreach (var item in configValues)
            {
                string replaceName = CamelToUnderScore(item.Value.Property);

                if (item.Value.Class != className)
                {
                    className = item.Value.Class;
                    sbMl.AppendLine();
                    sbMl.AppendLine($"      <!-- {className} settings  -->");
                }

                sbMl.AppendLine($"<Setting name=\"{item.Value.Property}\" value=\"{replaceName}\"/>");
            }

            sbMl.AppendLine($"      <!-- ServiceDefinition.csdef  -->");
            className = string.Empty;

            foreach (var item in configValues)
            {
                string replaceName = CamelToUnderScore(item.Value.Property);

                if (item.Value.Class != className)
                {
                    className = item.Value.Class;
                    sbMl.AppendLine();
                    sbMl.AppendLine($"      <!-- {className} settings  -->");
                }

                sbMl.AppendLine($"<Setting name=\"{item.Value.Property}\"/>");
            }

            bool first = true;
            className = string.Empty;
            sbMl.AppendLine($"      <!-- Bindings  -->");

            foreach (var item in configValues)
            {
                string name = CamelToUnderScore(item.Value.Property);

                if (!first)
                {
                    sbMl.AppendLine(",");
                }

                if (item.Value.Class != className)
                {
                    className = item.Value.Class;
                    sbMl.AppendLine();
                    sbMl.AppendLine($"***********************{className}***********************");
                }

                first = false;

                sbMl.AppendLine("        {");
                sbMl.AppendLine($"\"find\": \"{name}\",");
                string v = item.Value.Value.Replace("\"", "&quot;");
                if (item.Value.MultiLine)
                {

                    sbMl.AppendLine($"\"replaceWith\": \"{v}\",");
                }
                else
                {
                    sbMl.AppendLine($"\"replaceWith\": \"{v}\",");
                }
                sbMl.Append("        }");
            }


            Clipboard.SetText(sbMl.ToString());
        }
    }
}
