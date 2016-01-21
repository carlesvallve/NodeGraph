using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class NodeUi : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	private Node node;

	private bool mouseDown = false;
	private Vector3 startMousePos;
	private Vector3 startPos;


	public void Init(Node node) {
		this.node = node;

		name = node.name;
		transform.SetParent(Scene.instance.transform);
		transform.localPosition = new Vector3(node.X, node.Y, 0);

		Transform t = transform.Find("Text");
		if (t != null) {
			t.GetComponent<MeshRenderer>().sortingOrder = 10;
			TextMesh text = t.GetComponent<TextMesh>();
			//text.color = node.color;
			text.text = node.name;
		}

		RenderConnections();
	}


	public void OnPointerDown(PointerEventData ped) {
		mouseDown = true;
		startPos = transform.position;
		startMousePos = Input.mousePosition;
	}


	public void OnPointerUp(PointerEventData ped) {
		mouseDown = false;
	}


	void Update () {
		if (mouseDown) {
			Vector3 currentPos = Input.mousePosition;
			Vector3 diff = currentPos - startMousePos;
			Vector3 pos = startPos + diff;
			transform.position = pos;
			node.X = Mathf.RoundToInt(pos.x);
			node.Y =  Mathf.RoundToInt(pos.y);

			NodeUi[] arr = transform.root.GetComponentsInChildren<NodeUi>();
			foreach (NodeUi ui in arr) {
				ui.RenderConnections();
			}
		}
	}


	public void RenderConnections () {
		// draw lines towards friends
		Transform tr = transform.Find("Paths");
		if (tr != null) { 
			GameObject.Destroy(tr.gameObject); 
		}

		GameObject container = new GameObject();
		container.transform.SetParent(transform, false);
		container.name = "Paths";

		for (int i = 0; i < node.Links.Count; i++) {
			Node link = node.Links[i];
			GenerateConnection(container, link);
		}
	}


	private void GenerateConnection (GameObject rootContainer, Node friend) {
		// create path container inside the node
		GameObject container = new GameObject();
		container.transform.SetParent(rootContainer.transform, false);
		container.name = "Path";

		// set dot line parameters
		int dotRadius = 25;
		int distanceBetweenDots = 25;

		// get line vector
		Vector2 p1 = new Vector3(node.X, node.Y, 0); 
		Vector2 p2 = new Vector3(friend.X, friend.Y, 0);
		Vector2 vec = (p2 - p1);
		vec -= (vec.normalized * (dotRadius * 2));

		// get number of points to render, and at which step
		float length = vec.magnitude;
		int maxPoints = (int)Mathf.Round(length / distanceBetweenDots);
		float step = (length) / (maxPoints + 1);

		// create and locate line points
		for (int i = 1; i <= maxPoints; i++) {
			GameObject point = (GameObject)GameObject.Instantiate<GameObject>(Scene.instance.pathPrefab);
			point.name = "Point" + i;
			point.transform.SetParent(container.transform);
			point.transform.localPosition = Vector3.zero;
			point.transform.Translate(vec.normalized * step * i);
			point.transform.Translate(vec.normalized * dotRadius);
			point.gameObject.SetActive(true);
		}
	}

}
