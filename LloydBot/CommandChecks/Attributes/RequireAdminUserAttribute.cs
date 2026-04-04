using DSharpPlus.Commands.ContextChecks;

namespace LloydBot.CommandChecks.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireAdminUserAttribute : ContextCheckAttribute
{
}