using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEarthByContact : MonoBehaviour
{

	public GameObject Explosion;
	
	private int _collisionCounter;

	private void Start()
	{
		_collisionCounter = 0;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		_collisionCounter++;
		if (_collisionCounter < 2) return;
		Destroy(gameObject);
		Destroy(other.gameObject);
		Instantiate(Explosion, other.transform.position, other.transform.rotation);
	}

	private void OnTriggerExit(Collider other)
	{
		if (_collisionCounter > 0) _collisionCounter--;
	}
}
