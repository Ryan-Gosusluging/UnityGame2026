using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FormConfig
{
    public string FormName;
    public Sprite FormSprite;
    public float MoveSpeed = 5f;
    public float JumpForce = 10f;
    public float Acceleration = 15f;
    public float Mass = 1f;
    public float GravityScale = 1f;
    public bool CanPushObjects = true;
}

public class PlayerFormManager : MonoBehaviour, IPlayerStatsProvider
{
    [Header("Формы")]
    [SerializeField] private List<FormConfig> _forms = new List<FormConfig>();

    [Header("Компоненты")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D _rigidbody;

    [SerializeField] private LayerMask _groundLayer = 1 << 8;

    private int _currentFormIndex = 0;

    public float MoveSpeed => GetCurrentForm().MoveSpeed;
    public float JumpForce => GetCurrentForm().JumpForce;
    public float GroundAcceleration => GetCurrentForm().Acceleration;
    public float Mass => GetCurrentForm().Mass;
    public float GravityScale => GetCurrentForm().GravityScale;
    public bool CanPushObjects => GetCurrentForm().CanPushObjects;
    public LayerMask GroundLayer => _groundLayer;

    public event Action<FormConfig> OnFormChanged;

    private void Awake()
    {
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>();

        if (_forms.Count > 0)
        {
            ApplyForm(0);
        }
    }

    public void SwitchToNextForm()
    {
        if (_forms.Count <= 1) return;

        _currentFormIndex++;
        if (_currentFormIndex >= _forms.Count)
        {
            _currentFormIndex = 0; 
        }

        ApplyForm(_currentFormIndex);
    }

    public void SwitchToPreviousForm()
    {
        if (_forms.Count <= 1) return;

        _currentFormIndex--;
        if (_currentFormIndex < 0)
        {
            _currentFormIndex = _forms.Count - 1; 
        }

        ApplyForm(_currentFormIndex);
    }

    public void SwitchToFormByIndex(int Index)
    {
        if (Index < 0 || Index >= _forms.Count) return;

        _currentFormIndex = Index;
        ApplyForm(_currentFormIndex);
    }

    private void ApplyForm(int Index)
    {
        if (Index < 0 || Index >= _forms.Count) return;

        FormConfig form = _forms[Index];

        if (_spriteRenderer != null)
            _spriteRenderer.sprite = form.FormSprite;

        if (_rigidbody != null)
        {
            _rigidbody.mass = form.Mass;
            _rigidbody.gravityScale = form.GravityScale;
        }


        if (form.FormName == "Tiny")
            transform.localScale = Vector3.one * 0.5f;
        else
            transform.localScale = Vector3.one;

        OnFormChanged?.Invoke(form);
    }

    private FormConfig GetCurrentForm()
    {
        if (_forms.Count == 0)
        {
            Debug.LogWarning("Нет форм! Создаю заглушку.");
            return new FormConfig();
        }
        return _forms[_currentFormIndex];
    }

    public List<FormConfig> GetAllForms() => _forms;
    public int CurrentFormIndex => _currentFormIndex;
    public string CurrentFormName => GetCurrentForm().FormName;
}