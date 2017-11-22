﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour {

	Vector3 oriPositon;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown()
	{
		//自分の駒でないなら終わり
		if (GameController.underPlayerTurn != underPlayerAnimal (this.gameObject))
			return;
		
		Debug.Log ("OnMouseDown");
		oriPositon = this.transform.position;
	}

	void OnMouseDrag()
	{
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

				if (tile.name == this.transform.parent.name) {
					Debug.Log ("同じタイルを選択");
					this.transform.position = oriPositon;
					return;
				}

//				foreach (GameObject animal in tile) {
//
				if(GameObjectExtensions.HasChild(tile)){
				GameObject animal = tile.transform.GetChild(0).gameObject;
//				if(animal!=null){

//					Debug.Log ("animal");
//					Debug.Log ("animal rotation"+animal.transform.localRotation);
					Debug.Log ("animal rotation"+animal.transform.localRotation.eulerAngles);
					Debug.Log ("turn"+GameController.underPlayerTurn);

					//置いた先の動物が置いたプレイヤーのものなら、もとの位置に戻して終了
					if ((animal.transform.localRotation.eulerAngles.y == 180 && GameController.underPlayerTurn) || //先手が先手に
						(animal.transform.localRotation.eulerAngles.y == 0 && !GameController.underPlayerTurn)) {//後手が後手に
						Debug.Log ("own animal");
						this.transform.position = oriPositon;
						return;
					}
				}

				//位置と親を変更
				this.transform.parent = tile.transform;
				this.transform.position = new Vector3(tile.transform.position.x,tile.transform.position.y,-1);

				//ターン変更
				GameController.underPlayerTurn = !GameController.underPlayerTurn;


				return;
			}
		}

		this.transform.position = oriPositon;
	}

	//画面下のプレイヤーの駒
	bool underPlayerAnimal(GameObject animal){
		return (animal.transform.localRotation.eulerAngles.y == 180);
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