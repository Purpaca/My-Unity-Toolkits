using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Purpaca.UI
{
    /// <summary>
    /// UI面板基类
    /// </summary>
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public abstract class UIPanelBase : MonoBehaviour
    {
        #region 字段

        #region 成员字段
        private Canvas m_canvas;
        private CanvasGroup m_canvasGroup;

        private UIPanelBase previousPanel = null;
        private UIPanelBase nextPanel = null;

        private bool _isPanelActive = false;
        private UIPanelBase _root, _top;
        #endregion

        #region 静态字段
        private static EventSystem m_eventSystem;
        #endregion

        #endregion

        #region 属性
        /// <summary>
        /// 此UI面板是否处于激活状态？
        /// </summary>
        public bool IsPanelActive { get => _isPanelActive; }

        /// <summary>
        /// 此UI面板所在UI面板栈的最底层面板（谨慎）
        /// </summary>
        public UIPanelBase RootPanel
        {
            get
            {
                _root = _root == null ? this : _root;
                while (_root.PreviousPanel != null)
                {
                    _root = _root.PreviousPanel;
                    _root._root = _root;
                }

                return _root;
            }
        }

        /// <summary>
        /// 此UI面板所在UI面板栈的最上层面板
        /// </summary>
        public UIPanelBase TopPanel
        {
            get
            {
                _top = _top == null ? this : _top;
                while (_top.NextPanel != null)
                {
                    _top = _top.NextPanel;
                    _top._top = _top;
                }

                return _top;
            }
        }

        /// <summary>
        /// 此UI面板上层的UI面板
        /// </summary>
        public UIPanelBase NextPanel { get => nextPanel; private set => nextPanel = value; }

        /// <summary>
        /// 此UI面板下层的UI面板
        /// </summary>
        public UIPanelBase PreviousPanel { get => previousPanel; private set => previousPanel = value; }

        protected Canvas Canvas
        {
            get
            {
                if (m_canvas == null)
                {
                    m_canvas = GetComponent<Canvas>();
                }

                return m_canvas;
            }
        }

        protected CanvasGroup CanvasGroup
        {
            get
            {
                if (m_canvasGroup == null)
                {
                    m_canvasGroup = GetComponent<CanvasGroup>();
                }

                return m_canvasGroup;
            }
        }

        /// <summary>
        /// 是否在UI面板被创建时自动激活此UI面板？
        /// </summary>
        protected abstract bool ActivateOnAwake { get; }
        #endregion

        #region Public 方法
        /// <summary>
        /// 将指定的游戏物体设置为EventSystem当前选择的UI元素
        /// </summary>
        public static void SetSelectedUIElement(GameObject target)
        {
            InitEventSystem();
            m_eventSystem.SetSelectedGameObject(target);
        }

        /// <summary>
        /// 将一个UI面板设置到当前UI面板的顶层
        /// </summary>
        /// <param name="hidePrevious">是否隐藏之前的顶层UI面板？（无论是否隐藏，都将失活之前的顶层UI面板）</param>
        public void SetUIPanelToTop(UIPanelBase panel, bool hidePrevious = true)
        {
            var top = TopPanel;

            top.NextPanel = panel;
            top.CanvasGroup.alpha = hidePrevious ? 0 : top.CanvasGroup.alpha;
            top.CanvasGroup.interactable = false;
            top.OnDeactive();
            top._isPanelActive = false;

            panel.PreviousPanel = top;

            if (!panel.enabled) panel.enabled = true;
            if (!panel.gameObject.activeInHierarchy) panel.gameObject.SetActive(true);
            panel.Canvas.sortingOrder = top.Canvas.sortingOrder + 1;

            if (!_isPanelActive)
            {
                panel.OnActivate();
                panel.CanvasGroup.alpha = 1;
                panel.CanvasGroup.interactable = true;
                _isPanelActive = true;
            }
        }

        /// <summary>
        /// 关闭此UI面板（会连带依次关闭所有在此UI面板上层的UI面板）
        /// </summary>
        public void Close()
        {
            Destroy(gameObject);
        }
        #endregion

        #region Protected 方法
        /// <summary>
        /// 尝试激活此UI面板（如果此UI面板非其当前所在UI面板栈的顶部UI面板，则无法激活）
        /// </summary>
        protected void Activate() 
        {
            if (TopPanel != this) 
            {
                Debug.LogWarning($"无法激活UI面板 \"{GetType().FullName} (GameObject : {gameObject.name})\"，它不是其所在UI面板栈的顶部UI面板！");
                return;
            }
            else if (!_isPanelActive)
            {
                CanvasGroup.alpha = 1;
                CanvasGroup.interactable = true;
                OnActivate();
                _isPanelActive = true;
            }
        }

        /// <summary>
        /// 当此UI面板初始化时调用
        /// <c>（将在组件的Awake()生命周期方法中执行）</c>
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// 当此UI面板被激活时调用
        /// </summary>
        protected virtual void OnActivate() { }

        /// <summary>
        /// 当此UI面板被失活时调用（当此UI面板被销毁时不会调用此方法）
        /// </summary>
        protected virtual void OnDeactive() { }

        /// <summary>
        /// 当此UI面板被关闭时（或此UI面板被销毁时）调用
        /// </summary>
        protected virtual void OnClose() { }
        #endregion

        #region Private 方法
        /// <summary>
        /// 初始化EventSystem
        /// </summary>
        private static void InitEventSystem()
        {
            if (m_eventSystem == null)
            {
                if (EventSystem.current != null)
                {
                    m_eventSystem = EventSystem.current;
                }
                else
                {
                    var esObj = new GameObject("EventSystem");
                    m_eventSystem = esObj.AddComponent<EventSystem>();
                    esObj.AddComponent<InputSystemUIInputModule>();
                }
            }

            m_eventSystem.SetSelectedGameObject(null);
            DontDestroyOnLoad(m_eventSystem.gameObject);
        }

        /// <summary>
        /// 激活此UI面板下层的（先前的）UI面板
        /// </summary>
        private void ActivatePreviousPanel()
        {
            if (PreviousPanel != null)
            {
                PreviousPanel.CanvasGroup.alpha = 1;
                PreviousPanel.CanvasGroup.interactable = true;
                previousPanel.OnActivate();
                previousPanel._isPanelActive = true;
            }
        }
        #endregion

        #region Unity 消息
        private void Awake()
        {
            InitEventSystem();

            m_canvas = GetComponent<Canvas>();
            m_canvasGroup = GetComponent<CanvasGroup>();

            OnInit();

            if (ActivateOnAwake)
            {
                Activate();
            }
            else
            {
                CanvasGroup.alpha = 0;
                CanvasGroup.interactable = false;
                _isPanelActive = false;
            }
        }

        private void OnDestroy()
        {
            if (NextPanel != null)
            {
                NextPanel.Close();
            }

            ActivatePreviousPanel();
            OnClose();
        }
        #endregion
    }
}