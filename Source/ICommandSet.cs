using System.Collections.Generic;
using System;
using System.Reflection;

public interface ICommandSet : ICommandSetChild
{
    string SetName { get; }
    List<ICommandSetChild> Children { get; }
}