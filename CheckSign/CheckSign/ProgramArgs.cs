using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Interflow.Utility;

namespace CheckSign
{
    public class ProgramArgs : ProgramArguments
    {
        private static List<CommandData> myRules { get; } = new List<CommandData>
        {
            new CommandData(
                nameAndSynonyms: "drop,d",
                isRequired: true,
                hasParameter: true,
                parameterDescription: "Drop Path",
                description: @"Drop path to be checked. This should be a local copy of the build drop.",
                objectType: typeof(string)
                ),
            new CommandData(
                nameAndSynonyms: "expand,e",
                isRequired: true,
                hasParameter: true,
                parameterDescription: "Package expansion path.",
                description: @"Path where package files will be expaneded. Expanding the same place can generate paths that are too deep to process.",
                objectType: typeof(string)
                ),
            new CommandData(
                nameAndSynonyms: "maxdepth,m",
                isRequired: false,
                hasParameter: true,
                parameterDescription: "Maximum recursion depth.",
                description: @"Maximum recursion depth.",
                objectType: typeof(int),
                defaultValue:100
                ),
            new CommandData(
                nameAndSynonyms: "norecurse,n",
                isRequired: false,
                hasParameter: false,
                parameterDescription: "Do not use recursion.",
                description: @"This flag is used to prevent checking subdirectories of the drop folder.",
                objectType: typeof(bool),
                defaultValue:false
                ),
            new CommandData(
                nameAndSynonyms: "powershell,p",
                isRequired: false,
                hasParameter: false,
                parameterDescription: "Check Powershell (*.ps*) files.",
                description: @"Verifying signatures in *.ps* files.",
                objectType: typeof(bool),
                defaultValue:false
                ),
            new CommandData(
                nameAndSynonyms: "results,r",
                isRequired: true,
                hasParameter: true,
                parameterDescription: "Results Path.",
                description: @"A path where results will be written.",
                objectType: typeof(string)
                ),
            new CommandData(
                nameAndSynonyms: "skippackages,s",
                isRequired: false,
                hasParameter: false,
                parameterDescription: "Skip packages.",
                description: @"Skip Verifying signatures in packages.",
                objectType: typeof(bool),
                defaultValue:false
                ),
            new CommandData(
                nameAndSynonyms: "verbose,v",
                isRequired: false,
                hasParameter: false,
                parameterDescription: "Verbose console output.",
                description: @"Use more verbose console output.",
                objectType: typeof(bool),
                defaultValue:false
                ),
        };

        /// <summary>
        /// Prevents a default instance of the ProgramArguments class from being created.
        /// </summary>
        public ProgramArgs(string[] args) : base("Check Sign App",
            "This application is made to find unsigned files in a build drop.",
            AddRangeToCommands(myRules),
            args)
        {
            
        }


        private static List<CommandData> AddRangeToCommands(IList<CommandData> range)
        {
            CommandRules.AddRange(range);
            return CommandRules;
        }
    }
}
