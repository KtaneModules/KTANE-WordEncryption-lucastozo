using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using Math = ExMath;

public class WordEncryption : MonoBehaviour {

    private string CorrectEncryptedWord;
    private string Word;
    private int Offset;
    private int Variation;

    public TextMesh GivenWord;
    public TextMesh InputWord;
    public TextMesh InputTip;
    public KMSelectable[] keyboard;
    public GameObject[] gears;

    public KMBombInfo Bomb;
    public KMAudio Audio;

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;

    const short MAX_WORD_LENGTH = 8;
    private bool Tolerance = true; // this will tolerate the player entering wrong inputs while the word is not displayed

    void Awake () {
        ModuleId = ModuleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Activate;
        
        foreach (KMSelectable button in keyboard) {
            button.OnInteract += delegate () { keyboardPress(button); return false; };
        }

        DealColorTexts(true);
    }

    void keyboardPress (KMSelectable button)
    {
        button.AddInteractionPunch();
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, button.transform);
        if (ModuleSolved)
        {
            return;
        }

        for (int i = 0; i < keyboard.Length; i++)
        {
            if (button != keyboard[i])
            {
                continue;
            }
            ChangeInputWord(i);
        }
    }

    private Coroutine blinkCoroutine;
    private string text = "";

    IEnumerator BlinkUnderline()
    {
        while (!ModuleSolved && text.Length < MAX_WORD_LENGTH)
        {
            InputWord.text = text + "_";
            yield return new WaitForSeconds(0.5f);
            InputWord.text = text;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void ChangeInputWord(int code)
    {
        if (ModuleSolved)
        {
            return;
        }
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        if (code == 26)
        {
            if (text.Length != 0)
            {
                text = text.Substring(0, text.Length - 1);
            }
        }
        else if (code == 27)
        {
            GuessWord(); // submit
        }
        else if (text.Length < MAX_WORD_LENGTH)
        {
            text += (char)('A' + code);
            StartCoroutine(RotateGears());
        }
        InputWord.text = text;
        blinkCoroutine = StartCoroutine(BlinkUnderline());
    }

    IEnumerator RotateGears(bool strike = false)
    {
        var gearsToRotate = new List<int>();
        float duration = 0.1f;

        if (strike)
        {
            gearsToRotate.AddRange(new List<int> { 0, 1, 2 });
            duration = 1f;
        }
        else
        {
            gearsToRotate.Add(Rnd.Range(0, 3));
            if (Rnd.value < 0.5f) // make it more likely to rotate two gears
            {
                gearsToRotate.Add(Rnd.Range(0, 3));
            }
        }

        foreach (var gearToRotate in gearsToRotate)
        {
            int rotation;
            if (strike)
            {
                rotation = Rnd.value < 0.5f ? 210 : -150;
            }
            else
            {
                rotation = Rnd.Range(-8, -18);
            }
            StartCoroutine(RotateGear(gearToRotate, rotation, duration));
        }
        yield return new WaitForSeconds(duration);
    }

    IEnumerator RotateGear(int gearToRotate, int rotation, float duration = 0.1f)
    {
        var rotationValue = gears[gearToRotate].transform.localEulerAngles;
        var targetRotation = rotationValue.x + rotation;

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float newX = Mathf.Lerp(rotationValue.x, targetRotation, (elapsedTime / duration));
            gears[gearToRotate].transform.localEulerAngles = new Vector3(newX, 0, 90);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gears[gearToRotate].transform.localEulerAngles = new Vector3(targetRotation, 0, 90);
    }

    void GuessWord()
    {
        if (Tolerance)
        {
            return;
        }
        var text = InputWord.text;
        text = text.Replace("_", ""); // remove every _ 
        if (text == CorrectEncryptedWord)
        {
            ModuleSolved = true;
            Solve();
        }
        else
        {
            Strike();
        }
    }

    void Activate () {
        PickNewWord();
        DealColorTexts();
    }

    void Start () {
        InputWord.text = "";
        StartCoroutine(BlinkUnderline());

        Offset selectOffset = new Offset();
        bool[] config = new bool[4];
        config[0] = Bomb.GetPortCount(Port.Serial) > 0 || Bomb.GetPortCount(Port.StereoRCA) > 0 || Bomb.GetPortCount(Port.DVI) > 0 || Bomb.GetPortCount(Port.PS2) > 0;
        config[1] = Bomb.GetBatteryCount(1) > 0;
        config[2] = Bomb.GetBatteryCount(2) > 0 || Bomb.GetBatteryCount(3) > 0 || Bomb.GetBatteryCount(4) > 0;
        config[3] = Regex.IsMatch(Bomb.GetSerialNumber(), "[AEIOU]");
        Offset = selectOffset.SelectOffset(config);

        Variation selectVariation = new Variation();
        string[] indicators = Bomb.GetIndicators().ToArray();
        Variation = selectVariation.SelectVariation(indicators);
    }

    void Solve () {
        ModuleSolved = true;
        GetComponent<KMBombModule>().HandlePass();
    }

    void Strike () {
        GetComponent<KMBombModule>().HandleStrike();
        PickNewWord();
    }

    void PickNewWord()
    {
        Tolerance = true;
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
        Word = words[Rnd.Range(0, words.Length)];
        Encrypt encrypt = new Encrypt();
        CorrectEncryptedWord = encrypt.EncryptString(Word, Offset, Variation);
        StartCoroutine(DisplayNewWord());
        

        // Debugging
        Debug.Log("Original word: " + Word);
        Debug.Log("Offset: " + Offset);
        Debug.Log("Variation: " + Variation);
        Debug.Log("Encrypted word: " + CorrectEncryptedWord);
    }

    IEnumerator DisplayNewWord()
    {
        StartCoroutine(RotateGears(true));
        text = "";
        InputWord.text = text;
        GivenWord.text = "";
        yield return new WaitForSeconds(1f);
        GivenWord.text = Word;
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(BlinkUnderline());
        Tolerance = false;
    }

    void DealColorTexts(bool hide = false)
    {
        var white = new Color(1, 1, 1);
        var black = new Color(0, 0, 0);
        GivenWord.color = hide ? black : white;
        InputWord.color = hide ? black : white;
        InputTip.color = hide ? black : white;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand (string Command) {
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve () {
        yield return null;
    }
}