using UnityEngine;

namespace XEngine
{
    public class XBehaviour : MonoBehaviour
    {
        private Transform _transform;
        private GameObject _gameObject;

        /// <summary>
        /// 缓存的Transform组件
        /// </summary>
        public Transform CachedTransform
        {
            get
            {
                if (_transform == null && this != null)
                    _transform = transform;
                return _transform;
            }
        }

        /// <summary>
        /// 通过缓存关系获取
        /// </summary>
        public GameObject CachedGameObject
        {
            get
            {
                if (_gameObject == null && this != null)
                    _gameObject = gameObject;
                return _gameObject;
            }
        }
    }
}