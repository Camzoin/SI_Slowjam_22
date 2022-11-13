using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.Mathf;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (curLevel != null)
        {
                foreach (Idea i in curLevel.curIdeas)
                {
                    if (i != null)
                    {
                        if(closestIdea)
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




                if(closestIdea != null)
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
    }

   
    public void GoToNextLevel()
    {
        if (curLevel != null)
        {
            //Destroy(curLevel.gameObject);

            curLevel.psAnimator.Crumble();

            StartCoroutine(WaitToSpawnNextlevel());
        }
        else
        {
            var p = Instantiate(levelToSpawn);

            curLevel = p;

            p.gm = this;

            p.psAnimator.camTransform = Camera.main.transform;

            p.psAnimator.centerpoint = camCenterTransform;
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

        Destroy(curLevel.gameObject);

        var p = Instantiate(levelToSpawn);

        curLevel = p;

        p.gm = this;

        p.psAnimator.camTransform = Camera.main.transform;

        p.psAnimator.centerpoint = camCenterTransform;

        yield return null;
    }
}
