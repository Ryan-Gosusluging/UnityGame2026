using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFactory : MonoBehaviour
{
    
    [SerializeField] private GameObject _ballTemplate;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ball = Instantiate(_ballTemplate);
            ball.GetComponent<BallScript>().SetImpulse();
        }
    }
}
