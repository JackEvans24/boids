using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] private Flock[] flocks;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var flock in this.flocks)
            flock.CreateBoids();
    }
}
