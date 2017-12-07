using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public static bool underPlayerTurn = true;

	[SerializeField]
	GameObject underTurnText;
	[SerializeField]
	GameObject upperTurnText;

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
		if (underPlayerTurn) {
			underTurnText.SetActive(true);
			upperTurnText.SetActive(false);
		} else {
			underTurnText.SetActive(false);
			upperTurnText.SetActive(true);
		}
	}
}
