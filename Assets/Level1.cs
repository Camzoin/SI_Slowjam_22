using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level1 : MonoBehaviour
{
    public int ideaCount = 10;

    public GameObject ideaPrefab;

    public List<Idea> curIdeas;

    public List<TMP_FontAsset> fonts;

    public string adj, noun;

    private string[] splitAdj, splitNoun;

    public Material backgroundMat, objectMat;

    public Material curBackgroundMat, curObjectMat;

    public MeshRenderer backgroundMeshRenderer;

    public List<ParticleSystemRenderer> psRenderers;

    public GameManager gm;

    public List<float> crackValues;

    public int curFoundIdeas = 0;

    public bool isSpawning = true;

    public List<ParticleSystem> particleSystems;

    public AnimationCurve shapeScale;

    // Start is called before the first frame update
    void Start()
    {
        curBackgroundMat = new Material(backgroundMat);

        curObjectMat = new Material(objectMat);

        backgroundMeshRenderer.material = curBackgroundMat;

        foreach (ParticleSystemRenderer psr in psRenderers)
        {
            psr.material = curObjectMat;
        }

        splitAdj = adj.Split(new[] { " " },  System.StringSplitOptions.None);

        splitNoun = noun.Split(new[] { " " }, System.StringSplitOptions.None);

        //Sets base position of hue
        float hueAdditionalOffset = Random.Range(0f, 1f);

        for (int i = 0; i < ideaCount; i++)
        {
            var p = Instantiate(ideaPrefab).GetComponent<Idea>();

            curIdeas.Add(p);

            //This is bad but ¯\_(?)_/¯ 
            Quaternion newRotation = Random.rotation;

            if (curIdeas.Count > 1)
            {
                for (int k = 0; k < 1000; k++)
                {
                    foreach (Idea idea in curIdeas)
                    {
                        if(Mathf.Abs(idea.transform.rotation.eulerAngles.y - newRotation.eulerAngles.y) + Mathf.Abs(idea.transform.rotation.eulerAngles.x - newRotation.eulerAngles.x) + (Mathf.Abs(idea.transform.rotation.eulerAngles.z - newRotation.eulerAngles.z) / 2 ) < 200)
                        {
                            if (k > 998)
                            {
                                Debug.Log(i + " needed to be reset " + k + " times " + (Mathf.Abs(idea.transform.rotation.eulerAngles.y - newRotation.eulerAngles.y) + Mathf.Abs(idea.transform.rotation.eulerAngles.x - newRotation.eulerAngles.x) + (Mathf.Abs(idea.transform.rotation.eulerAngles.z - newRotation.eulerAngles.z) / 2)));
                            }
                            

                            newRotation = Random.rotation;
                        }
                    }
                }
            }


           

            p.transform.rotation = newRotation;

            float forwardMulti = Random.Range(1f, 5f);

            p.transform.position += p.transform.forward * forwardMulti;

            p.size = Random.Range(3f, 5f);

            p.curTextMat = new Material(p.curTextMat);         

            //Adds offet based on index 3 is magic number but good
            float hueOffset = (((float) i / ideaCount) / 4f) + hueAdditionalOffset;

            p.curTextMat.SetFloat("_HueOffset", hueOffset);

            p.textField.font = fonts[Random.Range(0, fonts.Count)];

            p.backboard.GetComponent<MeshRenderer>().material = curObjectMat;

            string sonet = splitAdj[Random.Range(0, splitAdj.Length)] + " " + splitNoun[Random.Range(0, splitNoun.Length)];

            p.displayString = sonet;         
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gm.curIdeaToDestroy != null)
        {
            for (int i = 0; i < ideaCount; i++)
            {
                if(curIdeas[i] != gm.curIdeaToDestroy)
                {
                    curIdeas[i].dp.material.SetFloat("_AlphaMulti", 0);
                }          
            }
        }
    }

    public void IdeaFound(Idea idea)
    {
        curFoundIdeas += 1;

        gm.curIdeaToDestroy = idea;

        gm.ppAnimator.SetTrigger("BaW");

        idea.hasBeenCollected = true;

        gm.camCenterTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        gm.camCenterTransform.rotation = idea.transform.rotation;

        idea.dp.material.SetFloat("_FreakOut", 1);

        //Crack specific
        //curObjectMat.SetFloat("_Crack", crackValues[curFoundIdeas]);
    }

    public void TurnOnIdeas()
    {
        foreach(Idea i in curIdeas)
        {
            i.dp.enabled = true;
        }

        StartCoroutine(SpawnTextAnim());
    }

    public void AnimateOrbs()
    {
        StartCoroutine(SpawnOrbsAnim());
    }

    public void DoneSpawning()
    {
        isSpawning = false;
    }

    IEnumerator SpawnTextAnim()
    {
        float elapsedTime = 0f;
        float waitTime = 1f;
        float backboardWaitTime = 1.4f;

        while (elapsedTime < backboardWaitTime)
        {
            // Make sure we got there THIS WILL BE TEXT LEGIBILITY + VISIBLITY

            foreach(Idea i in curIdeas)
            {
                float angle = Quaternion.Angle(i.transform.rotation, gm.camCenterTransform.transform.rotation);

                i.curTextMat.SetFloat("_ValueAdd", Mathf.Lerp(100, 0, (elapsedTime / waitTime)));

                i.curTextMat.SetFloat("_TextLegibility", Mathf.Lerp(1, Mathf.Lerp(1, 0, angle / 180 * gm.rotationOffsetSensitivity), (elapsedTime / waitTime)));

                i.curTextMat.SetFloat("_AlphaMulti", Mathf.Lerp(1, Mathf.Lerp(1, 0, angle / 180 * (gm.rotationOffsetSensitivity * 1.5f)), (elapsedTime / waitTime)));

                if (elapsedTime > waitTime / 2)
                {
                    i.backboard.localScale = Vector3.Lerp(Vector3.zero, new Vector3(i.size / 5, i.size / 5, i.size / 5), (((elapsedTime - (waitTime - waitTime / 2)) * 4) / backboardWaitTime));
                }
                
            }
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }
        // Make sure we got there THIS WILL BE TEXT LEGIBILITY + VISIBLITY

        foreach (Idea i in curIdeas)
        {
            float angle = Quaternion.Angle(i.transform.rotation, gm.camCenterTransform.transform.rotation);

            i.curTextMat.SetFloat("_ValueAdd", 0);

            i.curTextMat.SetFloat("_TextLegibility", Mathf.Lerp(1, 0, angle / 180 * gm.rotationOffsetSensitivity));

            i.curTextMat.SetFloat("_AlphaMulti", Mathf.Lerp(1, 0, angle / 180 * (gm.rotationOffsetSensitivity * 1.5f)));

            i.backboard.localScale = new Vector3(i.size / 5, i.size / 5, i.size / 5);
        }


        yield return null;
    }

    IEnumerator SpawnOrbsAnim()
    {
        float elapsedTime = 0f;
        float waitTime = 3f;

        Debug.Log("fasda");


        while (elapsedTime < waitTime)
        {
            // Make sure we got there THIS WILL BE TEXT LEGIBILITY + VISIBLITY

            foreach (ParticleSystem p in particleSystems)
            {
                //make particles larger and control how far in and out they go OMG USE ANIM CURVES SO SMART
                ParticleSystem.ShapeModule pShape = p.shape;
                Debug.Log("a");
                int particleCount = p.main.maxParticles;

                var particles = new ParticleSystem.Particle[particleCount];
                var currentAmount = p.GetParticles(particles);

                for (int i = 0; i < currentAmount; i++)
                {
                    Debug.Log("f " + particles[i].position);
                    particles[i].position = particles[i].position * shapeScale.Evaluate(elapsedTime);

                    Debug.Log("h " + particles[i].position);
                }
            }
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }
        // Make sure we got there THIS WILL BE TEXT LEGIBILITY + VISIBLITY

        yield return null;
    }
}
