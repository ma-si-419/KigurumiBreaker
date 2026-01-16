using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Buru : MonoBehaviour
{
    private Vector3 _originalPos;
    [SerializeField] private float _haba = 0;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        _originalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float haba = _haba * Random.Range(0.2f, 1f);

        float random = Random.Range(0, 360f);
        double dx = haba * Mathf.Cos(Mathf.Deg2Rad * random);
        double dy = haba * Mathf.Sin(Mathf.Deg2Rad * random);
        transform.localPosition = (_originalPos + new Vector3((float)dx, (float)dy, 0));
    }
}
