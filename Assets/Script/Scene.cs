using UnityEngine;
using System.Collections;

using DepthFirst;

public class Scene : MonoBehaviour {

	public static Scene instance;

	public GameObject personPrefab;
	public GameObject pathPrefab;


	void Start () {
		instance = this;

		DepthFirstAlgorithm b = new DepthFirstAlgorithm();
		Person root = b.BuildFriendGraph();
		
		Debug.Log("Traverse\n------");
		b.Traverse(root, personPrefab, pathPrefab);

		Debug.Log("\nSearch\n------");
		Person p = b.Search(root, "Catherine");
		Debug.Log(p == null ? "Person not found" : p.name);
	}
	
}
