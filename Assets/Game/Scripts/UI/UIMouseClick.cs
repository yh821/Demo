using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseClick : MonoBehaviour
{
	[SerializeField] private GameObject[] effects;
	private Canvas canvas;

	private void Start()
	{
		canvas = GetComponent<Canvas>();
	}

	private void Update()
	{
#if UNITY_IOS || UNITY_ANDROID
		UpdateTouch();
#else
		UpdateMouse();
#endif
	}

	private void UpdateTouch()
	{
		if (Input.touchCount <= 0) return;
		var touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
			ShowClickEffect();
	}

	private void UpdateMouse()
	{
		if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
			ShowClickEffect();
	}

	private void ShowClickEffect()
	{
		EventDispatcher.Instance.OnUIMouseClickEffect(effects, canvas, transform);
	}
}