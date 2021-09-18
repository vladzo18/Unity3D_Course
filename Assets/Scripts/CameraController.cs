using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    
    [SerializeField] private Transform _cameraPosition;
    [SerializeField] private Transform _unitPosition;
    
    [SerializeField] private float _verticalOffset;
    
    private void Update() {
        float horisontalPosition = _unitPosition.position.x;
        float verticalPosition = _unitPosition.position.y + _verticalOffset;
        
        _cameraPosition.position = new Vector3(horisontalPosition, verticalPosition, -10);
    }
}
