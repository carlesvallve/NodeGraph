using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	private Person root;

	private bool mouseDown = false;
	private Vector3 startMousePos;
	private Vector3 startPos;


	public void Init(Person root) {
		this.root = root;

		Transform t = transform.Find("Text");
		if (t != null) {
			t.GetComponent<MeshRenderer>().sortingOrder = 10;
			TextMesh text = t.GetComponent<TextMesh>();
			//text.color = root.color;
			text.text = root.name;
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
			root.X = Mathf.RoundToInt(pos.x);
			root.Y =  Mathf.RoundToInt(pos.y);

			DragAndDrop[] arr = transform.root.GetComponentsInChildren<DragAndDrop>();
			foreach (DragAndDrop ui in arr) {
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

		for (int i = 0; i < root.Friends.Count; i++) {
			Person friend = root.Friends[i];
			GenerateConnection(container, root, friend);
		}
	}


	private void GenerateConnection (GameObject rootContainer, Person root, Person friend) {
		int radius = 25;
		int distanceDots = 25;

		// get line vector
		Vector2 p1 = new Vector3(root.X, root.Y, 0); 
		Vector2 p2 = new Vector3(friend.X, friend.Y, 0);
		Vector2 vec = (p2 - p1);
		vec -= (vec.normalized * (radius * 2));

		// get number of points to render, and at which step
		float length = vec.magnitude;
		int maxPoints = (int)Mathf.Round(length / distanceDots);
		float step = (length) / (maxPoints + 1);

		// create pathPoint container inside the node
		GameObject container = new GameObject();
		container.transform.SetParent(rootContainer.transform, false);
		container.name = "Path";

		// create and locate line points
		for (int i = 1; i <= maxPoints; i++) {
			GameObject point = (GameObject)GameObject.Instantiate<GameObject>(Scene.instance.pathPrefab);
			point.name = "MapPathPoint" + i;
			point.transform.SetParent(container.transform);
			point.transform.localPosition = Vector3.zero;
			point.transform.Translate(vec.normalized * step * i);
			point.transform.Translate(vec.normalized * radius);
			point.gameObject.SetActive(true);
		}
	}
}