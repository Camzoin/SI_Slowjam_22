using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public Transform camCenterTransform;

    public float rotationOffsetSensitivity;

    public Animator ppAnimator;

    public Material ideaMat;

    public List<GameObject> savedLevels;

    public Level1 curLevel;

    public Idea curIdeaToDestroy;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach(Idea i in curLevel.curIdeas)
        {
            //for loop this for all ideas in level eventually
            float angle = Quaternion.Angle(i.transform.rotation, camCenterTransform.transform.rotation);

            //angle = angle - (Mathf.Abs(i.transform.rotation.eulerAngles.z) - (Mathf.Abs(camCenterTransform.transform.rotation.eulerAngles.z) % 360));

            //if angle is < 1 you have ""Found"" that idea

            if(angle < 5 && i.hasBeenCollected == false)
            {
                //NEED TO LERP CAMERA TO ALIGN TO TEXT DIRECTION
                DestroyIdea(i);
            }


            i.curTextMat.SetFloat("_TextLegibility", Mathf.Lerp(1, 0, angle / 180 * rotationOffsetSensitivity));

            i.curTextMat.SetFloat("_AlphaMulti", Mathf.Lerp(1, 0, angle / 180 * (rotationOffsetSensitivity * 1.5f)));
        }
    }

    public void SpawnIdea(float size)
    {

    }


    //THIS IS GOING TO BE WAY COOLER
    public void DestroyIdea(Idea idea)
    {
        curIdeaToDestroy = idea;

        ppAnimator.SetTrigger("BaW");

        idea.hasBeenCollected = true;

        camCenterTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        camCenterTransform.rotation = idea.transform.rotation;
    }

    public void GoToNextLevel()
    {

    }
}
