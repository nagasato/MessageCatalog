using System;

namespace MessageCatalog.SourceGenerator.Models
{
    /// <summary>
    /// Message definition model for TSV parsing
    /// </summary>
    internal class MessageDefinition
    {
        public string Id { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Severity { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
