using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField]
    private float radius = 0.3f;
    [SerializeField]
    private GameObject explosionPrefab = null;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Player.instance.onMineDetonation += Detonate;
    }

    private void Detonate()
    {
        CameraController.instance.TriggerShake();
        Player.instance.onMineDetonation -= Detonate;
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponent<Explosion>().Explode(GameManager.instance.RadiusExplosion);
        Destroy(gameObject);
    }
}
