using GGJ2026.Gameplay;
using GGJ2026.Gameplay.ScriptableObjects;
using GGJ2026.UI;
using System.Collections;
using UnityEngine;

namespace GGJ2026
{
    /// <summary>
    /// Transform交互组件 - 3D物体的Transform工具，提供游戏内Transform操作功能
    /// </summary>
    [RequireComponent(typeof(Feature))]
    public class TransformUtils : MonoBehaviour
    {
        // 静态全局选择管理器
        private static TransformUtils currentlySelected = null;
        private static TransformUtils currentlyDragging = null;

        [Header("位置信息")]
        [SerializeField] private FeaturePositionData positionData;

        [Header("操作设置")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotateSpeed = 360f;
        [SerializeField] private float scaleSpeed = 1f;
        [SerializeField] private KeyCode selectKey = KeyCode.Mouse0;
        [SerializeField] private KeyCode moveKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode scaleUpKey = KeyCode.W;
        [SerializeField] private KeyCode scaleDownKey = KeyCode.S;
        [SerializeField] private KeyCode rotateLeftKey = KeyCode.A;
        [SerializeField] private KeyCode rotateRightKey = KeyCode.D;

        [Header("视觉反馈")]
        [SerializeField] private Color selectionColor = Color.yellow;
        [SerializeField] private float selectionIntensity = 1.3f;
        [SerializeField] private float smoothness = 0.3f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("约束设置")]
        [SerializeField] private bool enablePosition = true;
        [SerializeField] private bool enableRotation = true;
        [SerializeField] private bool enableScale = true;
        [SerializeField] private bool lockXAxis = false;
        [SerializeField] private bool lockYAxis = false;
        [SerializeField] private bool lockZAxis = false;

        // 内部状态
        private bool isSelected = false;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Camera mainCamera;
        private Feature feature;
        private Vector3 lastPosition;
        private bool inSmoothMoving = false;

        #region Unity生命周期

        void Awake()
        {
            // 获取组件引用
            feature = GetComponent<Feature>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            mainCamera = Camera.main;

            // 检查关键组件是否存在
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null)
            {
                Debug.LogError($"物体 {gameObject.name} 缺少Collider组件，射线检测将失败！请添加Collider组件。");
            }
            else if (!collider.enabled)
            {
                Debug.LogWarning($"物体 {gameObject.name} 的Collider组件被禁用，射线检测将失败！");
            }

            if (spriteRenderer == null)
            {
                Debug.LogError($"物体 {gameObject.name} 缺少SpriteRenderer组件，无法设置颜色效果！");
            }

            if (mainCamera == null)
            {
                Debug.LogError("未找到主相机！");
            }

            // 保存原始颜色
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
        }

        void Start()
        {
            // 初始状态为非选中
            SetSelectionVisual(false);
            UpdatePositionType(false);
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (currentlyDragging != null)
                {
                    currentlyDragging.UpdatePositionType(true);
                    AudioManager.Instance.PlaySFX("DropFeature");
                    UIEventSystem.Instance.Publish(UIEventTypes.TIPS_HIDE);
                    currentlyDragging = null;
                }
            }

            if (currentlyDragging != null)
            {
                return;
            }

            // 每帧检测鼠标悬停状态并处理选择逻辑
            HandleSelectionLogic();

            if (Input.GetMouseButtonDown(0))
            {
                if (currentlySelected != null)
                {
                    AudioManager.Instance.PlaySFX("clickSound");
                    UIEventSystem.Instance.Publish(UIEventTypes.TIPS_SHOW);
                }

                currentlyDragging = currentlySelected;
            }

