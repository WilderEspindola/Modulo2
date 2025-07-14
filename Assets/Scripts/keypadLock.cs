using UnityEngine;
using TMPro;

public class KeypadLock : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshPro passCodeDisplay;
    public GameObject[] keyButtons; // Todos los objetos Key_*

    private string currentCode = "";
    private string savedCode = "?"; // Inicializado con "?"

    void Start()
    {
        passCodeDisplay.text = savedCode; // Mostrar "?" al inicio
        SetKeypadVisible(false); // Ocultar botones al iniciar
    }

    public void ToggleKeypad()
    {
        bool newState = !keyButtons[0].activeSelf;
        SetKeypadVisible(newState);

        // Al mostrar, limpiamos el código actual (excepto si ya estaba visible)
        if (newState) currentCode = "";
    }

    private void SetKeypadVisible(bool visible)
    {
        foreach (var button in keyButtons)
            button.SetActive(visible);

        passCodeDisplay.text = visible ? currentCode : savedCode;
    }

    public void AddDigit(string digit)
    {
        if (currentCode.Length < 10) // Limitar a 10 dígitos
            currentCode += digit;

        passCodeDisplay.text = currentCode;
    }

    public void ClearCode()
    {
        currentCode = "";
        passCodeDisplay.text = currentCode;
    }

    public void SaveCode()
    {
        savedCode = currentCode;
        if (string.IsNullOrEmpty(savedCode)) savedCode = "?"; // Volver a "?" si está vacío
        SetKeypadVisible(false); // Ocultar botones
    }

    public string GetSavedCode() => savedCode;
}