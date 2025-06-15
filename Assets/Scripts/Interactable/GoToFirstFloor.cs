using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToFirstFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.lifeBeforeSceneChanged = GameManager.Instance.playerLife.currentLife;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            TransitionCanvas.Instance.CallNextScene();
        }
    }
}
