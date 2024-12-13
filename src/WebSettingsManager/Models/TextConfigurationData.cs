using System.Text.Json.Serialization;
using WebSettingsManager.Interfaces;

namespace WebSettingsManager.Models
{
    public class TextConfigurationOptions : ITextConfigurationOptions, IEquatable<TextConfigurationOptions>, IEquatable<ITextConfigurationOptions>
    {
        [JsonConstructor]
        public TextConfigurationOptions(string fontName = "Consolas", int fontSize = 12)
        {
            this.FontName = fontName;
            this.FontSize = fontSize;
        }
        public TextConfigurationOptions(ITextConfigurationOptions textConfigurationData)
        { 
            this.FontName = textConfigurationData.FontName;
            this.FontSize = textConfigurationData.FontSize;
        }
        public string FontName { get; }

        public int FontSize { get; }

        public bool Equals(TextConfigurationOptions? other)
        {
            return this.Equals(other as ITextConfigurationOptions);
        }
        public bool Equals(ITextConfigurationOptions? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (this.FontName != other.FontName)
                return false;
            if (this.FontSize != other.FontSize)
                return false;
            return true;
        }
    }
}
