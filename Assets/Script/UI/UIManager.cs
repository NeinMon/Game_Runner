using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    [Header("Managers")]
    public SettingManager settingManager;

    public void Start()
    {
        Debug.Log("Init UI manager");
        settingManager.SetDisplayName();
        settingManager.SetInputsAndButton();
    }


}
