using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace RayTracer
{
    public class Vertice
    {
        Vector3 vertice;
        Vector3 verticeNormal;

        public Vertice(Vector3 vertice, Vector3 verticeNormal, Vector3 offset, double scale) {
            this.vertice = vertice*scale + offset;
            this.verticeNormal = verticeNormal;
        }

        public Vector3 getVertex() {
            return vertice;
        }

        public Vector3 getNormal() {
            return verticeNormal;
        }
    }

    
}