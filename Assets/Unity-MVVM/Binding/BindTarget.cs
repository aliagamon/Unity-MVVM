using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

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

        public BindTarget(object propOwner, string propName, string path = null, UnityEvent dstChangedEvent = null, bool isReactive = false)
        {
            propertyOwner = isReactive ? propOwner.GetType().GetField(propName).GetValue(propOwner) : propOwner;
            propertyName = propName;
            propertyPath = path;
            IsReactive = isReactive;

            if (propertyOwner == null)
            {
                Debug.LogErrorFormat("Could not find ViewModel for Property {0}", propName);
                return;
            }

            if (isReactive)
            {
                property = propertyOwner.GetType().GetProperty("Value");
            }
            else
                property = propertyOwner.GetType().GetProperty(propertyName);//.ResolvePath(path);

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
    }
}


