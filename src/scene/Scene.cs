using System;
using System.Collections.Generic;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a ray traced scene, including the objects,
    /// light sources, and associated rendering logic.
    /// </summary>
    public class Scene
    {
        private SceneOptions options;
        private ISet<SceneEntity> entities;
        private ISet<PointLight> lights;

        private static int MAXDEPTH = 10;
        private double offset = 0.0000001;

        /// <summary>
        /// Construct a new scene with provided options.
        /// </summary>
        /// <param name="options">Options data</param>
        public Scene(SceneOptions options = new SceneOptions())
        {
            this.options = options;
            this.entities = new HashSet<SceneEntity>();
            this.lights = new HashSet<PointLight>();
        }

        /// <summary>
        /// Add an entity to the scene that should be rendered.
        /// </summary>
        /// <param name="entity">Entity object</param>
        public void AddEntity(SceneEntity entity)
        {
            this.entities.Add(entity);
        }

        /// <summary>
        /// Add a point light to the scene that should be computed.
        /// </summary>
        /// <param name="light">Light structure</param>
        public void AddPointLight(PointLight light)
        {
            this.lights.Add(light);
        }

        /// <summary>
        /// Render the scene to an output image. This is where the bulk
        /// of your ray tracing logic should go... though you may wish to
        /// break it down into multiple functions as it gets more complex!
        /// </summary>
        /// <param name="outputImage">Image to store render output</param>
        public void Render(Image outputImage)
        {
            FireRays(outputImage);
        }

        private void FireRays(Image outputImage) {

            // Image values
            double imageWidth = outputImage.Width;
            double imageHeight = outputImage.Height;
            double aspectRatio = imageWidth / imageHeight;
            double imageFOV = 60;

            // AAMultiplier
            int AAMult = options.AAMultiplier;

            // Vector3 for storing origin and direction
            Vector3 origin = new Vector3(0, 0, 0);

            // Pixel Color
            Color pixelColor;

            int x, y;
            
            for (x=0; x<imageWidth; x++) {
                //Console.WriteLine(x);

                for (y=0; y<imageHeight; y++) {

                    pixelColor = antiAliasing(x, y, imageHeight, imageWidth, imageFOV, aspectRatio, AAMult);

                    outputImage.SetPixel(x, y, pixelColor);
                            
                }
            }

        }

        private Color antiAliasing(double xPos, double yPos, double imageHeight, double imageWidth, double imageFOV, double aspectRatio, double AAmult) {
            Color output = new Color(0, 0, 0);

            // Vector3 for storing origin and direction
            Vector3 origin = new Vector3(0, 0, 0);
            Vector3 direction;

            // Ray for storing current Ray
            Ray curRay;

            int depth;

            double xLimit = xPos + 1;
            double yLimit = yPos + 1;

            double xStep = (xLimit - xPos) / (AAmult);
            double yStep = (yLimit - yPos) / (AAmult);

            double xOffset = (xLimit - xPos) / (AAmult * 2);
            double yOffset = (yLimit - yPos) / (AAmult * 2);

            xPos = xPos + xOffset;
            yPos = yPos + yOffset;

            double x1Pos, y1Pos;

            double x, y;

            for (x = xPos; x<xLimit; x = x + xStep) {

                x1Pos = (((x + xOffset) / (double)imageWidth) * 2.0) - 1;
                x1Pos = x1Pos * Math.Tan((imageFOV / 2.0) * (Math.PI / 180.0));

                for (y = yPos; y<yLimit; y = y + yStep) {

                    y1Pos = 1 - (((y + yOffset) / (double)imageHeight) * 2.0);
                    y1Pos = (y1Pos * Math.Tan((imageFOV / 2.0) * (Math.PI / 180.0))) / aspectRatio;

                    direction = new Vector3(x1Pos, y1Pos, 1);

                    curRay = new Ray(origin, direction);

                    depth = 0;
                    output = output + checkHit(curRay, depth);

                }
            }

            xPos = xLimit;
            yPos = yLimit;

            output = output / (AAmult * AAmult);

            return output;
        }

        private Color checkHit(Ray ray, int depth) {
            Color currentColor = new Color(0, 0, 0);
            Material.MaterialType outputMaterial = Material.MaterialType.NA;
            double refracIndex = 1.0;
            RayHit outputHit = null;
            
            double closestHitTime = double.PositiveInfinity;


            foreach (SceneEntity entity in this.entities) {
                RayHit hit = entity.Intersect(ray);

                    if (hit != null) {

                        // Check it is closest object to camera
                        if (hit.HitTime < closestHitTime) {

                            currentColor = entity.Material.Color;
                            outputMaterial = entity.Material.Type;
                            refracIndex = entity.Material.RefractiveIndex;
                            outputHit = hit;
                            closestHitTime = hit.HitTime;
                        }
                    }
            }

            Color outputColor = new Color(0,0,0);
            foreach(PointLight light in lights) {

                Color lightColor = light.Color;

                
                if (outputMaterial != Material.MaterialType.NA) {
                    lightColor = Shadow(outputColor, outputHit, lightColor, light);
                }

                
                if (outputMaterial == Material.MaterialType.Diffuse) {
                    outputColor = Diffuse(currentColor, outputHit, lightColor, light) + outputColor;
                }
                
            }
                
              
            if (outputMaterial == Material.MaterialType.Reflective) {
                outputColor = Reflect(currentColor, outputHit, depth);
            } 
   
            if (outputMaterial == Material.MaterialType.Refractive) {
                outputColor = fresnalEffect(currentColor, outputHit, depth, refracIndex);  
            }
            
            return outputColor;

        }

        private Color Diffuse(Color color, RayHit hit, Color lightColor, PointLight light) {

            Color newColor = new Color(0,0,0);
                
            newColor += color * lightColor * ((hit.Normal).Dot((light.Position - hit.Position).Normalized()));

            newColor = newColor.unclip();

            color = newColor;

            return color;
        }

        private Color Shadow(Color color, RayHit hit, Color lightColor, PointLight light) {

            // direction to light source offset to account for self-intersection
            Vector3 toLight = (light.Position - hit.Position).Normalized();
            Ray ray = new Ray(hit.Position + offset * toLight, toLight);

            // time to light source so we dont check for hits with objects behind light source
            double timeTo = (light.Position - hit.Position).Length();

            foreach(SceneEntity entity in entities) {
                RayHit lightHit = entity.Intersect(ray);

                if (lightHit != null && lightHit.HitTime < timeTo) {
                    lightColor = new Color(0,0,0);
                }
            }
            
            return lightColor;
        }

        private Color Reflect(Color color, RayHit hit, int depth) {
            Color Black = new Color(0,0,0);
            Color output = new Color(0,0,0);

            depth++;
            if (depth >= MAXDEPTH) {
                return Black;
            }

            Vector3 reflectionVec =  hit.Incident - (2 * (hit.Normal.Dot(hit.Incident)) * hit.Normal);
            Ray reflection = new Ray(hit.Position + reflectionVec * offset, reflectionVec);

            output = checkHit(reflection, depth);

            return output;

        }
        
        private Color fresnalEffect(Color color, RayHit hit, int depth, double refracIndex) {
            Color Black = new Color(0,0,0);
            Color output = new Color(0,0,0);
            Color refracColor = new Color(0,0,0);
            Color reflecColor;

            depth++;
            if (depth >= MAXDEPTH) {
                return Black;
            }

            double kr = fresnel(hit, refracIndex);

            if (kr < 1) {
                refracColor = Refract(color, hit, depth, refracIndex);
            }

            reflecColor = Reflect(color, hit, depth);

            output = reflecColor * kr + refracColor * (1 - kr);

            return output;
        }


        private Color Refract(Color color, RayHit hit, int depth, double refracIndex) {
            Color Black = new Color(0,0,0);
            Color output = new Color(0,0,0);

            depth++;
            if (depth >= MAXDEPTH) {
                return Black;
            }

            Vector3 Nrefr = hit.Normal;
            double NdotI = Nrefr.Dot(hit.Incident);
            double etai = 1, etat = refracIndex, temp;

            double cosi = Math.Clamp(hit.Incident.Dot(hit.Normal), -1, 1);

            if (cosi < 0) {
                cosi = -cosi;
            }
            else {
                Nrefr = -hit.Normal;
                temp = etat;
                etat = etai;
                etai = temp;
            }

            double eta = etai / etat;
            double k = 1 - eta * eta * (1 - cosi * cosi);

            Vector3 refractionVec;
            if (k < 0) {
                refractionVec = new Vector3(0, 0, 0);
            }
            else {
                refractionVec = eta * hit.Incident + (eta * cosi - Math.Sqrt(k)) * Nrefr;
            }
            
           
            Ray refraction = new Ray(hit.Position + refractionVec * offset, refractionVec);

            output = checkHit(refraction, depth);

            return output;

        }

        private double fresnel(RayHit hit, double refracIndex) {

            double cosi = Math.Clamp(hit.Incident.Dot(hit.Normal), -1, 1);
            double etai = 1, etat = refracIndex, temp;
            double kr, rs, rp;
            double cost;

            if (cosi > 0) {
                temp = etat;
                etat = etai;
                etai = temp;
            }

            double sint = etai / etat * Math.Sqrt(Math.Max(0.0, 1 - cosi * cosi));
            
            if (sint >= 1) {
                kr = 1;
            }
            else {
                cost = Math.Sqrt(Math.Max(0.0, 1 - sint * sint));
                cosi = Math.Abs(cosi);
                rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost));
                rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost)); 
                kr = (rs * rs + rp * rp) / 2; 
            }

            return kr;
        }

    }
}
