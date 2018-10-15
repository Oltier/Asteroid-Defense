using UnityEngine;

public class DestroyDangerousByContact : MonoBehaviour
{

	public GameObject Explosion;
	
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Dangerous")) return;
		Destroy(gameObject);
		Destroy(other.gameObject);
		Instantiate(Explosion, other.transform.position, other.transform.rotation);
	}
}
