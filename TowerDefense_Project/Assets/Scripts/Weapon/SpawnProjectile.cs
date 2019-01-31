using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{
	public GameObject projectilePrefab;
	public Transform attackSource;

	public void Fire()
	{
		if (projectilePrefab)
			Instantiate(projectilePrefab, attackSource.position, Quaternion.identity);
	}
}
