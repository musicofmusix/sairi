// Provide an interface for the Unity animation system to control sprite switching

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwitch : MonoBehaviour
    {
    [SerializeField]
    [Range(0, 1)]
    public float progress = 0f;
    [SerializeField] Sprite[] sprites;

    private SpriteRenderer sprite_renderer;

    void Start()
        {
        sprite_renderer = GetComponent<SpriteRenderer>();
        }

    void Update()
        {
        for (int i = 0; i < sprites.Length; i++)
            {
            // Divide progress into sections by the number of sprites
            // Switch to a sprite when progress reaches its corresponding section
            if (Mathf.Min((int)(progress * sprites.Length), sprites.Length - 1) == i) { sprite_renderer.sprite = sprites[i]; }
            }
        }
    }
