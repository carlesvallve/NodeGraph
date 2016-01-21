using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Scene : MonoBehaviour {

	public static Scene instance;

	public GameObject nodePrefab;
	public GameObject pathPrefab;


	void Start () {
		instance = this;

		// initiaize map camera
		MapCamera mapCamera = GetComponent<MapCamera>();
		mapCamera.Init(
			Camera.main, 
			new Bounds(Vector3.zero, new Vector3(1000, 1000, 0))
		);

		// create node graph
		NodeGraph b = new NodeGraph();
		Dictionary<string, Node> nodes = b.BuildNodeGraph();
		
		// render node graph
		b.RenderNodeGraph(nodePrefab, nodes);

		// search path from node to node
		List<Node> path = b.SearchPath(nodes["Darian"], nodes["Derek"]);
	}
	
}
