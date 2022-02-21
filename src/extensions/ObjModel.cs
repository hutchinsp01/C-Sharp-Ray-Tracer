using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace RayTracer
{
    /// <summary>
    /// Add-on option C. You should implement your solution in this class template.
    /// </summary>
    public class ObjModel : SceneEntity
    {
        private Material material;
        private enum LineType{na, v, vn, f};
        private List<Vector3> verticeCoords;
        private List<Vector3> verticeNormals;

        private List<OBJFace> Faces;

        private List<Triangle> boundingBox;

        /// <summary>
        /// Construct a new OBJ model.
        /// </summary>
        /// <param name="objFilePath">File path of .obj</param>
        /// <param name="offset">Vector each vertex should be offset by</param>
        /// <param name="scale">Uniform scale applied to each vertex</param>
        /// <param name="material">Material applied to the model</param>
        public ObjModel(string objFilePath, Vector3 offset, double scale, Material material)
        {
            this.material = material;
            this.verticeCoords = new List<Vector3>();
            this.verticeNormals = new List<Vector3>();
            this.Faces = new List<OBJFace>();
            
            // Here's some code to get you started reading the file...
            string[] lines = File.ReadAllLines(objFilePath);
            string[] curLine;

            LineType lineType;

            for (int i = 0; i < lines.Length; i++)
            {
                curLine = lines[i].Split(" ");


                // try to read first item in cur line as one of our lineTypes
                try {
                    lineType = (LineType)Enum.Parse(typeof(LineType), curLine[0]);
                }
                catch (SystemException) {
                    lineType = LineType.na;
                }

                switch (lineType) {
                    case LineType.v:
                    {
                        verticeCoords.Add(new Vector3(
                                        double.Parse(curLine[1]),
                                        double.Parse(curLine[2]),
                                        double.Parse(curLine[3])
                                        ));
                        break;
                    }
                    case LineType.vn:
                    {
                        verticeNormals.Add(new Vector3(
                                        double.Parse(curLine[1]),
                                        double.Parse(curLine[2]),
                                        double.Parse(curLine[3])
                                        ));
                        break;
                    }
                    case LineType.f:
                    {
                        Faces.Add(new OBJFace(
                            verticeCoords[int.Parse(curLine[1].Split("//")[0]) - 1],
                            verticeNormals[int.Parse(curLine[1].Split("//")[1]) - 1],
                            verticeCoords[int.Parse(curLine[2].Split("//")[0]) - 1],
                            verticeNormals[int.Parse(curLine[2].Split("//")[1]) - 1],
                            verticeCoords[int.Parse(curLine[3].Split("//")[0]) - 1],
                            verticeNormals[int.Parse(curLine[3].Split("//")[1]) - 1],
                            offset, scale, material
                        ));
                        break;
                    }

                }

            }

            boundingBox = BoundingBox();
        }

        /// <summary>
        /// Given a ray, determine whether the ray hits the object
        /// and if so, return relevant hit data (otherwise null).
        /// </summary>
        /// <param name="ray">Ray data</param>
        /// <returns>Ray hit data, or null if no hit</returns>
        public RayHit Intersect(Ray ray)
        {
            bool boundingBoxCollision = boxCollision(ray);
            RayHit closestHit = null;
            OBJFace hitFace = null;

            if (boundingBoxCollision) {

                double closestHitTime = double.PositiveInfinity;
                RayHit curHit = null;
                foreach (OBJFace face in Faces) {
                    curHit = face.GetTriangle().Intersect(ray);

                    if (curHit != null) {
                        if (curHit.HitTime < closestHitTime) {
                            closestHitTime = curHit.HitTime;
                            closestHit = curHit;
                            hitFace = face;
                        }
                    }
                }
            }

            if (closestHit == null) {
                return closestHit;
            }

            //Console.WriteLine(hitFace.toString());

            return getVertexNormalHit(hitFace, closestHit);
            
        }

        public RayHit getVertexNormalHit(OBJFace face, RayHit hit) {
            Vector3 v0 = face.v0.getVertex();
            Vector3 v1 = face.v1.getVertex();
            Vector3 v2 = face.v2.getVertex();

            double areaABC = hit.Normal.Dot((v1-v0).Cross(v2-v0));
            double areaPBC = hit.Normal.Dot((v1-hit.Position).Cross(v2-hit.Position));
            double areaPCA = hit.Normal.Dot((v2-hit.Position).Cross(v0-hit.Position));

            double x = areaPBC / areaABC;
            double y = areaPCA / areaABC;

            Vector3 newNormal = (1-x-y) * face.v0.getNormal() + x * face.v1.getNormal() + y * face.v2.getNormal();

            return new RayHit(hit.Position, newNormal, hit.Incident, hit.HitTime, hit.Material);
        }

        public RayHit triangleIntersect(OBJFace face, Ray ray) {
            double hitTime;
            Vector3 intersect;
            Vector3 v0 = face.v0.getVertex();
            Vector3 v1 = face.v1.getVertex();
            Vector3 v2 = face.v2.getVertex();

            Vector3 normal = ((v1-v0).Cross(v2-v0)).Normalized(); 

            // Ray collision occured
            if (normal.Dot(ray.Direction) != 0) {
                
                // Hit time as intersection = origin + hitTime*direction
                hitTime = ((face.v0.getVertex() - ray.Origin).Dot(normal))/(ray.Direction.Dot(normal));

                // if hitTime < 0 intersection occurs behind camera
                if (hitTime <= 0) {
                    return null;
                }

                // Calculate intersection
                intersect = ray.Origin + hitTime * ray.Direction;

                // Calculate if intersect is within triangle
                if(insideTriangle(v0, v1, v2, intersect, normal)) {
                    return new RayHit(intersect, normal, ray.Direction, hitTime, material);
                }

            }
            return null;
        }
        
        private bool insideTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 intersect, Vector3 normal){

            // Using barycentric coordinate system, with example found on gamedev.stackexchange

            double areaABC = normal.Dot((v1-v0).Cross(v2-v0));
            double areaPBC = normal.Dot((v1-intersect).Cross(v2-intersect));
            double areaPCA = normal.Dot((v2-intersect).Cross(v0-intersect));

            double x = areaPBC / areaABC;
            double y = areaPCA / areaABC;
            double z = 1.0 - x - y;

            if ((0 <= x && x <= 1) && (0 <= y && y <= 1) && (0 <= z && z <= 1)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The material attached to this object.
        /// </summary>
        public Material Material { get { return this.material; } }

        public List<Triangle> BoundingBox() {
            double maxX = double.NegativeInfinity;
            double maxY = double.NegativeInfinity;
            double maxZ = double.NegativeInfinity;
            double minX = double.PositiveInfinity;
            double minY = double.PositiveInfinity;
            double minZ = double.PositiveInfinity;
            double curX, curY, curZ;

            Material type = new Material(Material.MaterialType.NA);
            List<Triangle> boundingBox = new List<Triangle>();


            foreach (OBJFace face in Faces) {
                curX = face.GetTriangle().getMinValues().X;
                curY = face.GetTriangle().getMinValues().Y;
                curZ = face.GetTriangle().getMinValues().Z;

                if (curX < minX) {
                    minX = curX;
                }
                if (curX > maxX) {
                    maxX = curX;
                }
                if (curY < minY) {
                    minY = curY;
                }
                if (curY > maxY) {
                    maxY = curY;
                }
                if (curZ < minZ) {
                    minZ = curZ;
                }
                if (curZ > maxZ) {
                    maxZ = curZ;
                }
            }

            // Front face
            boundingBox.Add(new Triangle(new Vector3(minX, minY, maxZ), new Vector3(maxX, maxY, maxZ), new Vector3(maxX, minY, maxZ), type));
            boundingBox.Add(new Triangle(new Vector3(minX, minY, maxZ), new Vector3(minX, maxY, maxZ), new Vector3(maxX, maxY, maxZ), type));

            // Back face
            boundingBox.Add(new Triangle(new Vector3(maxX, minY, minZ), new Vector3(minX, maxY, minZ), new Vector3(minX, minY, minZ), type));
            boundingBox.Add(new Triangle(new Vector3(maxX, minY, minZ), new Vector3(maxX, maxY, minZ), new Vector3(minX, maxY, minZ), type));

            // Left face
            boundingBox.Add(new Triangle(new Vector3(minX, minY, maxZ), new Vector3(minX, maxY, minZ), new Vector3(minX, minY, minZ), type));
            boundingBox.Add(new Triangle(new Vector3(minX, minY, maxZ), new Vector3(minX, maxY, maxZ), new Vector3(minX, maxY, minZ), type));

            // Right face
            boundingBox.Add(new Triangle(new Vector3(maxX, minY, minZ), new Vector3(maxX, maxY, maxZ), new Vector3(maxX, minY, maxZ), type));
            boundingBox.Add(new Triangle(new Vector3(maxX, minY, minZ), new Vector3(maxX, maxY, minZ), new Vector3(maxX, maxY, maxZ), type));

            // Top face
            boundingBox.Add(new Triangle(new Vector3(minX, maxY, minZ), new Vector3(maxX, maxY, maxZ), new Vector3(maxX, maxY, minZ), type));
            boundingBox.Add(new Triangle(new Vector3(minX, maxY, minZ), new Vector3(minX, maxY, maxZ), new Vector3(maxX, maxY, maxZ), type));

            // Bottom face
            boundingBox.Add(new Triangle(new Vector3(minX, minY, maxZ), new Vector3(maxX, minY, minZ), new Vector3(maxX, minY, maxZ), type));
            boundingBox.Add(new Triangle(new Vector3(minX, minY, maxZ), new Vector3(minX, minY, minZ), new Vector3(maxX, minY, minZ), type));


            return boundingBox;
        }

        private bool boxCollision(Ray ray) {
            
            foreach(Triangle triangle in boundingBox) {
                if (triangle.Intersect(ray) != null) {
                    return true;
                }
            }

            return false;

        }
    }

    

}
