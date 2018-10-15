using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{

	public float LifeTime;
	
	// Use this for initialization
	private void Start () {
		Destroy(gameObject, LifeTime);
	}
	
}
