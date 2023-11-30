using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    #pragma warning disable 0414

    [SerializeField]
    private float onScreenTime = 50f;
    [SerializeField]
    private GameObject holePrefab = null;
    [SerializeField]
    private Transform holeParent = null;


    public void Explode(float radius)
    {
        transform.localScale = new Vector3(radius, radius, 1);
        StartCoroutine(ExplodeCoroutine(radius));
        AudioManager.instance.Play(SoundClips.Explosion);
    }

    private IEnumerator ExplodeCoroutine(float radius)
    {
        yield return new WaitForSeconds(0.1f);

        float holeRadius = GameManager.instance.RadiusHole;
        if(holeRadius > 0)
        {
            GameObject hole = Instantiate(holePrefab, transform.position, Quaternion.identity, GameManager.instance.HolesParent);
            hole.GetComponent<Hole>().SetRadius(holeRadius);
        }
        
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy"))
        {
            Swarmer enemy = other.GetComponent<Swarmer>();
            enemy.Die();
        }
    }
}
