using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float zForce = 1.0f;
    [SerializeField] private float _yForce = 1.0f;
    [SerializeField] private Vector3 _forceDirection;
    [SerializeField] private Vector3 _initPosition;

    [SerializeField] private GameObject _ballTemplate;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetImpulse()
    {
        //transform.position = _initPosition;
        _rigidbody.ResetInertiaTensor();
        _rigidbody.velocity = Vector3.zero;
        
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _rigidbody.AddForce(_forceDirection, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            if (rigidbody.mass > 10)
            {
                _forceDirection = new Vector3(0, 10, 0);
                SetImpulse();
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
      
    }

    

    // Update is called once per frame
   
}
