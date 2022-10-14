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

    private int pose = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            {
            // Enter next pose
            pose = (pose + 1) % poses;

            foreach (Animator animator in animators)
                {
                animator.SetInteger("Pose", pose);
                }
            }
        }
    }
