using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsPool : MonoBehaviour
{
    [SerializeField]
    private int quantity = 10;

    [SerializeField]
    private GameObject prefab;

    public List<GameObject> prefabList = new List<GameObject>();

    public int Quantity { get => quantity; set => quantity = value; }

    void Awake()
    {
        for (int i = 0; i < Quantity; i++)
        {
            GameObject temp_go = Instantiate(prefab, transform);
            temp_go.transform.SetParent(transform);
            temp_go.name += " " + i.ToString();
            prefabList.Add(temp_go);
            temp_go.SetActive(false);
        }
    }
    public void ActivateEffect(EffectData data, Vector3 position)
    {
        foreach (GameObject go in prefabList)
        {
            if (!go.activeSelf)
            {
                if (go.TryGetComponent<Effect>(out Effect temp))
                {
                    temp.setEffectData(data);
                    go.transform.position = position;
                    go.SetActive(true);
                    return;
                }
            }
        }
    }
    private void ActivateAll()
    {
        foreach (GameObject go in prefabList)
        {
            if (!go.activeSelf)
            {
                if (go.TryGetComponent<Effect>(out Effect temp))
                {
                    go.SetActive(true);
                }
            }
        }
    }
}
