using WebSettingsManager.Interfaces;

namespace WebSettingsManager.Models
{
    public class TextConfigurationData : ITextConfigurationData, IEquatable<TextConfigurationData>, IEquatable<ITextConfigurationData>
    {
        public TextConfigurationData(string fontName = "Consolas", int fontSize = 12)
        {
            this.FontName = fontName;
            this.FontSize = fontSize;
        }
        public TextConfigurationData(ITextConfigurationData textConfigurationData)
        { 
            this.FontName = textConfigurationData.FontName;
            this.FontSize = textConfigurationData.FontSize;
        }
        public string FontName { get; }

        public int FontSize { get; }

        public bool Equals(TextConfigurationData? other)
        {
            return this.Equals(other as ITextConfigurationData);
        }
        public bool Equals(ITextConfigurationData? other)
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
