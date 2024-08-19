using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TalentReward : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Talent talent; // 包含的天赋信息
    private CanvasGroup canvasGroup;
    private bool isDragging = false;
    private Transform parentTransform;

    private float angle; // 当前的角度位置
    private float radius; // 距离中心点的半径
    private Vector2 originalSize; // 原始大小
    private Vector2 originalAnchorPosition; // 原始锚点位置

    private RectTransform rectTransform;
    private Image image; // 用于调整形状

    private Coroutine sizeCoroutine;
    private Coroutine positionCoroutine;

    public TextMeshProUGUI textMesh;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        parentTransform = transform.parent;
        // 记录原始大小
        originalSize = rectTransform.sizeDelta;
        originalAnchorPosition = rectTransform.anchoredPosition;

        // 计算半径（距离中心点的距离）
        radius = Vector3.Distance(transform.position, parentTransform.position);
        // 计算初始角度
        angle = Mathf.Atan2(transform.position.y - parentTransform.position.y, transform.position.x - parentTransform.position.x);

        GetComponent<Image>().color = talent.color;

        textMesh = transform.GetComponentInChildren<TextMeshProUGUI>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        canvasGroup.blocksRaycasts = false; // 允许拖动穿透
        canvasGroup.alpha = 0.6f; // 拖动时降低透明度

        // 停止正在进行的放大缩小动画
        if (sizeCoroutine != null)
            StopCoroutine(sizeCoroutine);

        // 启动缩小到圆形的渐变动画
        sizeCoroutine = StartCoroutine(ScaleOverTime(rectTransform.sizeDelta, new Vector2(50f, 50f), 0.2f)); // 变为小圆形
        textMesh.text = talent.type.ToString();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 使用屏幕坐标更新位置，确保拖动操作在UI中正确显示
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            Input.mousePosition,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        // 更新位置
        GetComponent<RectTransform>().anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;

        // 停止正在进行的放大缩小动画
        if (sizeCoroutine != null)
            StopCoroutine(sizeCoroutine);

        // 启动恢复到原始大小的渐变动画
        sizeCoroutine = StartCoroutine(ScaleOverTime(rectTransform.sizeDelta, originalSize, 0.2f));

        // 检查是否放置到天赋槽内
        TalentSlot slot = CheckForSlot();
        if (slot != null)
        {
            slot.FillSlotWithTalent(talent); // 将天赋装备到槽位上
            Destroy(gameObject); // 销毁天赋奖励
        }
        else
        {
            // 如果没有放置到槽内，启动位置渐变动画
            if (positionCoroutine != null)
                StopCoroutine(positionCoroutine);

            positionCoroutine = StartCoroutine(MoveToOriginalPosition(0.5f));
        }

        textMesh.text = talent.description;
    }

    private IEnumerator ScaleOverTime(Vector2 fromSize, Vector2 toSize, float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            rectTransform.sizeDelta = Vector2.Lerp(fromSize, toSize, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.sizeDelta = toSize;
    }

    private IEnumerator MoveToOriginalPosition(float duration)
    {
        Vector3 startPosition = rectTransform.anchoredPosition;
        Vector3 targetPosition = new Vector3(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius,
            0
        );

        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = targetPosition;
    }

    private TalentSlot CheckForSlot()
    {
        // 检查所有天赋槽位是否在鼠标位置附近
        TalentSlot[] slots = FindObjectsOfType<TalentSlot>();
        foreach (TalentSlot slot in slots)
        {
            RectTransform rectTransform = slot.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
            {
                return slot; // 找到合适的槽位
            }
        }
        return null;
    }
}
