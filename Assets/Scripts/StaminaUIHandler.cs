using UnityEngine;
using UnityEngine.UI;


public class StaminaUIHandler : MonoBehaviour
{

    public PlayerContext playerContext;
    public Image Foreground;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Foreground.fillAmount = playerContext.Stamina / 100;
    }
}
