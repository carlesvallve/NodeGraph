using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Collections;


 
//namespace DepthFirst
//{
class Program {
	public class Person {
		public Person(string name) {
			this.name = name;

			int d = 350;
			this.X = UnityEngine.Random.Range(-d, d);
			this.Y = UnityEngine.Random.Range(-d, d);
			this.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
		}

		public string name { get; set; }
		public List<Person> Friends {
			get {
				return FriendsList;
			}
		}

		public void isFriendOf(Person p) {
			FriendsList.Add(p);
		}

		List<Person> FriendsList = new List<Person>();

		public override string ToString() {
			return name;
		}


		public int X { get; set; }
		public int Y { get; set; }
		public Color color { get; set; }
	}


	public class DepthFirstAlgorithm {
		
		public Person BuildFriendGraph() {
			Person Aaron = new Person("Aaron");
			Person Betty = new Person("Betty");
			Person Brian = new Person("Brian");
			Aaron.isFriendOf(Betty);
			Aaron.isFriendOf(Brian);

			Person Catherine = new Person("Catherine");
			Person Carson = new Person("Carson");
			Person Darian = new Person("Darian");
			Person Derek = new Person("Derek");
			Betty.isFriendOf(Catherine);
			Betty.isFriendOf(Darian);
			Brian.isFriendOf(Carson);
			Brian.isFriendOf(Derek);

			return Aaron;
		}

		public Person Search(Person root, string nameToSearchFor) {
			Debug.Log (">>> " + nameToSearchFor + " -> " + root.name + " " + (nameToSearchFor == root.name));
			if (nameToSearchFor == root.name)
				return root;

			Person personFound = null;
			for (int i = 0; i < root.Friends.Count; i++) {
				personFound = Search(root.Friends[i], nameToSearchFor);
				if (personFound != null)
					break;
			}
			return personFound;
		}

		public void Traverse(Person root, GameObject personPrefab, GameObject pathPrefab) {
			Debug.Log(root.name);

			if (personPrefab != null) {
				Render(root, personPrefab, pathPrefab);
			}

			for (int i = 0; i < root.Friends.Count; i++) {
				Traverse(root.Friends[i], personPrefab, pathPrefab);
			}
		}


		private void Render (Person root, GameObject personPrefab, GameObject pathPrefab) {
			// instantiate person prefab
			GameObject go = (GameObject)GameObject.Instantiate(personPrefab);
			go.name = root.name;
			go.transform.localPosition = new Vector3(root.X, root.Y, 0);

			// set person text params
			Transform t = go.transform.Find("Text");
			if (t != null) {
				t.GetComponent<MeshRenderer>().sortingOrder = 10;
				TextMesh text = t.GetComponent<TextMesh>();
				//text.color = root.color;
				text.text = root.name;
			}

			// draw lines towards friends
			for (int i = 0; i < root.Friends.Count; i++) {
				Person friend = root.Friends[i];
				GeneratePathLine(root, friend, go, pathPrefab);
			}
		}


		public void GeneratePathLine (Person root, Person friend, GameObject go, GameObject pathPrefab) {
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
			container.transform.SetParent(go.transform, false);
			container.name = "Path";

			// create and locate line points
			for (int i = 1; i <= maxPoints; i++) {
				GameObject point = (GameObject)GameObject.Instantiate<GameObject>(pathPrefab);
				point.name = "MapPathPoint" + i;
				point.transform.SetParent(container.transform);
				point.transform.localPosition = Vector3.zero;
				point.transform.Translate(vec.normalized * step * i);
				point.transform.Translate(vec.normalized * radius);
				point.gameObject.SetActive(true);
			}
		}

	}



	

	public void Main(GameObject personPrefab, GameObject pathPrefab) { // string[] args
		

		DepthFirstAlgorithm b = new DepthFirstAlgorithm();
		Person root = b.BuildFriendGraph();
		
		Debug.Log("Traverse\n------");
		b.Traverse(root, personPrefab, pathPrefab);

		Debug.Log("\nSearch\n------");
		Person p = b.Search(root, "Catherine");
		Debug.Log(p == null ? "Person not found" : p.name);
		//p = b.Search(root, "Alex");
		//Debug.Log(p == null ? "Person not found" : p.name);
	}


	


	
}

//}