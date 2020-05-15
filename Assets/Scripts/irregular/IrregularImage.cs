using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class IrregularImage : Image
{
    #region params
    [SerializeField]
    private Vector2[] screenVertices;
    public Vector2[] ScreenVertices {
        get {
            return screenVertices;
        }
    }
    #endregion params

    #region funcs

    protected override void Awake()
    {
    }
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
#if UNITY_EDITOR
        DebugDraw(screenPoint);
#endif
        return Utils.Point.IsPointInTriangles(sprite.triangles, ScreenVertices, screenPoint);
    }

    private Vector3 GetWorldPosition(Vector2 vertice) {
        return this.transform.TransformPoint(CalcSpriteVertice(vertice));
    }

    private List<Vector2> TranslateSpriteVertices()
    {
        List<Vector2> vertices = new List<Vector2>();
        for (int i = 0; i < sprite.vertices.Length; i++)
        {
            Vector2 vec = GetWorldPosition(sprite.vertices[i]);
            vertices.Add(vec);
        }
        return vertices;
    }

    private Vector2 CalcSpriteVertice(Vector2 vertice)
    {
        Vector2 vec = Vector2.zero;
        vec.x = Mathf.Clamp((vertice.x - sprite.bounds.center.x - (sprite.textureRectOffset.x / sprite.texture.width) + sprite.bounds.extents.x) / (2.0f * sprite.bounds.extents.x) * sprite.rect.width, 0.0f, sprite.rect.width) - sprite.rect.width / 2.0f;
        vec.y = Mathf.Clamp((vertice.y - sprite.bounds.center.y - (sprite.textureRectOffset.y / sprite.texture.height) + sprite.bounds.extents.y) / (2.0f * sprite.bounds.extents.y) * sprite.rect.height, 0.0f, sprite.rect.height) - sprite.rect.height / 2.0f;
        return vec;
    }

    private void DebugDraw(Vector2 screenPoint)
    {
#if UNITY_EDITOR
        Vector3 forward = new Vector3(screenPoint.x, screenPoint.y, 0f) + Vector3.forward * 0.1f;
        UnityEngine.Debug.DrawRay(new Vector3(screenPoint.x, screenPoint.y, 0), forward, UnityEngine.Color.green, 1f);

        //draw triangles
        float duration = 10.0f;
        ushort[] triangles = sprite.triangles;
        Vector2[] vertices = ScreenVertices;

        int triangles_count = triangles.Length / 3;
        for (int i = 0; i < triangles_count; i++)
        {
            var idx = i * 3;
            var a = triangles[idx];
            var b = triangles[idx + 1];
            var c = triangles[idx + 2];

            UnityEngine.Debug.DrawLine(vertices[a], vertices[b], UnityEngine.Color.green, duration);
            UnityEngine.Debug.DrawLine(vertices[b], vertices[c], UnityEngine.Color.green, duration);
            UnityEngine.Debug.DrawLine(vertices[c], vertices[a], UnityEngine.Color.green, duration);
        }
#endif
    }
    #endregion funcs

#if UNITY_EDITOR
    #region ContextMenu
    [ContextMenu("Generate Screen Vertices")]
    public void GeneratorBySpriteEditor()
    {
#if UNITY_EDITOR
        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif

        List<Vector2> vertices = TranslateSpriteVertices();
        screenVertices = vertices.ToArray();

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"'{this.sprite.name}'generate screen vertices use time:{sw.ElapsedMilliseconds}ms");
#endif
    }
    #endregion 
#endif
}