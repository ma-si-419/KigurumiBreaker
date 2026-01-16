using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Applibot
{
    public class RadialBlur : CustomImageBase
    {
        public static readonly string SHADER_NAME = "Custom/Blur";

        public float BlurRadius = 30.0f;
        [Range(0, 30)] public int SampleCount = 10;

        private Shader _shader;
        private int _BlurRadiusId = Shader.PropertyToID("_BlurRadius");
        private int _SampleCountId = Shader.PropertyToID("_Count");

        protected override void UpdateMaterial(Material baseMaterial)
        {
            if (material == null)
            {
                Shader shader;
                shader = Shader.Find(SHADER_NAME);
                material = new Material(shader);
            }

            material.SetFloat(_BlurRadiusId, BlurRadius);
            material.SetInt(_SampleCountId, SampleCount);

            float scale = 0.0005f;
            if (canvasScaler != null)
            {
                Vector2 texureSize = canvasScaler.referenceResolution;
                scale = 1f / Mathf.Max(texureSize.x, texureSize.y);
            }
            material.SetFloat("_scaleFactor", scale);
        }
    }

}

