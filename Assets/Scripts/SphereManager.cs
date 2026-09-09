using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class SphereManager : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI distanceTraveledText;
    [SerializeField] private ParticleSystem fireworks;
    [SerializeField] private Gradient colorGradient;

    private float _maxSpeed = 8.0f;
    private float _speed;
    private bool _fireworksTriggered;
    private float _accumulatedAngle;
    private CharacterController _controller;
    private Renderer _rend;
    private bool _hasStarted;
    private float _totalDistanceTraveled;
    private float _distance;

    void Start()
    {
        _rend = GetComponent<Renderer>();
        _speed = 0f;
        _distance = 0f;
    }

    void Update()
    {
        float length = spline.CalculateLength();
        _distance += _speed * Time.deltaTime;
        _distance = Mathf.Min(_distance, length);
        float t = _distance / length;
        float progress = _distance / length;

        transform.position = spline.EvaluatePosition(t);

        _rend.material.color = colorGradient.Evaluate(progress);

        if (_distance >= length)
        {
            _fireworksTriggered = true;
            _speed = 0f;
            StartCoroutine(FadeOutAndFire());
        }
    }

    public void OnStartButtonPressed()
    {
        if (_hasStarted) return;
        _hasStarted = true;
        startButton.gameObject.SetActive(false);
        StartCoroutine(RestoreSpeedGradually(0f, 2f));
    }

    public void OnSpacePressed()
    {
        distanceTraveledText.gameObject.SetActive(true);
        distanceTraveledText.text = "Distance: " + _distance.ToString("F2");
        StopAllCoroutines();
        _speed = 0;
        StartCoroutine(RestoreSpeedGradually(5f, 2f));
    }

    private IEnumerator RestoreSpeedGradually(float delay, float rampDuration)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float startSpeed = _speed;

        while (elapsed < rampDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rampDuration;
            _speed = Mathf.Lerp(startSpeed, _maxSpeed, t);
            yield return null;
        }

        distanceTraveledText.gameObject.SetActive(false);
        _speed = _maxSpeed;
    }

    private IEnumerator FadeOutAndFire()
    {
        float duration = 1f;
        float elapsed = 0f;
        Color startColor = _rend.material.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, elapsed / duration);
            _rend.material.color = new Color(startColor.r, startColor.g, startColor.b, a);
            yield return null;
        }

        _rend.gameObject.SetActive(false);
        fireworks.transform.position = transform.position;
        fireworks.gameObject.SetActive(true);
        fireworks.Play();
    }
}