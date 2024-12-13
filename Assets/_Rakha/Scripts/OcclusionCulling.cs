using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionCulling : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private bool isOcclusionCulling = true;

    private void Awake()
    {
        mainCamera = Camera.main;
        mainCamera.useOcclusionCulling = isOcclusionCulling;
    }

    private void Update()
    {
        // if input ctrl is pressed, show cursor

        if (Input.GetKeyDown(KeyCode.LeftControl)){
            ToggleCursor();
        }
    }

    private void ToggleCursor(){
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void ToggleOcclusionCulling(){
        isOcclusionCulling = !isOcclusionCulling;
        mainCamera.useOcclusionCulling = isOcclusionCulling;
    }
}
