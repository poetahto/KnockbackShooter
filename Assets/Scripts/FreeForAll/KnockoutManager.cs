using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace FreeForAll
{
    public class KnockoutManager : MonoBehaviour
    {
        private readonly Subject<KnockoutData> _playerKnockedOut = new();
        private readonly List<GameObject> _targets = new();
        private readonly HashSet<KnockoutData> _queuedKnockouts = new();
        
        [SerializeField] 
        private KnockoutStrategy[] strategies;

        [SerializeField] 
        private GameObject[] autoRegister;
        
        public IReadOnlyList<GameObject> Targets => _targets;
        
        public IObservable<KnockoutData> AddObject(GameObject target)
        {
            _targets.Add(target);
            
            return _playerKnockedOut
                .Where(data => data.Object == target)
                .Take(1);
        }

        public void OnKnockout(KnockoutData data)
        {
            if (_queuedKnockouts.All(knockoutData => knockoutData.Object != data.Object))
                _queuedKnockouts.Add(data);
        }

        private void Start()
        {
            foreach (GameObject target in autoRegister)
            {
                AddObject(target).Subscribe(data => print($"knocked out {data.Object.name}!"));
            }
            
            foreach (KnockoutStrategy strategy in strategies)
            {
                strategy.Manager = this;
                strategy.Initialize();
            }
        }

        private void Update()
        {
            foreach (KnockoutStrategy strategy in strategies)
                strategy.OnLogic();

            foreach (KnockoutData data in _queuedKnockouts)
            {
                _targets.Remove(data.Object);
                _playerKnockedOut.OnNext(data);
            }
            
            _queuedKnockouts.Clear();
        }

        public struct KnockoutData
        {
            public GameObject Object;
        }
    }
}