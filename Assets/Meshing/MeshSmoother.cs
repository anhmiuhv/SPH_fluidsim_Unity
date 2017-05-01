using UnityEngine;
using System.Collections;

/*
	Apply to any meshed gameobject for smoothing.
 
        Works also by replacing MeshFilter with SkinnedMeshRenderer and use sharedMesh
 
	At present tests Laplacian Smooth Filter and HC Reduced Shrinkage Variant Filter
*/
public class MeshSmoother
{

    private Mesh sourceMesh;
    private Mesh workingMesh;

 

    // Clone a mesh
    public static Mesh CloneMesh(Mesh mesh)
    {
        Mesh clone = new Mesh();
        clone.vertices = mesh.vertices;
        clone.normals = mesh.normals;
        clone.tangents = mesh.tangents;
        clone.triangles = mesh.triangles;
        clone.uv = mesh.uv;
        clone.uv2 = mesh.uv2;
        clone.uv2 = mesh.uv2;
        clone.bindposes = mesh.bindposes;
        clone.boneWeights = mesh.boneWeights;
        clone.bounds = mesh.bounds;
        clone.colors = mesh.colors;
        clone.name = mesh.name;
        //TODO : Are we missing anything?
        return clone;
    }
}