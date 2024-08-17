using UnityEngine;

public class Member : MonoBehaviour
{
    Rigidbody rigid;
    Animator anim;

    public float speed = 5;
    public Rigidbody prefabBullet;
    public Transform firePos;
    public float bulletSpeed = 10;

    Transform master;
    bool isDie = false;

    Transform transCam;

    float turnAmount;
    float forwardAmount;

    float attackTime = 0;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        master = GameObject.FindGameObjectWithTag("Player").transform;
        transCam = master.GetComponent<Player>().transCam;
    }

    void Update()
    {
        Vector3 move = Vector3.zero;
        if (isDie)
        {
            forwardAmount = 0;
            turnAmount = 0;
            return;
        }

        attackTime -= Time.deltaTime;
        if (attackTime > 0)
        {
            return;
        }

        if (Vector3.Distance(transform.position, master.position) > 3)
        {
            move = (master.position - transform.position).normalized;
        }
        else
        {
            // 获取横向和纵向输入，变量范围0.0f ~ 1.0f
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // 根据摄像机位置，对move变换
            Vector3 forward = (transform.position - transCam.position).normalized;
            Vector3 right = Quaternion.Euler(0, 90, 0) * forward;

            move = v * Vector3.ProjectOnPlane(forward, Vector3.up) + h * Vector3.ProjectOnPlane(right, Vector3.up);

            if (move.magnitude > 1f)
            {
                move.Normalize();
            }
        }

        move = transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, Vector3.up);
        turnAmount = Mathf.Atan2(move.x, move.z);
        forwardAmount = move.z;

        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }

        // 设置动画变量
        anim.SetBool("Walk", move.magnitude > 0.1f);
    }

	void Fire()
	{
        attackTime = 0.5f;
        Rigidbody bullet = Instantiate(prefabBullet, firePos.position, Quaternion.identity);
        bullet.transform.forward = firePos.forward;
        bullet.velocity = firePos.forward * bulletSpeed;
        anim.SetTrigger("Attack");
	}

	private void FixedUpdate()
    {
        if (attackTime > 0)
        {
            rigid.velocity = Vector3.zero;
            return;
        }

        transform.Rotate(0, turnAmount * 72, 0);
        rigid.velocity = transform.forward * forwardAmount * speed;
    }

    public void Die()
    {
        isDie = true;
        anim.SetBool("Die", true);
        GetComponent<Collider>().enabled = false;
    }

    public void HitBack(Vector2 hitDir)
    {
        anim.SetTrigger("behit");
    }
}