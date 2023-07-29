using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    [Expandable]
    [SerializeField] SphereScriptableObject data;
    [SerializeField] float vertexGizmoRad;
    [SerializeField] Color vertexGizmoColor;

    private void Update()
    {
        GenerateShape();
    }

    public void GenerateShape()
    {
        switch (data.shape)
        {
            case Shape.Circle:
                GenerateCircle();
                break;
            case Shape.Basketball:
                GenerateSphereBasketball();
                break;
            case Shape.Sphere:
                GenerateSphere();
                break;
        }
    }
    //Generate the mesh for the asteroid based on data from the scriptableobject
    void GenerateCircle()
    {
        Mesh mesh = new Mesh();
        
        mesh.Clear();
        
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, data.radius));
        vertices.Add(new Vector3(data.radius, 0));

        //functions for a circle is r^2 = x^2 + y^2
        //my algorithm for creating a circle with evenly placed vertices: we will start with a quarter of the circle - 
        //start with only the two vertices at the intersection points with the axes. then, based on a resolution variable, 
        //create N points on the straight line between the two vertices you already have (the line
        //represented by the function y = -x + r), then transform (with some 
        //analytic geometry) each of the points to the corresponding point on the circle. That point will be the vertex.
        //Then simply copy the quarter four horizontally then vertically
        
        float step = data.radius / (data.resolution+1);
        for (int i = 1; i <= data.resolution; i++)
        {
            float x = step*i;
            float y = data.radius - x;

            Vector3 newVert = ProjectOntoCircle(x, y);
            vertices.Add(newVert);
        }

        //Mirror the quarter circle on the x axis
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 mirror = MirrorX(vertices[i]);
            if (!vertices.Contains(mirror))
            {
                vertices.Add(mirror);
            }
        }
        
        //Mirror the quarter circle on the y axis
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 mirror = MirrorY(vertices[i]);
            if (!vertices.Contains(mirror))
            {
                vertices.Add(mirror);
            }
        }

        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices.ToArray();
    }

    void GenerateSphere()
    {
        Mesh mesh = new Mesh();
        
        mesh.Clear();
        
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, data.radius, 0));
        vertices.Add(new Vector3(data.radius, 0, 0));
        vertices.Add(new Vector3(0, 0, data.radius));

        //functions for a sphere is r^2 = x^2 + y^2 + z^2 (centered at the origin, r being the radius)
        //my algorithm for creating a sphere will use the same concepts I put into place on the circle:
        //start with three points: one on each intersection point of your desired sphere with the three axes.
        //focus only on one quadrant, in this case the first. Find a plane that bisects the sphere, that also intersects 
        //with the three starting points, namely the plane represented by the function z = - y - x + r. 
        //Now, using the columns variable in the data object, put that number of points on the bottom side of the
        //triangle created by the plane your created and the axes. Draw lines from the top vertex of the triangle to those, and on those lines,
        //Generate a number of points based on the resolution variable in the data object. Then project all points on your triangle
        //to the sphere with analytic geometry, and the resulting points on the surface of the sphere will be your vertices
        float stepColumn = data.radius / (data.columns + 1);
        float stepRes = data.radius/ (data.resolution + 1);

        //Function for the line at the base of the triangle is z = -x + r, place the points on the base that will
        //act as anchors for the columns
        for (int i = 1; i <= data.columns; i++)
        {
            float x = stepColumn * i;
            float z = data.radius - x; 

            //(parametric) Functions of the lines that are the columns is:
            //I: x(t) = 0 - x*t
            //II: y(t) = r + r*t
            //III: z(t) = 0 - z*t
            
            for (int j = 1; j <= data.resolution; j++)
            {
                //We give it the z coord with step and solve for x and z from there
                float y1 = stepRes * j ;

                float t = (y1-data.radius)/data.radius;
                float x1 = -x*t;
                float z1 = -z*t;
                vertices.Add(ProjectOntoSphere(x1, y1, z1));
            }

            vertices.Add(ProjectOntoSphere(x, 0.00000001f, z));
        }
        
        //TODO: the other vertices of the starting triangle don't have columns of their own without these ugly loops
        for (int j = 1; j <= data.resolution; j++)
        {
            //We give it the z coord with step and solve for x and z from there
            float y1 = stepRes * j ;

            float t = (y1-data.radius)/data.radius;
            float x1 = -t;
            float z1 = 0.00000001f;
            vertices.Add(ProjectOntoSphere(x1, y1, z1));
        }

        for (int j = 1; j <= data.resolution; j++)
        {
            //We give it the z coord with step and solve for x and z from there
            float y1 = stepRes * j ;

            float t = (y1-data.radius)/data.radius;
            float z1 = -t;
            float x1 = 0.00000001f;
            vertices.Add(ProjectOntoSphere(x1, y1, z1));
        }
        
        //Mirror the eighth-sphere on the x axis
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 mirror = MirrorX(vertices[i]);
            if (!vertices.Contains(mirror))
            {
                vertices.Add(mirror);
            }
        }
        
        //Mirror the eighth-sphere on the y axis
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 mirror = MirrorY(vertices[i]);
            if (!vertices.Contains(mirror))
            {
                vertices.Add(mirror);
            }
        }
        
        //Mirror the eighth-sphere on the z axis
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 mirror = MirrorZ(vertices[i]);
            if (!vertices.Contains(mirror))
            {
                vertices.Add(mirror);
            }
        }

        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices.ToArray();
    }

    void GenerateSphereBasketball()
    {
        Mesh mesh = new Mesh();
        
        mesh.Clear();
        
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, data.radius, 0));
        vertices.Add(new Vector3(data.radius, 0, 0));
        vertices.Add(new Vector3(0, 0, data.radius));

        //functions for a sphere is r^2 = x^2 + y^2 + z^2 (centered at the origin, r being the radius)
        //my algorithm for creating a sphere will use the same concepts I put into place on the circle:
        //start with three points: one on each intersection point of your desired sphere with the three axes.
        //focus only on one quadrant, in this case the first. Find a plane that bisects the sphere, that also intersects 
        //with the three starting points, namely the plane represented by the function z = - y - x + r. 
        //Basketball algorithm (my original sphere algorithm):
        //Now, create a point in the middle of the three starting 
        //points on the triangle. the coordinates of this point will be (r/3, r/3, r/3). Now draw a line from this mid-point to each of 
        //your starting points, and the length of these lines will be (r*sqrt(2)*sqrt(3))/3 (solved with analytic geometry).
        //Use this distance, along with the desired resolution, to calculate the "step" - the length of each segment between
        //vertices. place points along that line according to this "step", then project all resulting points on your plane 
        //to the sphere, using the slope of the line function  between the origin and each point,
        //then finding that line's intersection with the sphere function. Use the resulting projected points as your vertices.

        Vector3 midPoint = new Vector3(data.radius / 3, data.radius / 3, data.radius / 3);
        vertices.Add(ProjectOntoSphere(midPoint.x, midPoint.y, midPoint.z));

        float sqrt2 = 1.4142135f;
        float sqrt3 = 1.7320508f;
        float centerDist = (data.radius * sqrt2 * sqrt3) / 3;
        float step = centerDist / (data.resolution + 1);

        for (int i = 1; i <= data.resolution/3; i++)
        {
            float x = step * Mathf.Pow(i, 1.1f);
            float y = (data.radius - x) / 2; //z and x are equal in this
            vertices.Add(ProjectOntoSphere(x, y, y));
            vertices.Add(ProjectOntoSphere(y, x, y));
            vertices.Add(ProjectOntoSphere(y, y, x));
            
            vertices.Add(ProjectOntoSphere(y, x, x));
            vertices.Add(ProjectOntoSphere(x, x, y));
            vertices.Add(ProjectOntoSphere(x, y, x));
        }

        //Mirror the eighth-sphere on the x axis
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 mirror = MirrorX(vertices[i]);
            if (!vertices.Contains(mirror))
            {
                vertices.Add(mirror);
            }
        }
        
        //Mirror the eighth-sphere on the y axis
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 mirror = MirrorY(vertices[i]);
            if (!vertices.Contains(mirror))
            {
                vertices.Add(mirror);
            }
        }
        
        //Mirror the eighth-sphere on the z axis
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 mirror = MirrorZ(vertices[i]);
            if (!vertices.Contains(mirror))
            {
                vertices.Add(mirror);
            }
        }

        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices.ToArray();
    }
    
    //Analytic Geometry transformation code from line to circle
    public Vector3 ProjectOntoCircle(float x, float y)
    {
        float slope = y / x;
        
        float newX = Mathf.Sqrt(Mathf.Pow(data.radius, 2) / (Mathf.Pow(slope, 2) + 1));
        float newY = Mathf.Sqrt(Mathf.Pow(data.radius, 2) - Mathf.Pow(newX, 2));
        Vector3 final = new Vector3(newX, newY);
        return final;
    }

    //Analytic Geometry projection code from inner plane to sphere
    public Vector3 ProjectOntoSphere(float x, float y, float z)
    {
        //the two slopes: m is for x and n is for z.
        //the way I structured 3D linear functions is - 
        //I: y = mx + nz + b
        //II: y = mx + nz + b
        //formula for finding slopes is the same as in 2D for m and swap the x's for z's for n

        if (x == 0 || y == 0)
        {
            throw new DivideByZeroException();
        }
        float m = y / x;
        float n = y / z;

        float newX = (data.radius * n) /
                     Mathf.Sqrt(Mathf.Pow(n, 2) + Mathf.Pow(n, 2) * Mathf.Pow(m, 2) + Mathf.Pow(m, 2));
        float newY = (data.radius * n * m) /
                     Mathf.Sqrt(Mathf.Pow(n, 2) + Mathf.Pow(n, 2) * Mathf.Pow(m, 2) + Mathf.Pow(m, 2));
        float newZ = (data.radius * m) /
                     Mathf.Sqrt(Mathf.Pow(n, 2) + Mathf.Pow(n, 2) * Mathf.Pow(m, 2) + Mathf.Pow(m, 2));

        return new Vector3(newX, newY, newZ);
    }

    public Vector3 MirrorX(Vector3 vector)
    {
        return new Vector3(-vector.x, vector.y, vector.z);
    }

    public Vector3 MirrorY(Vector3 vector)
    {
        return new Vector3(vector.x, -vector.y, vector.z);
    }
    
    public Vector3 MirrorZ(Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, -vector.z);
    }
    
    private void OnDrawGizmos()
    {
        if (GetComponent<MeshFilter>().mesh == null) return;
        
        Gizmos.color = vertexGizmoColor; 
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        
        foreach (Vector3 vertex in mesh.vertices) {
            Vector3 worldPos = transform.TransformPoint(vertex);
            Gizmos.DrawSphere(worldPos, vertexGizmoRad);
        }
    }
}
