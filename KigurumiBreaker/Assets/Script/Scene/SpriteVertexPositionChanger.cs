using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using Unity.Collections;
using UnityEngine.UI;
using System;


namespace Applibot
{
    public class SpriteVertexPositionChanger : MonoBehaviour
    {
        public float scale = 1.5f;
        [NonSerialized] private Image _image;

        // Start is called before the first frame update
        void Start()
        {
            _image = GetComponent<Image>();
            if(_image == null)
            {
                Debug.LogError("Imageコンポーネントが必要です");
                return;
            }

            _image.useSpriteMesh = true;
            Sprite sprite = _image.sprite;
            if(sprite.packed)
            {
                _image.rectTransform.sizeDelta *= scale;
            }

            ChangeMeshScale(sprite);
        }

        private void ChangeMeshScale(Sprite sprite)
        {
            NativeSlice<Vector3> vertices = sprite.GetVertexAttribute<Vector3>(VertexAttribute.Position);
            NativeArray<Vector3> copy = new NativeArray<Vector3>(vertices.Length, Allocator.Temp);
            for(int i = 0; i < vertices.Length; i++)
            {
                copy[i] = vertices[i] * scale;
            }

            sprite.SetVertexAttribute(VertexAttribute.Position, copy);
            copy.Dispose();
        }
    }

}
