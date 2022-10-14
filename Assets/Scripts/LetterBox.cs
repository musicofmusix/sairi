// Add letterboxes/pillarboxes at the edges of the screen
// to maintain a specific aspect ratio at all times

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LetterBox : MonoBehaviour
    {
    [SerializeField] public Vector2 target_ratio;
    // Add all cameras needed of resizing here
    [SerializeField] public Camera[] cameras;
    [SerializeField] public Color colour;

    private float aspect;

    // Set all letterbox colours
    void Start()
        {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
        // Only clear with colour before the first/base (not overlay) camera
        if (camera == cameras[0])
            {
            GL.Clear(true, true, colour);
            }
        }

    void OnDestroy()
        {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

    // Resize screen rects starting from here
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

        // Add letterboxes at top and bottom of screen
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

        // Add pillarboxes at left and right of screen
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
