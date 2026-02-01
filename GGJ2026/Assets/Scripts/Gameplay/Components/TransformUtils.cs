using UnityEngine;
using UnityEngine.EventSystems;

namespace GGJ2026
{
    /// <summary>
    /// Transform交互组件 - 3D物体的Transform工具，提供游戏内Transform操作功能
    /// </summary>
    public class TransformUtils : MonoBehaviour
    {
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

        [Header("约束设置")]
        [SerializeField] private bool enablePosition = true;
        [SerializeField] private bool enableRotation = true;
        [SerializeField] private bool enableScale = true;
        [SerializeField] private bool lockXAxis = false;
        [SerializeField] private bool lockYAxis = false;
        [SerializeField] private bool lockZAxis = false;

        // 内部状态
        private bool isSelected = false;
        private bool isDragging = false;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Camera mainCamera;

        #region Unity生命周期

        void Awake()
        {
            // 获取组件引用
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
        }

        void Update()
        {
            // 处理选择逻辑
            HandleSelection();

            // 如果被选中，处理操作
            if (isSelected)
            {
                HandleTransformOperations();
            }
        }

        void OnMouseDown()
        {
            isDragging = true;
        }

        void OnMouseDrag()
        {
            // 拖拽移动
            if (isSelected && enablePosition)
            {
                PerformDragMovement();
            }
        }

        private void OnMouseUp()
        {
            isDragging = false;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 选择该物体
        /// </summary>
        public void Select()
        {
            isSelected = true;
            OnSelectionChanged();
        }

        /// <summary>
        /// 取消选择该物体
        /// </summary>
        public void Deselect()
        {
            isSelected = false;
            isDragging = false;
            OnSelectionChanged();
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
                RotationType.North => Vector3.zero,
                RotationType.Northeast => new Vector3(0, 0, -45),
                RotationType.East => new Vector3(0, 0, -90),
                RotationType.Southeast => new Vector3(0, 0, -135),
                RotationType.South => new Vector3(0, 0, -180),
                RotationType.Southwest => new Vector3(0, 0, -225),
                RotationType.West => new Vector3(0, 0, -270),
                RotationType.Northwest => new Vector3(0, 0, -315),
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

        private void HandleSelection()
        {
            // 安全检查
            if (mainCamera == null)
            {
                Debug.LogWarning("Main camera not found, selection disabled");
                return;
            }

            // 执行射线检测（简化版本，避免复杂问题）
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // 使用最简单的射线检测，排除所有可能的干扰
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);
            RaycastHit2D raycastHit = default;

            if (hits.Length > 0)
            {
                raycastHit = hits[0];
                SpriteRenderer bestSR = null;
                SpriteRenderer currentSR;

                foreach (var hit in hits)
                {
                    SpriteRenderer sr = hit.collider.GetComponent<SpriteRenderer>();
                    if (sr)
                    {
                        // 1. 比较 Sorting Layer (层级)
                        // SortingLayer.GetLayerValueFromID 将 ID 转为 Inspector 里的排序权值
                        currentSR = sr;
                        bestSR = bestSR == null ? sr: bestSR;
                        int currentLayerValue = SortingLayer.GetLayerValueFromID(currentSR.sortingLayerID);
                        int bestLayerValue = SortingLayer.GetLayerValueFromID(bestSR.sortingLayerID);

                        if (currentLayerValue > bestLayerValue)
                        {
                            raycastHit = hit;
                            bestSR = currentSR;
                        }
                        else if (currentLayerValue == bestLayerValue)
                        {
                            // 2. 层级相同时，比较 Sorting Order (数字)
                            if (currentSR.sortingOrder > bestSR.sortingOrder)
                            {
                                raycastHit = hit;
                                bestSR = currentSR;
                            }
                            else if (currentSR.sortingOrder == bestSR.sortingOrder)
                            {
                                // 3. 数字也相同时，比较 Hierarchy 里的位置 (SiblingIndex)
                                // 索引越大，代表在面板越靠下，即越晚渲染（遮挡上方）
                                if (currentSR.transform.GetSiblingIndex() > bestSR.transform.GetSiblingIndex())
                                {
                                    raycastHit = hit;
                                    bestSR = currentSR;
                                }
                            }
                        }
                    }
                }
                Debug.Log("你真正点到的是最上层的：" + raycastHit.collider.name);
            }

            // 调试信息
            if (raycastHit)
            {
                Debug.Log($"射线命中: {raycastHit.collider?.gameObject.name} (Layer: {raycastHit.collider?.gameObject.layer})");
            }
            else
            {
                Debug.Log("射线未命中任何物体");
            }

            bool clickedThisObject = raycastHit && raycastHit.collider != null && raycastHit.collider.gameObject == gameObject;

            // 处理选择状态变化
            if (clickedThisObject)
            {
                // 点击了本物体 -> 切换选择状态
                if (!isSelected)
                {
                    Select();
                }
                // 如果已经选中，保持选中状态（不取消选择）
            }
            else if (isSelected && !isDragging)
            {
                // 点击了空白处且当前已选中且未拖拽 -> 取消选择
                Deselect();
            }
        }

        private void HandleTransformOperations()
        {
            
            if (!TryGetComponent<Feature>(out var feature))
            {
                Debug.LogWarning("Feature not found, transform operations disabled");
                return;
            }

            // 缩放操作
            if (enableScale)
            {
                if (Input.GetKeyDown(scaleUpKey))
                {
                    var next = ((int)feature.Scale + 1) % 3;
                    feature.Scale = (ScaleType)next;
                    ScaleBy((ScaleType)next);
                }
                if (Input.GetKeyDown(scaleDownKey))
                {
                    var last = ((int)feature.Scale - 1 + 3) % 3;
                    feature.Scale = (ScaleType)last;
                    ScaleBy((ScaleType)last);
                }
            }

            // 旋转操作
            if (enableRotation)
            {
                if (Input.GetKeyDown(rotateLeftKey))
                {
                    var next = ((int)feature.Rotation - 1 + 8) % 8;
                    feature.Rotation = (RotationType)next;
                    RotateBy((RotationType)next);
                }
                if (Input.GetKeyDown(rotateRightKey))
                {
                    var last = ((int)feature.Rotation + 1) % 8;
                    feature.Rotation = (RotationType)last;
                    RotateBy((RotationType)last);
                }
            }

            // 拖拽操作（在OnMouseDrag中处理）
        }

        private void PerformDragMovement()
        {
            if (mainCamera == null) return;

            // 获取鼠标在屏幕上的位置
            Vector3 mousePosition = Input.mousePosition;

            // 限制鼠标位置在屏幕范围内
            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Screen.height);

            // 计算基于深度的世界坐标
            float depth = mainCamera.WorldToScreenPoint(transform.position).z;
            Vector3 targetWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, depth));

            // 应用移动平滑和速度控制
            Vector3 newPosition = transform.position;
            Vector3 moveDelta = (targetWorldPosition - newPosition) * moveSpeed * Time.deltaTime;

            // 应用轴锁定
            if (!lockXAxis) newPosition.x += moveDelta.x;
            if (!lockYAxis) newPosition.y += moveDelta.y;
            if (!lockZAxis) newPosition.z += moveDelta.z;

            transform.position = newPosition;
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

            var feature = GetComponent<Feature>();
            if (feature)
            {
                // 可以在这里触发选择事件
                Debug.Log($"{gameObject.name} {(isSelected ? "selected" : "deselected")}" +
                $"- 当前位置: {transform.position} - 当前旋转: {feature.Rotation} - 当前缩放: {feature.Scale}");
            }
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
        public bool IsDragging => isDragging;

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
