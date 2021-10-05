using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTo : MonoBehaviour {
    [SerializeField] private GameObject _followTo;
    [SerializeField] private Transform _transform;
    
    private void Update() {
        _transform.position = new Vector3(_followTo.transform.position.x, _transform.position.y);
    }
}
