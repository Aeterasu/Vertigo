using Godot;
using System;

namespace Vertigo
{
    public enum LocaleEnum
    {
        EN,
        RU,
    }
    
    public class LocalizationManager : Node
    {
        public const string LOCALE_EN = "en";
        public const string LOCALE_RU = "ru";

        public static LocalizationManager Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;
        }

        public void UpdateLanguage()
        {

        }
        
        public static string LanguageEnumToString(LocaleEnum language)
        {
            switch (language)
            {
                case LocaleEnum.EN:
                    return LOCALE_EN;
                case LocaleEnum.RU:
                    return LOCALE_RU;
                default:
                    return LOCALE_EN;
            }
        }

        public static LocaleEnum LanguageStringToEnum(string language)
        {
            switch (language)
            {
                case LOCALE_EN:
                    return LocaleEnum.EN;
                case LOCALE_RU:
                    return LocaleEnum.RU;
                default:
                    return LocaleEnum.EN;
            }
        }
    }
}