using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessing : MonoBehaviour
{
    public List<Material> effects;
    
    void Start() {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var t = RenderTexture.GetTemporary(src.descriptor);
        var u = RenderTexture.GetTemporary(src.descriptor);

        Graphics.Blit(src, t);

        for (int i = 0; i < effects.Count; i++)
        {
            Graphics.Blit(i % 2 == 0? t : u, i % 2 == 0? u : t, effects[i]);
        }

        Graphics.Blit(effects.Count % 2 == 0? t : u, dest);
    
        RenderTexture.ReleaseTemporary(t);
        RenderTexture.ReleaseTemporary(u);

    }
}
