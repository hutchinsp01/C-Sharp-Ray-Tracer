using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent ray hit data, including the position and
    /// normal of a hit (and optionally other computed vectors).
    /// </summary>
    public class RayHit
    {
        private Vector3 position;
        private Vector3 normal;
        private Vector3 incident;
        private double hitTime;
        private Material material;

        public RayHit(Vector3 position, Vector3 normal, Vector3 incident, double hitTime, Material material)
        {
            this.position = position;
            this.normal = normal;
            this.incident = incident;
            this.hitTime = hitTime;
            this.material = material;
        }

        // You may wish to write methods to compute other vectors, 
        // e.g. reflection, transmission, etc

        public Vector3 Position { get { return this.position; } }

        public Vector3 Normal { get { return this.normal; } }

        public Vector3 Incident { get { return this.incident; } }

        public double HitTime { get { return this.hitTime; } }

        public Material Material { get { return this.material; } }
    }
}
