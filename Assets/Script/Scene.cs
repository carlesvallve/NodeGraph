using UnityEngine;
using System.Collections;

//using DepthFirst;

public class Scene : MonoBehaviour {

	public GameObject personPrefab;
	public GameObject pathPrefab;


	void Start () {
		Program program = new Program();
		program.Main(personPrefab, pathPrefab);
	}
	
}
