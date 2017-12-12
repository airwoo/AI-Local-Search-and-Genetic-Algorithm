using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics; //StopWatch

public class GUI_Controller : MonoBehaviour {

	public int[] dataPoints;

	public int n;
	public GameObject Tile;
	private GameObject[,] TileArr;

	private int[,] PrevJumpVal; 
	private int[,] PrevValFromStart;
	private int[,] BestJumpVal; 
	private int[,] BestValFromStart;

	private List<GameObject> currentQueue;
	private List<GameObject> visitedQueue;

	//Task 3
	public int iterations;
	public int restarts;
	private int[] deltaEval;

	private int evaluationValue;
	private Stopwatch timer;

	//Task 5
	public int t5_probability;
	public int bestNum = 0;

	//Task 6
	public float T0; 
	public float T;
	public float decay;

	//Task 7
	public GameObject[] Population;
	public int maxPopulation;
	public int populationCount;
	public GameObject[] tilesParent;
	public List<GameObject> matingPool;
	public GameObject[] partnerA;
	public GameObject[] partnerB;
	public GameObject[] partnerChild;
	public List<int> childEvaluations;
	public int populationInterations;

	public TextAsset TextFile;
	private string[] words;
	private int[] txtNumbers;

	public GameObject TilesParent;

	void Start () {
		T = T0;
		decay = 1 - decay;
		if (t5_probability == 0) {t5_probability = 3;}
		if(iterations == 0){iterations = 50;}
		if (n == 0) {n = 5;} //If there is no value set for n, set it to 5.
		TileArr = new GameObject[n,n];
		dataPoints = new int[iterations];

		PrevJumpVal = new int[n, n];
		PrevValFromStart = new int[n, n];
		BestJumpVal = new int[n, n];
		BestValFromStart = new int[n, n];


		currentQueue = new List<GameObject>();
		visitedQueue = new List<GameObject>();
		//Task1 ();
	}


	//--------------------Task 1--------------------
	void Task1(){
		Vector3 tilePos = new Vector3 (-n,0,n);
		int randomValue;
		for(int i = 0; i < n; i++){
			for (int j = 0; j < n; j++) {
				TileArr[i,j] = Instantiate (Tile, tilePos, Quaternion.identity);
				randomValue = GetRandomValue (i,j);
				TileArr [i, j].transform.parent = GameObject.Find ("TilesParent").transform;
				TileArr [i, j].transform.GetChild (0).GetComponent<TextMesh>().text = randomValue.ToString ();
				TileArr [i, j].GetComponent<TileData> ().value = randomValue;
				TileArr [i, j].GetComponent<TileData> ().i = i;
				TileArr [i, j].GetComponent<TileData> ().j = j;

				tilePos = new Vector3 (tilePos.x + 3, tilePos.y, tilePos.z);
			} 
			tilePos = new Vector3 (-n,0,tilePos.z - 2);
		}
		TileArr [n - 1, n - 1].transform.GetChild (0).GetComponent<TextMesh> ().text = "G";
		TileArr [n - 1, n - 1].GetComponent<TileData> ().value = 0;


		//Task2 ();
	}
	int GetRandomValue(int r, int c){
		return Random.Range(1,Mathf.Max ((n - 1) - r, r, (n - 1) - c, c) + 1);
	}
	//--------------------Task 1--------------------






