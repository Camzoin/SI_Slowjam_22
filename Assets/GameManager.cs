using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform ideaTransform;

    public Transform camCenterTransform;

    public float rotationOffsetSensitivity;

    public Material ideaMat;

    public List<GameObject> savedLevels;

    public Idea idea;

    private Material curIdeaMat;

    // Start is called before the first frame update
    void Start()
    {
        curIdeaMat = new Material(ideaMat);
    }

    // Update is called once per frame
    void Update()
    {

        //for loop this for all ideas in level eventually
        float angle = (Mathf.Abs(ideaTransform.rotation.x - camCenterTransform.rotation.x) + Mathf.Abs(ideaTransform.rotation.y - camCenterTransform.rotation.y)) * 100;

        //if angle is < 1 you have ""Found"" that idea

        idea.curTextMat.SetFloat("_TextLegibility", Mathf.Lerp(1, 0, (angle / 100) * rotationOffsetSensitivity));
    }

    public void SpawnIdea(float size)
    {

    }

    public void GoToNextLevel()
    {

    }
}
