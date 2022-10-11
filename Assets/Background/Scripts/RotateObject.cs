// Rotate the object based on mouse movement, much like an FPS camera controller

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
	{
	// Use negative sensitivity for flipped horizontal and vertical rotation
	[SerializeField]
	[Range(-1f, 1f)]
	public float horizontal_sensitivity = 0.1f;
	[SerializeField]
	[Range(-1f, 1f)]
	public float vertical_sensitivity = 0.1f;
	[SerializeField]
	[Range(0f, 90f)]
	public float horizontal_limit = 5f;
	[SerializeField]
	[Range(0f, 90f)]
	public float vertical_limit = 1.5f;

	Vector2 rotation;
	// For when the object's initial rotation is not zero
	Vector2 start_rotation;

	void Start()
		{
		start_rotation = transform.localEulerAngles;
		rotation = Vector2.zero;
		}

	void Update()
		{
		rotation.x += Input.GetAxis("Mouse X") * horizontal_sensitivity;
		rotation.y += Input.GetAxis("Mouse Y") * vertical_sensitivity;

		rotation.x = ClampAngle(rotation.x, -horizontal_limit, horizontal_limit);
		rotation.y = ClampAngle(rotation.y, -vertical_limit, vertical_limit);

		
		transform.localEulerAngles = new Vector3(start_rotation.x - rotation.y, start_rotation.y + rotation.x, 0);
		}

	float ClampAngle(float angle, float min, float max)
        {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
        }
	}
