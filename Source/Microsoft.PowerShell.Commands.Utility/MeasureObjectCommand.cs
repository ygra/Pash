using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Microsoft.PowerShell.Commands.Utility
{
    [CmdletAttribute("Measure", "Object", DefaultParameterSetName = "GenericMeasure")]
    public sealed class MeasureObjectCommand : PSCmdlet
    {
        [ValidateNotNullOrEmpty]
        [Parameter(Position = 0)]
        public string[] Property { get; set; }

        [Parameter(ParameterSetName = "GenericMeasure")]
        public SwitchParameter Average { get; set; }

        [Parameter(ValueFromPipeline = true)]
        public PSObject InputObject { get; set; }

        [Parameter(ParameterSetName = "GenericMeasure")]
        public SwitchParameter Maximum { get; set; }

        [Parameter(ParameterSetName = "GenericMeasure")]
        public SwitchParameter Minimum { get; set; }

        [Parameter(ParameterSetName = "GenericMeasure")]
        public SwitchParameter Sum { get; set; }

        [Parameter(ParameterSetName = "TextMeasure")]
        public SwitchParameter Character { get; set; }

        [Parameter(ParameterSetName = "TextMeasure")]
        public SwitchParameter IgnoreWhiteSpace { get; set; }

        [Parameter(ParameterSetName = "TextMeasure")]
        public SwitchParameter Line { get; set; }

        [Parameter(ParameterSetName = "TextMeasure")]
        public SwitchParameter Word { get; set; }

        private MeasureInfo[] info;

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
        }

        protected override void EndProcessing()
        {
        }
    }

    public abstract class MeasureInfo
    {
        public string Property { get; set; }
        protected MeasureInfo() { }
    }

    public sealed class GenericMeasureInfo : MeasureInfo
    {
        public double? Average { get; set; }
        public int Count { get; set; }
        public double? Maximum { get; set; }
        public double? Minimum { get; set; }
        public double? Sum { get; set; }
        public GenericMeasureInfo() { }
    }

    public sealed class GenericObjectMeasureInfo : MeasureInfo
    {
        public double? Average { get; set; }
        public int Count { get; set; }
        public object Maximum { get; set; }
        public object Minimum { get; set; }
        public double? Sum { get; set; }
        public GenericObjectMeasureInfo() { }
    }

    public sealed class TextMeasureInfo : MeasureInfo
    {
        public int? Characters { get; set; }
        public int? Lines { get; set; }
        public int? Words { get; set; }
        public TextMeasureInfo() { }
    }
}
