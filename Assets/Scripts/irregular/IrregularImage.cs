using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Unicorn.UI
{
    [ExecuteAlways]
    public class IrregularImage : Image
    {
        #region params
        [SerializeField]
        private List<Vector2> screenVertices = new List<Vector2>();
        public Vector2[] ScreenVertices
        {
            get
            {
                return screenVertices.ToArray();
            }
        }
        #endregion params

        #region funcs
        public Sprite GetSprite()
        {
            return this.sprite;
        }

        protected override void Awake()
        {
            if (this.sprite == null)
                UnityEngine.Debug.LogError($"SomeVariable has not been assigned.{this.gameObject.name}");
        }
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            DebugDraw(screenPoint);
            Vector2 localPoint;
            bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, screenPoint, eventCamera, out localPoint);
            if (inside)
                return Utils.Point.IsPointInTriangles(GetSprite().triangles, ScreenVertices, localPoint);
            else
                return false;
        }

        private Vector3 GetWorldPosition(Vector2 vertice)
        {
            return this.transform.TransformPoint(vertice);
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            DebugDraw(Vector2.zero);
#endif
        }

        private List<Vector2> TranslateSpriteVertices()
        {
            List<Vector2> vertices = new List<Vector2>();
            Sprite curSprite = GetSprite();
            for (int i = 0; i < curSprite.vertices.Length; i++)
            {
                Vector2 vec = CalcSpriteVertice(curSprite.vertices[i]);
                vertices.Add(vec);
            }
            return vertices;
        }

        private Vector2 CalcSpriteVertice(Vector2 vertice)
        {
            Sprite curSprite = GetSprite();
            Vector2 vec = Vector2.zero;
            vec.x = Mathf.Clamp((vertice.x - curSprite.bounds.center.x - (curSprite.textureRectOffset.x / curSprite.texture.width) + curSprite.bounds.extents.x) / (2.0f * curSprite.bounds.extents.x) * curSprite.rect.width, 0.0f, curSprite.rect.width) - curSprite.rect.width / 2.0f;
            vec.y = Mathf.Clamp((vertice.y - curSprite.bounds.center.y - (curSprite.textureRectOffset.y / curSprite.texture.height) + curSprite.bounds.extents.y) / (2.0f * curSprite.bounds.extents.y) * curSprite.rect.height, 0.0f, curSprite.rect.height) - curSprite.rect.height / 2.0f;
            return vec;
        }

        private void DebugDraw(Vector2 screenPoint)
        {
#if UNITY_EDITOR
            Vector3 forward = new Vector3(screenPoint.x, screenPoint.y, 0f) + Vector3.forward * 0.1f;
            UnityEngine.Debug.DrawRay(new Vector3(screenPoint.x, screenPoint.y, 0), forward, UnityEngine.Color.green, 1f);

            //draw triangles
            float duration = 10.0f;
            Sprite curSprite = GetSprite();
            ushort[] triangles = curSprite.triangles;
            Vector2[] vertices = ScreenVertices;

            int triangles_count = triangles.Length / 3;
            for (int i = 0; i < triangles_count; i++)
            {
                var idx = i * 3;
                var a = triangles[idx];
                var b = triangles[idx + 1];
                var c = triangles[idx + 2];

                UnityEngine.Debug.DrawLine(GetWorldPosition(vertices[a]), GetWorldPosition(vertices[b]), UnityEngine.Color.green, duration);
                UnityEngine.Debug.DrawLine(GetWorldPosition(vertices[b]), GetWorldPosition(vertices[c]), UnityEngine.Color.green, duration);
                UnityEngine.Debug.DrawLine(GetWorldPosition(vertices[c]), GetWorldPosition(vertices[a]), UnityEngine.Color.green, duration);
            }
#endif
        }
        #endregion funcs

#if UNITY_EDITOR
        #region ContextMenu
        [ContextMenu("生成顶点列表")]
        public void GeneratorBySpriteEditor()
        {
#if UNITY_EDITOR
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            //this.useSpriteMesh = true;
            screenVertices.Clear();
            screenVertices = TranslateSpriteVertices();

#if UNITY_EDITOR
            sw.Stop();
            UnityEngine.Debug.Log($"'{GetSprite().name}'generate screen vertices use time:{sw.ElapsedMilliseconds}ms");
#endif
        }
        #endregion
#endif
    }
}
