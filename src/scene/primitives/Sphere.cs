using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent an (infinite) plane in a scene.
    /// </summary>
    public class Sphere : SceneEntity
    {
        private Vector3 center;
        private double radius;
        private Material material;

        /// <summary>
        /// Construct a sphere given its center point and a radius.
        /// </summary>
        /// <param name="center">Center of the sphere</param>
        /// <param name="radius">Radius of the spher</param>
        /// <param name="material">Material assigned to the sphere</param>
        public Sphere(Vector3 center, double radius, Material material)
        {
            this.center = center;
            this.radius = radius;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the sphere, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            double hitTime;
            

            // Adapted from viclw17.github.io
            Vector3 oc = (ray.Origin - center);
            double a = ray.Direction.Dot(ray.Direction);
            double b = 2.0 * oc.Dot(ray.Direction);
            double c = oc.Dot(oc) - radius*radius;
            double discrim = b*b - 4*a*c;

            if(discrim < 0.0){
                return null;
            }
            else{

                if (-b - Math.Sqrt(discrim) > 0.0) {
                    hitTime = (-b - Math.Sqrt(discrim)) / (2.0 * a);
                }

                else if (-b + Math.Sqrt(discrim) > 0.0) {
                    hitTime = (-b + Math.Sqrt(discrim)) / (2.0 * a);
                }

                else {
                    return null;
                }
            }


            Vector3 hitPos = ray.Origin + hitTime * ray.Direction;
            Vector3 normal = (hitPos - center).Normalized();

            return new RayHit(hitPos, normal, ray.Direction, hitTime, material);
        }

        /// <summary>
        /// The material of the sphere.
        /// </summary>
        public Material Material { get { return this.material; } }
    }

}
