using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pokeball : MonoBehaviour
{
	Rigidbody rigid;

	bool hitEnemy = false;

	[HideInInspector]
	public Transform player;

	void Start()
	{
		rigid = GetComponent<Rigidbody>();


		Invoke("DestroySelf", 0.7f);
	}

	private void Update()
	{
		if (hitEnemy)
		{
			rigid.velocity = Vector3.zero;
			rigid.angularVelocity = Vector3.zero;
			rigid.isKinematic = true;
		}
	}

	void DestroySelf()
	{
		if (hitEnemy)
		{
			return;
		}
		Destroy(gameObject);
	}

	private void OnCollisionEnter(Collision collision)
	{
		Enemy enemy = collision.gameObject.GetComponent<Enemy>();
		if (!enemy)
		{
			return;
		}

		enemy.OnPalBallHit(transform);

		GetComponent<Collider>().enabled = false;

		hitEnemy = true;

		StartCoroutine(CoDoCatching(enemy));
	}

	IEnumerator CoDoCatching(Enemy enemy)
	{
		// 晃一晃
		for (int k = 0; k < 3; k++)
		{
			for (int i = 0; i < 2; i++)
			{
				transform.Rotate(0, 0, -5);
				yield return new WaitForSeconds(0.05f);
			}
			for (int i = 0; i < 4; i++)
			{
				transform.Rotate(0, 0, 5);
				yield return new WaitForSeconds(0.05f);
			}
			for (int i = 0; i < 2; i++)
			{
				transform.Rotate(0, 0, -5);
				yield return new WaitForSeconds(0.05f);
			}

			yield return new WaitForSeconds(0.15f);
		}

		int r = Random.Range(0, 100);

		if (r < 50)
		{
			// 成功
			float t = 0;
			while (t < 0.5f)
			{
				transform.position = Vector3.Lerp(transform.position, player.position + new Vector3(0,1,0), 0.03f);
				t += Time.deltaTime;
				yield return null;
			}

			if (GameMode.Instance)
			{
				GameMode.Instance.AddPal(enemy);
			}

			Destroy(enemy.gameObject);
			Destroy(gameObject);
		}
		else
		{
			// 失败
			yield return new WaitForSeconds(0.4f);
			enemy.OnCatchingFailed();
			Destroy(gameObject);
		}
	}
}
