using UnityEngine;
using UnityEngine;
using UnityEngine.UIElements;
 
public class OptionsMenuController : MonoBehaviour
{
    // Default values
    private float defaultBrightness = 70f;
    private float defaultVolume     = 80f;
    private int   defaultQuality    = 2;
 
    private Slider      brightnessSlider;
    private Slider      volumeSlider;
    private DropdownField graphicsDropdown;
    private Label       brightnessLabel;
    private Label       volumeLabel;
 
    void OnEnable()
    {
        // Get the root of the UI from the UIDocument on this GameObject
        var root = GetComponent<UIDocument>().rootVisualElement;
 
        // Find each element by its "name" from the UXML file
        brightnessSlider  = root.Q<Slider>("brightnessSlider");
        volumeSlider      = root.Q<Slider>("volumeSlider");
        graphicsDropdown  = root.Q<DropdownField>("graphicsDropdown");
        brightnessLabel   = root.Q<Label>("brightnessLabel");
        volumeLabel       = root.Q<Label>("volumeLabel");
 
        // Wire up sliders so the label updates as you drag
        brightnessSlider.RegisterValueChangedCallback(evt =>
        {
            brightnessLabel.text = Mathf.RoundToInt(evt.newValue) + "%";
        });
 
        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            volumeLabel.text = Mathf.RoundToInt(evt.newValue) + "%";
        });
 
        // Wire up buttons
        root.Q<Button>("applyButton").clicked  += ApplySettings;
        root.Q<Button>("resetButton").clicked  += ResetSettings;
    }
 
    void ApplySettings()
    {
        
        AudioListener.volume = volumeSlider.value / 100f;
 
        
        QualitySettings.SetQualityLevel(graphicsDropdown.index, true);
 
        
        RenderSettings.ambientIntensity = (brightnessSlider.value / 100f) * 2f;
 
        Debug.Log("Settings applied! " +
                  "Volume: " + AudioListener.volume +
                  " | Quality: " + graphicsDropdown.value +
                  " | Brightness: " + RenderSettings.ambientIntensity);
    }
 
    void ResetSettings()
    {
        // Reset sliders and dropdown back to defaults
        brightnessSlider.value   = defaultBrightness;
        volumeSlider.value       = defaultVolume;
        graphicsDropdown.index   = defaultQuality;
 
        // Update labels
        brightnessLabel.text = Mathf.RoundToInt(defaultBrightness) + "%";
        volumeLabel.text     = Mathf.RoundToInt(defaultVolume) + "%";
 
        // Re-apply the defaults
        ApplySettings();
 
        Debug.Log("Settings reset to defaults.");
    }
}
