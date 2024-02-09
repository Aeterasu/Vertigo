using Godot;
using System;

public class LocalizationManager : Node
{
    public const string LOCALE_EN = "en";
    public const string LOCALE_RU = "ru";

    public override void _Ready()
    {
        base._Ready();

        TranslationServer.SetLocale(LOCALE_EN);
    }
}
