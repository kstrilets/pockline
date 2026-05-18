using System.Collections.Generic;
using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Simple MonoBehaviour object pool for GemView prefabs.
    /// Keeps inactive gems in a stack; grows the pool on demand.
    /// </summary>
    public class GemViewPool : MonoBehaviour
    {
        [SerializeField] private GemView _prefab;
        [SerializeField] private int     _initialSize = 64;

        private readonly Stack<GemView> _pool = new Stack<GemView>();

        private void Awake()
        {
            for (int i = 0; i < _initialSize; i++)
                _pool.Push(CreateInstance());
        }

        public GemView Rent()
        {
            var view = _pool.Count > 0 ? _pool.Pop() : CreateInstance();
            view.gameObject.SetActive(true);
            return view;
        }

        public void Return(GemView view)
        {
            view.gameObject.SetActive(false);
            view.transform.SetParent(transform);
            _pool.Push(view);
        }

        private GemView CreateInstance()
        {
            var go = Instantiate(_prefab, transform);
            go.gameObject.SetActive(false);
            return go;
        }
    }
}
