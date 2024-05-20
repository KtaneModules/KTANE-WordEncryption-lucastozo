using System;

public class Words
{
    public string PickWord()
    {
        string[] words = new string[]
        {
            "MODULE",
            "KEYPAD",
            "BUTTON",
            "STRIKE",
            "TYPING",
            "HONEST",
            "LOVELY",
            "KITTEN",
            "GENTLE",
            "FLOWER",
            "BREEZE",
            "SUNSET",
            "ATTACK",
            "ATOMIC",
            "DOZENS",
            "CLEVER",
            "FUTURE",
            "DESERT",
            "UNITED",
            "GARDEN",
            "DANGER",
            "SILENT",
            "SISTER",
            "PARENT",
            "LAPTOP",
            "WINTER",

            "OCEAN",
            "LEMON",
            "HEART",
            "SOLVE",
            "BOMBA",
            "WORLD",

            "BOMB",
            "NAVY",
            "WIRE"
        };
        return words[new Random().Next(words.Length)];
    }
}