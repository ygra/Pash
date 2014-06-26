// Copyright (C) Pash Contributors. License: GPL/BSD. See https://github.com/Pash-Project/Pash/
using System.Management.Automation;
using System.Linq;
using Microsoft.PowerShell.Commands.Utility.Internal;

namespace Microsoft.PowerShell.Commands
{
    [Cmdlet("Get", "Variable")]
    public class GetVariableCommand : PSCmdlet
    {
        [Parameter]
        public string[] Exclude { get; set; }

        [Parameter]
        public string[] Include { get; set; }

        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true), ValidateNotNull]
        public string[] Name { get; set; }

        [Parameter]
        public SwitchParameter ValueOnly { get; set; }

        public GetVariableCommand()
        {
        }

        protected override void ProcessRecord()
        {
            var selection = new VariableSelection(SessionState, Name, Include, Exclude);
            selection.Process(
                delegate(PSVariable variable)
                {
                    WriteObject(ValueOnly.ToBool() ? variable.Value : variable);
                });
        }
    }
}
