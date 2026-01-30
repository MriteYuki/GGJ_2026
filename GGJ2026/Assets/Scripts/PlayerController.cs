using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    public float followSpeed = 10f;

    public GameObject target;

    public float minVerticalDistance = 1.2f;
    public float followStrength = 1.0f;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -mainCamera.transform.position.z;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        transform.position = Vector3.Lerp(
            transform.position,
            mouseWorldPos,
            followSpeed * Time.deltaTime
        );

        target.transform.position = UpdateTargetY(mouseWorldPos.y, target.transform.position.y);
    }

    Vector3 UpdateTargetY(float mouseY,float currentTargetY)
    {
        // 同步上下
        float targetY = Mathf.Lerp(currentTargetY, mouseY, followStrength);

        // 最小垂直距离约束
        float deltaY = targetY - mouseY;

        if (Mathf.Abs(deltaY) < minVerticalDistance)
        {
            float sign = Mathf.Sign(deltaY == 0 ? 1 : deltaY);
            targetY = mouseY + sign * minVerticalDistance;
        }

        return new Vector3(0, targetY, 0);
    }


}
