using System;
using System.Linq;
using System.Reflection;
#if NET452_OR_GREATER || NETCOREAPP
using System.Xml.Serialization;
using SabreTools.Models;
#endif
using SabreTools.Models.Metadata;

namespace SabreTools.Filter
{
    public static class TypeHelper
    {
        /// <summary>
        /// Get constant values for the given type, if possible
        /// </summary>
        public static string[]? GetConstants(Type? type)
        {
            if (type == null)
                return null;

            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (fields == null)
                return null;

#if NET20 || NET35 || NET40
            // TODO: Figure out how to do this, try using the following:
            // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.customattributedata?view=net-8.0
            return null;
#else
            return fields
                .Where(f => f.IsLiteral && !f.IsInitOnly)
                .Where(f => !f.CustomAttributes.Any(a => a.AttributeType == typeof(NoFilterAttribute)))
                .Select(f => f.GetRawConstantValue() as string)
                .Where(v => v != null)
                .ToArray()!;
#endif
        }

        /// <summary>
        /// Attempt to get all DatItem types
        /// </summary>
        public static string?[] GetDatItemTypeNames()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(DatItem).IsAssignableFrom(t) && t.IsClass)
                .Select(GetXmlRootAttributeElementName)
                .ToArray();
        }

        /// <summary>
        /// Attempt to get the DatItem type from the name
        /// </summary>
        public static Type? GetDatItemType(string? itemType)
        {
            if (string.IsNullOrEmpty(itemType))
                return null;

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(DatItem).IsAssignableFrom(t) && t.IsClass)
                .FirstOrDefault(t => string.Equals(GetXmlRootAttributeElementName(t), itemType, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Attempt to get the XmlRootAttribute.ElementName value from a type
        /// </summary>
        public static string? GetXmlRootAttributeElementName(Type? type)
        {
            if (type == null)
                return null;

#if NET20 || NET35 || NET40
            // TODO: Figure out how to do this, try using the following:
            // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.customattributedata?view=net-8.0
            return null;
#else
            return type.GetCustomAttribute<XmlRootAttribute>()?.ElementName;
#endif
        }
    }
}
