// Swap out all children's TextMeshPro text to gibberish whenever progress is not 0 or 1

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class TextTransition : MonoBehaviour
    {
    [SerializeField]
    [Range(0, 1)]
    public float progress = 0f;
    [SerializeField] public string text1;
    [SerializeField] public string text2;
    [SerializeField] public string gibberish_text;
    // Seconds to wait before generating new gibberish
    // Independent from animation playback speed
    [SerializeField] public float wait_seconds = 0.5f;
    [SerializeField] public int minimum_chars = 10;
    [SerializeField] public int maximum_chars = 15;

    private Component[] tmps;
    private float seconds = 0f;
    private char[] gibberish;

    void Start()
        {
        // Include inactive children generated by VerticalArray
        tmps = transform.GetComponentsInChildren(typeof(TextMeshPro), true);
        gibberish = gibberish_text.ToCharArray();
        }

    void Update()
        {
        // Use regex for text replacement
        var reg = new Regex("(?<=>)(.*)(?=</font>)");

        if (Mathf.Approximately(progress, 0f))
            {
            foreach (TextMeshPro tmp in tmps)
                {
                tmp.text = reg.Replace(tmp.text, text1);
                }
            }
        else if (Mathf.Approximately(progress, 1f))
            {
            foreach (TextMeshPro tmp in tmps)
                {
                tmp.text = reg.Replace(tmp.text, text2);
                }
            }
        else
            {
            if (seconds <= 0f)
                {
                string cache = GenerateGibberish(Random.Range(minimum_chars, maximum_chars));

                foreach (TextMeshPro tmp in tmps)
                    {
                    tmp.text = reg.Replace(tmp.text, cache);
                    }
                seconds = wait_seconds;
                }
            seconds -= Time.deltaTime;
            }
        }

    string GenerateGibberish(int length)
        {
        string staging = "";

        for (int i = 0; i < length; i++)
            {
            staging += gibberish[Random.Range(0, gibberish.Length - 1)];
            }

        return staging;
        }
    }
