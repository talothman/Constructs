using UnityEngine;
using System.Collections;

public class Pole_Controller : MonoBehaviour {
	private MeshRenderer mesh;
	private Transform colliderParent;
	private GameObject grabbedObject;

	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshRenderer> ();
	}
	
	void OnTriggerEnter(Collider other){
		print ("inside pole trigger" + other.gameObject.name);
		if (other.gameObject.name == "Body" || other.gameObject.name == "SideB" || other.gameObject.name == "SideA" || other.gameObject.name == "Head")
			return;

		grabbedObject = other.gameObject;

		mesh.enabled = false;
		colliderParent = GetComponent<Collider>().transform.parent;
		grabbedObject.transform.parent = transform.parent;
		grabbedObject.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		grabbedObject.gameObject.GetComponent<Rigidbody> ().useGravity = false;
	}

	void OnTriggerExit(Collider other){

		if (other.gameObject.name == "Body" || other.gameObject.name == "SideB" || other.gameObject.name == "SideA" || other.gameObject.name == "Head")
			return;

		grabbedObject.gameObject.GetComponent<Rigidbody> ().useGravity = true;
		grabbedObject.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		grabbedObject.transform.parent = null;
		grabbedObject = null;
		mesh.enabled = true;
	}
}
