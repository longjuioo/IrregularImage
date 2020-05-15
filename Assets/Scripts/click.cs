using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class click : MonoBehaviour
{
    private Image img = null;
    // Start is called before the first frame update
    void Start()
    {
        img = this.GetComponent<Image>();
        this.GetComponent<Button>().onClick.AddListener(Print);
    }

    public void Print()
    {
        Debug.Log(img.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
