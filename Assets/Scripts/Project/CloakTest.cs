using Project.Utilities;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Project
{
    public class CloakTest : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _mr;
        [SerializeField] private MeshFilter _mf;
        [SerializeField] private RenderTexture _renderTexture;

        private Mesh _mesh;
        private Material _material;

        private Camera _camera;
        private RenderTexture _prevTarget;

        private void Start()
        {
            _mesh = _mf.mesh;
            _material = _mr.material;
        }

        private void Update()
        {
            if (_camera != null) // Wait for 1 frame to render into texture
            {
                TextureUtilities.ProjectCameraRenderOntoMesh(_camera, this.transform, _mesh, _material);
                _camera.targetTexture = _prevTarget;
                _camera = null;
                _mr.enabled = true;
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                // Resize texture to camera
                _camera = Camera.main;
                _renderTexture.Release();
                _renderTexture.width = _camera.pixelWidth;
                _renderTexture.height = _camera.pixelHeight;
                _prevTarget = _camera.targetTexture;
                _camera.targetTexture = _renderTexture;
                _mr.enabled = false; // Don't render this object during next frame so we can see behind it
            }
        }
    }
}