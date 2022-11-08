using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform camCenterTransform;

    public float rotationOffsetSensitivity;

    public Animator ppAnimator;

    public Material ideaMat;

    public List<GameObject> savedLevels;

    public Level1 curLevel;

    public Idea curIdeaToDestroy;

    public Level1 levelToSpawn;

    public float turningRate;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(curLevel != null)
        {
            if (curLevel.isSpawning == false)
            {
                foreach (Idea i in curLevel.curIdeas)
                {
                    //for loop this for all ideas in level eventually
                    float angle = Quaternion.Angle(i.transform.rotation, camCenterTransform.transform.rotation);

                    //angle = angle - (Mathf.Abs(i.transform.rotation.eulerAngles.z) - (Mathf.Abs(camCenterTransform.transform.rotation.eulerAngles.z) % 360));

                    //if angle is < 1 you have ""Found"" that idea

                    if (angle < 7f && i.hasBeenCollected == false)
                    {
                        camCenterTransform.rotation = Quaternion.RotateTowards(camCenterTransform.rotation, i.transform.rotation, turningRate * Time.deltaTime);
                    }

                    if (angle < 0.05f && i.hasBeenCollected == false)
                    {
                        curLevel.IdeaFound(i);
                    }


                    i.curTextMat.SetFloat("_TextLegibility", Mathf.Lerp(1, 0, angle / 180 * rotationOffsetSensitivity));

                    if (curIdeaToDestroy == null)
                    {
                        i.curTextMat.SetFloat("_AlphaMulti", Mathf.Lerp(1, 0, angle / 180 * (rotationOffsetSensitivity * 1.5f)));
                    }
                }
            }
        }
    }

    public void GoToNextLevel()
    {
        if (curLevel != null)
        {
            Destroy(curLevel.gameObject);
        }

        var p = Instantiate(levelToSpawn);

        curLevel = p;

        p.gm = this;
    }
}
