using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VerticalArray : MonoBehaviour
    {
    [SerializeField] public TextMeshPro original;
    [SerializeField]
    [Range(0, 1)]
    public float progress = 0f;
    [SerializeField] public TMP_FontAsset font;
    [SerializeField] public Material clone_material;
    [SerializeField] public string text;
    [SerializeField] public float y_offset = 1f;
    [SerializeField] public int single_side_clones = 3;

    private TextMeshPro[] top_clones;
    private TextMeshPro[] bottom_clones;

    void Awake()
        {
        top_clones = new TextMeshPro[single_side_clones];
        bottom_clones = new TextMeshPro[single_side_clones];

        for (int i = 0; i < single_side_clones; i++)
            {
            TextMeshPro top_tmp = Instantiate(original, original.transform.parent);
            TextMeshPro bottom_tmp = Instantiate(original, original.transform.parent);

            top_tmp.text = "<font=\"" + font.name + "\" material=\"" + clone_material.name + "\">" + text + "</font>";
            bottom_tmp.text = top_tmp.text;

            top_tmp.gameObject.SetActive(false);
            bottom_tmp.gameObject.SetActive(false);

            top_clones[i] = top_tmp;
            bottom_clones[i] = bottom_tmp;
            }
        }

    void Update()
        {
        // Progress is divided into (the number of clone pairs + 1) sections
        // Each section displays 0, 1, 2... pairs of clones
        int active_index = (int)Mathf.Min(Mathf.Floor(progress * (single_side_clones + 1)), single_side_clones);

        if (single_side_clones > 0)
            {
            for (int i = 0; i < active_index; i++)
                {
                top_clones[i].gameObject.SetActive(true);
                bottom_clones[i].gameObject.SetActive(true);

                top_clones[i].transform.localPosition =
                    original.transform.localPosition + new Vector3(0, y_offset * (i + 1), 0);
                bottom_clones[i].transform.localPosition =
                    original.transform.localPosition + new Vector3(0, -y_offset * (i + 1), 0);
                }

            for (int i = active_index; i < single_side_clones; i++)
                {
                top_clones[i].gameObject.SetActive(false);
                bottom_clones[i].gameObject.SetActive(false);
                }
            }
        }
    }
