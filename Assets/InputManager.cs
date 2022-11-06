using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Rigidbody rb;

    public Camera mainCam;

    public float mouseSensitivity = 1;

    public Vector2 camZoomMinMax = new Vector2(4, 10);

    public bool playerCanAct = true;

    private Vector3 oldMousePos;

    private float zoomLerp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerCanAct == true)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                oldMousePos = Input.mousePosition;
            }

            if (Input.GetButton("Fire1"))
            {
                Vector2 mouseMovemnet = Input.mousePosition - oldMousePos;

                mouseMovemnet = mouseMovemnet * mouseSensitivity;

                //add torque based on mouse movement last frame
                rb.AddRelativeTorque(new Vector3(-mouseMovemnet.y, mouseMovemnet.x, 0));

                oldMousePos = Input.mousePosition;
            }

            if (Input.GetButton("Fire2"))
            {
                if (zoomLerp < 1)
                {
                    zoomLerp += Time.deltaTime * 5;
                }
                else if (zoomLerp > 1)
                {
                    zoomLerp = 1;
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


            if (Input.GetButton("RotateLeft"))
            {
                rb.AddRelativeTorque(new Vector3(0, 0, 100 * Time.deltaTime));
            }

            if (Input.GetButton("RotateRight"))
            {
                rb.AddRelativeTorque(new Vector3(0, 0, -100 * Time.deltaTime));
            }


            mainCam.orthographicSize = Mathf.Lerp(camZoomMinMax.y, camZoomMinMax.x, zoomLerp);
        } 
    }
}
