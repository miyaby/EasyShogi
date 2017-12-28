using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public static bool underPlayerTurn = true;
	public static bool finished = false;

	[SerializeField]
	GameObject underTurnText;
	[SerializeField]
	GameObject upperTurnText;
	[SerializeField]
	GameObject underWinText;
	[SerializeField]
	GameObject upperWinText;
	[SerializeField]
	GameObject restartButton;

	public Material chickMat;
	public Material chickenMat;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown()
	{
		Debug.Log ("OnMouseDown");
	}

	public void updateTurnText()
	{
		if(finished)
			return;

		if (underPlayerTurn) {
			underTurnText.SetActive(true);
			upperTurnText.SetActive(false);
		} else {
			underTurnText.SetActive(false);
			upperTurnText.SetActive(true);
		}
	}

	public void finishGame()
	{
		Debug.Log ("ゲーム終了");

		finished = true;
		restartButton.SetActive(true);

		underTurnText.SetActive(false);
		upperTurnText.SetActive(false);

		if (underPlayerTurn) {
			underWinText.SetActive(true);
		} else {
			upperWinText.SetActive(true);
		}
	}
}
