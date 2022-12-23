using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PainterControl : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private TextMeshProUGUI percentageTxt;
    
    [SerializeField] private TexturePainter _painter;
    private Ray ray = new Ray();

    private void Awake()
    {
        _painter.Initialize(Color.red);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _painter.Paint(hit.textureCoord);
                percentageTxt.text = "Color Percentage: " + Mathf.RoundToInt(_painter.GetPercent(Color.red)) + "%";
            }
        }
    }

    private void OnDestroy()
    {
        _painter.OnDestroy();
    }
}
