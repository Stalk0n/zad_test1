using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SphereManager : MonoBehaviour
{
    public float radius = 5f;
    public float spiralSpeed = 0.5f;
    public float maxSpeed = 1.0f;
    public float speed = 0f;
    private float accumulatedAngle = 0f;
    private Vector3 center  = Vector3.zero;
    private CharacterController controller;
    private Renderer rend;
    private bool hasStarted = false;
    public Button startButton;
    public TextMeshProUGUI distanceTraceledText;
    public ParticleSystem fireworks;
    public bool fireworksTriggered = false;
    private float totalDistanceTraveled = 0f;
    private Vector3 lastPosition;
    void Start()
    {
        rend = GetComponent<Renderer>();
        lastPosition = transform.position;
        speed = 0f;
    }
    void Update()
    {
        accumulatedAngle += speed * Time.deltaTime;
        float currentRadius = radius * accumulatedAngle * spiralSpeed;
        float x = center.x + Mathf.Cos(accumulatedAngle) * currentRadius;
        float z = center.z + Mathf.Sin(accumulatedAngle) * currentRadius;
        
        transform.position = new Vector3(x, center.y, z);
        
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        totalDistanceTraveled += distanceThisFrame;
        lastPosition = transform.position;
        
        float t = accumulatedAngle;
        float r = Mathf.Sin(t) * 0.5f + 0.5f;
        float g = Mathf.Sin(t + 2f) * 0.5f + 0.5f;
        float b = Mathf.Sin(t + 4f) * 0.5f + 0.5f;
        rend.material.color  = new Color(r, g, b);

        if (currentRadius >= 8f && !fireworksTriggered)
        {
            fireworksTriggered = true;
            speed = 0f;
            StartCoroutine(FadeOutAndFire());
        }
    }
    public void OnStartButtonPressed()
    {
        if (hasStarted) return;
        hasStarted = true;
        startButton.gameObject.SetActive(false);
        Debug.Log("Movement started!");
        StartCoroutine(RestoreSpeedGradually(0f, 2f));
    }
    public void OnSpacePressed()
    {
        Debug.Log("Space pressed!");
        distanceTraceledText.gameObject.SetActive(true);
        distanceTraceledText.text = "Distance: " + totalDistanceTraveled.ToString("F2");
        StopAllCoroutines();
        speed = 0;
        StartCoroutine(RestoreSpeedGradually(5f, 2f));
    }

    private IEnumerator RestoreSpeedGradually(float delay, float rampDuration)
    {
        
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float startSpeed = speed;

        while (elapsed < rampDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rampDuration;
            speed = Mathf.Lerp(startSpeed, maxSpeed, t);
            yield return null;
        }
        distanceTraceledText.gameObject.SetActive(false);
        speed = maxSpeed; 
    }
    private IEnumerator FadeOutAndFire()
    {
        float duration = 1f;
        float elapsed = 0f;
        Color startColor = rend.material.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, elapsed / duration);
            rend.material.color = new Color(startColor.r, startColor.g, startColor.b, a);
            yield return null;
        }
        rend.gameObject.SetActive(false);
        fireworks.transform.position = transform.position;
        fireworks.gameObject.SetActive(true);
        fireworks.Play();
    }
}
