using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ButtonControls : MonoBehaviour
{
    [SerializeField]
    private UniversalRendererData UniversalRendererData;
    private ScriptableRendererFeature[] RendererFeatures;
    [SerializeField]
    private Material KuwaharaMaterial;
    [SerializeField]
    private Material BoxBlurMaterial;

    [SerializeField] private Slider KuwaharaSlider;
    [SerializeField] private Slider BoxSlider;
    private void Start()
    {
        RendererFeatures = UniversalRendererData.rendererFeatures.ToArray();
        foreach (var rendererFeature in RendererFeatures)
        {
            Debug.Log(rendererFeature.name);    
        }
        
    }

    public void CannyEdgeDetector()
    {
        foreach (var rendererFeature in RendererFeatures)
        {
            rendererFeature.SetActive(false);
        }
        RendererFeatures[0].SetActive(true);
    }
    public void CannyEdgeDetectorOverlay()
    {
        foreach (var rendererFeature in RendererFeatures)
        {
            rendererFeature.SetActive(false);
        }
        RendererFeatures[1].SetActive(true);
    }
    public void BoxBlur()
    {
        foreach (var rendererFeature in RendererFeatures)
        {
            rendererFeature.SetActive(false);
        }
        RendererFeatures[2].SetActive(true);
    }
    public void Kuwahara()
    {
        foreach (var rendererFeature in RendererFeatures)
        {
            rendererFeature.SetActive(false);
        }
        RendererFeatures[3].SetActive(true);
    }

    public void KuwaharaSliderSetter()
    {
        KuwaharaMaterial.SetFloat("_Range",KuwaharaSlider.value);
    }
    public void BoxSliderSetter()
    {
        BoxBlurMaterial.SetFloat("_Range",BoxSlider.value);
    }
    public void None()
    {
        foreach (var rendererFeature in RendererFeatures)
        {
            rendererFeature.SetActive(false);
        }
    }
}
