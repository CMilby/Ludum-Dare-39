using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] GameObject following_target;

    private float zoom = 3f;

    [SerializeField] float max_zoom = 0.0f;
    [SerializeField] float min_zoom = 0.0f;

    [SerializeField] float zoom_speed = 0.0f;
    [SerializeField] float damp = 0.0f;

    public Camera camera;

    public Transform cam_transform;

    private bool isShaking = false;
    private float shake_duration;
    private float shake_intensity;
    private float intensity_falloff = 1.0f;
    private float falloff = 1.0f;

    private Vector2 origin;

	void Start () {

        origin = cam_transform.localPosition;

        if (!following_target)
            following_target = GameObject.FindGameObjectWithTag("Player");

	}

    void Update()
    {

        float scroll_val = Input.GetAxis("Mouse ScrollWheel");

        zoom -= scroll_val * zoom_speed;

        if (zoom > max_zoom)
            zoom = max_zoom;
        if (zoom < min_zoom)
            zoom = min_zoom;

        camera.orthographicSize = zoom;

        if (isShaking)
            shaking();

    }

    private void shaking()
    {
        if (shake_duration > 0)
        {
            cam_transform.localPosition = origin + Random.insideUnitCircle * shake_intensity;

            shake_intensity = Mathf.Lerp(shake_intensity, 0.0f, Time.deltaTime * 3.0f);
            shake_duration -= Time.deltaTime * falloff;
        }
        else
        {
            shake_duration = 0f;
            cam_transform.localPosition = origin;
            isShaking = false;
        }
    }

    public void shake(float intensity, float duration)
    {

        shake_duration = duration;
        shake_intensity = intensity;

        isShaking = true;
    }

    void FixedUpdate () {

        Vector2 follow_pos = following_target.transform.position;
        Vector3 temp_pos = transform.position;

        if (following_target)
        {

            temp_pos = Vector2.Lerp(transform.position, follow_pos, damp);
           
        }

        temp_pos.z = -30;

        transform.position = temp_pos;

    }
}
