using System;
using UnityEngine.Events;

namespace Purpaca.Events
{
    /// <summary>
    /// 非泛型的需要参数的监听者
    /// </summary>
    internal class ListenerRequiresParameter : IEventListener, IEventListener<object>
    {
        #region 字段
        private bool m_invokeWithoutParameter;
        private Type m_parameterType;
        private UnityAction<object> m_callback;
        #endregion

        #region 构造器
        /// <param name="callback">回调方法</param>
        /// <param name="parameterType">所需参数的类型</param>
        public ListenerRequiresParameter(UnityAction<object> callback, Type parameterType)
        {
            m_invokeWithoutParameter = false;
            m_callback = callback;
            m_parameterType = parameterType;
        }

        /// <param name="callback">回调方法</param>
        /// <param name="parameterType">所需参数的类型</param>
        /// <param name="invokeWithoutParameter">当监听的事件以未提供参数的方式广播时，是否调用回调方法？</param>
        public ListenerRequiresParameter(UnityAction<object> callback, Type parameterType, bool invokeWithoutParameter)
        {
            m_callback = callback;
            m_parameterType = parameterType;
            m_invokeWithoutParameter = invokeWithoutParameter;
        }
        #endregion

        #region 属性
        public Type ParameterType { get => m_parameterType; }
        #endregion

        #region Public 方法
        public void Invoke()
        {
            if (m_invokeWithoutParameter)
            {
                Invoke(null);
            }
        }

        public void Invoke(object parameter)
        {
            if(parameter == null && !m_invokeWithoutParameter) 
            {
                return;
            }

            m_callback?.Invoke(parameter);
        }
        #endregion
    }

    /// <summary>
    /// 泛型的需要参数的监听者
    /// </summary>
    /// <typeparam name="T">所需参数的类型</typeparam>
    internal class ListenerRequiresParameter<T> : IEventListener, IEventListener<T>
    {
        #region 字段
        private bool m_invokeWithoutParameter;
        private UnityAction<T> m_callback;
        #endregion

        #region 构造器
        /// <param name="callback">回调方法</param>
        public ListenerRequiresParameter(UnityAction<T> callback)
        {
            m_invokeWithoutParameter = false;
            m_callback = callback;
        }

        /// <param name="callback">回调方法</param>
        /// <param name="invokeWithoutParameter">当监听的事件以未提供参数的方式广播时，是否调用回调方法？</param>
        public ListenerRequiresParameter(UnityAction<T> callback, bool invokeWithoutParameter)
        {
            m_callback = callback;
            m_invokeWithoutParameter = invokeWithoutParameter;
        }
        #endregion

        #region Public 方法
        public void Invoke() 
        {
            if (m_invokeWithoutParameter) 
            {
                Invoke(default);
            }
        }

        public void Invoke(T parameter)
        {
            if (parameter == null && !m_invokeWithoutParameter) 
            {
                return;
            }

            m_callback?.Invoke(parameter);
        }
        #endregion
    }
}