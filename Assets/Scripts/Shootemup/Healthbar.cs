using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField]
    private Image healthImage;
    [SerializeField]
    private Image backImage;

    // Start is called before the first frame update

    public void UpdateHealth(float health)
    {
        float clampedHealth = Mathf.Clamp01(health);
        healthImage.fillAmount = clampedHealth;

        float hue = health / 3f;

        healthImage.color = Color.HSVToRGB(hue, 1f, 1f);

        if (health <= 0f)
        {
            healthImage.enabled = false;
            backImage.enabled = false;
        }
    }
}
