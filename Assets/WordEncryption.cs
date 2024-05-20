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

    private string CorrectEncryptedWord = "";
    private short IndexWordEncryption = 0;
    private string Word;
    private int Offset, OgOffset; // use to reset the offset
    private int Variation;

    public TextMesh GivenWord, InputWord, InputTip;
    public KMSelectable[] keyboard;
    public GameObject[] gears;

    public KMBombInfo Bomb;
    public KMAudio Audio;

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;

    const short MAX_WORD_LENGTH = 6;
    private bool FirstTime = true; // this will be used to not play the "reset" sound at the start
    private bool Tolerance = true; // this will tolerate the player entering wrong inputs while the word is not displayed
    string[] SFX = { "lock1", "lock2", "lock3", "lock4", "lock5", "reset" };

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
        text = InputWord.text.Replace("_", "");
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
                text = text.Remove(text.Length - 1);
                
                if (InputWord.text.Length <= Word.Length)
                {
                    CorrectEncryptedWord = CorrectEncryptedWord.Remove(CorrectEncryptedWord.Length - 1);
                    IndexWordEncryption--;
                }
            }
            else
            {
                Offset = OgOffset;
                CleanEncryptedAndIndex();
                PlaySFX("reset");
                StartCoroutine(RotateGears(true));
            }
        }
        else if (code == 27)
        {
            GuessWord(); // submit
        }
        else if (text.Length < MAX_WORD_LENGTH)
        {
            text += (char)('A' + code);
            if (IndexWordEncryption < Word.Length)
            {
                if (IndexWordEncryption < 0)
                {
                    IndexWordEncryption = 0;
                }
                ConstructEncryptedWord(Word[IndexWordEncryption]);
            }
            Offset = Offset + Variation;
            StartCoroutine(RotateGears());
            PlaySFX("lock");
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
        if (text.Length < Word.Length)
        {
            Strike();
            return;
        }
        if (text != CorrectEncryptedWord || text.Length != Word.Length)
        {
            Strike();
            Debug.Log("Text: " + text);
            Debug.Log("Correct: " + CorrectEncryptedWord);
            return;
        }
        ModuleSolved = true;
        Solve();
    }

    void Activate () {
        PickNewWord();
        DealColorTexts();
        FirstTime = false;
        Tolerance = false;
    }

    void Start () {
        InputWord.text = "";
        StartCoroutine(BlinkUnderline());

        GetOffset();
        GetVariation();
        Words words = new Words();
        Word = words.PickWord();
        GivenWord.text = Word;
        Debug.Log("Offset: " + Offset);
        Debug.Log("Variation: " + Variation);
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
        if(FirstTime)
        {
            return;
        }
        Offset = OgOffset;
        Tolerance = true;
        Words words = new Words();
        Word = words.PickWord();
        StartCoroutine(DisplayNewWord());

        CleanEncryptedAndIndex();
    }

    IEnumerator DisplayNewWord()
    {
        if(!FirstTime)
        {
            PlaySFX("reset");
            StartCoroutine(RotateGears(true));
        }
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

    void GetOffset()
    {
        Offset selectOffset = new Offset();
        bool[] config = new bool[4];
        config[0] = Bomb.GetPortCount(Port.StereoRCA) > 0 || Bomb.GetPortCount(Port.PS2) > 0;
        config[1] = Bomb.GetBatteryCount(1) > 0;
        config[2] = Bomb.GetBatteryCount(2) > 0 || Bomb.GetBatteryCount(3) > 0 || Bomb.GetBatteryCount(4) > 0;
        config[3] = Regex.IsMatch(Bomb.GetSerialNumber(), "[AEIOU]");
        Offset = selectOffset.SelectOffset(config);
        OgOffset = Offset;
    }

    void GetVariation()
    {
        Variation selectVariation = new Variation();
        string[] indicators = Bomb.GetIndicators().ToArray();
        Variation = selectVariation.SelectVariation(indicators);
    }

    void ConstructEncryptedWord(char c)
    {
        if (text.Length > Word.Length)
        {
            return;
        }
        if (CorrectEncryptedWord.Length > Word.Length-1) // silly fix to avoid a bug. TODO: fix this properly
        {
            CorrectEncryptedWord = CorrectEncryptedWord.Remove(CorrectEncryptedWord.Length - 1);
        }
        Debug.Log("Index: " + IndexWordEncryption);
        Encrypt encrypt = new Encrypt();
        CorrectEncryptedWord += encrypt.EncryptString(c, Offset);
        Debug.Log("Encrypted word: " + CorrectEncryptedWord);
        if (IndexWordEncryption < Word.Length-1)
        {
            IndexWordEncryption++;
        }
    }

    void PlaySFX(string sfx)
    {
        switch (sfx)
        {
            case "lock":
                Audio.PlaySoundAtTransform(SFX[Rnd.Range(0, SFX.Length-1)], transform);
                break;
            case "reset":
                Audio.PlaySoundAtTransform(SFX[5], transform);
                break;
        }
    }

    void CleanEncryptedAndIndex()
    {
        CorrectEncryptedWord = "";
        IndexWordEncryption = 0;
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