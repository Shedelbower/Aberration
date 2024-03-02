using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering;

namespace Project.Utilities
{
    public class TextureUtilities
    {
        // public static void CopyCameraTargetTextureToDestination(Camera camera, Texture dest)
        // {
        //     camera.targetTexture
        // }

        public static void ProjectCameraRenderOntoMesh(Camera camera, Transform transform, Mesh mesh, Material material)
        {
            // Copy current texture to material
            var target = camera.targetTexture;
            if (target == null)
            {
                Debug.LogError("Camera has no render texture target");
                return;
            }

            if (material.mainTexture == null)
            {
                material.mainTexture = target;
            }
            
            // var destination = material.mainTexture;
            // if (destination == null)
            // {
            //     Debug.Log("Generating new texture for material...");
            //     Debug.Log(target.format);
            //     material.mainTexture = new Texture2D(target.width, target.height, TextureFormat.ARGB32, false);
            //     destination = material.mainTexture;
            // }
            // Graphics.CopyTexture(target, destination);
            
            // Update mesh UVs to screen space coordinates
            var vertices = mesh.vertices;
            var uvs = new Vector2[vertices.Length];
            for (int vi = 0; vi < vertices.Length; vi++)
            {
                var localPos = vertices[vi];
                var worldPos = transform.TransformPoint(localPos);
                var viewPos = camera.WorldToViewportPoint(worldPos);
                var uv = new Vector2(viewPos.x, viewPos.y);
                uvs[vi] = uv;
            }
            
            mesh.SetUVs(0, uvs);
        }
        
        
        
    }
}