/*
Function: Apply the transformations to the car and wheels, so that the car moves 
in a straight line with the wheels and rotate this ones.

Francisco Martinez Gallardo Lascurain A01782250
15/11/2023

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyTransforms : MonoBehaviour
{
    // Declare general Movment variables
    [SerializeField] Vector3 displacement; // General displacement vector
    [SerializeField] AXIS rotationAxis; // General Rotation axis
    [SerializeField] GameObject wheel1; // Wheel prefab
    AXIS wheelAxis = AXIS.X; // Wheel rotation axis
    float wheelAngle = -30; // Wheel rotation angle
    float scaleFactor = 0.3f; // Scaling factor

    // Declare variables for the the car (Meshes and Vertices)
    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] newVertices;
    // Declare variables for the wheels (Meshes, Vertices, and GameObjects for the wheels and their base)
    Mesh[] meshWheels = new Mesh[4]; 
    Vector3[][] baseVerticesWheels = new Vector3[4][];
    Vector3[][] newVerticesWheels = new Vector3[4][];
    GameObject[] wheels = new GameObject[4];
    
    void Start()
    {
        // Initialize the variables for the car
        mesh = GetComponentInChildren<MeshFilter>().mesh; // Get the mesh from the car
        baseVertices = mesh.vertices; // Get the vertices from the mesh

        // Initialize the variables for the wheels
        newVertices = new Vector3[baseVertices.Length]; // Create a new array of vertices
        for (int i = 0; i < baseVertices.Length; i++)
        {
            // Copy the vertices from the mesh to the new array
            newVertices[i] = baseVertices[i];
        }

        //Initialize the variables for the wheels

        // Instantiate the wheels
        wheels[0] = Instantiate(wheel1, new Vector3(0f,0f,0f), Quaternion.identity);
        wheels[1] = Instantiate(wheel1, new Vector3(0f,0f,0f), Quaternion.identity);
        wheels[2] = Instantiate(wheel1, new Vector3(0f,0f,0f), Quaternion.identity);
        wheels[3] = Instantiate(wheel1, new Vector3(0f,0f,0f), Quaternion.identity);
        

        // Get the meshes from the wheels, and the vertices from the meshes
        for (int i = 0; i < wheels.Length; i++)
        {
            meshWheels[i] = wheels[i].GetComponentInChildren<MeshFilter>().mesh;
            baseVerticesWheels[i] = meshWheels[i].vertices;
            newVerticesWheels[i] = new Vector3[baseVerticesWheels[i].Length];
        }

        // Copy the vertices from the meshes to the new arrays
        for (int i = 0; i < baseVerticesWheels.Length; i++)
        {
            for (int j = 0; j < baseVerticesWheels[i].Length; j++)
            {
                newVerticesWheels[i][j] = baseVerticesWheels[i][j];
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Call the DoTransform function to apply the transformations to car and wheels
        DoTransform();
    }


    void DoTransform(){
        // Calculate the angle based on vectors
        float angleRadians = Mathf.Atan2(displacement.z, displacement.x);
        float angle = angleRadians * Mathf.Rad2Deg-90;
        // Create a translation matrix
        Matrix4x4 move= HW_Transforms.TranslationMat(displacement.x *Time.time , displacement.y *Time.time, displacement.z *Time.time);
        // Create to move origin matrix
        Matrix4x4 moveOrigin= HW_Transforms.TranslationMat(-displacement.x, -displacement.y, -displacement.z);
        //Create a move to move Object again to the origin
        Matrix4x4 moveObject= HW_Transforms.TranslationMat(displacement.x, displacement.y, displacement.z);
        // Create a rotation matrix
        Matrix4x4 rotate = HW_Transforms.RotateMat(angle, rotationAxis );
        // Create a composite matrix
        Matrix4x4 composite = move * rotate;

        // Update new vertices
        for (int i=0; i<newVertices.Length; i++)
        {
            Vector4 temp = new Vector4(baseVertices[i].x, baseVertices[i].y, baseVertices[i].z, 1);

            newVertices[i] = composite * temp;
        }
        
        // Update the vertices of the car and recalculate normals
        mesh.vertices = newVertices;
        mesh.RecalculateNormals();
        
        // Now do the same for the wheels
        // Create a rotation matrix for the wheels
        Matrix4x4 rotateWheels=HW_Transforms.RotateMat(wheelAngle * Time.time, wheelAxis );
        // Create a scaling matrix
        Matrix4x4 scaleMatrix = HW_Transforms.ScaleMat(scaleFactor, scaleFactor, scaleFactor);
        // Create a matrix for the position of the wheels
        Matrix4x4[] wheelPositions = {
            HW_Transforms.TranslationMat(0.757f, 0.367f, -1.23f),
            HW_Transforms.TranslationMat(-0.757f, 0.367f, -1.23f),
            HW_Transforms.TranslationMat(0.757f, 0.367f, 1.23f),
            HW_Transforms.TranslationMat(-0.757f, 0.367f, 1.23f)
        };

        
        for (int i = 0; i < wheels.Length; i++)
        {   
            // Update the new vertices of the wheels
            for (int j = 0; j < baseVerticesWheels[i].Length; j++)
            {
                Vector4 temp = new Vector4(baseVerticesWheels[i][j].x, baseVerticesWheels[i][j].y, baseVerticesWheels[i][j].z, 1);
                newVerticesWheels[i][j] = composite * wheelPositions[i] * rotateWheels * scaleMatrix * temp;
            }
             // Update the vertices of the wheels and recalculate normals
            meshWheels[i].vertices = newVerticesWheels[i];
            meshWheels[i].RecalculateNormals();
        }
    
    }
}

