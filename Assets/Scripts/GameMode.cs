using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PalInfo
{
	public string name;
	public GameObject pet;
}

public class GameMode : MonoBehaviour
{
    private List<string> pets = new List<string>();
    public static GameMode Instance { get; private set; }

	Player player;

    public GameObject gameOverPanel;

    public List<PalInfo> listPalInfo;


	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (gameOverPanel)
		{
			gameOverPanel.SetActive(false);
		}

		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}

	private void Update()
	{
		if (player && !player.gameObject.activeInHierarchy && gameOverPanel && !gameOverPanel.activeInHierarchy)
		{
			GameOver();
		}
	}

	public PalInfo GetPalInfo(string name)
	{
		PalInfo info = listPalInfo.Find((pal) => pal.name == name);
		if (info == null)
		{
			Debug.LogWarning("没找到帕鲁的信息：" + name);
			return null;
		}

		return info;
	}

	public void GameOver()
	{
		gameOverPanel?.SetActive(true);
	}

	public void Restart()
	{
		if (player)
		{
			player.Revive();
		}
		gameOverPanel.SetActive(false);
	}

	// 捕捉到帕鲁
	public void AddPal(Enemy enemy)
	{
		pets.Add(enemy.palName);
	}

	// 释放帕鲁到场景中
	public GameObject ReleasePal()
	{
		if (pets.Count == 0)
		{
			return null;
		}

		string name = pets[0];
		PalInfo palInfo = GetPalInfo(name);
		if (palInfo == null)
		{
			return null;
		}

		pets.RemoveAt(0);

		GameObject obj = Instantiate(palInfo.pet);
		Vector3 rand = Random.onUnitSphere * 2;
		rand.y = 0;
		obj.transform.position = player.transform.position + rand;
		return obj;
	}
}
