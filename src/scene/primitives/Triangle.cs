using System;
using System.Collections;
using System.Collections.Generic;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a triangle in a scene represented by three vertices.
    /// </summary>
    public class Triangle : SceneEntity
    {
        private Vector3 v0, v1, v2;
        private Material material;

        /// <summary>
        /// Construct a triangle object given three vertices.
        /// </summary>
        /// <param name="v0">First vertex position</param>
        /// <param name="v1">Second vertex position</param>
        /// <param name="v2">Third vertex position</param>
        /// <param name="material">Material assigned to the triangle</param>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Material material)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the triangle, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            double hitTime;
            Vector3 intersect;

            Vector3 normal = ((v1-v0).Cross(v2-v0)).Normalized();

            // Ray collision occured
            if (normal.Dot(ray.Direction) != 0) {
                
                // Hit time as intersection = origin + hitTime*direction
                hitTime = ((v0 - ray.Origin).Dot(normal))/(ray.Direction.Dot(normal));

                // if hitTime < 0 intersection occurs behind camera
                if (hitTime <= 0) {
                    return null;
                }

                // Calculate intersection
                intersect = ray.Origin + hitTime * ray.Direction;

                // Calculate if intersect is within triangle
                if(insideTriangle(intersect, normal)) {
                    // Return RayHit
                    return new RayHit(intersect, normal, ray.Direction, hitTime, material);
                }

            }
            return null;
        }


        private bool insideTriangle(Vector3 intersect, Vector3 normal){

            // Using barycentric coordinate system, with example found on gamedev.stackexchange

            double areaABC = normal.Dot((v1-v0).Cross(v2-v0));
            double areaPBC = normal.Dot((v1-intersect).Cross(v2-intersect));
            double areaPCA = normal.Dot((v2-intersect).Cross(v0-intersect));

            double x = areaPBC / areaABC;
            double y = areaPCA / areaABC;
            double z = 1.0 - x - y;

            if ((-0.0001 <= x && x <= 1.0001) && (-0.0001 <= y && y <= 1.0001) && (-0.0001 <= z && z <= 1.0001)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The material of the triangle.
        /// </summary>
        public Material Material { get { return this.material; } }

        public Vector3 getMaxValues() {
            List<Vector3> vertices = new List<Vector3>();
            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);

            double X = double.NegativeInfinity;
            double Y = double.NegativeInfinity;
            double Z = double.NegativeInfinity;

            foreach(Vector3 vertex in vertices) {
                if (vertex.X > X) {
                    X = vertex.X;
                }
                if (vertex.Y > Y) {
                    Y = vertex.Y;
                }
                if (vertex.Z > Z) {
                    Z = vertex.Z;
                }
            }

            return new Vector3(X, Y, Z);

        }

        public Vector3 getMinValues() {
            List<Vector3> vertices = new List<Vector3>();
            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);

            double X = double.PositiveInfinity;
            double Y = double.PositiveInfinity;
            double Z = double.PositiveInfinity;

            foreach(Vector3 vertex in vertices) {
                if (vertex.X < X) {
                    X = vertex.X;
                }
                if (vertex.Y < Y) {
                    Y = vertex.Y;
                }
                if (vertex.Z < Z) {
                    Z = vertex.Z;
                }
            }

            return new Vector3(X, Y, Z);

        }
    }

}
