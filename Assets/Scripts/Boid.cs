using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [HideInInspector] public Flock Flock;
    [HideInInspector] public float Speed;
    [HideInInspector] public float RotationSpeed;

    [SerializeField] private float speedSmoothing;

    private float currentSpeedSmoothVelocity;

    private void Update()
    {
        this.ApplyRules();
        transform.Translate(0, 0, this.Speed * Time.deltaTime);
    }

    private void ApplyRules()
    {
        var (direction, averageSpeed) = this.Flock.GetTargetDirectionAndSpeed(this);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), this.RotationSpeed * Time.deltaTime);

        this.Speed = Mathf.SmoothDamp(this.Speed, averageSpeed, ref this.currentSpeedSmoothVelocity, this.speedSmoothing);
    }
}
