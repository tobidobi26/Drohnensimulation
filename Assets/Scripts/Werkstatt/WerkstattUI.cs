using System.Collections.Generic;
using UnityEngine;

public class WerkstattUI : MonoBehaviour
{
    public Transform contentParent; // dein Content-Objekt
    public GameObject itemPrefab;   // dein Kachel-Prefab
    public List<Bauteil> alleBauteile;

    void Start()
    {
        BefuelleListe();
    }

    void BefuelleListe()
    {
        foreach (var teil in alleBauteile)
        {
            GameObject item = Instantiate(itemPrefab, contentParent);
            item.GetComponent<BauteilItemUI>().Setup(teil);
        }
    }
}