// Add black bars at the top/bottom or left/right of screen
// to maintain a specific aspect ratio at all times

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBars : MonoBehaviour
    {
    [SerializeField] public Vector2 target_ratio;
    // Add all cameras needed of resizing here
    [SerializeField] public Camera[] cameras;

    private float aspect;

    void Awake()
        {
        aspect = (float)Screen.width / (float)Screen.height;
        AddBlackBars();
        }

    void Update()
        {
        float new_aspect = (float)Screen.width / (float)Screen.height;
        if (new_aspect != aspect)
            {
            aspect = new_aspect;
            AddBlackBars();      
            }
        }

    void AddBlackBars()
        {
        float target_aspect = target_ratio.x / target_ratio.y;
        float scaled_height = aspect / target_aspect;

        // Add black bars at top and bottom of screen
        if (scaled_height < 1)
            {
            foreach (Camera camera in cameras)
                {
                Rect new_rect = camera.rect;

                new_rect.width = 1;
                new_rect.height = scaled_height;

                new_rect.x = 0;
                new_rect.y = (1 - scaled_height) / 2;

                camera.rect = new_rect;
                }
            }

        // Add black bars at left and right of screen
        else if (scaled_height > 1)
            {
            foreach (Camera camera in cameras)
                {
                float scaled_width = 1 / scaled_height;

                Rect new_rect = camera.rect;

                new_rect.width = scaled_width;
                new_rect.height = 1;

                new_rect.x = (1 - scaled_width) / 2;
                new_rect.y = 0;

                camera.rect = new_rect;
                }            
            }
        }
    }
