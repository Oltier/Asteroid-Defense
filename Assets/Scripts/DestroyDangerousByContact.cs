using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDangerousByContact : MonoBehaviour {

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Dangerous")) return;
		Destroy(gameObject);
		Destroy(other.gameObject);
	}
}
