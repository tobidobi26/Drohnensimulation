using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BauteilItemUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;

    private Bauteil aktuellerTeil;

    public void Setup(Bauteil teil)
    {
        aktuellerTeil = teil;
        iconImage.sprite = teil.icon;
        nameText.text = teil.name;
    }

    public Bauteil GetBauteil()
    {
        return aktuellerTeil;
    }
}