using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform; // Ссылка на основную камеру
    public float parallaxFactor;     // Скорость движения слоя (от 0 до 1)
    
    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        // Вычисляем, насколько сдвинулась камера
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        // Двигаем слой фона вслед за камерой, умножая на коэффициент
        // Коэффициент 0 = слой движется вместе с камерой (кажется статичным, подходит для неба)
        // Коэффициент ближе к 1 = слой почти не движется за камерой (кажется очень близким к игроку)
        transform.position += new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * parallaxFactor, 0);
        
        lastCameraPosition = cameraTransform.position;
    }
}