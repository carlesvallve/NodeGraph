using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Scene : MonoBehaviour {

	public static Scene instance;

	public GameObject playerPrefab;
	public GameObject nodePrefab;
	public GameObject pathPrefab;

	private NodeGraph graph;
	private Player player;


	void Start () {
		instance = this;

		// initialize background
		Sprite bg  = transform.Find("Background").GetComponent<SpriteRenderer>().sprite;

		// initialize map camera
		MapCamera mapCamera = gameObject.AddComponent<MapCamera>();
		mapCamera.Init(
			Camera.main, 
			bg.bounds
			//new Bounds(Vector3.zero, new Vector3(1000, 1000, 0))
		);

		// create node graph
		graph = new NodeGraph();
		Dictionary<string, Node> nodes = graph.BuildNodeGraph();
		
		// render node graph
		graph.RenderNodeGraph(nodePrefab, nodes);

		// search path from node to node
		List<Node> path = graph.SearchPath(nodes["Darian"], nodes["Derek"]);

		// create player
		player = Instantiate<GameObject>(playerPrefab).GetComponent<Player>();
		player.gameObject.transform.SetParent(transform, false);
		player.Init(nodes["Aaron"]);
	}


	public void ClickOnNode (Node node) {
		print ("Clicked on node " + node);

		List<Node> path = graph.SearchPath(player.CurrentNode, node);
		player.FollowPath(path);

		//player.currentNode = node;
		//player.SetPosition(node.X, node.Y);


	}


	/*private void SetEventListeners () {
		
	}
	

	private void OnClick (NodeUi nodeUi) {
		print ("Clicked on node " + nodeUi);
	}*/
}
