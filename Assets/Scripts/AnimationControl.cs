// Allow transitions between poses, transforms, and expressions for Sairi

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
    {
    // All animators that change state depending on pose go here
    [SerializeField] public Animator[] animators;
    // Total number of poses to change between
    [SerializeField] public int poses = 1;
    // Time in seconds to wait before allowing next pose change
    [SerializeField] public float wait_seconds = 1f;

    private int pose = 0;
    private float seconds = 0f;
    void Update()
        {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
            if (seconds <= 0f)
                {
                // Enter next pose
                pose = (pose + 1) % poses;

                foreach (Animator animator in animators)
                    {
                    animator.SetInteger("Pose", pose);
                    }
                seconds = wait_seconds;
                return;
                }
            }

        seconds -= Time.deltaTime;
        }
    }
