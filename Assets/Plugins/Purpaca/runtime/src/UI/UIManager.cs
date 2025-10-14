using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Purpaca
{
    public class UIManager : MonoManagerBase<UIManager>
    {
        private EventSystem m_eventSystem;

        /// <summary>
        /// 将指定的游戏物体设置为EventSystem当前选择的UI元素
        /// </summary>
        public static void SetSelectedUIElement(GameObject target)
        {
            instance.InitEventSystem();
            instance.m_eventSystem.SetSelectedGameObject(target);
        }

        protected override void OnInit()
        {
            InitEventSystem();
        }

        /// <summary>
        /// 初始化EventSystem
        /// </summary>
        private void InitEventSystem() 
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
    }
}
