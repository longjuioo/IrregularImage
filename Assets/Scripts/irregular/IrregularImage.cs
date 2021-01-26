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
        Vector3 worldpos = Vector3.zero;

        public List<Vector2> ScreenVertices
        {
            get
            {
                return screenVertices;
            }
        }
        #endregion params

        #region funcs
        public Sprite GetSprite()
        {
            return this.sprite;
        }

        void NotifyNotAssigned()
        {
            if (this.sprite == null)
            {
                UnityEngine.Debug.LogError($"SomeVariable has not been assigned.'{this.gameObject.name}'");
                this.enabled = false;
            }
        }

        public bool CanRaycast()
        {
            return !GetSprite() && ScreenVertices.Count > 0;
        }

        protected override void Awake()
        {
            NotifyNotAssigned();
        }
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (CanRaycast())
                return false;
            DebugDraw(screenPoint);
            Vector2 localPoint;

            bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, screenPoint, eventCamera, out localPoint);
#if UNITY_EDITOR
            RectTransformUtility.ScreenPointToWorldPointInRectangle(this.rectTransform, screenPoint, eventCamera, out worldpos);
            UnityEngine.Debug.DrawLine(eventCamera.transform.position, worldpos, Color.red, 1.0f);
#endif
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
            float width = this.rectTransform.rect.width;
            float height = this.rectTransform.rect.height;
            Vector2 vec = Vector2.zero;
            vec.x = Mathf.Clamp((vertice.x - curSprite.bounds.center.x /*- (curSprite.textureRectOffset.x / curSprite.texture.width) */+ curSprite.bounds.extents.x) / (2.0f * curSprite.bounds.extents.x) * width, 0.0f, width) - width / 2.0f;
            vec.y = Mathf.Clamp((vertice.y - curSprite.bounds.center.y /*- (curSprite.textureRectOffset.y / curSprite.texture.height)*/ + curSprite.bounds.extents.y) / (2.0f * curSprite.bounds.extents.y) * height, 0.0f, height) - height / 2.0f;
            return vec;
        }

        private void DebugDraw(Vector2 screenPoint)
        {
#if UNITY_EDITOR
            if (!this.enabled)
                return;
            Vector3 forward = new Vector3(screenPoint.x, screenPoint.y, 0f) + Vector3.forward * 0.1f;
            UnityEngine.Debug.DrawRay(new Vector3(screenPoint.x, screenPoint.y, 0), forward, UnityEngine.Color.green, 1f);

            //draw triangles
            float duration = 1.0f;
            Sprite curSprite = GetSprite();
            if (curSprite == null)
                return;
            ushort[] triangles = curSprite.triangles;
            if (triangles.Length == 0 || ScreenVertices.Count == 0)
                return;

            int vLen = ScreenVertices.Count;
            int triangles_count = triangles.Length / 3;
            if (triangles_count > vLen)
            {
                return;
            }
            for (int i = 0; i < triangles_count; i++)
            {
                var idx = i * 3;
                var a = triangles[idx];
                var b = triangles[idx + 1];
                var c = triangles[idx + 2];
                if (vLen < a || vLen < b || vLen < c)
                {
                    UnityEngine.Debug.LogError($"请重新生成不规则图片'{curSprite.name}.png'");
                    return;
                }
                UnityEngine.Debug.DrawLine(GetWorldPosition(ScreenVertices[a]), GetWorldPosition(ScreenVertices[b]), UnityEngine.Color.yellow, duration);
                UnityEngine.Debug.DrawLine(GetWorldPosition(ScreenVertices[b]), GetWorldPosition(ScreenVertices[c]), UnityEngine.Color.yellow, duration);
                UnityEngine.Debug.DrawLine(GetWorldPosition(ScreenVertices[c]), GetWorldPosition(ScreenVertices[a]), UnityEngine.Color.yellow, duration);
            }
#endif
        }
        #endregion funcs

#if UNITY_EDITOR
        private void OnGUI()
        {
            DebugDraw(Vector2.zero);
        }
#endif

#if UNITY_EDITOR
        #region ContextMenu
        [ContextMenu("生成顶点列表")]
        public void GeneratorBySpriteEditor()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //this.useSpriteMesh = true;
            screenVertices.Clear();
            screenVertices = TranslateSpriteVertices();

            sw.Stop();
            UnityEngine.Debug.Log($"'{GetSprite().name}'generate screen vertices count:{screenVertices.Count}, use time:{sw.ElapsedMilliseconds}ms");
        }
        #endregion
#endif
    }
}
