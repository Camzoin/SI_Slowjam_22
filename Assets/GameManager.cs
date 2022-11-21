using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.Mathf;
using UnityEngine.UI;
using System;

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

    public List<string> allStringsCollected;

    public int ideaCollectedCount;

    public InputManager inputManager;

    public RectTransform ideaMenuTransform;

    public GameObject ideaMenuIdea;

    public float textBlurAngle = 50f, textDisapearAngle = 100f;

    public List<Idea> collectedIdeas;

    public float timeToCollectCurLevel = 60f, curtimeToCollectCurLevel = 0f;

    public Image timerFillImage;

    public MeshRenderer backgroundRenderer;

    public string adj, noun;

    public string[] splitAdj, splitNoun;

    public GameObject coreExplosion;

    public GameObject uiGameObject;

    public Image wordsFoundThisCloudBar, wordsFoundTotalBar;

    public AudioSource clickToStart, congrats;

    private int highestPossibleWordCount;

    private float clickToStartTimer = 5f, curClickToStartTime = 0;

    public Animator tutAnimator;

    public AudioSource noteCollectionSoundSource, noteCollectionSoundSource2;

    private float collectionAudioLoopVol = 0;

    public bool collectionAudioLoopVolGoingDown = true;

    public List<AudioClip> songs;

    public List<AudioClip> songsToUse;

    public Animator gameEndAnimator;

    public TextMeshProUGUI firstFrac, secondFrac, thirdFrac;

    [Header("Movement Settings")]
    [Tooltip("Constant drag applied to latteral movement")]
    public float planarDrag = 5f;
    [Tooltip("Constant drag applied to rolling movement")]
    public float rollDrag = 5f;
    [Tooltip("The scalar applied to latteral movement inputs, higher = faster")]
    public float planarScalar = .4f;
    [Tooltip("The scalar applied to roll movement inputs, higher = faster")]
    public float rollScalar = .3f;


    [Header("Aim Assist Settings")]

    [Tooltip("A curve describing the scale of AimAssist gravity based on distance")]
    public AnimationCurve gravCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 0f, 0f, 0f) });
    [Tooltip("A curve describing the scale of AimAssist drag based on distance.")]
    public AnimationCurve gravDragCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 1f, -2.75f, -2.75f), new Keyframe(1f, 0f, -.05f, -.05f) });

    [Tooltip("The angle at which the planar Aim Assist will begin to apply")]
    public float planarGravMaxAngle = 10f;
    [Tooltip("The strength of the planar Aim Assist gravity")]
    public float planarGravStength = 70f;
    [Tooltip("The stength of the planar Aim Assist drag")]
    public float planarGravDragStrength = 12f;

    [Tooltip("The angle at which the roll Aim Assist will begin to apply")]
    public float rollGravMaxAngle = 8f;
    [Tooltip("The strength of the roll Aim Assist gravity")]
    public float rollGravStrength = 85f;
    [Tooltip("The stength of the roll Aim Assist drag")]
    public float rollGravDragStrength = 8f;




    // Input Activity behaviour stuff, treated kinda like a velocity. Making it const cause you probably don't need to fuck with these.
    // Although if you want player-input to make the AA weaker for longer, the drag could be brought down.
    public float activityStrength = .008f;
    public float activityDrag = 1.8f;


    public Vector2 planarVelocity;
    public float rollVelocity;
    public float inputActivity;

    private Idea closestIdea = null;

    // Start is called before the first frame update
    void Start()
    {
        ResetSongList();

        splitAdj = adj.Split(new[] { " " }, System.StringSplitOptions.None);

        splitNoun = noun.Split(new[] { " " }, System.StringSplitOptions.None);

        if (splitAdj.Length > splitNoun.Length)
        {
            highestPossibleWordCount = splitNoun.Length;
        }
        {
            highestPossibleWordCount = splitAdj.Length;
        }

        backgroundRenderer.material = new Material(backgroundRenderer.material);
    }

    // Update is called once per frame
    void Update()
    {
        if (curLevel != null)
        {
            firstFrac.text = 120 - (int)curtimeToCollectCurLevel + "/" + timeToCollectCurLevel;

            secondFrac.text = 7 - curLevel.curFoundIdeas + "/" + curLevel.spawnedIdeaCount;

            if (splitAdj.Length > splitNoun.Length)
            {
                thirdFrac.text = (splitNoun.Length + curLevel.ideaCount - curLevel.curFoundIdeas) + "/" + (float)highestPossibleWordCount;
            }
            else
            {
                thirdFrac.text = (splitAdj.Length + curLevel.ideaCount - curLevel.curFoundIdeas) + "/" + (float)highestPossibleWordCount;
            }
        }
        else
        {
            firstFrac.text = timeToCollectCurLevel + "/" + timeToCollectCurLevel;

            secondFrac.text = 7 + "/" + 7;

            if (splitAdj.Length > splitNoun.Length)
            {
                thirdFrac.text = splitNoun.Length + "/" + (float)highestPossibleWordCount;
            }
            else
            {
                thirdFrac.text = splitAdj.Length + "/" + (float)highestPossibleWordCount;
            }
        }




        if (ideaMenuIdea.transform.childCount > 8)
        {
            //Do the first option
        }

        if (curLevel == null)
        {
            curClickToStartTime += Time.deltaTime;
        }

        if (curClickToStartTime > clickToStartTimer)
        {
            clickToStart.Play();

            curClickToStartTime = 0;
        }

        if (inputManager.tutorialComplete == true)
        {
            tutAnimator.SetTrigger("CloseTutorial");
        }




        if (curLevel != null)
        {
            //PP + smooth stuff

            if (collectionAudioLoopVolGoingDown == true && collectionAudioLoopVol >= 0)
            {
                collectionAudioLoopVol = collectionAudioLoopVol - (Time.deltaTime / 2);

                curLevel.musicVol = Clamp01(curLevel.musicVol + (Time.deltaTime / 2));
            }
            else if (curLevel.isSpawning == false)
            {
                curLevel.musicVol = Clamp01(curLevel.musicVol + (Time.deltaTime / 2));
            }

            noteCollectionSoundSource.volume = collectionAudioLoopVol;


            if (inputManager.tutorialComplete == false && curLevel.isSpawning == false)
            {
                tutAnimator.SetTrigger("OpenTutorial");
            }

            if (curLevel.psAnimator.hasStarted == false && curLevel.psAnimator.hasDied == false && curLevel.isSpawning == false)
            {
                curtimeToCollectCurLevel += Time.deltaTime;
            }

            float percentTimeRemaining = 1 - (curtimeToCollectCurLevel / timeToCollectCurLevel);

            if (curLevel.psAnimator.hasDied == true)
            {
                backgroundRenderer.material.SetFloat("_ColorLerp", Clamp01(backgroundRenderer.material.GetFloat("_ColorLerp") + (Time.deltaTime / 2)));

                percentTimeRemaining = 0;
            }

            if (percentTimeRemaining < 0.1f && percentTimeRemaining >= 0.05f)
            {
                backgroundRenderer.material.SetFloat("_ColorLerp", Clamp01(backgroundRenderer.material.GetFloat("_ColorLerp") + (Time.deltaTime * 2)));

                foreach (ParticleSystemRenderer psr in curLevel.psRenderers)
                {
                    psr.material.SetFloat("_ColorLerp", backgroundRenderer.material.GetFloat("_ColorLerp"));
                }

                foreach (Idea ide in curLevel.curIdeas)
                {
                    if (ide.backboardRenderer != null)
                    {
                        ide.backboardRenderer.material.SetFloat("_ColorLerp", backgroundRenderer.material.GetFloat("_ColorLerp"));
                    }
                }
            }
            if ((percentTimeRemaining < 0.05f && percentTimeRemaining >= 0) || percentTimeRemaining < 0)
            {
                float adjPercentTimeRemaining = 1 - (percentTimeRemaining * 20);

                ppAnimator.SetLayerWeight(0, Lerp(1, 0, adjPercentTimeRemaining));

                ppAnimator.SetLayerWeight(1, Lerp(0, 0.05f, adjPercentTimeRemaining));

                backgroundRenderer.material.SetFloat("_ColorLerp", Clamp01(backgroundRenderer.material.GetFloat("_ColorLerp") + (Time.deltaTime * 2)));

                foreach (ParticleSystemRenderer psr in curLevel.psRenderers)
                {
                    psr.material.SetFloat("_ColorLerp", 0);

                    psr.material.SetColor("_Color_2", Color.Lerp(psr.material.GetColor("_Color_2"), new Color(0.2641509f, 0.2641509f, 0.2641509f, 1), adjPercentTimeRemaining));
                }

                foreach (Idea ide in curLevel.curIdeas)
                {
                    if (ide.backboardRenderer != null)
                    {
                        ide.backboardRenderer.material.SetFloat("_ColorLerp", backgroundRenderer.material.GetFloat("_ColorLerp"));

                        ide.backboardRenderer.material.SetColor("_Color_2", new Color(0.2641509f, 0.2641509f, 0.2641509f, 1));
                    }
                }
            }
            else
            {
                foreach (ParticleSystemRenderer psr in curLevel.psRenderers)
                {
                    psr.material.SetFloat("_ColorLerp", Clamp01(psr.material.GetFloat("_ColorLerp") - (Time.deltaTime * 2)));
                }

                backgroundRenderer.material.SetFloat("_ColorLerp", Clamp01(backgroundRenderer.material.GetFloat("_ColorLerp") - (Time.deltaTime / 2)));

                ppAnimator.SetLayerWeight(0, Clamp01(ppAnimator.GetLayerWeight(0) + Time.deltaTime));

                ppAnimator.SetLayerWeight(1, 0);
            }

            timerFillImage.fillAmount = percentTimeRemaining;

            //If you go over time explode

            if (percentTimeRemaining < 0)
            {
                //DIE

                foreach (Idea yu in curLevel.curIdeas)
                {
                    if (yu != null)
                    {
                        yu.ShrinkMe();
                    }
                }

                curLevel.FadeMusicOutFunc();

                curtimeToCollectCurLevel = 0;

                curLevel.psAnimator.Explode();

                splitAdj = adj.Split(new[] { " " }, System.StringSplitOptions.None);

                splitNoun = noun.Split(new[] { " " }, System.StringSplitOptions.None);

                SetWordsFoundToFull(wordsFoundThisCloudBar);

                SetWordsFoundToFull(wordsFoundTotalBar);

                StartCoroutine(WaitToSpawnNextlevel());
            }


            foreach (Idea i in curLevel.curIdeas)
            {
                if (i != null)
                {
                    if (closestIdea)
                    {
                        //FIND THE CLOSEST IDEA
                        if (Vector3.Angle(i.transform.rotation.eulerAngles, camCenterTransform.rotation.eulerAngles) < Vector3.Angle(closestIdea.transform.rotation.eulerAngles, camCenterTransform.rotation.eulerAngles))
                        {
                            closestIdea = i;
                        }
                    }
                    else
                    {
                        closestIdea = i;
                    }



                    float dot = (Vector3.Dot(i.transform.forward, camCenterTransform.forward) + 1) / 2;

                    i.curTextMat.SetFloat("_TextLegibility", Lerp(0, 1, Clamp01(dot - textBlurAngle)));

                    if (curIdeaToDestroy == null)
                    {
                        i.curTextMat.SetFloat("_AlphaMulti", Lerp(0, 1, Clamp01(dot - textDisapearAngle)));
                    }


                }
            }

            if (curLevel.curIdeas.Count > 0)
            {
                if (closestIdea == null && curLevel.curIdeas[0] != null)
                {
                    closestIdea = curLevel.curIdeas[0];
                }
            }





        }

        

        if (closestIdea != null)
        {
            //      Basic idea here is that instead of the aim-assist being a direct movement applied to our object, we extrapolate the aim-assist
            // and the player's input into forces that are applied to a single velocity, making things feel more 'physical' and therefore intuitive.
            // A couple key ideas:
            //      > "1f - Time.deltaTime * drag" is our drag formula. It's essentially an estimation of drag forces for discrete time-steps.
            //          Gets less accurate with lower framerates but should be fine as long as nothing goes sub-twenty.
            //      > The goal of "inputActivity" is to decrease the aim-assist whenever we're actively moving the camera. It's treated essentially
            //          as a velocity on an invisible axis, with forces applied alongside inputs and a constant drag applied.

            // IMPLEMENTATION DETAILS:
            // When you're implementing, the primary things you'll wanna replace are 'target' and 'pivot'
            // For calculations, getting the right transforms is really all your need,
            // so just replace these assignments  with whatever and that's all good
            Transform pivot = camCenterTransform; // <-- probably just your camera transform
            Transform target = closestIdea.transform; // <-- probably just the nearest word

            // For the application of the velocity, if you don't have a pivot transform, you may need to do 'RotateAround' once for each local axis
            // Honestly I'd recommend just putting the camera under a pivot transform at the center, but whatever works ¯\_(?)_/¯   





            #region ============= AIM ASSIST =============

            #region === PLANAR-ASSIST ===
            // 0 to 1 scalar, if we're inputting it'll down and 
            float activityScalar = 1f - Clamp01(inputActivity);
            // if we're not within a 90 degree angle to the target, dont do any AA, to prevent effects on the flip-side
            if (Vector3.Dot(target.forward, pivot.forward) < 0f)
                activityScalar = 0f;

            Vector3 planarRelativeForward = pivot.InverseTransformDirection(target.forward);
            Vector2 planarDiff = new Vector2(planarRelativeForward.x, planarRelativeForward.y);

            float planarMaxDiff = Sin((planarGravMaxAngle / 180f) * PI); // max angle converted to linear distance

            float planarDist = Clamp01(planarDiff.magnitude / planarMaxDiff); // 0-1 distance used for curve eval
            float planarGrav = gravCurve.Evaluate(planarDist) * activityScalar * planarGravStength;
            float planarGravDrag = gravDragCurve.Evaluate(planarDist) * activityScalar * activityScalar * planarGravDragStrength; // activityScalar is squared here so drag applys mostly only when completely inactive

            // apply our grav force to the velocity 
            planarVelocity += -planarDiff.normalized * planarGrav * Time.deltaTime; // apply planar-assist gravity
            planarVelocity *= 1f - Time.deltaTime * planarGravDrag; // apply planar-assist drag
            #endregion === ----------- ===

            #region === ROLL-ASSIST ===
            // square our planar distance to use as a scalar, so we only do roll assist when we're nearby. This could be cubed if you really wanna crack down.
            float planarDistMult = (1f - planarDist) * (1f - planarDist);

            // Roll calculations are just a 1D copy of the planar stuff
            Vector3 rollRelativeUp = pivot.InverseTransformDirection(target.up);
            float rollDiff = rollRelativeUp.x;

            float rollMaxDiff = Sin((rollGravMaxAngle / 180f) * PI);

            float rollDist = Clamp01(Abs(rollDiff) / rollMaxDiff);
            float rollGrav = gravCurve.Evaluate(rollDist) * activityScalar * rollGravStrength * planarDistMult; // added planar dist scalar to gravity and drag
            float rollGravDrag = gravDragCurve.Evaluate(rollDist) * activityScalar * activityScalar * rollGravDragStrength * planarDistMult;

            rollVelocity += Sign(-rollDiff) * rollGrav * Time.deltaTime;
            rollVelocity *= 1f - Time.deltaTime * rollGravDrag;
            #endregion === ----------- ===

            #endregion ============= ---------- =============

            #region ============= DRAG =============
            // apply planar drag
            float pVel = planarVelocity.magnitude;
            pVel *= 1f - Time.deltaTime * planarDrag;
            planarVelocity = planarVelocity.normalized * pVel;

            // apply roll drag
            rollVelocity *= 1f - Time.deltaTime * rollDrag;
            #endregion ============= ---- =============

            // apply our velocity as a rotation-over-time, relative to the local axes
            Vector3 rotation = new Vector3(planarVelocity.y, -planarVelocity.x, rollVelocity) * Time.deltaTime;
            pivot.Rotate(rotation, Space.Self);

            float dot = (Vector3.Dot(closestIdea.transform.forward, camCenterTransform.forward) + 1) / 2;

            if (dot > 0.99999f && closestIdea.hasBeenCollected == false)
            {
                curLevel.IdeaFound(closestIdea);

                closestIdea.hasBeenCollected = true;

                planarVelocity = Vector2.zero;

                rollVelocity = 0f;

                noteCollectionSoundSource.volume = 1;

                noteCollectionSoundSource.pitch = UnityEngine.Random.Range(0.4f, 1.5f);

                collectionAudioLoopVol = 1;

                curLevel.musicVol = 0;

                collectionAudioLoopVolGoingDown = false;

                noteCollectionSoundSource2.Play();

                collectedIdeas.Add(closestIdea);

                var spawnedIdea = Instantiate(ideaMenuIdea, ideaMenuTransform);

                TextMeshProUGUI ideaText = spawnedIdea.GetComponent<TextMeshProUGUI>();

                ideaText.font = closestIdea.textField.font;

                ideaText.text = closestIdea.displayString;
            }
        }
        else
        {
            #region ============= DRAG =============
            // apply planar drag
            float pVel = planarVelocity.magnitude;
            pVel *= 1f - Time.deltaTime * planarDrag;
            planarVelocity = planarVelocity.normalized * pVel;

            // apply roll drag
            rollVelocity *= 1f - Time.deltaTime * rollDrag;
            #endregion ============= ---- =============

            // apply our velocity as a rotation-over-time, relative to the local axes
            Vector3 rotation = new Vector3(planarVelocity.y, -planarVelocity.x, rollVelocity) * Time.deltaTime;
            camCenterTransform.Rotate(rotation, Space.Self);
        }
    }


    public void GoToNextLevel()
    {
        noteCollectionSoundSource.Stop();
        noteCollectionSoundSource.Play();

        

        

        //If I just won the game
        if (splitNoun.Length == 0 || splitAdj.Length == 0)
        {
            curtimeToCollectCurLevel = 0;

            curLevel.psAnimator.Won();

            Instantiate(coreExplosion, Vector3.zero, Quaternion.identity);

            splitAdj = adj.Split(new[] { " " }, System.StringSplitOptions.None);

            splitNoun = noun.Split(new[] { " " }, System.StringSplitOptions.None);

            StartCoroutine(WonTheGame());
        }
        //If I did not just win the game
        else
        {
            if (curLevel != null)
            {
                curtimeToCollectCurLevel = 0;

                curLevel.psAnimator.Crumble();

                StartCoroutine(WaitToSpawnNextlevel());
            }
            else
            {
                var p = Instantiate(levelToSpawn);

                curtimeToCollectCurLevel = 0;

                curLevel = p;

                if (songsToUse.Count == 0)
                {
                    ResetSongList();
                }

                int randy = UnityEngine.Random.Range(0, songsToUse.Count);              

                p.music.PlayOneShot(songsToUse[randy]);

                songsToUse.Remove(songsToUse[randy]);

                p.FadeMusicInFunc();

                p.gm = this;

                p.psAnimator.camTransform = Camera.main.transform;

                p.psAnimator.centerpoint = camCenterTransform;

                SetImageFills();
            }
        }
    }

    public IEnumerator WaitToSpawnNextlevel()
    {
        float elapsedTime = 0f;
        float waitTime = 3f;

        while (elapsedTime < waitTime)
        {

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetImageFills();

        Destroy(curLevel.gameObject);

        var p = Instantiate(levelToSpawn);

        curtimeToCollectCurLevel = 0f;

        curLevel = p;

        if (songsToUse.Count == 0)
        {
            ResetSongList();
        }

        int randy = UnityEngine.Random.Range(0, songsToUse.Count);      

        p.music.PlayOneShot(songsToUse[randy]);

        songsToUse.Remove(songsToUse[randy]);

        p.FadeMusicInFunc();

        p.gm = this;

        p.psAnimator.camTransform = Camera.main.transform;

        p.psAnimator.centerpoint = camCenterTransform;

        SetWordsFoundToFull(wordsFoundThisCloudBar);

        yield return null;
    }

    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        for (int a = index; a < arr.Length - 1; a++)
        {
            // moving elements downwards, to fill the gap at [index]
            arr[a] = arr[a + 1];
        }
        // finally, let's decrement Array's size by one
        Array.Resize(ref arr, arr.Length - 1);
    }

    public void SetImageFills()
    {
        wordsFoundThisCloudBar.fillAmount = 1 - (curLevel.curFoundIdeas / (float)curLevel.ideaCount);


        if (splitAdj.Length > splitNoun.Length)
        {
            wordsFoundTotalBar.fillAmount = (splitNoun.Length + curLevel.ideaCount - curLevel.curFoundIdeas) / (float)highestPossibleWordCount;
        }
        else
        {
            wordsFoundTotalBar.fillAmount = (splitAdj.Length + curLevel.ideaCount - curLevel.curFoundIdeas) / (float)highestPossibleWordCount;
        }


    }

    public void SetWordsFoundToFull(Image bar)
    {
        StartCoroutine(RefillBar(bar));
    }

    public IEnumerator RefillBar(Image bar)
    {
        float elapsedTime = 0f;
        float waitTime = 3.5f;

        while (elapsedTime < waitTime)
        {
            if (elapsedTime / waitTime > bar.fillAmount)
            {
                bar.fillAmount = elapsedTime / waitTime;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bar.fillAmount = 1;
    }

    public IEnumerator WonTheGame()
    {
        float elapsedTime = 0f;
        float waitTime = 3f;

        while (elapsedTime < waitTime)
        {

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        wordsFoundThisCloudBar.fillAmount = 1;

        wordsFoundTotalBar.fillAmount = 1;

        timerFillImage.fillAmount = 1;

        Destroy(curLevel.gameObject);

        curClickToStartTime = 0;

        curtimeToCollectCurLevel = 0f;

        congrats.Play();

        yield return null;
    }

    public void ResetSongList()
    {
        songsToUse = new List<AudioClip>(songs);
    }

    public void CloseGame()
    {
        gameEndAnimator.SetTrigger("Play");
    }
}
