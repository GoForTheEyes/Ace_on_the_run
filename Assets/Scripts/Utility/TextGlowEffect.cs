using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextGlowEffect : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] float timeFactor;
#pragma warning restore

    TextMeshProUGUI textMesh;

    // Use this for initialization
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.fontSharedMaterial.SetFloat("_GlowPower", Mathf.PingPong(Time.time / timeFactor, 1.0f));
    }
}

