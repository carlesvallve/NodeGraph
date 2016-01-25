using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;

public class NodeGraph {

	// initializes nodegraph and fills it with nodes and relationships between them

	public Dictionary<string, Node> BuildNodeGraph() {
		// set nodes
		Dictionary<string, Node> nodes = new Dictionary<string, Node>() {
			{ "Aaron", new Node("Aaron") },
			{ "Betty", new Node("Betty") },
			{ "Brian", new Node("Brian") },
			{ "Catherine", new Node("Catherine") },
			{ "Carson", new Node("Carson") },
			{ "Darian", new Node("Darian") },
			{ "Derek", new Node("Derek") }
		};

		// set node connections
		nodes["Aaron"].SetLink(nodes["Betty"]);
		nodes["Aaron"].SetLink(nodes["Darian"]);
		nodes["Aaron"].SetLink(nodes["Brian"]);
		nodes["Betty"].SetLink(nodes["Catherine"]);
		nodes["Betty"].SetLink(nodes["Darian"]);
		nodes["Brian"].SetLink(nodes["Carson"]);
		nodes["Brian"].SetLink(nodes["Derek"]);

		return nodes;
	}


	// creates node gameobjects and itializes their ui

	public void RenderNodeGraph(GameObject nodePrefab, Dictionary<string, Node> nodes) {
		GameObject container = new GameObject();
		container.transform.SetParent(Scene.instance.transform);
		container.name = "Nodes";

		foreach(KeyValuePair<string, Node> entry in nodes) {
			Node node = entry.Value;

			// instantiate node prefab and initialize his ui
			GameObject go = (GameObject)GameObject.Instantiate(nodePrefab);
			node.ui = go.GetComponent<NodeUi>();
			node.ui.Init(container, node);

			node.ui.OnClick += () => {
				Scene.instance.ClickOnNode(node);
			};

			node.ui.OnDragging += () => {
				Scene.instance.DragOnNode(node);
			};
		}
	}


	// Recursevily traverses all nodes in the nodegraph
	// Not used for anything at the moment

	public void Traverse(Node root) {
		Debug.Log(root.name);
		
		for (int i = 0; i < root.Links.Count; i++) {
			Traverse(root.Links[i]);
		}
	}

}
