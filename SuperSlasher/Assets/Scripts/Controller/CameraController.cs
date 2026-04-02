using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraController : MonoBehaviour
{
    PlayerMove playerMove;
    CinemachineFreeLook freeLook;

    public Transform rCamPos;
    public Transform lCamPos;
    public Transform fCamPos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        freeLook = GetComponent<CinemachineFreeLook>();
        playerMove = FindObjectOfType<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCurrentPos()
    {
        if (playerMove.isRunning == true)
        {
            Debug.Log("중앙캠");
            freeLook.Follow = GameObject.Find("fCamPos").transform;
            freeLook.LookAt = GameObject.Find("fCamPos").transform;
        }
        else
        {
            freeLook.Follow = GameObject.Find("rCamPos").transform;
            freeLook.LookAt = GameObject.Find("rCamPos").transform;
        }
    }
}
