using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


public static class CommandSetHelper
{
    /// <summary>
    /// 生成这个类本身带有Command特性的所有方法(指令)
    /// </summary>
    /// <param name="ins">CommandSet实例</param>
    public static List<Command> GetCommands<T>(T ins) where T : ICommandSet
    {
        //TODO 上次在cmd这里
        var type = typeof(T);
        var methods = type.GetMethods();
        var cmds = new List<Command>();
        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<CommandAttribute>();
            if (attr is not null)
            {
                cmds.Add(
                    Command.Create(
                        attr.Name,
                        attr.ArgumentCounts,
                        (args, msg) =>
                        method.Invoke(ins, new object[] { args, msg }),
                        ins
                        )
                    );
            }
        }
        return cmds;
    }
}