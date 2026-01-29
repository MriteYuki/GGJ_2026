using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAimToTarget : MonoBehaviour
{
    public Transform target;   // 拖一个空物体或敌人进来
    public float angleOffset;

    void Start()
    {
    }

    void Update()
    {
        if (target == null) return;

        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle + angleOffset);
    }
}
