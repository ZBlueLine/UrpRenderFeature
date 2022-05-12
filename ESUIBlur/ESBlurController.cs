using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ESBlurController : MonoBehaviour
{
    public int FrameRate = 10;
    public bool Static = false;
    public GameObject PauseCanvas;

    private ESUIBlur m_Esuiblur;
    private float m_TimeInterval = 1;
    private float m_TimeSinceStart = 0;


    void Start()
    {
        var VolumeObject = GameObject.Find("UIRoot/UI Linear Correct Volume");
        Volume volume = VolumeObject.GetComponent<Volume>();
        volume.profile.TryGet<ESUIBlur>(out m_Esuiblur);
        m_Esuiblur.active = false;
    }

    IEnumerator GrabCompletion()
    {
        m_Esuiblur.active = true;
        yield return null;
        m_Esuiblur.active = false;
        PauseCanvas.active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!Static)
        {
            m_TimeInterval = 1.0f / FrameRate;
            m_TimeSinceStart += Time.deltaTime;
            if (m_TimeSinceStart > m_TimeInterval)
            {
                StartCoroutine(GrabCompletion());
                m_TimeSinceStart = 0.0f;
            }
        }
    }

    public void OpenPauseCanvas()
    {
        if (Static)
        {
            StartCoroutine(GrabCompletion());
        }
        else
        {
            PauseCanvas.SetActive(true);
        }
    }
    public void ClosePauseCanvas()
    {
        PauseCanvas.SetActive(false);
    }
}
