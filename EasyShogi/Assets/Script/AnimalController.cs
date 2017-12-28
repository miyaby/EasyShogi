using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour {

	Vector3 oriPositon;
	GameObject director;

	bool dragTakenAnimal = false;

	// Use this for initialization
	void Start () {
		director = GameObject.Find ("GameDirector");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown()
	{
		//ゲームが終了している
		if(GameController.finished)
			return;

		//自分の駒でないなら終わり
		if (GameController.underPlayerTurn != underPlayerAnimal (this.gameObject))
			return;
		
		Debug.Log ("OnMouseDown");
		oriPositon = this.transform.position;

//		Debug.Log ("NAME:"+transform.root.gameObject.name);
		//手駒をドラッグ
		dragTakenAnimal = (transform.root.gameObject.name == "TakenBorad");
	}

	void OnMouseDrag()
	{
		//ゲームが終了している
		if(GameController.finished)
			return;

		//自分の駒でないなら終わり
		if (GameController.underPlayerTurn != underPlayerAnimal (this.gameObject))
			return;
		
		Vector3 objectPointInScreen
		= Camera.main.WorldToScreenPoint(this.transform.position);

		Vector3 mousePointInScreen
		= new Vector3(Input.mousePosition.x,
			Input.mousePosition.y,
			objectPointInScreen.z);

		Vector3 mousePointInWorld = Camera.main.ScreenToWorldPoint(mousePointInScreen);
		mousePointInWorld.z = -2;
		this.transform.position = mousePointInWorld;
	}

	void OnMouseUp()
	{
		//ゲームが終了している
		if(GameController.finished)
			return;

		//自分の駒でないなら終わり
		if (GameController.underPlayerTurn != underPlayerAnimal (this.gameObject))
			return;
		
		Debug.Log ("OnMouseUp");

		GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tiles");

		Vector3 mousePointInScreen
		= new Vector3(Input.mousePosition.x,Input.mousePosition.y,0);
		Vector3 mousePointInWorld = Camera.main.ScreenToWorldPoint(mousePointInScreen);

		//全タイルについて走査する
		foreach (GameObject tile in tiles) {

			Vector3 tilePointInWorld = Camera.main.ScreenToWorldPoint (tile.transform.position);

//			Debug.Log ("mouse"+mousePointInWorld);
//			Debug.Log ("tile"+tile.transform.position);
//			Debug.Log ("scale"+tile.transform.lossyScale);

			//タイルの内部でクリックが終わった
			if (mousePointInWorld.x > tile.transform.position.x - tile.transform.lossyScale.x / 2 &&
				mousePointInWorld.x < tile.transform.position.x + tile.transform.lossyScale.x / 2 &&
				mousePointInWorld.y > tile.transform.position.y - tile.transform.lossyScale.y / 2 &&
				mousePointInWorld.y < tile.transform.position.y + tile.transform.lossyScale.y / 2) {

				//移動前と移動先が同じ
				if (tile.name == this.transform.parent.name) {
					Debug.Log ("同じタイルを選択");
					this.transform.position = oriPositon;
					return;
				}

				//遷移前・遷移先の差分が、現在持っている駒の移動可能先にあるかどうか
				if (!dragTakenAnimal) {
					bool reachable = false;
					foreach (KeyValuePair<int, int> pair in reachableArea(this.name)) {

						//画面上プレイヤーの場合、進行方向は逆
						int dir = 1;
						if(!GameController.underPlayerTurn)
							dir = -1;
							
						if (pair.Key*dir == move (this.transform.parent.gameObject,tile).Key &&
							pair.Value*dir == move (this.transform.parent.gameObject,tile).Value)
							reachable = true;
					}
					//なかったら終わり
					if (!reachable) {
						this.transform.position = oriPositon;
						Debug.Log ("選択中の駒が到達不可能なタイルです");
						return;
					}
				}

				//置いた先に駒がある
				if(GameObjectExtensions.HasChild(tile)){

					//手駒から出している時は、駒があるタイルには置けない
					if (dragTakenAnimal) {
						Debug.Log ("手駒から出している時は、駒があるタイルには置けない");
						this.transform.position = oriPositon;
						return;
					}
						
					GameObject animal = tile.transform.GetChild(0).gameObject;

//					Debug.Log ("animal rotation"+animal.transform.localRotation);
					Debug.Log ("animal rotation"+animal.transform.localRotation.eulerAngles);
					Debug.Log ("turn"+GameController.underPlayerTurn);

					//置いた先の動物が置いたプレイヤーのものなら、もとの位置に戻して終了
					if ((underPlayerAnimal(animal) && GameController.underPlayerTurn) || //先手が先手に
						(!underPlayerAnimal(animal) && !GameController.underPlayerTurn)) {//後手が後手に
						Debug.Log ("own animal");
						this.transform.position = oriPositon;
						return;
					}

					//置いた先の動物が相手プレイヤーのものなら、手駒にする
					if ((!underPlayerAnimal(animal) && GameController.underPlayerTurn) || //先手が後手に
						(underPlayerAnimal(animal) && !GameController.underPlayerTurn)) {//後手が先手に
						Debug.Log ("enemy animal");
						takeAnimal (animal);
						powerDownAnimal (animal);

						if (animal.name == "lion"){
							director.GetComponent<GameController> ().finishGame ();
							return;
						}
					}
				}

				//位置と親を変更
				this.transform.parent = tile.transform;
				this.transform.position = new Vector3(tile.transform.position.x,tile.transform.position.y,-1);

				//手駒からではなく、最終列に到達した
				if (!dragTakenAnimal) {
					int y = int.Parse (tile.name.Substring (5, 1));
					if ((!GameController.underPlayerTurn && y == 4) || (GameController.underPlayerTurn && y == 1)) {
						powerUpAnimal ();

						if (this.name == "lion"){
							director.GetComponent<GameController> ().finishGame ();
							return;
						}
					}
				}

				//ターン変更
				GameController.underPlayerTurn = !GameController.underPlayerTurn;
				director.GetComponent<GameController> ().updateTurnText ();

				//ターンが正しく完了した
				Debug.Log ("ターン終了");

				return;
			}
		}

		this.transform.position = oriPositon;
	}

	//プレイヤー(画面下)の駒かどうか
	bool underPlayerAnimal(GameObject animal){
		return (animal.transform.localRotation.eulerAngles.y == 180);
	}

	//駒をターン中プレイヤーの手駒にする
	void takeAnimal(GameObject animal){

		GameObject board;
		if (GameController.underPlayerTurn) {
			board = GameObject.Find ("UnderTakenBoard");
			animal.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
		} else {
			board = GameObject.Find ("UpperTakenBoard");
			animal.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		}

//		Debug.Log ("B scale："+board.transform.lossyScale);
//		Debug.Log ("takenAnimalCount："+takenAnimalCount);

		//駒の親を手駒ボードに変更
		animal.transform.parent = board.transform;

		//手駒ボードを整理する
		int pos = -2;
		foreach (Transform takenAnimal in board.transform) {
			takenAnimal.position = new Vector3 (pos, board.transform.position.y, -1);
			pos++;
		}
	}

	//２つのタイルから駒の移動を返す
	KeyValuePair<int,int> move(GameObject fromTile,GameObject toTile){

		int fromX = int.Parse(fromTile.name.Substring (4,1));
		int fromY = int.Parse(fromTile.name.Substring (5,1));
		int toX = int.Parse(toTile.name.Substring (4,1));
		int toY = int.Parse(toTile.name.Substring (5,1));

		int disX = fromX - toX;
		int disY = fromY - toY;

		return new KeyValuePair<int, int> (disX, disY);
	}

	//駒の行き先を返す
	List<KeyValuePair<int,int>> reachableArea(string animalName){

		switch (animalName) {
		case "chick":
			return new List<KeyValuePair<int, int>>{
				new KeyValuePair<int, int>(0,1)
			};
		case "chicken":
			return new List<KeyValuePair<int, int>>{
				new KeyValuePair<int, int>(-1,1),
				new KeyValuePair<int, int>(0,1),
				new KeyValuePair<int, int>(1,1),
				new KeyValuePair<int, int>(-1,0),
				new KeyValuePair<int, int>(1,0),
				new KeyValuePair<int, int>(0,-1)
			};
		case "giraffe":
			return new List<KeyValuePair<int, int>>{
				new KeyValuePair<int, int>(0,1),
				new KeyValuePair<int, int>(-1,0),
				new KeyValuePair<int, int>(1,0),
				new KeyValuePair<int, int>(0,-1)
			};
		case "elephant":
			return new List<KeyValuePair<int, int>>{
				new KeyValuePair<int, int>(-1,1),
				new KeyValuePair<int, int>(1,1),
				new KeyValuePair<int, int>(-1,-1),
				new KeyValuePair<int, int>(1,-1)
			};
		case "lion":
			return new List<KeyValuePair<int, int>>{
				new KeyValuePair<int, int>(-1,1),
				new KeyValuePair<int, int>(0,1),
				new KeyValuePair<int, int>(1,1),
				new KeyValuePair<int, int>(-1,0),
				new KeyValuePair<int, int>(1,0),
				new KeyValuePair<int, int>(-1,-1),
				new KeyValuePair<int, int>(0,-1),
				new KeyValuePair<int, int>(1,-1)
			};
		}

		return new List<KeyValuePair<int, int>>{ };
	}

	//動物がパワーアップする
	void powerUpAnimal(){
		
		switch (this.name) {
		case "chick":
			this.name = "chicken";
			GetComponent<Renderer> ().material = director.GetComponent<GameController> ().chickenMat;
			return;
		}
	}

	//動物がパワーダウンする
	void powerDownAnimal(GameObject animal){

		switch (animal.name) {
		case "chicken":
			animal.name = "chick";
			animal.GetComponent<Renderer> ().material = director.GetComponent<GameController> ().chickMat;
			return;
		}
	}
}

/// GameObject 型の拡張メソッドを管理するクラス
public static partial class GameObjectExtensions
{
	public static bool HasChild(this GameObject gameObject)
	{
		return 0 < gameObject.transform.childCount;
	}
}

/// Transform 型の拡張メソッドを管理するクラス
public static partial class TransformExtensions
{
	public static bool HasChild(this Transform transform)
	{
		return 0 < transform.childCount;
	}
}