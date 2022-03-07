using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMove : MonoBehaviour
{
    gameManager GameManager;

    void Start ()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        GameManager = gameController.GetComponent<gameManager>();
    }

    void Update ()
    {
        transform.Translate(GameManager.moveVector * GameManager.moveSpeed * Time.deltaTime);
    }
}
