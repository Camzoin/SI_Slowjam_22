using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class Idea : MonoBehaviour
{
    public DecalProjector dp;
    public float size = 5;
    public Transform backboard;
    public string displayString;
    public TextMeshProUGUI textField;
    public Material textMat;
    public Camera highResCam, medResCam, lowResCam;

    //[HideInInspector]
    public Material curTextMat;

    private RenderTexture highResTexture, medResTexture, lowResTexture;

    // Start is called before the first frame update
    void Start()
    {
        curTextMat = new Material(textMat);

        dp.material = curTextMat;

        backboard.localScale = new Vector3(size / 5, size / 5, size / 5);

        dp.size = new Vector3(size, size, 30);


        highResTexture = new RenderTexture(512, 512, 16, RenderTextureFormat.Default, 8);
        highResTexture.Create();

        highResCam.targetTexture = highResTexture;

        medResTexture = new RenderTexture(128, 128, 16, RenderTextureFormat.Default);
        medResTexture.Create();

        medResCam.targetTexture = medResTexture;

        lowResTexture = new RenderTexture(16, 16, 16, RenderTextureFormat.Default);
        lowResTexture.Create();

        lowResCam.targetTexture = lowResTexture;


        curTextMat.SetTexture("_HighResTexture", highResTexture);
        curTextMat.SetTexture("_MediumResTexture", medResTexture);
        curTextMat.SetTexture("_LowResTexture", lowResTexture);

        textField.text = displayString;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
