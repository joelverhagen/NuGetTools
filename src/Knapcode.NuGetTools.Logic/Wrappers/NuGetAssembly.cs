using System.Collections.ObjectModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Knapcode.NuGetTools.Logic.Wrappers;

public record NuGetAssembly(
    string RelativePath,
    string AssemblyName,
    string Sha256Hash,
    long FileSize,
    string CustomAttributes)
{
    public static NuGetAssembly FromAssembly(string relativePath, Assembly assembly)
    {
        var assemblyName = assembly.FullName ?? throw new ArgumentException("No full name could be found.");

        string sha256Hash;
        long fileSize;
        using (var fileStream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read))
        {
            sha256Hash = Convert.ToHexString(SHA256.HashData(fileStream));
            fileSize = fileStream.Length;
        }

        var customAttributes = GetCustomAttributes(assembly);

        return new NuGetAssembly(
            relativePath,
            assemblyName,
            sha256Hash,
            fileSize,
            customAttributes);
    }

    private static string GetCustomAttributes(Assembly assembly)
    {
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

        return builder.ToString();
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
