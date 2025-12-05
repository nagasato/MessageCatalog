using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MessageCatalog.SourceGenerator.Models;

namespace MessageCatalog.SourceGenerator
{
    [Generator]
    public class MessageCatalogGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Find all additional files ending with messages.tsv
            var tsvFiles = context.AdditionalTextsProvider
                                  .Where(file => file.Path.EndsWith("messages.tsv"));

            // Read and parse TSV files with their names
            var messagesProvider = tsvFiles.Select((file, cancellationToken) =>
            {
                var content = file.GetText(cancellationToken)?.ToString();
                if (string.IsNullOrEmpty(content)) 
                { 
                    return(null, null); 
                }

                try
                {
                    var messages = ParseTsvContent(content);
                    var fileName = Path.GetFileName(file.Path);
                    return (messages, fileName);
                }
                catch
                {
                    return (null, null);
                }
            }).Where(item => item.Item1 != null && item.Item2 != null);

            // Generate source code for each TSV file
            context.RegisterSourceOutput(messagesProvider, (spc, item) =>
            {
                var (messages, fileName) = item;
                if (messages == null || messages.Length == 0 || fileName == null)
                {
                    return;
                }

                // Extract class name prefix from filename
                var classPrefix = ExtractClassPrefix(fileName);

                var categories = messages
                    .Select(m => m.Category)
                    .Where(c => !string.IsNullOrEmpty(c) && c != "None")
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                var severities = messages
                    .Select(m => m.Severity)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                // Generate MessageItem class
                var messageItemSource = GenerateMessageItem(categories, severities, classPrefix);
                spc.AddSource($"{classPrefix}MessageItem.g.cs", SourceText.From(messageItemSource, Encoding.UTF8));

                // Generate MessageCatalog class
                var messageCatalogSource = GenerateMessageCatalog(messages, classPrefix, fileName);
                spc.AddSource($"{classPrefix}MessageCatalog.g.cs", SourceText.From(messageCatalogSource, Encoding.UTF8));
            });
        }

        private MessageDefinition[] ParseTsvContent(string content)
        {
            var lines = content.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) // Need at least header + 1 data row
            { 
                return null; 
            }

            var messages = new List<MessageDefinition>();
            
            // Parse header to find column indices
            var header = lines[0].Split('\t');
            var idIndex = System.Array.IndexOf(header, "Id");
            var textIndex = System.Array.IndexOf(header, "Text");
            var categoryIndex = System.Array.IndexOf(header, "Category");
            var severityIndex = System.Array.IndexOf(header, "Severity");
            var descriptionIndex = System.Array.IndexOf(header, "Description");

            if (idIndex < 0 || textIndex < 0)
            {
                return null; // Id and Text are required
            }

            // Parse data rows
            for (int i = 1; i < lines.Length; i++)
            {
                var fields = lines[i].Split('\t');
                if (fields.Length == 0)
                {
                    continue;
                }

                var message = new MessageDefinition
                {
                    Id = idIndex >= 0 && idIndex < fields.Length ? fields[idIndex] : string.Empty,
                    Text = textIndex >= 0 && textIndex < fields.Length ? fields[textIndex] : string.Empty,
                    Category = categoryIndex >= 0 && categoryIndex < fields.Length ? fields[categoryIndex] : string.Empty,
                    Severity = severityIndex >= 0 && severityIndex < fields.Length ? fields[severityIndex] : string.Empty,
                    Description = descriptionIndex >= 0 && descriptionIndex < fields.Length ? fields[descriptionIndex] : string.Empty
                };

                if (!string.IsNullOrEmpty(message.Id))
                {
                    messages.Add(message);
                }
            }

            return messages.ToArray();
        }

        private string ExtractClassPrefix(string fileName)
        {
            // Remove .tsv extension
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            
            // Check if it matches the pattern "Prefix_messages"
            var underscoreIndex = nameWithoutExtension.IndexOf('_');
            if (underscoreIndex > 0)
            {
                var prefix = nameWithoutExtension.Substring(0, underscoreIndex);
                var suffix = nameWithoutExtension.Substring(underscoreIndex + 1);
                
                // If suffix is "messages", use the prefix
                if (suffix == "messages")
                {
                    return prefix;
                }
            }
            
            // For "messages.tsv" or other patterns, return empty string
            return string.Empty;
        }

        private string GenerateMessageItem(List<string> categories, List<string> severities, string classPrefix)
        {
            var categoryMembers = string.Join(",\n        ", categories);
            var severityMembers = string.Join(",\n        ", severities);
            
            var itemClassName = string.IsNullOrEmpty(classPrefix) ? "DefaultMessageItem" : $"{classPrefix}MessageItem";
            var categoryEnumName = string.IsNullOrEmpty(classPrefix) ? "DefaultMessageCategory" : $"{classPrefix}MessageCategory";
            var severityEnumName = string.IsNullOrEmpty(classPrefix) ? "DefaultMessageSeverity" : $"{classPrefix}MessageSeverity";

            return $$"""
                // <auto-generated />
                namespace MessageCatalog.Core
                {
                    /// <summary>
                    /// Message Item
                    /// </summary>
                    public class {{itemClassName}}
                    {
                        private readonly string _id;
                        private readonly System.Func<string, string> _textResolver;

                        public {{itemClassName}}(string id, string text, {{categoryEnumName}} category, {{severityEnumName}} severity, string description, System.Func<string, string> textResolver)
                        {
                            _id = id;
                            _textResolver = textResolver;
                            Text = text;
                            Category = category;
                            Severity = severity;
                            Description = description;
                        }

                        /// <summary>
                        /// Message text
                        /// </summary>
                        public string Text 
                        { 
                            get 
                            {
                                if (_textResolver != null)
                                {
                                    var resolvedText = _textResolver(_id);
                                    if (!string.IsNullOrEmpty(resolvedText))
                                        return resolvedText;
                                }
                                return _text;
                            }
                            private set { _text = value; }
                        }
                        private string _text = string.Empty;

                        /// <summary>
                        /// Message category
                        /// </summary>
                        public {{categoryEnumName}} Category { get; } = {{categoryEnumName}}.None;

                        /// <summary>
                        /// Message severity
                        /// </summary>
                        public {{severityEnumName}} Severity { get; } = {{severityEnumName}}.None;

                        /// <summary>
                        /// Message description
                        /// </summary>
                        /// <remarks>
                        /// Optional description for developers about when this message is used.
                        /// </remarks>
                        public string Description { get; } = string.Empty;

                        public string Format(params object[] args)
                        {
                            return string.Format(Text, args);
                        }

                        public override string ToString()
                        {
                            return Text;
                        }
                    }

                    /// <summary>
                    /// Message category
                    /// </summary>
                    public enum {{categoryEnumName}}
                    {
                        None,
                        {{categoryMembers}}
                    }

                    /// <summary>
                    /// Message severity
                    /// </summary>
                    public enum {{severityEnumName}}
                    {
                        None,
                        {{severityMembers}}
                    }
                }
                """;
        }

        private string GenerateMessageCatalog(MessageDefinition[] messages, string classPrefix, string tsvFileName)
        {
            var catalogClassName = string.IsNullOrEmpty(classPrefix) ? "DefaultMessageCatalog" : $"{classPrefix}MessageCatalog";
            var itemClassName = string.IsNullOrEmpty(classPrefix) ? "DefaultMessageItem" : $"{classPrefix}MessageItem";
            var categoryEnumName = string.IsNullOrEmpty(classPrefix) ? "DefaultMessageCategory" : $"{classPrefix}MessageCategory";
            var severityEnumName = string.IsNullOrEmpty(classPrefix) ? "DefaultMessageSeverity" : $"{classPrefix}MessageSeverity";

            var propertiesWithDocs = string.Join("\n\n        ", messages.Select(m =>
            {
                var category = string.IsNullOrEmpty(m.Category) ? "None" : m.Category;
                var severity = string.IsNullOrEmpty(m.Severity) ? "None" : m.Severity;
                var descriptionLine = string.IsNullOrEmpty(m.Description) 
                    ? "" 
                    : $"<br/>  Description: {EscapeXmlComment(m.Description)}";
                
                return $$"""
                    /// <summary>
                    /// {{EscapeXmlComment(m.Text)}}<br/>
                    ///   Severity: {{severity}}<br/>
                    ///   Category: {{category}}{{descriptionLine}}
                    /// </summary>
                    public {{itemClassName}} {{m.Id}} { get; }
                    """;
            }));
            
            var initializations = string.Join("\n            ", messages.Select(m =>
            {
                var category = string.IsNullOrEmpty(m.Category) ? "None" : m.Category;
                var severity = string.IsNullOrEmpty(m.Severity) ? "None" : m.Severity;
                var description = string.IsNullOrEmpty(m.Description) ? "" : EscapeString(m.Description);
                return $"{m.Id} = new {itemClassName}(\"{m.Id}\", \"{EscapeString(m.Text)}\", {categoryEnumName}.{category}, {severityEnumName}.{severity}, \"{description}\", _textResolver);";
            }));

            return $$"""
                // <auto-generated />
                using System.Collections.Generic;
                using System.IO;
                using System.Text;

                namespace MessageCatalog.Core
                {
                    /// <summary>
                    /// Message catalog
                    /// </summary>
                    public class {{catalogClassName}}
                    {
                        private readonly System.Func<string, string> _textResolver;

                        {{propertiesWithDocs}}

                        public {{catalogClassName}}(string tsvFilePath = null)
                        {
                            var runtimeTexts = LoadRuntimeTexts(tsvFilePath ?? "{{tsvFileName}}");
                            _textResolver = (id) => runtimeTexts != null && runtimeTexts.ContainsKey(id) ? runtimeTexts[id] : null;

                            {{initializations}}
                        }

                        private Dictionary<string, string> LoadRuntimeTexts(string tsvFilePath)
                        {
                            try
                            {
                                if (!File.Exists(tsvFilePath))
                                {
                                    var appPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, tsvFilePath);
                                    if (!File.Exists(appPath))
                                    {
                                        return null;
                                    }
                                    tsvFilePath = appPath;
                                }

                                var lines = File.ReadAllLines(tsvFilePath, Encoding.UTF8);
                                if (lines.Length < 2)
                                {
                                    return null;
                                }

                                var dict = new Dictionary<string, string>();
                                
                                // Parse header
                                var header = lines[0].Split('\t');
                                var idIndex = System.Array.IndexOf(header, "Id");
                                var textIndex = System.Array.IndexOf(header, "Text");
                                
                                if (idIndex < 0 || textIndex < 0)
                                {
                                    return null;
                                }

                                // Parse data rows
                                for (int i = 1; i < lines.Length; i++)
                                {
                                    var fields = lines[i].Split('\t');
                                    if (fields.Length > idIndex && fields.Length > textIndex)
                                    {
                                        var id = fields[idIndex];
                                        var text = fields[textIndex];
                                        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(text))
                                        {
                                            dict[id] = text;
                                        }
                                    }
                                }
                                
                                return dict;
                            }
                            catch
                            {
                                return null;
                            }
                        }
                    }
                }
                """;
        }

        private string EscapeString(string input)
        {
            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        private string EscapeXmlComment(string input)
        {
            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}
