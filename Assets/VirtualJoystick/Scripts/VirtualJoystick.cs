using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 虚拟摇杆类
[System.Serializable]
[RequireComponent(typeof(Image), typeof(RectTransform))]
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // 摇杆图像
    public Image controlStick;

    // 调试模式：是否在控制台输出轴值
    [Header("Debug")]
    public bool consolePrintAxis = false;

    // 设置
    [Header("Settings")]
    public bool onlyOnMobile = false; // 是否只在移动平台上启用
    public Color dragColor = new Color(0.9f, 0.9f, 0.9f, 1f); // 拖动时的颜色
    public float sensitivity = 2f; // 灵敏度
    [Range(0, 2)] public float radius = 0.7f; // 半径
    [Range(0, 1)] public float deadzone = 0.3f; // 死区
    public bool edgeSnap; // 是否只在边缘吸附
    [Range(0, 16)] public int directions = 0; // 方向数，0为自由移动
    public bool snapsToTouch = false; // 是否吸附到触摸点
    public Rect boundaries; // 边界区域

    // 私有变量
    static bool isBoundaries; // 是否在边界区域
    Vector2 desiredPosition, axis, origin;
    Color originalColor; // 存储原始颜色
    int currentPointerId = -2;

    // 所有活动的VirtualJoystick实例
    private static List<VirtualJoystick> instances = new List<VirtualJoystick>();

    // 版本信息
    public const string VERSION = "1.0.2";
    public const string DATE = "3 April 2024";

    Vector2Int lastScreen; // 记录上一次的屏幕大小

    // 获取活动实例数
    public static int CountActiveInstances()
    {
        int count = 0;
        foreach (VirtualJoystick j in instances)
        {
            if (j.isActiveAndEnabled) count++;
        }
        return count;
    }

    // 获取指定索引的实例的轴值
    public static float GetAxis(string axe, int index = 0)
    {
        switch (axe.ToLower())
        {
            case "horizontal":
            case "h":
            case "x":
                return instances[index].axis.x;
            case "vertical":
            case "v":
            case "y":
                return instances[index].axis.y;
        }
        return 0;
    }

    // 获取指定索引的实例的原始轴值(0或1/-1)
    public static float GetAxisRaw(string axe, int index = 0)
    {
        float f = GetAxis(axe, index);
        if (Mathf.Approximately(f, 0)) return 0;
        return Mathf.Sign(GetAxis(axe, index));
    }

    // 获取摇杆的半径
    public float GetRadius()
    {
        RectTransform t = transform as RectTransform;
        if (t) return radius * t.rect.width * 0.5f;
        return radius;
    }

    // 处理指针按下事件
    public void OnPointerDown(PointerEventData data)
    {
        currentPointerId = data.pointerId;
        SetPosition(data.position);
        controlStick.color = dragColor;
    }

    // 处理指针抬起事件
    public void OnPointerUp(PointerEventData data)
    {
        desiredPosition = transform.position;
        controlStick.color = originalColor;
        currentPointerId = -2;
    }

    // 设置期望位置
    protected void SetPosition(Vector2 position)
    {
        // 计算期望位置与摇杆中心的位置差
        Vector2 diff = position - (Vector2)transform.position;

        // 如果没有设置方向数，则自由移动
        if (directions <= 0)
        {
            desiredPosition = (Vector2)transform.position + Vector2.ClampMagnitude(diff, GetRadius());
        }
        else
        {
            // 计算最近的方向向量
            Vector2 snapDirection = SnapDirection(diff.normalized, directions, 360 / directions * Mathf.Deg2Rad);
            if ((diff / GetRadius()).magnitude > deadzone)
            {
                desiredPosition = (Vector2)transform.position + snapDirection * GetRadius();
            }
            else if (!edgeSnap)
            {
                desiredPosition = position;
            }
            else
            {
                desiredPosition = (Vector2)transform.position + snapDirection * diff.magnitude;
            }
        }
    }

    // 计算最近的方向向量
    private Vector2 SnapDirection(Vector2 vector, int directions, float symmetryAngle)
    {
        Vector2 symmetryLine = new Vector2(Mathf.Cos(symmetryAngle), Mathf.Sin(symmetryAngle));
        float angle = Vector2.SignedAngle(symmetryLine, vector);
        angle /= 180f / directions;
        angle = (angle >= 0f) ? Mathf.Floor(angle) : Mathf.Ceil(angle);
        if ((int)Mathf.Abs(angle) % 2 == 1)
        {
            angle += (angle >= 0f) ? 1 : -1;
        }
        angle *= 180f / directions;
        angle *= Mathf.Deg2Rad;
        Vector2 result = new Vector2(Mathf.Cos(angle + symmetryAngle), Mathf.Sin(angle + symmetryAngle));
        result *= vector.magnitude;
        return result;
    }

    // 自动寻找并赋值controlStick
    void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Image img = transform.GetChild(i).GetComponent<Image>();
            if (img)
            {
                controlStick = img;
                break;
            }
        }
    }

    // 获取边界区域
    public Rect GetBounds()
    {
        return new Rect(boundaries.x, boundaries.y, Screen.width * boundaries.width, Screen.height * boundaries.height);
    }

