using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InvisiLoadToMain : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            GoBackToMain();
        }
    }

    /// <summary>
    /// Also called by Button Click
    /// </summary>
    public void GoBackToMain()
    {
        SceneManager.LoadScene(0);
    }
}
