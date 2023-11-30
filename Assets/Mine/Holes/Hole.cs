using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private float radius = 1f;

    public void SetRadius(float r)
    {
        radius = r;

        transform.localScale = new Vector3(radius, radius, 1);
    }
}
