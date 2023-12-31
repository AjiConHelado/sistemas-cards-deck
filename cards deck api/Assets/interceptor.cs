using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class interceptor : MonoBehaviour
{
    public List<Character> characters { get; private set; }
    public UnityEvent charactersLoaded;
    public static interceptor manager;
    public int[] deck = { 0, 0, 0, 0, 0 };

    [SerializeField] private TMP_InputField field;
    [SerializeField] private TMP_Text errorMessage, userName;
    private string api = "https://pokeapi.co/api/v2/pokemon/";
    private string server = "https://my-json-server.typicode.com/AjiConHelado/sistemas-cards-deck/users/";
    private int index = 0;
    private string id;

    private void Awake()
    {
        characters = new List<Character>();
        manager = this;
    }

    #region CardRelated

    public void Request()
    {
        errorMessage.text = "";
        characters.Clear();
        GetCharactersMethod(deck);
    }

    public void GetCharactersMethod(int[] cards)
    {
        StartCoroutine(GetCharacter(cards));
    }

    public void SetImage(int index, RawImage img)
    {
        StartCoroutine(DownloadImage("https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/"+ characters.ElementAt(index).id +".png", img));
    }

    IEnumerator GetCharacter(int[] cards)
    {
        errorMessage.text = "";

        UnityWebRequest www = UnityWebRequest.Get(api + cards[index]);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Connection Error: " + www.error);
            errorMessage.text = "Connection Error: " + www.error;
        }
        else
        {
            if (www.responseCode == 200)
            {
                string responseText = www.downloadHandler.text;

                Character character = JsonUtility.FromJson<Character>(responseText);
                characters.Add(character);
            }
            else
            {
                string message = "Status: " + www.responseCode;
                message += "\nContent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                errorMessage.text = message;
            }
        }

        if (index < 4)
        {
            index++;
            StartCoroutine(GetCharacter(cards));
        }
        else
        {
            index = 0;
            charactersLoaded.Invoke();
            StopCoroutine(GetCharacter(deck));
        }
    }

    IEnumerator DownloadImage(string url, RawImage targetTxt)
    {
        errorMessage.text = "";

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            targetTxt.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    #endregion

    #region UserRelated

    public void LoadUser()
    {
        if (!string.IsNullOrEmpty(field.text) && int.TryParse(id, out int numericValue)) StartCoroutine(GetUser(int.Parse(id)));
        else if (string.IsNullOrEmpty(field.text)) errorMessage.text = "busca un numero(0-3)";
        else if (!int.TryParse(id, out int numericValue1)) errorMessage.text = "formato equivocado";
    }

    public void IdToCheck()
    {
        id = field.text;
    }

    IEnumerator GetUser(int id)
    {
        errorMessage.text = "";

        UnityWebRequest www = UnityWebRequest.Get(server + id);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Connection Error: " + www.error);
            errorMessage.text = "Connection Error: " + www.error;
        }
        else
        {
            if (www.responseCode == 200)
            {
                string responseText = www.downloadHandler.text;

                User user = JsonUtility.FromJson<User>(responseText);

                deck = user.deck;
                userName.text = user.name;

                Request();
            }
            else
            {
                string message = "Status: " + www.responseCode;
                message += "\nContent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                errorMessage.text = message;
            }
        }
    }

    #endregion
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string image;
}

[System.Serializable]
public class User
{
    public int id;
    public string name;
    public int[] deck;

}