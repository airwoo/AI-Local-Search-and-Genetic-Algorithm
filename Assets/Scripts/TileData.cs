using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour {

	public int value; //Jump value
	public GameObject[] childrenArr;

	public GameObject rightChild;
	public GameObject leftChild; 
	public GameObject topChild;
	public GameObject botChild;

	public int valFromStart;

	public int i;
	public int j;

	// Use this for initialization
	void Start () {
		childrenArr = new GameObject[4];
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
