﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using Pash.Implementation;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;

namespace Pash.ParserIntrinsics.AstNodes
{
    public class command_astnode : _astnode
    {
        private Collection<PSObject> _results;

        public command_astnode(AstContext astContext, ParseTreeNode parseTreeNode)
            : base(astContext, parseTreeNode)
        {
        }

        internal override object Execute_old(ExecutionContext context, ICommandRuntime commandRuntime)
        {
            ////        command:
            ////            command_name   command_elements_opt
            ////            command_invocation_operator   command_module_opt  command_name_expr   command_elements_opt

            var commandText = parseTreeNode.ChildNodes[0].FindTokenAndGetText();
            CommandInfo commandInfo = ((LocalRunspace)context.CurrentRunspace).CommandManager.FindCommand(commandText);

            if (commandInfo == null)
                throw new InvalidOperationException(commandText);

            IEnumerable<CommandParameter> parameters = new CommandParameter[] { };
            if (parseTreeNode.ChildNodes.Count == 2)
            {
                var commandElementsAstNode = ChildAstNodes.Skip(1).First().As<command_elements_astnode>();
                parameters = (IEnumerable<CommandParameter>)commandElementsAstNode.Execute_old(context, commandRuntime);
            }

            // MUST: fix this with the commandRuntime
            Pipeline pipeline = context.CurrentRunspace.CreateNestedPipeline();

            pipeline.Input.Write(context.inputStreamReader.ReadToEnd(), true);

            context.PushPipeline(pipeline);

            try
            {
                var command = new Command(commandText);
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
                pipeline.Commands.Add(command);
                return pipeline.Invoke();
            }
            finally
            {
                context.PopPipeline();
            }

        }
    }
}
