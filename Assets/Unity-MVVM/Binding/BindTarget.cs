using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace UnityMVVM.Binding
{
    [System.Serializable]
    public class BindTarget
    {
        public object propertyOwner;

        public string propertyName;

        public string propertyPath;

        public PropertyInfo property;

        public readonly bool IsReactive;

        public readonly bool IsCommand;

        private readonly object _command;

        public BindTarget(object propOwner, string propName, string path = null, UnityEvent dstChangedEvent = null, bool isReactive = false, bool isCommand = false)
        {
            if (isReactive)
            {
                    propertyOwner = propOwner.GetType().GetField(propName).GetValue(propOwner);
                    if (isCommand)
                    {
                        _command = propertyOwner;
                        propertyOwner = propertyOwner.GetType()
                            .GetFieldRecursive("canExecute", BindingFlags.Instance | BindingFlags.NonPublic)
                            .GetValue(propertyOwner);
                        
                    }

            }
            else
                propertyOwner = propOwner;
            propertyName = propName;
            propertyPath = path;
            IsReactive = isReactive;
            IsCommand = isCommand;

            if (propertyOwner == null)
            {
                Debug.LogErrorFormat("Could not find ViewModel for Property {0}", propName);
                return;
            }

            property = propertyOwner.GetType().GetProperty(isReactive ? "Value" : propertyName);

            if (dstChangedEvent != null)
                dstChangedEvent.AddListener(new UnityAction(() =>
                {

                }));
        }

        public object GetValue()
        {
            if (string.IsNullOrEmpty(propertyPath))
                return property != null ? property.GetValue(propertyOwner, null) : null;

            else
            {
                var parentProp = property.GetValue(propertyOwner, null);
                var parts = propertyPath.Split('.');

                object owner = parentProp;
                PropertyInfo prop = null;

                foreach (var part in parts)
                {
                    prop = owner.GetType().GetProperty(propertyPath);
                    owner = prop.GetValue(owner, null);
                }

                return owner;
            }
        }

        public void SetValue(object src)
        {
            if (property == null) return;

            if (string.IsNullOrEmpty(propertyPath))
                property.SetValue(propertyOwner, src, null) ;
            else
            {
                var parentProp = property.GetValue(propertyOwner, null);
                var parts = propertyPath.Split('.');

                object owner = parentProp;
                PropertyInfo prop = null;

                foreach (var part in parts)
                {
                    prop = owner.GetType().GetProperty(propertyPath);
                }

                prop.SetValue(owner, src, null);
            }
        }

        public IDisposable ReactiveBind(PropertyChangedEventHandler handler)
        {
            MethodInfo methodInfo;
            if (IsCommand)
                methodInfo = _command.GetType()
                    .GetMethod("NonGenericSubscribe", BindingFlags.NonPublic | BindingFlags.Instance);
            else
                methodInfo = propertyOwner.GetType()
                    .GetMethod("NonGenericSubscribe", BindingFlags.NonPublic | BindingFlags.Instance);

            return (IDisposable) methodInfo.Invoke(IsCommand ? _command : propertyOwner,
                new[]
                {
                    new Action<object>(o => handler(propertyOwner,
                        new PropertyChangedEventArgs(propertyName)))
                });
        }
    }
}


