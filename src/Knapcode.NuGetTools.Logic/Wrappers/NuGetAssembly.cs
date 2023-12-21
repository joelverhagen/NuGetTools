using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Knapcode.NuGetTools.Logic.Wrappers;

public record NuGetAssembly(
    string AssemblyName,
    string AssemblyAttributes)
{
    public static NuGetAssembly FromType<T>()
    {
        var assembly = typeof(T).Assembly;
        var assemblyName = assembly.FullName ?? throw new ArgumentException("No full name could be found.");

        var builder = new StringBuilder();
        foreach (var attribute in assembly.CustomAttributes)
        {
            builder.Append("[assembly: ");

            var attributeName = attribute.AttributeType.Name;
            const string suffix = nameof(Attribute);
            if (attributeName.EndsWith(suffix, StringComparison.Ordinal))
            {
                attributeName = attributeName.Substring(0, attributeName.Length - suffix.Length);
            }

            builder.Append(attributeName);
            builder.Append('(');

            var addedAttribute = false;

            foreach (var arg in attribute.ConstructorArguments)
            {
                if (addedAttribute)
                {
                    builder.Append(", ");
                }
                else
                {
                    addedAttribute = true;
                }

                AppendArgument(builder, arg);

            }

            foreach (var arg in attribute.NamedArguments)
            {
                if (addedAttribute)
                {
                    builder.Append(", ");
                }
                else
                {
                    addedAttribute = true;
                }

                builder.Append(arg.MemberName);
                builder.Append(" = ");
                AppendArgument(builder, arg.TypedValue);
            }

            builder.AppendLine(")]");
        }

        var assemblyAttributes = builder.ToString();

        return new NuGetAssembly(assemblyName, assemblyAttributes);
    }

    private static void AppendArgument(StringBuilder builder, CustomAttributeTypedArgument arg)
    {
        switch (arg.Value)
        {
            case ReadOnlyCollection<CustomAttributeTypedArgument> collectionValue:
                builder.AppendFormat("new[] { ");
                foreach (var innerArg in collectionValue)
                {
                    AppendArgument(builder, innerArg);
                }
                builder.Append(" }");
                break;
            default:
                var primitive = SymbolDisplay.FormatPrimitive(arg.Value!, quoteStrings: true, useHexadecimalNumbers: false);
                if (primitive is null)
                {
                    throw new NotImplementedException($"Writing out attribute value of type {arg.ArgumentType.FullName} is not implemented yet.");
                }
                builder.Append(primitive);
                break;
        }
    }
}
