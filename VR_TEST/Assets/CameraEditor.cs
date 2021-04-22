using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEditor : MonoBehaviour
{
#if UNITY_EDITOR

    private const string AXIS_MOUSE_X = "Mouse X";
    private const string AXIS_MOUSE_Y = "Mouse Y";

    private static readonly Vector3 NECK_OFFSET = new Vector3(0, 0.075f, 0.08f);

    private float mouseX = 0;
    private float mouseY = 0;
    private float mouseZ = 0;

    public Vector3 HeadPosition { get; private set; }
    public Quaternion HeadRotation { get; private set; }
    void Start()
    {
        Recenter();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEditorEmulation();
    }

    private bool CanChangeYawPitch()
    {
        return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
    }

    private bool CanChangeRoll()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }

    private void ApplyHeadOrientationToVRCameras()
    {
        Camera.main.transform.localPosition = HeadPosition * Camera.main.transform.lossyScale.y;
        Camera.main.transform.localRotation = HeadRotation;
    }

    private void UpdateHeadPositionAndRotation()
    {
        HeadRotation = Quaternion.Euler(mouseY, mouseX, mouseZ);
        HeadPosition = (HeadRotation * NECK_OFFSET) - (NECK_OFFSET.y * Vector3.up);
    }

    public void Recenter()
    {
        mouseX = mouseZ = 0;
        UpdateHeadPositionAndRotation();
        ApplyHeadOrientationToVRCameras();
    }

    public void UpdateEditorEmulation()
    {
        bool rolled = false;

        if(CanChangeYawPitch())
        {
            mouseX += Input.GetAxis(AXIS_MOUSE_X) * 5;

            if(mouseX <= -180)
            {
                mouseX += 360;
            }
            else if(mouseX > 180)
            {
                mouseX -= 360;
            }

            mouseY -= Input.GetAxis(AXIS_MOUSE_Y) * 2.4f;
            mouseY = Mathf.Clamp(mouseY, -85, 85);
        }
        else if(CanChangeRoll())
        {
            rolled = true;
            mouseZ += Input.GetAxis(AXIS_MOUSE_X) * 5;
            mouseZ = Mathf.Clamp(mouseZ, -85, 85);
        }

        if(!rolled)
        {
            mouseZ = Mathf.Lerp(mouseZ, 0, Time.deltaTime / (Time.deltaTime + 0.1f));
        }

        UpdateHeadPositionAndRotation();
        ApplyHeadOrientationToVRCameras();
        
    }

#endif
}
