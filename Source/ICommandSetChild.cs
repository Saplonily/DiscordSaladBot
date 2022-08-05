using System;

namespace SaladBot;

/// <summary>
/// 被Command和CommandSet继承
/// </summary>
public interface ICommandSetChild
{
    bool IsSet { get; }
    ICommandSet BelongTo { get; }
}