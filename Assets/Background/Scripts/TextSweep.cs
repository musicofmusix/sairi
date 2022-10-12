// Provide an interface for the Unity animation system to control text visuals via a progress variable
// This script allows two methods of animation: 'fill sweeping' and 'odd/even word switching'

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSweep : MonoBehaviour
    {
    [SerializeField]
    [Range(0, 1)]
    public float progress = 0f;
    // Set to true to activate odd/even word switching instead of sweep
    [SerializeField] public bool is_switch_mode = false;
    [SerializeField] public TMP_FontAsset font;
    [SerializeField] public Material on_material;
    [SerializeField] public Material off_material;

    private TextMeshPro tmp;
    private string[] words;

    void Start()
        {
        tmp = GetComponent<TextMeshPro>();
        // Use '$' for dividing text
        words = tmp.text.Split('$');
        }

    void Update()
        {
        string staging = "";
        for (int i = 0; i < words.Length; i++)
            {
            string material_name = off_material.name;

            // Oscillate between all even words being on and all odd words being on
            if (is_switch_mode)
                {
                // When 0 <= progress < 0.5, odd words are turned on
                // When 0.5 <= progress <= 1, even words are turned on
                if (Mathf.Min((int)(progress * 2), 1) != (i % 2)) { material_name = on_material.name; }
                }
            else
                {
                // Divide progress into sections by the number of words
                // Turn on a word when progress reaches its corresponding section
                if (Mathf.Min((int)(progress * words.Length), words.Length - 1) == i) { material_name = on_material.name; }
                }

            staging += "<font=\"" + font.name + "\" material=\"" + material_name + "\">" + words[i] + "</font>";
            }

        tmp.text = staging;
        }
    }
