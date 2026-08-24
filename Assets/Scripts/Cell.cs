using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Header("Links")]
    [SerializeField]private MeshRenderer _focus;
    [SerializeField]private MeshRenderer _select;

    private bool _isClicked = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isClicked != null)
        {
            _isClicked = true;

            // Гасим Plane наведения и включаем Plane клика
            if (_focus != null) _focus.enabled = false;
            _select.enabled = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_focus!=null)
        {
            _focus.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_focus != null)
        {
            _focus.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