#if UNITY_EDITOR
    // 在场景视图中绘制摇杆和边界的可视化表示
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GetRadius());

        if (GetBounds().size.sqrMagnitude > 0)
        {
            Gizmos.color = Color.yellow;
            Vector3 a = new Vector3(boundaries.x, boundaries.y),
                    b = new Vector3(boundaries.x, boundaries.y + Screen.height * boundaries.height),
                    c = new Vector2(boundaries.x + Screen.width * boundaries.width, boundaries.y + Screen.height * boundaries.height),
                    d = new Vector3(boundaries.x + Screen.width * boundaries.width, boundaries.y);
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }

        Gizmos.color = Color.green;
    }
#endif

    void OnEnable()
    {
        if (!Application.isMobilePlatform && onlyOnMobile)
        {
            Debug.LogWarning("VirtualJoystick: This component is disabled on non-mobile platforms.");
            gameObject.SetActive(false);
            return;
        }

        origin = desiredPosition = transform.position;
        StartCoroutine(Activate());
        originalColor = controlStick.color;
        lastScreen = new Vector2Int(Screen.width, Screen.height);
        instances.Insert(0, this);
    }

    // 在一帧后重新设置摇杆的初始位置,解决画布缩放问题
    IEnumerator Activate()
    {
        yield return new WaitForEndOfFrame();
        origin = desiredPosition = transform.position;
    }

    void OnDisable()
    {
        instances.Remove(this);
    }

    void Update()
    {
        PositionUpdate();

        if (lastScreen.x != Screen.width || lastScreen.y != Screen.height)
        {
            lastScreen = new Vector2Int(Screen.width, Screen.height);
            OnEnable();
        }

        if (currentPointerId > -2)
        {
            if (currentPointerId > -1)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);
                    if (t.fingerId == currentPointerId)
                    {
                        SetPosition(t.position);
                        break;
                    }
                }
            }
            else
            {
                SetPosition(Input.mousePosition);
            }
        }

        controlStick.transform.position = Vector2.MoveTowards(controlStick.transform.position, desiredPosition, sensitivity);

        axis = (controlStick.transform.position - transform.position) / GetRadius();
        if (axis.magnitude < deadzone / 2) axis = Vector2.zero;

        if (axis.magnitude > deadzone)
        {
            isBoundaries = true;
        }
        else
        {
            isBoundaries = false;
        }

        if (axis.sqrMagnitude > 0)
        {
            string output = string.Format("Virtual Joystick ({0}): {1}", name, axis);
            if (consolePrintAxis) Debug.Log(output);
        }
    }

    // 判断是否在边界内
    public static bool IsRadiusBound()
    {
        return !isBoundaries;
    }

    // 处理输入事件,更新摇杆位置
    void PositionUpdate()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                switch (t.phase)
                {
                    case TouchPhase.Began:
                        if (GetBounds().Contains(t.position) && currentPointerId < -1)
                        {
                            Uproot(t.position, t.fingerId);
                            return;
                        }
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (currentPointerId == t.fingerId)
                            OnPointerUp(new PointerEventData(EventSystem.current));
                        break;
                }
            }

        }
        else if (Input.GetMouseButtonDown(0) && currentPointerId < -1)
        {
            if (GetBounds().Contains(Input.mousePosition))
            {
                Uproot(Input.mousePosition);
            }
        }
        else
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector2 keyboardInput = new Vector2(horizontal, vertical);
            if (keyboardInput.magnitude > 0)
            {
                SetPosition((Vector2)transform.position + keyboardInput * GetRadius());
                controlStick.color = dragColor;
                currentPointerId = 0;
            }
            else if (currentPointerId == 0)
            {
                SetPosition(origin);
                controlStick.color = originalColor;
            }
        }

        if (Input.GetMouseButtonUp(0) && currentPointerId == -1)
        {
            OnPointerUp(new PointerEventData(EventSystem.current));
        }
    }

    // 将摇杆重新定位到新的位置,并模拟拖动事件
    public void Uproot(Vector2 newPos, int newPointerId = -1)
    {
        if (Vector2.Distance(transform.position, newPos) < radius) return;

        transform.position = newPos;
        desiredPosition = transform.position;

        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = newPos;
        data.pointerId = newPointerId;
        OnPointerDown(data);
    }
}
