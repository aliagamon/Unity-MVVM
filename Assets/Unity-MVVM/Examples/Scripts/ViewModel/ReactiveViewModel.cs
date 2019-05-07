using System;
using UnityMVVM.ViewModel;
using UnityEngine;
using System.Collections;
using UniRx;
using UnityMVVM.Reactive;
using BoolReactiveProperty = UnityMVVM.Reactive.BoolReactiveProperty;
using ColorReactiveProperty = UnityMVVM.Reactive.ColorReactiveProperty;
using Random = UnityEngine.Random;
using StringReactiveProperty = UnityMVVM.Reactive.StringReactiveProperty;

namespace UnityMVVM.Examples
{
    public class ReactiveViewModel : ViewModelBase
    {
        public Reactive.ReactiveProperty<ApplicationState> State = new Reactive.ReactiveProperty<ApplicationState>();
        public UniRx.ReactiveProperty<int> IntProp = new UniRx.ReactiveProperty<int>(10);
        public StringReactiveProperty Text = new StringReactiveProperty();
        public BoolReactiveProperty BoolProp = new BoolReactiveProperty();
        public ColorReactiveProperty Color = new ColorReactiveProperty();
        public UniRx.ReactiveCommand<int> TestCommand;
        public Reactive.ReactiveCommand ChangeColor;
        public BoolReactiveProperty Flagger = new BoolReactiveProperty();

        public int Result;

        public void Awake()
        {
            TestCommand = IntProp.Select(x => x > 0).ToReactiveCommand<int>();
            TestCommand.Subscribe(val =>
            {
                Debug.unityLogger.Log($"val = {val} IntProp.Value = {IntProp.Value}");
                IntProp.Value -= 1;
            });
            ChangeColor = Observable.EveryUpdate().Select(l => l % 2 == 0).ToMVVMReactiveCommand();
            ChangeColor.Subscribe(_ => Color.Value = Random.ColorHSV());
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
