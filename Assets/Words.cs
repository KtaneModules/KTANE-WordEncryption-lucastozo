using System;

public class Words
{
    public string PickWord()
    {
        string[] words = new string[]
        {
            "WORLD",
            "BOMBA",
            "BOMB",
            "MODULE",
            "KEYPAD",
            "BUTTON",
            "STRIKE",
            "SOLVE",
            "TYPING",
            "KEYBOARD",
            "DISPLAY",
            "BATTERY",
            "WIRE",
            "HONEST",
            "LOVELY",
            "KITTEN",
            "GENTLE",
            "KINDNESS",
            "FLOWER",
            "BREEZE",
            "SUNSET",
            "SUNRISE",
            "RAINBOW",
            "ATTACK",
            "DEFENSE",
            "CARDINAL",
            "ATOMIC",
            "DOZENS",
            "CLEVER",
            "FUTURE",
            "HEART",
            "DESERT",
            "ELEPHANT",
            "UNITED",
            "GARDEN",
            "FANTASY",
            "DANGER"
        };
        return words[new Random().Next(words.Length)];
    }
}