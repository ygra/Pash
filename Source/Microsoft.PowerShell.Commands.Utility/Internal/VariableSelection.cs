using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.PowerShell.Commands.Utility.Internal
{
    /// <summary>
    /// Helper class to ease selecting several variables via
    /// the parameters Name, Include and Exclude. Those parameters
    /// appear on several cmdlets related to variables, e.g.
    /// Get-Variable, Remove-Variable, and Set-Variable, which all
    /// should have the same semantics regarding the behaviour of
    /// those parameters.
    /// </summary>
    public class VariableSelection
    {
        public SessionState State { get; set; }

        public string[] Name { get; private set; }

        public string[] Include { get; private set; }

        public string[] Exclude { get; private set; }

        private Dictionary<string, PSVariable> variables;

        public VariableSelection(SessionState state, string[] name, string[] include, string[] exclude)
        {
            State = state;
            Name = name;
            Include = include;
            Exclude = exclude;
        }

        private Dictionary<string, PSVariable> EnumVariables()
        {
            if (variables == null)
            {
                // Initial selection is done by name, if present
                var vars = State.PSVariable.GetAll().Select(kv => kv);
                if (Name != null)
                {
                    // TODO: This could be made faster, I guess
                    var wildcards = Name.Select(name => new WildcardPattern(name, WildcardOptions.IgnoreCase));
                    vars = vars.Where(kv => wildcards.Any(wc => wc.IsMatch(kv.Key)));
                }

                // Then the selection is filtered by the Include list
                if (Include != null)
                {
                    // TODO: See above
                    var wildcards = Include.Select(name => new WildcardPattern(name, WildcardOptions.IgnoreCase));
                    vars = vars.Where(kv => wildcards.Any(wc => wc.IsMatch(kv.Key)));
                }

                // Then Excludes are handled
                if (Exclude != null)
                {
                    // TODO: See above
                    var wildcards = Exclude.Select(name => new WildcardPattern(name, WildcardOptions.IgnoreCase));
                    vars = vars.Where(kv => !wildcards.Any(wc => wc.IsMatch(kv.Key)));
                }

                variables = vars.ToDictionary(kv => kv.Key, kv => kv.Value);
            }
            return variables;
        }

        /// <summary>
        /// Performs an action on every selected variable. Optionally an
        /// action can be performed on variable names that failed to match.
        /// By default the behaviour for variable names that failed to match
        /// is to throw, but this would be unwanted for e.g. Set-Variable.
        /// </summary>
        /// <param name="action">The action to perform on the selected variables.</param>
        /// <param name="missingAction">
        /// The action to perform on variable names that failed to match.
        /// If this is <code>null</code> the default behaviour is to throw
        /// an exception.
        /// </param>
        public void Process(Action<PSVariable> action, Action<string> missingAction = null)
        {
            if (missingAction == null)
            {
                missingAction = (variableName) => {
                    // TODO: A better exception type for this?
                    throw new Exception(string.Format("The variable {0} could not be found.", variableName));
                };
            }

            var variables = EnumVariables();
            foreach (var variable in variables)
            {
                if (variable.Value == null)
                {
                    missingAction(variable.Key);
                }
                else
                {
                    action(variable.Value);
                }
            }
        }
    }
}

