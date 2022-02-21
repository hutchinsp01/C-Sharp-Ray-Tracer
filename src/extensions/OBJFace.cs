namespace RayTracer
{
    public class OBJFace
    {
        public Vertice v0;
        public Vertice v1;
        public Vertice v2;
        Triangle face;

        public OBJFace(Vector3 v0, Vector3 v0N, Vector3 v1, Vector3 v1N, Vector3 v2, Vector3 v2N, Vector3 offset, double scale, Material material) {
            this.v0 = new Vertice(v0, v0N, offset, scale);
            this.v1 = new Vertice(v1, v1N, offset, scale);
            this.v2 = new Vertice(v2, v2N, offset, scale);
            face = new Triangle(this.v0.getVertex(), this.v1.getVertex(), this.v2.getVertex(), material);
        }

        public Triangle GetTriangle() {
            return face;
        }

        public string toString() {
            return "Vec " + v0.getVertex().ToString() + v1.getVertex().ToString() + v2.getVertex().ToString() + "Norm " + v0.getNormal().ToString() + v1.getNormal().ToString() + v2.getNormal().ToString(); 
        }

    }
}