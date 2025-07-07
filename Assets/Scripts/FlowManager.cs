using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FlowManager : MonoBehaviour
{
    public int A, B, C, D, E, F; // Valores aleatorios para cubos

    // Variables para guardar el valor colocado en cada socket
    public int SocketValue1 = 0;
    public int SocketValue2 = 0;
    public int SocketValue3 = 0;
    public int SocketValue4 = 0;
    public int SocketValue5 = 0;
    public int SocketValue6 = 0;

    public List<GameObject> Sockets = new List<GameObject>(); // Lista con 6 sockets
    public GameObject Socket4;
    public GameObject HandAnimation;

    void Start()
    {
        GameManager.Instance.UI_Messages.text = "Hi! Let's Calculate Numbers. Do a thumbs up to move ahead.";
        GameManager.Instance.Timer.enabled = false;
        GameManager.Instance.MathematicsValues.gameObject.SetActive(false);
        GameManager.Instance.RightThumbsUp.gameObject.SetActive(true);
        GameManager.Instance.LeftThumbsUp.gameObject.SetActive(false);
        GameManager.Instance.RightShaka.gameObject.SetActive(false);
        DisableSockets();
        Socket4.SetActive(false);
        HandAnimation.SetActive(false);
    }

    public void RightHandThumpsUpPerformed()
    {
        GameManager.Instance.RightShaka.gameObject.SetActive(true);
        GameManager.Instance.MathematicsValues.gameObject.SetActive(true);
        EnableSockets();
        Socket4.SetActive(true);
        GameManager.Instance.RightThumbsUp.gameObject.SetActive(false);
        GameManager.Instance.UI_Messages.text = "Place the cubes in the sockets so that the left side equals the right side. Perform a Shaka gesture to START.";
        HandAnimation.SetActive(true);
    }

    public void RightShakaPerformed()
    {
        GameManager.Instance.RightShaka.gameObject.SetActive(false);
        HandAnimation.SetActive(false);
        GenerateValuesABCDEF();
        StartCountdown();
        Socket4.SetActive(true);
    }

    public void LeftHandThumpsUpPerformed()
    {
        RestartScene();
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

    // Actualiza el valor almacenado para un socket específico según el cubo colocado
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
            default: break;
        }
    }

    public void CalculateValue()
    {
        // Primero actualizamos los valores de cada socket
        for (int i = 0; i < Sockets.Count; i++)
        {
            UpdateSocketValue(i);
        }

        int leftSum = SocketValue1 + SocketValue2 + SocketValue3;
        int rightSum = SocketValue4 + SocketValue5 + SocketValue6;

        Debug.Log($"Suma Izquierda: {leftSum}, Suma Derecha: {rightSum}");

        if (leftSum == rightSum)
        {
            GameManager.Instance.UI_Messages.text = "✅ Correct! Both sides are equal.\nRaise your left hand and do a thumbs-up to play again.";
            GameManager.Instance.LeftThumbsUp.gameObject.SetActive(true);
        }
        else
        {
            GameManager.Instance.UI_Messages.text = "❌ Incorrect! Try again.\nRaise your left hand and do a thumbs-up to retry.";
            GameManager.Instance.LeftThumbsUp.gameObject.SetActive(true);
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
        catch (Exception e)
        {
            Debug.LogError("Error al asignar textos: " + e.Message);
        }

        Debug.Log($"Generados: A={A}, B={B}, C={C} / D={D}, E={E}, F={F}");
    }
}
