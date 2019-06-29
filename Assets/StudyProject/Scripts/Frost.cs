using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class Frost : MonoBehaviour
{
    private const CameraEvent FrostBlurEvent = CameraEvent.AfterImageEffectsOpaque;
    private static Material blurMaterial;
    private static readonly Dictionary<Camera, CameraInfo> Cameras = new Dictionary<Camera, CameraInfo>();
    private static readonly int GrabTex = Shader.PropertyToID("_GrabTex");
    private static readonly int[] FrostTex =
    {
        Shader.PropertyToID("_FrostTex0"),
        Shader.PropertyToID("_FrostTex1"),
        Shader.PropertyToID("_FrostTex2"),
        Shader.PropertyToID("_FrostTex3")
    };
    private static readonly int[] FrostTexT =
    {
        Shader.PropertyToID("_FrostTex0T"),
        Shader.PropertyToID("_FrostTex1T"),
        Shader.PropertyToID("_FrostTex2T"),
        Shader.PropertyToID("_FrostTex3T")
    };

    private static void AddCommandBufferIfNeeded(Camera cam)
    {
        if (Cameras.ContainsKey(cam))
        {
            return;
        }

        var cb = new CommandBuffer
        {
            name = "FrostBlur"
        };
        cam.AddCommandBuffer(FrostBlurEvent, cb);
        Cameras.Add(cam, new CameraInfo(cb, 0, 0));
    }

    private static void CleanCameras()
    {
        foreach (var pair in Cameras)
        {
            if (pair.Key != null)
            {
                pair.Key.RemoveCommandBuffer(FrostBlurEvent, pair.Value.CommandBuffer);
            }
        }

        Cameras.Clear();
    }

    private void OnEnable()
    {
        CleanCameras();
    }

    private void OnDisable()
    {
        CleanCameras();
    }

    private void OnWillRenderObject()
    {
        if (!this.enabled || !this.gameObject.activeInHierarchy)
        {
            CleanCameras();
            return;
        }
        var cam = Camera.current;
        if (cam == null)
        {
            return;
        }
        AddCommandBufferIfNeeded(cam);
        var camInfo = Cameras[cam];
        var width = cam.pixelWidth;
        var height = cam.pixelHeight;
        if ((width == camInfo.Width) && (height == camInfo.Height))
        {
            return;
        }
        var cb = camInfo.CommandBuffer;
        Cameras[cam] = new CameraInfo(cb, width, height);
        var blur = blurMaterial;
        if (blur == null)
        {
            blur = new Material(Shader.Find("Hidden/FrostBlur"));
            blurMaterial = blur;
        }

        // 必要に応じ（画面サイズが変わった場合）コマンドバッファを再構成する
        cb.Clear();
        // 一時テクスチャを取得
        // _GrabTexは画面と同サイズ、_FrostTex0～4と_FrostTex0T～4Tは
        // 0が画面と同サイズで、以降はサイズを半々に小さくする
        cb.GetTemporaryRT(GrabTex, width, height, 0, FilterMode.Bilinear);
        for (int i = 0, w = width, h = height; i < 4; i++, w >>= 1, h >>= 1)
        {
            cb.GetTemporaryRT(FrostTex[i], w, h, 0, FilterMode.Bilinear);
            cb.GetTemporaryRT(FrostTexT[i], w, h, 0, FilterMode.Bilinear);
        }
        // 現在のレンダリング結果を_GrabTexにコピー
        cb.Blit(BuiltinRenderTextureType.CurrentActive, GrabTex);
        // _FrostTex0に_GrabTexをぼかしたものを格納
        cb.Blit(GrabTex, FrostTex[0]); // そのままコピー
        cb.Blit(FrostTex[0], FrostTexT[0], blur, 0); // 水平ぼかしコピー
        cb.Blit(FrostTexT[0], FrostTex[0], blur, 1); // 垂直ぼかしコピー
        // 段階的に縮小テクスチャへコピーしつつ、さらにぼかしをかけていく
        for (var i = 1; i < 4; i++)
        {
            cb.Blit(FrostTex[i - 1], FrostTex[i]); // 縮小コピー
            cb.Blit(FrostTex[i], FrostTexT[i], blur, 0); // 水平ぼかしコピー
            cb.Blit(FrostTexT[i], FrostTex[i], blur, 1); // 垂直ぼかしコピー
        }
        // Blitなどのコストを無視すれば、ぼかしテクスチャ総面積は画面面積×1.33で
        // サンプリング回数はピクセルあたり9×縦横2回で18回、さらに後でグラブテクスチャと
        // 各ぼかしテクスチャから1回ずつサンプリングするので合計23回となり、
        // 総サンプリング回数はおよそ画面面積×30.5回となるはず...
        // 当初の方式でのσ=8クラスのぼかし（およそ描画面積×2600回のサンプリングが必要）と
        // 比べると、同等のぼかし量ながらだいぶ処理コストを削減できたのではないでしょうか?
    }

    private struct CameraInfo
    {
        public readonly CommandBuffer CommandBuffer;
        public readonly int Width;
        public readonly int Height;

        public CameraInfo(CommandBuffer cb, int w, int h)
        {
            this.CommandBuffer = cb;
            this.Width = w;
            this.Height = h;
        }
    }
}