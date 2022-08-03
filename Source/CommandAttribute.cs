using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CommandAttribute : Attribute
{
    public string Name { get; private set; } = "null";
    public int ArgumentCounts { get; private set; } = 0;

    public CommandAttribute(string name, int argumentCounts)
    {
        this.Name = name;
        this.ArgumentCounts = argumentCounts;
    }
}