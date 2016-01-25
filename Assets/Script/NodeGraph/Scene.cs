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

	private Node currentNode;
	private Node targetNode;

	private MapCamera mapCamera;




	void Start () {
		instance = this;

		// initialize background
		Sprite bg  = transform.Find("Background").GetComponent<SpriteRenderer>().sprite;

		// initialize camera
		mapCamera = gameObject.AddComponent<MapCamera>();
		mapCamera.Init(Camera.main, bg.bounds);

		// create node graph
		graph = new NodeGraph();
		Dictionary<string, Node> nodes = graph.BuildNodeGraph();
		
		// render node graph
		graph.RenderNodeGraph(nodePrefab, nodes);

		// create player
		player = Instantiate<GameObject>(playerPrefab).GetComponent<Player>();
		player.gameObject.transform.SetParent(transform, false);
		player.Init(nodes["Aaron"]);

		player.OnArrivedToNode += () => {
			PlayerArrivedToNode();
		};
	}


	// Event Listeners

	public void DragOnNode (Node node) {
		player.SetPosition(player.CurrentNode.X, player.CurrentNode.Y);
	}


	public void ClickOnNode (Node node) {

		// set current node and color it yellow
		if (currentNode != null) { currentNode.ui.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1); }
		node.ui.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = new Color(1, 1, 0);
		currentNode = node;

		if (player.moving) {
			targetNode = node;
		} else {
			Pathfinder pathfinder = new Pathfinder();
			List<Node> path = pathfinder.SearchPath(player.CurrentNode, node);
			float duration = player.FollowPath(path);

			print (node.X + " " + node.Y + " " + node.ui.transform.localPosition + " " + node.ui.transform.position);

			// move camera towards destination point
			mapCamera.InterpolatePosition(node.X, node.Y, duration);
		}
	}


	public void PlayerArrivedToNode () {
		if (targetNode != null) {
			Pathfinder pathfinder = new Pathfinder();
			List<Node> path = pathfinder.SearchPath(player.CurrentNode, targetNode);
			float duration = player.FollowPath(path);
			targetNode = null;

			// move camera towards destination point
			mapCamera.InterpolatePosition(targetNode.X, targetNode.Y, duration);
		}
	}

}
