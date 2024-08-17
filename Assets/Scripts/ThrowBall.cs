using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    [Tooltip("精灵球父物体")]
    public GameObject ballParent;

    [Tooltip("要发射出去的精灵球预制体")]
    public Rigidbody prefabBall;

    [Tooltip("扔出球的初速度")]
    public float throwSpeed = 10;

    [Tooltip("扔出球的仰角")]
    public float throwAngle = 20;

    void Start()
    {
        ballParent.SetActive(false);
    }

    void Update()
    {
		// 按下鼠标右键显示精灵球父物体
		if (Input.GetButtonDown("Fire2"))
        {
			ballParent.SetActive(true);
        }

        // 松开鼠标右键隐藏精灵球父物体，并发射
        if (Input.GetButtonUp("Fire2"))
        {
			ballParent.SetActive(false);
			
            Throw();
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameMode.Instance)
            {
                GameMode.Instance.ReleasePal();
            }
        }

        // 确定鼠标指向的方向，让精灵球父物体指向该方向
        if (ballParent.activeInHierarchy)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, 10000, LayerMask.GetMask("Ground")))
            {
                Vector3 dir = hit.point - transform.position;
                dir.y = 0;
				ballParent.transform.forward = dir;
            }
        }
    }

    void Throw()
    {
		Rigidbody ball = Instantiate(prefabBall);
        ball.transform.position = ballParent.transform.GetChild(0).position;

		Vector3 dir = (ballParent.transform.rotation * Quaternion.Euler(-throwAngle, 0, 0)) * Vector3.forward;

		ball.velocity = throwSpeed * dir;
        ball.GetComponent<Pokeball>().player = transform;
	}
}
