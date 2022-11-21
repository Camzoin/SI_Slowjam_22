using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class InputManager : MonoBehaviour
{
    public Camera mainCam;

    public float mouseSensitivity = 1;

    public Vector2 camZoomMinMax = new Vector2(4, 10);

    public bool playerCanAct = true;

    private Vector2 oldMousePos;

    private float zoomLerp;

    public GameManager gm;

    public Animator ideaTab;

    public bool tutorialComplete = false;

    private bool lmbDone = false, rmbDone = false, spaceDone = false;

    public GameObject pauseMenu;

    public bool isPaused = false;

    public Slider soundSlider;

    public AudioMixer AL, musicMixer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AL.SetFloat("Vol", Mathf.Log10(soundSlider.value) * 20f);

        musicMixer.SetFloat("Vol", Mathf.Log10(soundSlider.value) * 20f);

        if (playerCanAct == true && ideaTab.GetCurrentAnimatorStateInfo(0).IsName("Closed") == true && isPaused == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                oldMousePos = Input.mousePosition;

                if(gm.curLevel == null)
                {
                    gm.GoToNextLevel();
                }
            }

            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 mouseDelta = oldMousePos - mousePos;

            // === INPUT FORCES ===
            // Self explanatory for the most part, we apply the mouse delta as a force to both the velocities and the input activity "velocity"

            if (Input.GetMouseButton(0))
            {
                gm.planarVelocity += mouseDelta * gm.planarScalar;
                gm.inputActivity += mouseDelta.magnitude * gm.activityStrength;

                if (lmbDone == false)
                {
                    lmbDone = true;
                }
            }

            if (Input.GetMouseButton(1))
            {
                gm.rollVelocity += mouseDelta.x * gm.rollScalar;
                gm.inputActivity += mouseDelta.x * gm.activityStrength;

                if (rmbDone == false)
                {
                    rmbDone = true;
                }
            }

            // apply drag to the input activity "velocity"
            gm.inputActivity *= 1f - Time.deltaTime * gm.activityDrag;


            //if (Input.GetMouseButton(0))
            //{
            //    Vector2 mouseMovemnet = Input.mousePosition - oldMousePos;

            //    mouseMovemnet = mouseMovemnet * mouseSensitivity;

            //    //add torque based on mouse movement last frame
            //    rb.AddRelativeTorque(new Vector3(-mouseMovemnet.y, mouseMovemnet.x, 0));

            //    oldMousePos = Input.mousePosition;
            //}

            if (Input.GetButton("Jump"))
            {
                if (zoomLerp < 1)
                {
                    zoomLerp += Time.deltaTime * 5;
                }
                else if (zoomLerp > 1)
                {
                    zoomLerp = 1;
                }

                if (spaceDone == false)
                {
                    spaceDone = true;
                }
            }
            else if (zoomLerp > 0)
            {
                zoomLerp -= Time.deltaTime * 5;

                if (zoomLerp < 0)
                {
                    zoomLerp = 0;
                }
            }

           


            //if (Input.GetButton("RotateLeft"))
            //{
            //    rb.AddRelativeTorque(new Vector3(0, 0, 100 * Time.deltaTime));
            //}

            //if (Input.GetButton("RotateRight"))
            //{
            //    rb.AddRelativeTorque(new Vector3(0, 0, -100 * Time.deltaTime));
            //}

            if (lmbDone == true && rmbDone == true && spaceDone == true && tutorialComplete == false)
            {
                tutorialComplete = true;
            }

            mainCam.orthographicSize = Mathf.Lerp(camZoomMinMax.y, camZoomMinMax.x, zoomLerp);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);

            isPaused = !isPaused;
        }

        if (0.7f > Input.mousePosition.x / Screen.width)
        {
            CloseIdeasMenu();
        }

        oldMousePos = Input.mousePosition;
    }

    public void OpenIdeasMenu()
    {
        ideaTab.SetBool("isOpen", true);
    }

    public void CloseIdeasMenu()
    {
        ideaTab.SetBool("isOpen", false);
    }
}
