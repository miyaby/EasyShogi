using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

	public void onClickRestart() {
		GameController.underPlayerTurn = true;
		GameController.finished = false;

		SceneManager.LoadScene (0);
	}
}