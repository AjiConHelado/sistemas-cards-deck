using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class carta : MonoBehaviour
{
    
    [SerializeField] private RawImage img;
    [SerializeField] private TMP_Text id, cardName;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        interceptor.manager.charactersLoaded.AddListener(SetImage);
    }

    void SetImage()
    {
        interceptor.manager.SetImage(index, img);
        id.text = interceptor.manager.characters.ElementAt(index).id.ToString();
        cardName.text = interceptor.manager.characters.ElementAt(index).name;
        
    }
}
