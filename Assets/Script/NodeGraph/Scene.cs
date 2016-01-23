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

	private Node targetNode;


	void Start () {
		instance = this;

		// initialize background
		Sprite bg  = transform.Find("Background").GetComponent<SpriteRenderer>().sprite;

		// initialize camera
		MapCamera mapCamera = gameObject.AddComponent<MapCamera>();
		mapCamera.Init(Camera.main, bg.bounds);

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

		player.OnArrivedToNode += () => {
			PlayerArrivedToNode();
		};
	}


	public void ClickOnNode (Node node) {
		if (player.moving) {
			targetNode = node;
		} else {
			List<Node> path = graph.SearchPath(player.CurrentNode, node);
			player.FollowPath(path);
		}
		
		//player.FollowPath(path);
		//player.SetNewPath(path);
	}

	public void DragOnNode (Node node) {
		player.SetPosition(player.CurrentNode.X, player.CurrentNode.Y);
	}


	public void PlayerArrivedToNode () {
		if (targetNode != null) {
			List<Node> path = graph.SearchPath(player.CurrentNode, targetNode);
			player.FollowPath(path);
			targetNode = null;
		}
	}

}
