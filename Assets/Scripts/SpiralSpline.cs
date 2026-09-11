using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class SpiralSpline : MonoBehaviour
{
    [SerializeField] int turns = 3;
    [SerializeField] float startRadius = 1f;
    [SerializeField] float endRadius = 10f;
    [SerializeField] int pointsPerTurn = 20;

    void OnValidate()
    {
        Generate();
    }

    void Generate()
    {
        var spline = GetComponent<SplineContainer>();
        spline.Spline.Clear();

        int count = turns * pointsPerTurn + 1;

        for (int i = 0; i < count; i++)
        {
            float progress = (float)i / (count - 1);
            float angle = progress * turns * 2f * Mathf.PI;
            float radius = Mathf.Lerp(startRadius, endRadius, progress);

            spline.Spline.Add(new BezierKnot(new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            )));
        }

        spline.Spline.SetTangentMode(TangentMode.AutoSmooth);
    }
}