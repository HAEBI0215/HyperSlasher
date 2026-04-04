using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    public int currentIndex = 0;

    Vector3[] pathPoints;
    bool isSelecting = false;

    void Start()
    {
        pathPoints = new Vector3[3];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isSelecting = !isSelecting;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = isSelecting;

            Debug.Log(isSelecting ? "위치 지정 시작" : "위치 지정 종료");
        }

        if (isSelecting && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (currentIndex < pathPoints.Length)
                {
                    pathPoints[currentIndex] = hit.point;
                    Debug.Log($"포인트 {currentIndex}: {hit.point}");

                    currentIndex++;
                }
            }

            if (currentIndex == pathPoints.Length)
            {
                Debug.Log("위치 지정 완료");
                CreatPath();

                currentIndex = 0;
                isSelecting = false;

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    void CreatPath()
    {
        for (int i = 0; i < pathPoints.Length; i++)
        {
            Debug.Log($"포인트 {i}: {pathPoints[i]}");
        }
    }

    void OnDrawGizmos()
    {
        if (pathPoints == null) return;

        Gizmos.color = Color.red;

        for (int i = 0; i < pathPoints.Length; i++)
        {
            Gizmos.DrawSphere(pathPoints[i], 0.2f);
        }
    }
}