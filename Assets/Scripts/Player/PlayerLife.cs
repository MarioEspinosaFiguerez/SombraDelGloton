using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour, ILife
{
    public float maxLife;
    public float currentLife;
    public Image lifeBar;
    [SerializeField] private float fillSpeed = 2f;
    private Coroutine fillCoroutine;
    private void Start()
    {
        maxLife = maxLife != 0 ? maxLife : 100.0f;

        if (GameManager.Instance != null)
        {
            currentLife = GameManager.Instance.lifeBeforeSceneChanged > 0 
                ? GameManager.Instance.lifeBeforeSceneChanged 
                : maxLife;
        }
        else
        {
            currentLife = maxLife;
        }

        UpdateLifeBar();
    }

    public void ChangeCurrentLife(float damage)
    {
        if (damage < -3)
        {
            Debug.Log("Recibistes daño");
            StartCoroutine(transform.GetComponent<PlayerController>().ApplyKnockback());
        }
        currentLife += damage;
        currentLife = Mathf.Clamp(currentLife, 0, maxLife);
        // GameManager.Instance.currentPlayerLife = currentLife;

        UpdateLifeBarAnimated();

        if (currentLife <= 0)
        {
            // Llamar Die
            Die();
        }
    }

    private void UpdateLifeBar()
    {
        if(lifeBar != null)
        {
            lifeBar.fillAmount = currentLife / maxLife;
        }
    }

    private void UpdateLifeBarAnimated()
    {
        if(lifeBar != null)
        {
            if(fillCoroutine != null) StopCoroutine(fillCoroutine);
            fillCoroutine = StartCoroutine(SmoothFill(currentLife / maxLife));
        }
    }

    private IEnumerator SmoothFill(float targetFill)
    {
        float startFill = lifeBar.fillAmount;
        float timer = 0f;

        while(timer < 1f)
        {
            timer += Time.deltaTime * fillSpeed;
            lifeBar.fillAmount = Mathf.Lerp(startFill, targetFill, timer);
            yield return null;
        }
        lifeBar.fillAmount = targetFill;
    }


    private void Die()
    {
        GameOverCanvas.Instance.CallGameOverMenu();
        //Debug.Log("Tanuki has died");
        //gameObject.SetActive(false);
        // Destroy(gameObject);
    }
}
