using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
using UnityEngine.UI;


namespace Applibot
{
    public class UIOutLine : CustomImageBase
    {
        public static readonly string SHADER_NAME = "Custom/UiOutLine";

        [ColorUsage(true, true)]
        [SerializeField]
        private Color _OutlineColor = Color.white;

        private Image _Image;

        [SerializeField] private bool _isStatic = false;
        private int _OutLineColor = Shader.PropertyToID("_OutLineColor");
        private readonly int _SrcFactor = Shader.PropertyToID("_SrcFactor");
        private readonly int _DstFactor = Shader.PropertyToID("_DstFactor");

        protected override void UpdateMaterial(Material baseMaterial)
        {
            if(material == null)
            {
                Shader shader = Shader.Find(SHADER_NAME);
                material = new Material(shader);
            }

            material.SetColor(_OutLineColor, _OutlineColor);
            material.SetInt(_SrcFactor, (int)BlendMode.SrcAlpha);
            material.SetInt(_DstFactor, (int)BlendMode.OneMinusSrcAlpha);

            if(canvasScaler != null)
            {
                Vector2 canvasResolution = canvasScaler.referenceResolution;
                Vector2 textureSize = Vector2.one;

                if(_Image != null && _Image.sprite.packed)
                {
                    //sprite atlas使用時はatlasサイズを取得
                    Rect r = _Image.sprite.textureRect;
                    textureSize = new Vector2(r.width, r.height);
                }
                else
                {
                    Texture mainTexture = graphic.mainTexture;
                    textureSize = new Vector2(mainTexture.width, mainTexture.height);
                }

                //textureSizeによって線の太さに差が出ないように調整、 canvasに対しての比率をshaderでかけ合わせる
                float x = textureSize.x / canvasResolution.x;
                float y = textureSize.y / canvasResolution.y;
                material.SetVector("_scaleFactor", new Vector4(x, y));
            }
        }

        private void Awake()
        {
            _Image = graphic as Image;

            if(Application.isPlaying == false)
            {
                return;
            }

            if(_isStatic)
            {
                Capture();
            }
        }

        public void Capture()
        {
            UpdateMaterial(null);
            material.SetInt(_SrcFactor, (int)BlendMode.One);
            material.SetInt(_DstFactor, (int)BlendMode.Zero);

            if(TryGetComponent(out RawImage rawImage))
            {
                Texture mainTexture = graphic.mainTexture;
                float w = (transform as RectTransform).rect.width;
                float h = (transform as RectTransform).rect.height;

                var rt = new RenderTexture((int)w, (int)h, 0, RenderTextureFormat.ARGBHalf);
                Graphics.Blit(mainTexture, rt, material);
                rawImage.texture = rt;

                DestroyMaterial();
                enabled = false;
            }
        }
    }
}



