using UnityEngine;

[CreateAssetMenu(menuName = "Flock")]
public class Flock : ScriptableObject
{
    [Header("Instantiation")]
    [SerializeField] private GameObject boidPrefab;
    [SerializeField] private int boidCount;
    [SerializeField] private Vector3 instantiateCenter;
    [SerializeField] private Vector3 instantiateLimits = new Vector3(5, 5, 5);

    [Header("Movement")]
    [SerializeField] private Vector2 speedLimits = new Vector2(0f, 5f);
    [SerializeField, Min(0.1f)] private float maxNeighbourDistance = 2f;
    [SerializeField, Min(0.1f)] private float maxAvoidanceDistance = 0.5f;
    [SerializeField, Min(0.1f)] private float rotationSpeed = 2f;
    [SerializeField] private bool hasTarget = false;
    [SerializeField] private Vector3 target = Vector3.zero;

    [Header("Collision")]
    [SerializeField] private float sphereCastRadius = 0.25f;
    [SerializeField] private float collisionAvoidDistance = 5f;
    [SerializeField] private float avoidCollisionWeight = 10f;
    [SerializeField] private LayerMask collisionMask;

    private Boid[] allBoids;

    private void Awake()
    {
        if (this.instantiateCenter == null)
            this.instantiateCenter = Vector3.zero;
    }

    public void CreateBoids()
    {
        this.allBoids = new Boid[this.boidCount];

        for (int i = 0; i < this.boidCount; i++)
        {
            var pos = this.instantiateCenter + new Vector3(
                Random.Range(-this.instantiateLimits.x, this.instantiateLimits.x),
                Random.Range(-this.instantiateLimits.y, this.instantiateLimits.y),
                Random.Range(-this.instantiateLimits.z, this.instantiateLimits.z)
            );

            var boid = Instantiate(this.boidPrefab, pos, Quaternion.identity).GetComponent<Boid>();
            if (boid == null)
                throw new System.Exception("Boid prefab does not contain the Boid.cs script");

            this.allBoids[i] = boid;

            boid.Flock = this;
            boid.Speed = Random.Range(this.speedLimits.x, this.speedLimits.y);
            boid.RotationSpeed = this.rotationSpeed;
        }
    }

    public (Vector3, float) GetTargetDirectionAndSpeed(Boid currentBoid)
    {
        var aggregateCenter = currentBoid.transform.position;
        var aggregateHeading = currentBoid.transform.forward;
        var avoidance = Vector3.zero;
        var aggregateSpeed = currentBoid.Speed;
        var groupSize = 1;

        foreach (var boid in this.allBoids)
        {
            if (boid == currentBoid)
                continue;

            var neighbourDistance = Vector3.Distance(boid.transform.position, currentBoid.transform.position);
            if (neighbourDistance > this.maxNeighbourDistance)
                continue;

            aggregateCenter += boid.transform.position;
            aggregateHeading += boid.transform.forward;
            groupSize++;

            if (neighbourDistance <= this.maxAvoidanceDistance)
                avoidance += currentBoid.transform.position - boid.transform.position;

            aggregateSpeed += boid.Speed;
        }

        var groupCenter = aggregateCenter / groupSize;
        var averageHeading = aggregateHeading / groupSize;
        var averageSpeed = aggregateSpeed / groupSize;

        var collisionForce = Vector3.zero;
        if (this.IsHeadingForCollision(currentBoid))
        {
            var collisionAvoidDir = ObstacleRays(currentBoid);
            collisionForce = collisionAvoidDir * this.avoidCollisionWeight;
        }

        if (this.hasTarget)
            groupCenter += this.target - currentBoid.transform.position;

        return (groupCenter + avoidance + averageHeading + collisionForce - currentBoid.transform.position, averageSpeed);
    }

    private bool IsHeadingForCollision(Boid currentBoid)
    {
        var ray = new Ray(currentBoid.transform.position, currentBoid.transform.forward);
        return Physics.SphereCast(ray, this.sphereCastRadius, this.collisionAvoidDistance, this.collisionMask);
    }

    private Vector3 ObstacleRays(Boid currentBoid)
    {
        var rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            var rayDirection = currentBoid.transform.TransformDirection(rayDirections[i]);
            var ray = new Ray(currentBoid.transform.position, rayDirection);

            if (!Physics.SphereCast(ray, this.sphereCastRadius, this.collisionAvoidDistance, this.collisionMask))
                return rayDirection;
        }

        return currentBoid.transform.forward;
    }
}
