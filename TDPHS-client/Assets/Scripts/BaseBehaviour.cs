using UnityEngine;
using System.Collections;
namespace LP
{
    public class BaseBehaviour : MonoBehaviour
    {
        private Transform _transform;
        private GameObject _gameObject;

        public Transform CachedTransform
        {
            get
            {
                if(_transform == null && this != null)
                    _transform = transform;
                return _transform;
            }
        }

        public GameObject CachedGameObject
        {
            get
            {
                if(_gameObject == null && this != null)
                    _gameObject = gameObject;
                return _gameObject;
            }
        }
    }
}
