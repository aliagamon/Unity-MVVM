using System;
using UnityMVVM.ViewModel;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityMVVM.Examples
{
    public class ReactiveViewModel : ViewModelBase
    {
        public ReactiveProperty<int> IntProp = new ReactiveProperty<int>(10);
        public ReactiveCommand<int> TestCommand;

        public void Awake()
        {
            TestCommand = IntProp.Select(x => x > 0).ToReactiveCommand<int>();
            TestCommand.Subscribe(val =>
            {
                Debug.unityLogger.Log($"val = {val} IntProp.Value = {IntProp.Value}");
                IntProp.Value -= 1;
            });
        }
    }
}
