using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[ExecuteAlways]
public sealed class Blit : MonoBehaviour {

  public Material blitMaterial = null;


  string cmdName;

  void OnEnable() {
    cmdName = $"Blit :: {gameObject.name}";
    var data = GetComponent<HDAdditionalCameraData>();
    if (data) data.customRender += CustomRender;
  }

  void OnDisable() {
    var data = GetComponent<HDAdditionalCameraData>();
    if (data) data.customRender -= CustomRender;
  }


  void CustomRender(ScriptableRenderContext context, HDCamera hdCam) {

    int tempId = Shader.PropertyToID("_TemporaryColorTexture");

    var cmd = CommandBufferPool.Get(cmdName);

    // Can't read and write to same color target, create a temp render target to blit. 
    cmd.GetTemporaryRT(tempId, hdCam.actualWidth, hdCam.actualHeight, 0, FilterMode.Bilinear);

    var source = hdCam.camera.activeTexture;

    cmd.Blit(source, tempId, blitMaterial, -1);
    cmd.Blit(tempId, source);

    context.ExecuteCommandBuffer(cmd);
    CommandBufferPool.Release(cmd);
  }
}