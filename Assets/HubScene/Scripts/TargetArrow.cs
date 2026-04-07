using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TargetArrow2D : MonoBehaviour
{
    [Header("Посилання")]
    public Transform player;
    public Transform target;

    [Tooltip("Колайдер гравця (обов'язково)")]
    public Collider2D playerCollider;

    [Tooltip("Колайдер цілі (BoxCollider2D)")]
    public Collider2D targetCollider;

    public LineRenderer lineRenderer;

    [Header("Лінія")]
    public float offsetFromPlayer = 0.5f;
    public float lineWidth = 0.2f;

    [Header("Пунктир (вигляд)")]
    public Color dashColor = Color.red;
    public int dashPixels = 12;
    public int gapPixels = 3;
    public float textureRepeatPerUnit = 10f;

    Texture2D dashTex;
    Material mat;
    MaterialPropertyBlock mpb;

    int texProp;
    int stProp;

    static readonly int MainTex = Shader.PropertyToID("_MainTex");
    static readonly int MainTexST = Shader.PropertyToID("_MainTex_ST");
    static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
    static readonly int BaseMapST = Shader.PropertyToID("_BaseMap_ST");

    void Awake()
    {
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();

        int w = Mathf.Max(2, dashPixels + gapPixels);
        dashTex = new Texture2D(w, 1, TextureFormat.RGBA32, false);
        dashTex.filterMode = FilterMode.Point;
        dashTex.wrapMode = TextureWrapMode.Repeat;

        for (int x = 0; x < w; x++)
        {
            bool isDash = x < dashPixels;
            dashTex.SetPixel(x, 0, isDash ? dashColor : new Color(0, 0, 0, 0));
        }
        dashTex.Apply();

        Shader sh = Shader.Find("Universal Render Pipeline/Unlit");
        if (sh == null) sh = Shader.Find("Sprites/Default");
        mat = new Material(sh);

        if (mat.HasProperty(BaseMap))
        {
            texProp = BaseMap;
            stProp = BaseMapST;
            mat.SetTexture(BaseMap, dashTex);
        }
        else
        {
            texProp = MainTex;
            stProp = MainTexST;
            mat.SetTexture(MainTex, dashTex);
        }

        lineRenderer.material = mat;
        lineRenderer.positionCount = 2;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.enabled = false;

        mpb = new MaterialPropertyBlock();

        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder = 10;
    }

    void Start()
    {
        if (!playerCollider && player) playerCollider = player.GetComponent<Collider2D>();
        if (!targetCollider && target) targetCollider = target.GetComponent<Collider2D>();
    }

    public void Show() => lineRenderer.enabled = true;

    public void Hide() => lineRenderer.enabled = false;

    void Update()
    {
        if (!lineRenderer.enabled || player == null || target == null) return;

        if (playerCollider && targetCollider)
        {
            var d = Physics2D.Distance(playerCollider, targetCollider);
            if (d.isOverlapped || d.distance <= 0f)
            {
                Hide();
                return;
            }
        }

        Vector3 dir = target.position - player.position;
        float dist = dir.magnitude;
        if (dist <= 0.001f) { Hide(); return; }

        dir.Normalize();

        Vector3 startPos = player.position + dir * offsetFromPlayer;
        Vector3 endPos = target.position;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        float lineLen = Vector3.Distance(startPos, endPos);
        float repeats = Mathf.Max(0.001f, lineLen * textureRepeatPerUnit);

        lineRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture(texProp, dashTex);
        mpb.SetVector(stProp, new Vector4(repeats, 1f, 0f, 0f));
        lineRenderer.SetPropertyBlock(mpb);
    }
}