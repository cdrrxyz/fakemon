using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Attack,
    Die,
}

public class Enemy : MonoBehaviour
{
    [Tooltip("搜索目标的半径")]
    public float lookRadius = 25;

    [Tooltip("发射子弹/近战攻击 的距离")]
    public float fireDist = 8;

    [Tooltip("子弹预制体")]
    public Transform prefabBullet;

    [Tooltip("子弹发射速度")]
    public float bulletSpeed = 10;

    [Tooltip("发射子弹/近战攻击 冷却时间（秒）")]
    public float fireCD = 1;

    [Tooltip("用一个子物体，确定开火的方向和角度")]
    public Transform firePos;

    [Tooltip("做出动作后，几秒发射子弹/近战攻击")]
    public float fireDelay = 0.4f;

    float lastFireTime;
    NavMeshAgent agent;
    AIState state;

    Transform target;
    Animator animator;

    public string palName;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        switch (state)
        {
            case AIState.Idle:
                {
                    Collider[] colliders = Physics.OverlapSphere(transform.position, lookRadius, LayerMask.GetMask("Player", "Member"));
                    if (colliders.Length > 0)
                    {
                        target = colliders[0].transform;    
                        state = AIState.Attack;
                    }
                    animator.SetBool("Walk", false);
                }
                break;
            case AIState.Attack:
                {
                    if (!target)
                    {
                        state = AIState.Idle;
                        break;
                    }
                    
                    float dist = Vector3.Distance(transform.position, target.position);
                    if (dist < fireDist)
                    {
                        transform.LookAt(target.position);
                        Fire();
                        agent.isStopped = true;
						animator.SetBool("Walk", false);
					}
                    else if (dist < lookRadius)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(target.position);
                        animator.SetBool("Walk", true);
                    }
                    else
                    {
                        agent.isStopped = true;
                        state = AIState.Idle;
                    }
                }
                break;
            case AIState.Die:

                break;
        }
    }

    public void Fire()
    {
        if (Time.time < lastFireTime + fireCD)
        {
            return;
        }
        lastFireTime = Time.time;

        animator.SetTrigger("Attack");

		Invoke("Bullet", fireDelay);
    }

    void Bullet()
    {
        Transform bullet = Instantiate(prefabBullet, firePos.position, Quaternion.identity);
        bullet.forward = firePos.forward;
        Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bullet.forward * bulletSpeed;
    }

	// 被精灵球捕捉
	public void OnPalBallHit(Transform transBall)
	{
		StartCoroutine(CoOnPalBallHit(transBall));
	}

	IEnumerator CoOnPalBallHit(Transform transBall)
	{
		Vector3 pos = transBall.position;
		float time = 0.4f;
		while (time > 0)
		{
			float t = 1 - time / 0.4f;
			transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, t);
			transform.position = Vector3.Lerp(transform.position, pos, t);
			time -= Time.deltaTime;
			yield return null;
		}
		//GetComponent<Collider2D>().enabled = false;
		gameObject.SetActive(false);
	}

	// 捕捉失败
	public void OnCatchingFailed()
	{
		gameObject.SetActive(true);
		StartCoroutine(CoOnCatchingFailed());
	}

	IEnumerator CoOnCatchingFailed()
	{
		float time = 0.4f;
		Vector3 orig = transform.localScale;
		while (time > 0)
		{
			float t = 1 - time / 0.4f;
			transform.localScale = Vector3.Lerp(orig, Vector3.one, t);
			time -= Time.deltaTime;
			yield return null;
		}
	}

	public void Die()
	{
		agent.isStopped = true;
		animator.SetBool("Die", true);

        state = AIState.Die;

        GetComponent<Collider>().enabled = false;
	}
}
