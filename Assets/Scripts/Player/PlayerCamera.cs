﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour {

    public Transform cameraTarget;
    [SerializeField] private float lerpSpeed = 12f;

    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private GameObject camPivot;
    [SerializeField] private GameObject camRoot;

    public AnimationCurve distCurve;
    private float pitchValue, pitchValueAdj;
    public float maxDistance = 6f;
    public float maxPitchDown = 60f;
    public float maxPitchUp = 50f;
    private float xAxisClamp;

    [SerializeField] LayerMask obstacleLayers;
    private float distOffset;
    private bool cameraIsColliding;
    private bool playerBlocked;

    private void Awake()
    {
        LockCursor();
        xAxisClamp = 0;
    }

    void Start()
    {

    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update () 
    {
        // Calculate vertical rotation value (0-1 float) where 0 is looking fully up and 1 is looking fully down
        Vector3 eRot = camPivot.transform.localRotation.eulerAngles;
        pitchValue = Mathf.DeltaAngle(eRot.x, 270f) / -180f;
        pitchValueAdj = Mathf.DeltaAngle(eRot.x, 360f - maxPitchUp) / -(maxPitchUp + maxPitchDown);
        pitchValue = Mathf.Clamp(pitchValue, 0.0f, 1.0f);

        // Update camera distance
        if (camRoot != null)
        {
            camRoot.transform.localPosition = new Vector3(camRoot.transform.localPosition.x, camRoot.transform.localPosition.y, -maxDistance * distCurve.Evaluate(pitchValue));

        }

        // Update camera rotation
        CameraRotation();

        // Check camera for obstacles
        ObstacleCheck();
    }

    private void FixedUpdate()
    {
        if (cameraTarget != null)
        {
            Vector3 b = cameraTarget.position;

            transform.position = Vector3.Lerp(transform.position, b, lerpSpeed * Time.smoothDeltaTime);
        }
        
    }

    private void CameraRotation()
    {
        float mouseX = Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        // Clamp vertical rotation
        xAxisClamp += mouseY;

        if (xAxisClamp > maxPitchUp)
        {
            xAxisClamp = maxPitchUp;
            mouseY = 0f;
            ClampXaxisRotationToValue(360f - maxPitchUp);
        }
        else if (xAxisClamp < -maxPitchDown)
        {
            xAxisClamp = -maxPitchDown;
            mouseY = 0f;
            ClampXaxisRotationToValue(maxPitchDown);
        }

        camPivot.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void ClampXaxisRotationToValue(float value)
    {
        Vector3 eulerRotation = camPivot.transform.eulerAngles;
        eulerRotation.x = value;
        camPivot.transform.eulerAngles = eulerRotation;
    }

    private void ObstacleCheck()
    {

    }

    public void Shake()
    {
        Vector3 b = cameraTarget.position;
        //transform.DOShakeRotation(3f, 15f, 7, 90f, true);
    }

}
