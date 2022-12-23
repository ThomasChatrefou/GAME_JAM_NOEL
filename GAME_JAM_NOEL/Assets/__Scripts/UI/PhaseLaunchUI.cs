using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PhaseLaunchUI : MonoBehaviour
{
    //[]
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        text.text = $" just started !";
    }
}
