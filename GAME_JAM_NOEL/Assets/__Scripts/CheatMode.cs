using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(KonamiCode))]
public class CheatMode : MonoBehaviour
{
    private KonamiCode code;

    public Text successText;

    void Awake()
    {
        code = GetComponent<KonamiCode>();
    }

    void Update()
    {
        if (code.success)
        {
            successText.gameObject.SetActive(true);
        }
    }
}
