# COMP30019 Assignment 1 - Ray Tracer

**Name:** Paul Hutchins \
**Student Number:** 1160468 \
**Username:** PHutchins \
**Email:** PHutchins@student.unimelb.edu.au

## Completed stages

##### Stage 1

- [x] Stage 1.1 - Familiarise yourself with the template
- [x] Stage 1.2 - Implement vector mathematics
- [x] Stage 1.3 - Fire a ray for each pixel
- [x] Stage 1.4 - Calculate ray-entity intersections
- [x] Stage 1.5 - Output primitives as solid colours

##### Stage 2

- [x] Stage 2.1 - Diffuse materials
- [x] Stage 2.2 - Shadow rays
- [x] Stage 2.3 - Reflective materials
- [x] Stage 2.4 - Refractive materials
- [x] Stage 2.5 - The Fresnel effect
- [x] Stage 2.6 - Anti-aliasing

##### Stage 3

- [ ] Option A - Emissive materials (+6)
- [ ] Option B - Ambient lighting/occlusion (+6)
- [x] Option C - OBJ models (+6)
- [ ] Option D - Glossy materials (+3)
- [ ] Option E - Custom camera orientation (+3)
- [ ] Option F - Beer's law (+3)
- [ ] Option G - Depth of field (+3)

I have implented a OBJ reader which stores an array of Faces that have 3 Vertices, 3 Vertice Normals, and the Triangle of these 3 vertices.
It has a bounding box implemented which is a cube constructed of 12 triangles around the max and min values of the obj model to speed up the rendering of the scene, as collision with the obj are only calulcated if an intersection occurs with the bounding box
I have also implemented Gouraud shading, which using the barycentric coordinates of the intersection to calculate the intersection normal with regards to the vertice normals.


## Final scene render

My final render!

![My final render](/images/final_scene.png)

This render took **1574** minutes and **0** seconds on my PC.
Yes I left my computer running for 26 hours for this!

I used the following command to render the image exactly as shown:

```
dotnet run -- -f tests/final_scene.txt -o images/final_scene.png -x 4 -w 2000 -h 2000
```

## My outputs

I have included my outputs for the sample scenes run with the following commands

###### Sample Scene 1
```
dotnet run -- -f tests/sample_scene_1.txt -o images/sample_scene_output.png -x 4
```
<p float="left">
  <img src="/images/sample_scene_1_output.png" />
</p>

###### Sample Scene 2

```
dotnet run -- -f tests/sample_scene_2.txt -o images/sample_scene_2_output.png -x 4
```
<p float="left">
  <img src="/images/sample_scene_2_output.png" />
</p>

###### Sample Scene OBJ

```
dotnet run -- -f tests/sample_scene_obj.txt -o images/sample_scene_obj_output.png
```
<p float="left">
  <img src="/images/sample_scene_obj_output.png" />
</p>



## References

Working through a ray tracer, from the head of the xbox games studio: https://www.linkedin.com/pulse/writing-simple-ray-tracer-c-matt-booty/

*Ray Tracing in a Weekend*: https://raytracing.github.io/

Great walkthrough of some of the basic maths: https://blog.scottlogic.com/2020/03/10/raytracer-how-to.html

Scratchapixel: intro to ray tracing: https://www.scratchapixel.com/lessons/3d-basic-rendering/introduction-to-ray-tracing/how-does-it-work

What's the most efficient way to find barycentric coordinates?: https://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates

Raytracing - Ray Sphere Intersection: https://viclw17.github.io/2018/07/16/raytracing-ray-sphere-intersection/

How to calculate reflection vector: https://www.fabrizioduroni.it/2017/08/25/how-to-calculate-reflection-vector/

Introduction to Shading: https://www.scratchapixel.com/lessons/3d-basic-rendering/introduction-to-shading/reflection-refraction-fresnel

Alias/WaveFront Object (.obj) File Format: https://people.cs.clemson.edu/~dhouse/courses/405/docs/brief-obj-file-format.html

Scaling (geometry): https://en.wikipedia.org/wiki/Scaling_(geometry)

Bounding Volume Hierachy: https://en.wikipedia.org/wiki/Bounding_volume_hierarchy

Introduction to Shading: https://www.scratchapixel.com/lessons/3d-basic-rendering/introduction-to-shading/shading-normals




