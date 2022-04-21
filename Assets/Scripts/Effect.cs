using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public EffectData data;
    public MMF_Player endPlayer;
    public MMF_Player initPlayer;
    // Start is called before the first frame update
    private void Awake()
    {
        initPlayer.Initialization();
        endPlayer.Initialization();
    }
    void OnEnable()
    {
        initPlayer.PlayFeedbacks();
        //Cambiamos color
        if (gameObject.TryGetComponent<Renderer>(out Renderer item))
        {
            item.material.color = data.color;
            item.material.SetColor("_EmissionColor", data.color*data.intensity);
        }
        //Ponemos duración
        StartCoroutine(DelayedDeactivate());
    }
    IEnumerator DelayedDeactivate()
    {
        yield return new WaitForSeconds(5);
        endPlayer.PlayFeedbacks();
    }

    public void setEffectData(EffectData data)
    {
        this.data = data;
    }
}