            // 如果被选中，处理操作
            if (isSelected)
            {
                HandleTransformOperations();
            }
        }

        void OnMouseDrag()
        {
            // Debug.LogError($"OnMouseDrag：{(currentlyDragging != null ? currentlyDragging.name : "null")}");

            // 拖拽移动 - 只有当前正在拖拽的物体才能移动
            PerformDragMovement(currentlyDragging);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 选择该物体
        /// </summary>
        public void Select()
        {
            // 取消之前选中的物体
            if (currentlySelected != null && currentlySelected != this)
            {
                currentlySelected.Deselect();
            }

            isSelected = true;
            currentlySelected = this;

            OnSelectionChanged();

            OnShowDesc();

            // Debug.LogError($"Select {currentlySelected.name}");
        }

        /// <summary>
        /// 取消选择该物体
        /// </summary>
        public void Deselect()
        {
            // Debug.LogError($"Deselect {currentlySelected.name}");

            isSelected = false;
            if (currentlySelected == this)
            {
                currentlySelected = null;
            }
            OnSelectionChanged();

            UIEventSystem.Instance.Publish(UIEventTypes.DESC_HIDE);
        }

        private void OnShowDesc()
        {
            UIEventSystem.Instance.Publish(UIEventTypes.DESC_SHOW,
                    new ItemData(feature.Name, feature.Description, feature.Scale));
        }

        public void UpdatePositionType(bool doSmooth)
        {
            if (!TryGetComponent<Feature>(out var feature))
            {
                return;
            }

            var curPosition = transform.position;

            if (lastPosition != curPosition)
            {
                if (positionData)
                {
                    feature.Position = positionData.GetCurPositionType(feature.Type, curPosition);
                }

                // Debug.LogError($"计算SmoothPosition: {feature.Position}, curPosition: {transform.position}, lastPosition: {lastPosition}");
                lastPosition = curPosition;
            }

            if (feature.Position == PositionType.None)
            {
                return;
            }

            // Debug.LogError($"执行SmoothPosition, targetPos: {feature.Position}");

            if (doSmooth)
            {
                StartCoroutine(SmoothPosition(feature.Position));
            }
            else
            {
                MoveTo(positionData.Convert2Pos(feature.Position));
            }
        }

        private IEnumerator SmoothPosition(PositionType positionType)
        {
            inSmoothMoving = true;
            float elapsed = 0f;
            var startPos = transform.position;
            var targetPos = positionData.Convert2Pos(positionType);

            if (Vector3.Distance(startPos, targetPos) < 0.001f)
            {
                transform.position = targetPos;
                yield break;
            }

            while (elapsed < smoothness)
            {
                elapsed += Time.deltaTime;
                // 计算归一化时间 (0 到 1)
                float t = Mathf.Clamp01(elapsed / smoothness);
                float curveT = animationCurve.Evaluate(t);

                // 使用 Lerp 进行平滑插值
                transform.position = Vector3.Lerp(startPos, targetPos, curveT);
                yield return null;
            }

            transform.position = targetPos;
        }

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        public void MoveTo(Vector3 targetPosition)
        {
            if (!enablePosition) return;

            Vector3 newPosition = targetPosition;
            if (lockXAxis) newPosition.x = transform.position.x;
            if (lockYAxis) newPosition.y = transform.position.y;
            if (lockZAxis) newPosition.z = transform.position.z;

            transform.position = newPosition;
        }

        /// <summary>
        /// 旋转到指定角度
        /// </summary>
        public void RotateBy(RotationType rotationType)
        {
            if (!enableRotation) return;
            Vector3 newRotation = rotationType switch
            {
                RotationType.Up => Vector3.zero,
                RotationType.UpRight => new Vector3(0, 0, -45),
                RotationType.Right => new Vector3(0, 0, -90),
                RotationType.DownRight => new Vector3(0, 0, -135),
                RotationType.Down => new Vector3(0, 0, -180),
                RotationType.DownLeft => new Vector3(0, 0, -225),
                RotationType.Left => new Vector3(0, 0, -270),
                RotationType.UpLeft => new Vector3(0, 0, -315),
                _ => Vector3.zero
            };
            if (lockXAxis) newRotation.x = transform.eulerAngles.x;
            if (lockYAxis) newRotation.y = transform.eulerAngles.y;
            if (lockZAxis) newRotation.z = transform.eulerAngles.z;

            transform.eulerAngles = newRotation;
        }

        /// <summary>
        /// 缩放指定比例
        /// </summary>
        public void ScaleBy(ScaleType scaleType)
        {
            if (!enableScale) return;

            Vector3 newScale = scaleType switch
            {
                ScaleType.Small => Vector2.one * 0.7f,
                ScaleType.Medium => Vector2.one,
                ScaleType.Large => Vector2.one * 1.4f,
                _ => Vector2.one
            };

            if (lockXAxis) newScale.x = transform.localScale.x;
            if (lockYAxis) newScale.y = transform.localScale.y;
            if (lockZAxis) newScale.z = transform.localScale.z;

            // 防止缩放到负值
            newScale = Vector3.Max(newScale, Vector3.one * 0.1f);
            transform.localScale = newScale;
        }

        #endregion

        #region 私有实现方法

        private void HandleSelectionLogic()
        {
            if (Camera.main == null)
            {
                return;
            }

            // 执行射线检测
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);

            // 找到最上层的可交互物体
            TransformUtils topObject = null;
            int highestPriority = int.MinValue;

            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    TransformUtils obj = hit.collider.GetComponent<TransformUtils>();
                    if (obj != null)
                    {
                        // 计算优先级：Sorting Layer + Sorting Order + Sibling Index
                        SpriteRenderer sr = obj.spriteRenderer;
                        if (sr != null)
                        {
                            int priority = SortingLayer.GetLayerValueFromID(sr.sortingLayerID) * 10000 +
                                          sr.sortingOrder * 100 +
                                          obj.transform.GetSiblingIndex();

                            if (priority > highestPriority)
                            {
                                highestPriority = priority;
                                topObject = obj;
                            }
                        }
                    }
                }
            }

            // 处理选择逻辑
            if (topObject != null)
            {
                // 点击了某个可交互物体
                if (topObject == this)
                {
                    // 点击了本物体 -> 选择
                    if (!isSelected)
                    {
                        Select();
                    }
                }
                else
                {
                    // 点击了其他物体 -> 取消本物体的选择
                    if (isSelected)
                    {
                        Deselect();
                    }
                }
            }
            else
            {
                // 点击了空白处 -> 取消所有选择
                if (isSelected)
                {
                    Deselect();
                }
            }
        }

        private void HandleTransformOperations()
        {
            // 缩放操作
            if (enableScale)
            {
                if (Input.GetKeyDown(scaleUpKey))
                {
                    var next = ((int)feature.Scale + 1) % 3;
                    feature.Scale = (ScaleType)next;
                    ScaleBy((ScaleType)next);
                    OnShowDesc();
                    AudioManager.Instance.PlaySFX("clickSound");
                }
                if (Input.GetKeyDown(scaleDownKey))
                {
                    var last = ((int)feature.Scale - 1 + 3) % 3;
                    feature.Scale = (ScaleType)last;
                    ScaleBy((ScaleType)last);
                    OnShowDesc();
                    AudioManager.Instance.PlaySFX("clickSound");
                }
            }

            // 旋转操作
            if (enableRotation)
            {
                if (Input.GetKeyDown(rotateRightKey))
                {
                    var next = ((int)feature.Rotation + 1) % 8;
                    feature.Rotation = (RotationType)next;
                    RotateBy((RotationType)next);
                    OnShowDesc();
                    AudioManager.Instance.PlaySFX("clickSound");
                }
                if (Input.GetKeyDown(rotateLeftKey))
                {
                    var last = ((int)feature.Rotation + 7) % 8;
                    feature.Rotation = (RotationType)last;
                    RotateBy((RotationType)last);
                    OnShowDesc();
                    AudioManager.Instance.PlaySFX("clickSound");
                }
            }

            // 拖拽操作（在OnMouseDrag中处理）
        }

        private void PerformDragMovement(TransformUtils transformUtils)
        {
            if (!mainCamera || !transformUtils || !currentlyDragging)
            {
                return;
            }

            // 获取鼠标在屏幕上的位置
            Vector3 mousePosition = Input.mousePosition;

            // 限制鼠标位置在屏幕范围内
            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Screen.height);

            // 计算基于深度的世界坐标
            float depth = mainCamera.WorldToScreenPoint(transformUtils.transform.position).z;
            Vector3 targetWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, depth));

            // 应用移动平滑和速度控制
            Vector3 newPosition = transformUtils.transform.position;
            Vector3 moveDelta = (targetWorldPosition - newPosition) * moveSpeed * Time.deltaTime;

            // 应用轴锁定
            if (!lockXAxis) newPosition.x += moveDelta.x;
            if (!lockYAxis) newPosition.y += moveDelta.y;
            if (!lockZAxis) newPosition.z += moveDelta.z;

            currentlyDragging.transform.position = newPosition;
        }

        private Color GetSelectionColor()
        {
            // 计算选中颜色，增强亮度
            Color enhancedColor = selectionColor * selectionIntensity;
            enhancedColor.a = originalColor.a; // 保持原始透明度
            return enhancedColor;
        }

        private void SetSelectionVisual(bool selected)
        {
            if (spriteRenderer == null) return;

            if (selected)
            {
                // 应用选中颜色
                spriteRenderer.color = GetSelectionColor();
            }
            else
            {
                // 恢复原始颜色
                spriteRenderer.color = originalColor;
            }
        }



        private void OnSelectionChanged()
        {
            SetSelectionVisual(isSelected);

            // 可以在这里触发选择事件
            Debug.Log($"{gameObject.name} {(isSelected ? "selected" : "deselected")}" +
                $"- 当前位置: {transform.position} - 当前旋转: {feature.Rotation} - 当前缩放: {feature.Scale}");
        }

        #endregion

        #region 属性访问器

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected => isSelected;

        /// <summary>
        /// 是否正在拖拽
        /// </summary>
        public bool IsDragging => currentlyDragging != null;

        /// <summary>
        /// 是否正在平滑移动
        /// </summary>
        public bool InSmoothMoving => inSmoothMoving;

        /// <summary>
        /// 当前位置
        /// </summary>
        public Vector3 Position => transform.position;

        /// <summary>
        /// 当前旋转
        /// </summary>
        public Vector3 Rotation => transform.eulerAngles;

        /// <summary>
        /// 当前缩放
        /// </summary>
        public Vector3 Scale => transform.localScale;

        #endregion
    }
}
