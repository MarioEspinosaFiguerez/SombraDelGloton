using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class PergaminoBoton : MonoBehaviour
{
    public Animator animator;
    public string triggerName = "Cerrar";
    public string escenaADesplegar = "";
    public float delay = 0.5f;
    public float textoDelay = 0.2f;

    public Button botonUI; 
    public GameObject textoUI;
    public AudioSource audioSource;

    public void AlPulsarBoton()
    {
        if (botonUI != null)
            botonUI.interactable = false;

        if (audioSource != null)
        audioSource.Play();

        if (textoUI != null)
            Invoke(nameof(OcultarTexto), textoDelay);

        if (animator != null)
        {
            animator.SetTrigger(triggerName);
            Invoke(nameof(AccionTrasAnimacion), delay);
        }
    }

    void OcultarTexto()
    {
        textoUI.SetActive(false);
    }

    void AccionTrasAnimacion()
    {
        if (!string.IsNullOrEmpty(escenaADesplegar))
            SceneManager.LoadScene(escenaADesplegar);
    }
}
