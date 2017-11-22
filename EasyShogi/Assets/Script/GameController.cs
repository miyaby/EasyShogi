using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public static bool underPlayerTurn = true;

	[SerializeField]
	GameObject turnText;

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

	void updateTurnText()
	{
		Vector3 textPos = turnText.GetComponent<Text> ().transform.position;
		textPos.y = textPos.y * -1;
		turnText.GetComponent<Text> ().transform.position = textPos;
	}
}
