using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonThroughKeySelection : MonoBehaviour
{
    public string key;

    public void Update()
    {
        if (Input.GetKeyDown(key))
        {
            EventSystem.current.SetSelectedGameObject(
                     this.gameObject);
        }
    }
}

