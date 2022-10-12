// Allow the particle effect object to follow Sairi's head position during movement

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartsFollowHead : MonoBehaviour
    {
    [SerializeField] public Transform head_transform;
    [SerializeField] public Vector3 offset;
    // Provide animators an iterface to control particle fade through opacity
    [SerializeField]
    [Range(0, 1)]
    public float opacity = 1;

    private ParticleSystem.MainModule particle_system_main;

    void Start()
        {
        particle_system_main = GetComponent<ParticleSystem>().main;
        }

    void Update()
        {
        transform.position = head_transform.position + offset;
        Color start_color = particle_system_main.startColor.color;
        particle_system_main.startColor = new Color(start_color.r, start_color.g, start_color.b, opacity);
        }
    }
