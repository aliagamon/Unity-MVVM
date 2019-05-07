using System;
using UniRx;
using UnityMVVM.ViewModel;
using UnityEngine;
using System.Collections;
using BoolReactiveProperty = UnityMVVM.Reactive.BoolReactiveProperty;
using ColorReactiveProperty = UnityMVVM.Reactive.ColorReactiveProperty;
using Random = UnityEngine.Random;
using StringReactiveProperty = UnityMVVM.Reactive.StringReactiveProperty;

namespace UnityMVVM.Examples
{
    public class ReactiveViewModel : ViewModelBase
    {
        public Reactive.ReactiveProperty<ApplicationState> State = new Reactive.ReactiveProperty<ApplicationState>();
        public Reactive.ReactiveProperty<int> IntProp = new Reactive.ReactiveProperty<int>(10);
        public StringReactiveProperty Text = new StringReactiveProperty();
        public BoolReactiveProperty BoolProp = new BoolReactiveProperty();
        public ColorReactiveProperty Color = new ColorReactiveProperty();
        public ReactiveCommand<int> TestCommand;
        public ReactiveCommand ChangeColor = new ReactiveCommand();

        public int Result;

        public void Awake()
        {
            TestCommand = IntProp.Select(x => x > 0).ToReactiveCommand<int>();
            TestCommand.Subscribe(val =>
            {
                Debug.unityLogger.Log($"val = {val} IntProp.Value = {IntProp.Value}");
                IntProp.Value -= 1;
            });
            ChangeColor.Subscribe((_) => Color.Value = Random.ColorHSV());
        }

        public void Start()
        {
            Text.Value = DateTime.Now.ToShortTimeString();
            StartCoroutine(StateChangeRoutine());
        }

        public void Update()
        {
            if(DateTime.Now.Second % 5 == 0)
                Text.Value = DateTime.Now.ToShortTimeString();
            BoolProp.Value = (DateTime.Now.Second % 2 == 0);
        }

        private IEnumerator StateChangeRoutine()
        {
            while (true)
            {
                State.Value = (ApplicationState)((int)(State.Value + 1) % 3);
                yield return new WaitForSeconds(3f);
            }
        }
    }
}
