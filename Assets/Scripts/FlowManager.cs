using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FlowManager : MonoBehaviour
{
    // Valores de los cubos
    public int A, B, C, D, E, F;

    // Valores actuales en los sockets
    public int SocketValue1 = 0;
    public int SocketValue2 = 0;
    public int SocketValue3 = 0;
    public int SocketValue4 = 0;
    public int SocketValue5 = 0;
    public int SocketValue6 = 0;

    public List<GameObject> Sockets = new List<GameObject>();
    public GameObject HandAnimation;
    public bool IsPracticeMode = false;

    void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Configuración inicial con ambos gestos visibles
        GameManager.Instance.UI_Messages.text = "Usa ✋ Thumbs Up para practicar o 🤙 Shaka para comenzar";
        GameManager.Instance.Timer.enabled = false;
        GameManager.Instance.MathematicsValues.gameObject.SetActive(false);
        GameManager.Instance.RightThumbsUp.gameObject.SetActive(true);
        GameManager.Instance.RightShaka.gameObject.SetActive(true);
        GameManager.Instance.LeftThumbsUp.gameObject.SetActive(false);
        DisableSockets();
        HandAnimation.SetActive(false);
        IsPracticeMode = false;
    }

    public void RightHandThumpsUpPerformed()
    {
        // Entrar en modo práctica
        IsPracticeMode = true;
        GameManager.Instance.UI_Messages.text = "Modo Práctica: Coloca los cubos para practicar. Usa ✋ Thumbs Up izquierdo para salir.";
        GameManager.Instance.RightThumbsUp.gameObject.SetActive(false);
        GameManager.Instance.RightShaka.gameObject.SetActive(false);
        GameManager.Instance.LeftThumbsUp.gameObject.SetActive(true);
        GameManager.Instance.MathematicsValues.gameObject.SetActive(true);
        EnableSockets();
        HandAnimation.SetActive(true);
    }

    public void RightShakaPerformed()
    {
        // Comenzar juego directamente
        IsPracticeMode = false;
        GameManager.Instance.UI_Messages.text = "¡Comienza el juego! Coloca los cubos para igualar ambos lados.";
        GameManager.Instance.RightThumbsUp.gameObject.SetActive(false);
        GameManager.Instance.RightShaka.gameObject.SetActive(false);
        GameManager.Instance.MathematicsValues.gameObject.SetActive(true);
        EnableSockets();
        GenerateValuesABCDEF();
        StartCountdown();
    }

    public void LeftHandThumpsUpPerformed()
    {
        if (IsPracticeMode)
        {
            // Salir del modo práctica
            InitializeGame();
        }
        else
        {
            // Reiniciar escena al finalizar el juego
            RestartScene();
        }
    }

    public void StartCountdown()
    {
        GameManager.Instance.Timer.enabled = true;
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        int countdownTime = 10;

        while (countdownTime >= 0)
        {
            GameManager.Instance.Timer.GetComponent<TextMeshProUGUI>().text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        OnCountdownFinished();
    }

    private void OnCountdownFinished()
    {
        GameManager.Instance.Timer.enabled = false;
        CalculateValue();
    }

    public void CalculateValue()
    {
        // Reiniciar valores antes de cada cálculo
        SocketValue1 = 0;
        SocketValue2 = 0;
        SocketValue3 = 0;
        SocketValue4 = 0;
        SocketValue5 = 0;
        SocketValue6 = 0;

        // Actualizar los valores en función de los cubos colocados
        for (int i = 0; i < Sockets.Count; i++)
        {
            UpdateSocketValue(i);
        }

        int leftSum = SocketValue1 + SocketValue2 + SocketValue3;
        int rightSum = SocketValue4 + SocketValue5 + SocketValue6;

        Debug.Log($"Left Sum: {leftSum} / Right Sum: {rightSum}");

        if (leftSum == rightSum && (leftSum != 0 || rightSum != 0))
        {
            GameManager.Instance.UI_Messages.text = $"✅ Correcto! {leftSum} = {rightSum}\nUsa ✋ Thumbs Up izquierdo para jugar de nuevo.";
            GameManager.Instance.LeftThumbsUp.gameObject.SetActive(true);
        }
        else
        {
            GameManager.Instance.UI_Messages.text = $"❌ Incorrecto! {leftSum} ≠ {rightSum}\nUsa ✋ Thumbs Up izquierdo para reintentar.";
            GameManager.Instance.LeftThumbsUp.gameObject.SetActive(true);
        }
    }

    public void UpdateSocketValue(int socketIndex)
    {
        var interactor = Sockets[socketIndex].GetComponent<XRSocketInteractor>();
        if (interactor == null)
        {
            SetSocketValue(socketIndex, 0);
            return;
        }

        var selected = interactor.GetOldestInteractableSelected();
        if (selected == null)
        {
            SetSocketValue(socketIndex, 0);
            return;
        }

        string cubeName = selected.transform.name;
        int value = GetCubeValueByName(cubeName);
        SetSocketValue(socketIndex, value);
    }

    private void SetSocketValue(int index, int value)
    {
        switch (index)
        {
            case 0: SocketValue1 = value; break;
            case 1: SocketValue2 = value; break;
            case 2: SocketValue3 = value; break;
            case 3: SocketValue4 = value; break;
            case 4: SocketValue5 = value; break;
            case 5: SocketValue6 = value; break;
        }
    }

    public int GetCubeValueByName(string cubeName)
    {
        switch (cubeName)
        {
            case "CubeA": return A;
            case "CubeB": return B;
            case "CubeC": return C;
            case "CubeD": return D;
            case "CubeE": return E;
            case "CubeF": return F;
            default: return 0;
        }
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void DisableSockets()
    {
        foreach (var socket in Sockets)
        {
            if (socket != null)
            {
                socket.SetActive(false);
                socket.GetComponent<XRSocketInteractor>().enabled = false;
            }
        }
    }

    private void EnableSockets()
    {
        foreach (var socket in Sockets)
        {
            if (socket != null)
            {
                socket.SetActive(true);
                socket.GetComponent<XRSocketInteractor>().enabled = true;
            }
        }
    }

    public void GenerateValuesABCDEF()
    {
        int[] validNumbers = { 1, 2, 3 };

        A = validNumbers[UnityEngine.Random.Range(0, validNumbers.Length)];
        B = validNumbers[UnityEngine.Random.Range(0, validNumbers.Length)];
        C = validNumbers[UnityEngine.Random.Range(0, validNumbers.Length)];
        D = validNumbers[UnityEngine.Random.Range(0, validNumbers.Length)];
        E = validNumbers[UnityEngine.Random.Range(0, validNumbers.Length)];
        F = validNumbers[UnityEngine.Random.Range(0, validNumbers.Length)];

        Transform values = GameManager.Instance.MathematicsValues.transform;

        try
        {
            values.GetChild(0).GetComponent<TextMeshPro>().text = A.ToString(); // A
            values.GetChild(2).GetComponent<TextMeshPro>().text = B.ToString(); // B
            values.GetChild(4).GetComponent<TextMeshPro>().text = C.ToString(); // C
            values.GetChild(6).GetComponent<TextMeshPro>().text = D.ToString(); // D
            values.GetChild(8).GetComponent<TextMeshPro>().text = E.ToString(); // E
            values.GetChild(10).GetComponent<TextMeshPro>().text = F.ToString(); // F
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error assigning values to text elements: " + e.Message);
        }

        Debug.Log($"Generated values: A={A}, B={B}, C={C} | D={D}, E={E}, F={F}");
    }
}