	//--------------------Task 2--------------------
	void Task2(){

		visitedQueue.Clear ();
		currentQueue.Clear ();
		currentQueue.Add(TileArr[0,0]);
		visitedQueue.Add (TileArr [0, 0]);
		TileArr [0, 0].GetComponent<TileData> ().valFromStart = 0;

		GameObject currentNode;
		while (currentQueue.Count != 0) {
			currentNode = currentQueue [0];
			currentNode.GetComponent<TileData> ().rightChild = null;
			currentNode.GetComponent<TileData> ().botChild = null;
			currentNode.GetComponent<TileData> ().topChild = null;
			currentNode.GetComponent<TileData> ().leftChild = null;
			AddNeighborsToQueue (currentNode);
			currentQueue.RemoveAt (0);
		}

		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				if (!visitedQueue.Contains (TileArr [i, j])) {
					TileArr [i, j].GetComponent<TileData> ().valFromStart = -1;
				}
			}
		}
		UpdateGridToEvaluation ();
		//Task3 ();
	}

	void AddNeighborsToQueue(GameObject currentNode){
		int jumpVal = currentNode.GetComponent<TileData> ().value;
		int i = currentNode.GetComponent<TileData>().i;
		int j = currentNode.GetComponent<TileData> ().j;
		if (currentNode.GetComponent<TileData> ().value == 0) {
			return;
		}
		if (j + jumpVal < n && !visitedQueue.Contains(TileArr[i,j+jumpVal])) {//Node Right
			currentQueue.Add(TileArr[i,j+jumpVal]);
			visitedQueue.Add (TileArr [i, j + jumpVal]);
			currentNode.GetComponent<TileData> ().rightChild = TileArr [i, j + jumpVal];
			TileArr [i, j + jumpVal].GetComponent<TileData> ().valFromStart = currentNode.GetComponent<TileData> ().valFromStart + 1;
		}
		if (j - jumpVal >= 0 && !visitedQueue.Contains(TileArr[i,j - jumpVal])) {//Left
			currentQueue.Add(TileArr[i,j-jumpVal]);
			visitedQueue.Add (TileArr [i, j - jumpVal]);

			currentNode.GetComponent<TileData> ().leftChild = TileArr [i, j - jumpVal];
			TileArr [i, j - jumpVal].GetComponent<TileData> ().valFromStart = currentNode.GetComponent<TileData> ().valFromStart + 1;
		}
		if (i + jumpVal < n && !visitedQueue.Contains(TileArr[i+jumpVal , j])) {//Down
			currentQueue.Add(TileArr[i+jumpVal,j]);	
			visitedQueue.Add (TileArr [i+jumpVal, j]);
			currentNode.GetComponent<TileData> ().botChild = TileArr [i + jumpVal, j];
			TileArr [i + jumpVal,j].GetComponent<TileData> ().valFromStart = currentNode.GetComponent<TileData> ().valFromStart + 1;
		}
		if (i - jumpVal >= 0 && !visitedQueue.Contains(TileArr[i-jumpVal,j])) {//Up
			currentQueue.Add(TileArr[i - jumpVal,j]);
			visitedQueue.Add (TileArr [i - jumpVal, j]);
			currentNode.GetComponent<TileData> ().topChild= TileArr [i - jumpVal, j];
			TileArr [i - jumpVal,j].GetComponent<TileData> ().valFromStart = currentNode.GetComponent<TileData> ().valFromStart + 1;
		}
	}

	void UpdateGridToEvaluation(){
		int unreachable = 0;
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				if (TileArr [i, j].GetComponent<TileData> ().valFromStart == -1) {
					TileArr [i, j].transform.GetChild (0).GetComponent<TextMesh> ().text = "x";
					unreachable++;
				} else {
					TileArr [i, j].transform.GetChild (0).GetComponent<TextMesh> ().text = TileArr [i, j].GetComponent<TileData> ().valFromStart.ToString ();
				}
			}
		}
		if (TileArr [n - 1, n - 1].GetComponent<TileData> ().valFromStart == -1) {
			evaluationValue = -unreachable;
		} else {
			evaluationValue = TileArr [n - 1, n - 1].GetComponent<TileData> ().valFromStart;
		}

	}
	//--------------------Task 2--------------------






	//--------------------Task 3--------------------
	void Task3(){
		timer = new Stopwatch ();
		timer.Start ();
		int prevEvalVal = evaluationValue;
		int prevJumpVal;
		SetBestGrid ();

		for (int i = 0; i < iterations; i++) {
			SetPrevGrid ();
			prevEvalVal = evaluationValue;
			int r = Random.Range (0, n);
			int c = Random.Range (0, n); 
			prevJumpVal = TileArr [r, c].GetComponent<TileData> ().value;
			while(prevJumpVal == TileArr[r,c].GetComponent<TileData>().value){ //Make sure the new jump value isnt the same as the previous one.
				TileArr [r, c].GetComponent<TileData> ().value = GetRandomValue (r, c);
			}
			Task2 ();

			if (evaluationValue >= prevEvalVal) {
				SetBestGrid();
				//dataPoints [i] = evaluationValue; 
			} else {
				SetCurrentToPrev ();
				evaluationValue = prevEvalVal;
			}
		}
		timer.Stop();
		print ("Best Evaluation Value: " + evaluationValue);
		print("Task 3 Time: " + timer.Elapsed);
	}

	void Task4(){
		timer = new Stopwatch ();
		timer.Start ();
		int prevEvalVal = evaluationValue;
		int prevJumpVal;
		SetBestGrid ();

		for(int rr = 0; rr < restarts; rr++){
			for (int i = 0; i < iterations; i++) {
				SetPrevGrid ();
				prevEvalVal = evaluationValue;
				int r = Random.Range (0, n);
				int c = Random.Range (0, n); 
				prevJumpVal = TileArr [r, c].GetComponent<TileData> ().value;
				while(prevJumpVal == TileArr[r,c].GetComponent<TileData>().value){ //Make sure the new jump value isnt the same as the previous one.
					TileArr [r, c].GetComponent<TileData> ().value = GetRandomValue (r, c);
				}
				Task2 ();

				if (evaluationValue >= prevEvalVal) {// && evaluationValue > 0 || prevEvalVal < 0
					SetBestGrid();
				} else {
					SetCurrentToPrev ();
					evaluationValue = prevEvalVal;
				}
			}
			for(int i = 0; i < n; i++){
				for (int j = 0; j < n; j++) {

					int randomValue = GetRandomValue (i,j);
					TileArr [i, j].transform.GetChild (0).GetComponent<TextMesh>().text = randomValue.ToString ();
					TileArr [i, j].GetComponent<TileData> ().value = randomValue;
					TileArr [i, j].GetComponent<TileData> ().i = i;
					TileArr [i, j].GetComponent<TileData> ().j = j;
				}

			}
			TileArr [n - 1, n - 1].transform.GetChild (0).GetComponent<TextMesh> ().text = "G";
			TileArr [n - 1, n - 1].GetComponent<TileData> ().value = 0;

			Task3 ();

		}
		timer.Stop();
		print ("Best Evaluation Value from all Hill Climbing Processes: " + evaluationValue);
		print("Task 4 Time: " + timer.Elapsed);
	}


	void SetBestGrid(){
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				BestJumpVal [i, j] = TileArr [i, j].GetComponent<TileData> ().value;
				BestValFromStart [i, j] = TileArr [i, j].GetComponent<TileData> ().valFromStart;
			}
		}
	}
	void SetPrevGrid(){
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				PrevJumpVal [i, j] = TileArr [i, j].GetComponent<TileData> ().value;
				PrevValFromStart [i, j] = TileArr [i, j].GetComponent<TileData>().valFromStart;
			}
		}
		PrevJumpVal [n - 1, n - 1] = 0;

	}
	void SetCurrentToPrev(){
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				TileArr [i, j].GetComponent<TileData>().value = PrevJumpVal [i, j];
				TileArr [i, j].GetComponent<TileData>().valFromStart = PrevValFromStart[i,j];
			}
		}
		TileArr [n - 1, n - 1].GetComponent<TileData> ().value = 0;
	}

	void UpdateGridToBestTileArr(){
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				TileArr [i, j].GetComponent<TileData> ().value = BestJumpVal [i, j];
				TileArr [i, j].GetComponent<TileData> ().valFromStart = BestValFromStart [i, j];
				if (TileArr [i, j].GetComponent<TileData> ().valFromStart == -1) {
					TileArr [i, j].transform.GetChild (0).GetComponent<TextMesh> ().text = "x";
				} else {
					TileArr [i, j].transform.GetChild (0).GetComponent<TextMesh> ().text = TileArr [i, j].GetComponent<TileData> ().valFromStart.ToString ();
				}
			}
		}
	}
	//Update the GUI to Jump Values
	void UpdateGridToJumpValues(){
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				TileArr [i, j].transform.GetChild (0).GetComponent<TextMesh> ().text = TileArr [i, j].GetComponent<TileData> ().value.ToString ();
			}
		}
	}
	//--------------------Task 3--------------------

	//--------------------Task 5--------------------
	void Task5(){

		timer = new Stopwatch ();
		timer.Start ();
		int prevEvalVal = evaluationValue;
		int prevJumpVal;
		SetBestGrid ();

		for (int i = 0; i < iterations; i++) {
			SetPrevGrid ();
			prevEvalVal = evaluationValue;
			int r = Random.Range (0, n);
			int c = Random.Range (0, n); 
			prevJumpVal = TileArr [r, c].GetComponent<TileData> ().value;
			while(prevJumpVal == TileArr[r,c].GetComponent<TileData>().value){ //Make sure the new jump value isnt the same as the previous one.
				TileArr [r, c].GetComponent<TileData> ().value = GetRandomValue (r, c);
			}
			Task2 ();
			if (evaluationValue > bestNum) {
				bestNum = evaluationValue;
			}

			if (evaluationValue >= prevEvalVal) {
				SetBestGrid();
			} else {
				int randomNum = Random.Range (0, 101);
				if (randomNum > t5_probability) { //If the random number is greater than the probability revert it. If its less than we accept it. 
					SetCurrentToPrev ();
					evaluationValue = prevEvalVal;
				}
			}
		}
		timer.Stop();
		print ("Best Evaluation Value: " + bestNum);
		print("Task 5 Time: " + timer.Elapsed);
	}		
	//--------------------Task 5--------------------

	//Variables that were added
	//local: t_6probability
	//Global T
	//Global decay
	//Global T0
	//--------------------Task 6--------------------
	void Task6(){

		timer = new Stopwatch ();
		timer.Start ();
		int prevEvalVal = evaluationValue;
		int prevJumpVal;
		SetBestGrid ();

		for (int i = 0; i < iterations; i++) {
			SetPrevGrid ();
			prevEvalVal = evaluationValue;

			//print ("Iteration: " + i + " " + T);
			int r = Random.Range (0, n);
			int c = Random.Range (0, n); 
			prevJumpVal = TileArr [r, c].GetComponent<TileData> ().value;
			while(prevJumpVal == TileArr[r,c].GetComponent<TileData>().value){ //Make sure the new jump value isnt the same as the previous one.
				TileArr [r, c].GetComponent<TileData> ().value = GetRandomValue (r, c);
			}
			Task2 ();
			if (evaluationValue > bestNum) {
				bestNum = evaluationValue;
			}

			if (evaluationValue >= prevEvalVal) {
				SetBestGrid ();
			} else {
				float t6_probability = Mathf.Exp ((evaluationValue - prevEvalVal) / T);
				t6_probability *= 100;
				//print ("Iteration: " + i + " " + t6_probability);
				float randomNum = Random.Range (0, 101);
				//print ("R " + randomNum);
				if (randomNum > t6_probability) {
					SetCurrentToPrev ();
					evaluationValue = prevEvalVal;
				}
			}
			T *= decay;
		}
		timer.Stop();
		print ("Best Evaluation Value: " + bestNum);
		print("Task 6 Time: " + timer.Elapsed);
	}		
	//--------------------Task 6--------------------

	void Task7 ()
	{
		//timer = new Stopwatch ();
		//timer.Start ();
		//-------------------------------------- Creates p puzzles and each puzzle and its data into an array
		int puzzles = 0;
		int elementTile = 0;

		matingPool = new List<GameObject> ();
		int matingPoolCounter = 0;

		Population = new GameObject[maxPopulation];
		populationCount = 0;




		int prevEvalVal = evaluationValue;
		int prevJumpVal;
		SetBestGrid ();
		for (int p = 0; p < maxPopulation; p++) {
			tilesParent = new GameObject[n * n];
			SetPrevGrid ();
			prevEvalVal = evaluationValue;
			for (int i = 0; i < n; i++) {
				for (int j = 0; j < n; j++) {
					int randomValue = GetRandomValue (i, j);
					TileArr [i, j].GetComponent<TileData> ().value = randomValue;
					TileArr [i, j].GetComponent<TileData> ().i = i;
					TileArr [i, j].GetComponent<TileData> ().j = j;
					Task2 ();
					tilesParent [elementTile] = TileArr [i, j];

					elementTile++;

				}
			}
			TileArr [n - 1, n - 1].transform.GetChild (0).GetComponent<TextMesh> ().text = "G";
			TileArr [n - 1, n - 1].GetComponent<TileData> ().value = 0;
			Population [p] = tilesParent [n * n - 1];
			//print (evaluationValue);


			float fitness = (TileArr [n - 1, n - 1].GetComponent<TileData> ().valFromStart);
			if (fitness < 0) {
				fitness = 0;
			}
			float timesInMatingPool = fitness;

			for (int t = 0; t < timesInMatingPool; t++) {
				//print (Population [l, m, 1]);
				matingPool.Add (Population [p]);


			}
			matingPoolCounter++;
			puzzles++;
			elementTile = 0;
			populationCount++;
			if (evaluationValue >= prevEvalVal) {// && evaluationValue > 0 || prevEvalVal < 0
				SetBestGrid();
			} else {
				SetCurrentToPrev ();
				evaluationValue = prevEvalVal;
			}
		}
		//print (tilesParent.Length);
		//print (populationCount);
		//print (matingPoolCounter);

		//-------------------------------------- 
		partnerA = new GameObject[n * n * n];
		partnerB = new GameObject[n * n * n];
		partnerChild = new GameObject[n * n * n];


		for (int num = 0; num < populationCount; num++) {
			int randomPick1 = Random.Range (0, matingPoolCounter);
			int randomPick2 = Random.Range (0, matingPoolCounter);
			int increment = 0;
			//print (randomPick1);
			//print (randomPick2);
			for (int r = 0; r < n * n; r++) {
				partnerA [r] = tilesParent [r];
				partnerB [r] = tilesParent [r];
				increment++;
			}
			for (int c = 0; c < 20; c++) {
				partnerChild [c] = partnerA [c];
			}
			for (int d = 20; d < n * n; d++) {
				partnerChild [d] = partnerB [d];
			}

		}




		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {

				int randomMutationTile = Random.Range (0, n * n);
				int randomValueReroll = GetRandomValue (i, j);
				float randomMutationChance = 0.9f;
				if (randomMutationChance < Random.Range (0, 1)) {
					partnerChild [randomMutationTile].GetComponent<TileData>().value = randomValueReroll;
				}

			}

		}
		for (int r = 0; r < n * n; r++) {
			//print (partnerChild [r]);
		}
		childEvaluations.Add (partnerChild [n*n-1].GetComponent<TileData>().value );

		//timer.Stop();
		print ("Best Evaluation Value from all the Children: " + evaluationValue);
		//print("Task 7 Time: " + timer.Elapsed);
	}

	//--------------------Read Text File------------

	void ReadTxtFile(){
		int counter = 1;
		Destroy (TilesParent);

		if (TextFile != null)
		{
			// Add each line of the text file to
			// the array using a space and tab
			// as the delimiter

			words = (TextFile.text.Split('\n',' '));


		}

		txtNumbers = new int[words.Length];

		for (int i = 0; i < words.Length; i++) {
			txtNumbers [i] = int.Parse (words [i]);
		}

		n = txtNumbers [0];
		TileArr = new GameObject[n,n];
		TilesParent = new GameObject ("TilesParent");

		Vector3 tilePos = new Vector3 (-n,0,n);
		int randomValue;
		for(int i = 0; i < n; i++){
			for (int j = 0; j < n; j++) {
				TileArr[i,j] = Instantiate (Tile, tilePos, Quaternion.identity);
				randomValue = GetRandomValue (i,j);

				TileArr [i, j].transform.GetChild (0).GetComponent<TextMesh>().text = randomValue.ToString ();
				TileArr [i, j].GetComponent<TileData> ().value = randomValue;
				TileArr [i, j].GetComponent<TileData> ().i = i;
				TileArr [i, j].GetComponent<TileData> ().j = j;

				tilePos = new Vector3 (tilePos.x + 3, tilePos.y, tilePos.z);
			}
			tilePos = new Vector3 (-n,0,tilePos.z - 2);
		}

		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				TileArr [i, j].GetComponent<TileData> ().value = txtNumbers [counter];
				TileArr [i, j].transform.GetChild (0).GetComponent<TextMesh>().text = txtNumbers [counter].ToString();
				counter++;

			}
		}



	}





	//--------------------Read Text File------------

	//Update
	void Update(){
		if (Input.GetKeyDown("space")) {
			UpdateGridToBestTileArr ();
		}
		if (Input.GetKeyDown ("1")) {
			Task1 ();
		}
		if (Input.GetKeyDown ("2")) {
			Task2 ();
		}

		if (Input.GetKeyDown ("3")) {
			Task3 ();
		}

		if (Input.GetKeyDown ("5")) {
			Task5 ();
		}

		if (Input.GetKeyDown ("d")) {
			UpdateGridToJumpValues ();
		}

		if (Input.GetKeyDown ("2")) {
			Task2 ();
		}

		if (Input.GetKeyDown ("4")) {
			Task4 ();
		}

		if (Input.GetKeyDown ("6")) {
			Task6 ();
		}
		if (Input.GetKeyDown ("7")) {
			Task1 ();
			timer = new Stopwatch ();
			timer.Start ();
			for (int i = 0; i < populationInterations; i++) {
				Task7 ();
			}
			timer.Stop();
			print (timer.Elapsed);
		}

	}




}

