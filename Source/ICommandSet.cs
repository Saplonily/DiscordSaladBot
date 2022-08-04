using System.Collections.Generic;
using System;
using System.Reflection;

public interface ICommandSet : ICommandSetChild
{
    string SetName { get; }
    List<ICommandSet> ChildCommandSets { get; }
    List<Command> ChildCommands { get; }
}