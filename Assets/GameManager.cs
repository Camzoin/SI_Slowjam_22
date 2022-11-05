using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform ideaTransform;

    public Transform camCenterTransform;

    public float rotationOffsetSensitivity;

    public Material ideaMat;

    private Material curIdeaMat;

    // Start is called before the first frame update
    void Start()
    {
        curIdeaMat = ideaMat;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = (Mathf.Abs(ideaTransform.rotation.x - camCenterTransform.rotation.x) + Mathf.Abs(ideaTransform.rotation.y - camCenterTransform.rotation.y)) * 100;

        //if angle is < 1 you have ""Found"" that idea

        curIdeaMat.SetFloat("_TextLegibility", Mathf.Lerp(1, 0, (angle / 100) * rotationOffsetSensitivity));
    }

    public void SpawnIdea(float size)
    {

    }
}
