using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PanelEdit : BasePanel
{
    // Components
    [SerializeField] private TextMeshProUGUI t_date = null;
    [SerializeField] private TextMeshProUGUI t_prompt = null;
    private string[] availablePrompts =
    {
        "Bullet-point today's ideal workday. Best believable version. Then, close eyes your and visualize it.",
        "Bullet-point today's ideal workday. Best believable version. Then, close eyes your and visualize it.",
        "Bullet-point today's ideal workday. Best believable version. Then, close eyes your and visualize it.",
        "Bullet-point today's ideal workday. Best believable version. Then, close eyes your and visualize it.",
        "Bullet-point today's ideal workday. Best believable version. Then, close eyes your and visualize it.",
        "Bullet-point today's ideal workday. Best believable version. Then, close eyes your and visualize it.",
        "3+ things I'm looking forward to!",
        "3+ things I'm looking forward to!",
        "3+ things I'm looking forward to!",
        "3+ things I'm looking forward to!",
        "3+ things I'm looking forward to!",
        "3+ things I'm looking forward to!",
        "3+ things I'm looking forward to!",
        "3+ things I'm looking forward to!",


        "Add 2+ nice/motivating messages to my future self in Bright Notes!",
        "3+ good things that recently happened",
        "3+ things that bring me joy",
        "2+ things I admire about other people",
        "3+ things I like about myself",
        "What am I afraid of today?",
        "1+ fear that's been holding me back recently",
        "How have I been challenged in the last week? And how have I grown?",
        "1 way I will treat myself today, and why! (Set self reminder, too!)",
        "1 nice thing I will do for someone else today (plus a follow-up lingering notification to confirm I’ve done it)",
        "3+ nice things I can do for others today, but don’t have to do",
        "1+ thing I want to work on on myself, I want to get better at",
        "1+ thought or thing I want to do less of today",
        "2+ things I want to attract into my life this week (plus follow-up notifications, reminding about it)",
        "Write a love letter to yourself. “Dear Brett… Love, Brett.”",
        "Come up with a terrible pun for the word: {RANDOM_WORD}.",
        "Write a playful/silly/inspiring/loving poem using the prompt: {RANDOM_WORD}.",
        "What’s on your mind today? Explain how it relates to this random word: {RANDOM_WORD}.",
        "3 + reasons you’re proud of yourself.",
        "Who’s someone you love? Why?",
        "2 + things I’ve recently learned. (You’re getting smarter every day!)",
        "2 ways you can make the world a better place today",
        "What’s something I’ve been putting off that I will do today? (Follow-up lingering notification)",
    };



    // ================================================================
    //  Start
    // ================================================================
    void Start()
    {
        // Update texts, yo.
        t_date.text = TextUtils.MediumDateString(DateTime.Today);
        t_prompt.text = GetRandomPrompt();

        // By default, copy the prompt to our clipboard!
        Debug_CopyPromptToClipboard();
    }

    private string GetRandomPrompt()
    {
        string prompt = availablePrompts[Mathf.FloorToInt(UnityEngine.Random.Range(0, availablePrompts.Length))];
        //prompt = prompt.Replace("{RANDOM_WORD}", GetRandomWord().ToUpper());
        return prompt;
    }

    public void Debug_GetNewPrompt()
    {
        t_prompt.text = GetRandomPrompt();
        Debug_CopyPromptToClipboard();
    }
    public void Debug_CopyPromptToClipboard()
    {
        string text = TextUtils.MediumDateString(DateTime.Today);
        text += "\n" + t_prompt.text;
        text += "\n";
        GameUtils.CopyToClipboard(text);
    }
}







