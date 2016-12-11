using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour {
	private Vector3 screenPoint;
	private Vector3 offset;

	private Vector3 originalScale = new Vector3(1f, 1f, 1f);
	private Vector3 lastSetScale = new Vector3 (1f, 1f, 1f);

	void OnMouseDown(){
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

		StopAllCoroutines ();
		StartCoroutine (ScaleDownLerp ());
	}

	IEnumerator ScaleDownLerp(){
		float t = 0.0f;

		while(t < 1){
			transform.localScale = Vector3.Lerp(originalScale, new Vector3 (0.5f, 0.5f, 0.5f), t);
			t += (Time.deltaTime * 3);
			lastSetScale = transform.localScale;
			yield return null;
		}
	}

	void OnMouseDrag(){
		Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
		transform.position = Vector3.Lerp(transform.position, cursorPosition, Time.deltaTime * 10);
	}

	void OnMouseUp(){
		StopAllCoroutines ();
		StartCoroutine (ScaleUpLerp ());
	}

	IEnumerator ScaleUpLerp(){
		float t = 0.0f;

		while(t < 1){
			transform.localScale = Vector3.Lerp(lastSetScale, originalScale, t);
			t += (Time.deltaTime * 3);
			yield return null;
		}
	}
}
