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
    public float offsetFromPlayer = 0.5f;  // відступ від гравця
    public float lineWidth = 0.2f;         // товщина лінії

    [Header("Пунктир (вигляд)")]
    public Color dashColor = Color.red;
    public int dashPixels = 12;            // довжина штриха (в пікселях текстури)
    public int gapPixels = 3;              // проміжок між штрихами (в пікселях текстури)
    public float textureRepeatPerUnit = 10f; // щільність штрихів по довжині (більше = частіше)

    Texture2D dashTex;
    Material mat;
    MaterialPropertyBlock mpb;

    int texProp; // _MainTex або _BaseMap
    int stProp;  // _MainTex_ST або _BaseMap_ST

    static readonly int MainTex = Shader.PropertyToID("_MainTex");
    static readonly int MainTexST = Shader.PropertyToID("_MainTex_ST");
    static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
    static readonly int BaseMapST = Shader.PropertyToID("_BaseMap_ST");

    void Awake()
    {
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();

        // 1) Створюємо текстуру пунктира: [dashPixels] кольору + [gapPixels] прозорих
        int w = Mathf.Max(2, dashPixels + gapPixels);
        dashTex = new Texture2D(w, 1, TextureFormat.RGBA32, false);
        dashTex.filterMode = FilterMode.Point;      // чіткі "піксельні" краї
        dashTex.wrapMode = TextureWrapMode.Repeat;  // повторення по довжині

        for (int x = 0; x < w; x++)
        {
            bool isDash = x < dashPixels;
            dashTex.SetPixel(x, 0, isDash ? dashColor : new Color(0, 0, 0, 0));
        }
        dashTex.Apply();

        // 2) Створюємо матеріал (URP Unlit якщо є, інакше Sprites/Default)
        Shader sh = Shader.Find("Universal Render Pipeline/Unlit");
        if (sh == null) sh = Shader.Find("Sprites/Default");
        mat = new Material(sh);

        // 3) Визначаємо, яке поле текстури використовується шейдером: _BaseMap чи _MainTex
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

        // 4) Налаштовуємо LineRenderer
        lineRenderer.material = mat;
        lineRenderer.positionCount = 2;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.enabled = false;

        // 5) PropertyBlock потрібен, щоб коректно задавати тайлінг для різних шейдерів
        mpb = new MaterialPropertyBlock();

        // (необов'язково) сортування для 2D
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder = 10;
    }

    void Start()
    {
        // Якщо колайдери не вказані вручну — пробуємо знайти на об'єктах
        if (!playerCollider && player) playerCollider = player.GetComponent<Collider2D>();
        if (!targetCollider && target) targetCollider = target.GetComponent<Collider2D>();
    }

    // Показати лінію
    public void Show() => lineRenderer.enabled = true;

    // Сховати лінію
    public void Hide() => lineRenderer.enabled = false;

    void Update()
    {
        if (!lineRenderer.enabled || player == null || target == null) return;
        // 1) Ховаємо пунктир, коли гравець торкнувся колайдера цілі
        // (не залежить від Trigger/Collision подій)
        if (playerCollider && targetCollider)
        {
            var d = Physics2D.Distance(playerCollider, targetCollider);
            if (d.isOverlapped || d.distance <= 0f)
            {
                Hide();
                return;
            }
        }

        // 2) Рахуємо напрямок і виставляємо точки лінії
        Vector3 dir = target.position - player.position;
        float dist = dir.magnitude;
        if (dist <= 0.001f) { Hide(); return; }

        dir.Normalize();

        Vector3 startPos = player.position + dir * offsetFromPlayer;
        Vector3 endPos = target.position;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // 3) Налаштовуємо щільність пунктира по довжині лінії (ТІЛЬКИ ТАЙЛІНГ, БЕЗ РУХУ)
        float lineLen = Vector3.Distance(startPos, endPos);
        float repeats = Mathf.Max(0.001f, lineLen * textureRepeatPerUnit);

        // Встановлюємо scale (repeats) і offset (0) через _ST вектор
        lineRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture(texProp, dashTex);
        mpb.SetVector(stProp, new Vector4(repeats, 1f, 0f, 0f)); // (scaleX, scaleY, offsetX, offsetY)
        lineRenderer.SetPropertyBlock(mpb);
    }
}