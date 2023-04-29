using System;
using UnityEngine;

namespace KtaneEdBalls
{
	public class Button
	{
		private const float ButtonPressDistance = 0.005f;

		public KMAudio Audio;
		public TextMesh Label;
		public Action<Button> OnPress;

		private KMSelectable Actuator;

		private float TargetY;

		public Button(GameObject buttonObject)
		{
			Actuator = buttonObject.GetComponentInChildren<KMSelectable>();
			Label = Actuator.GetComponentInChildren<TextMesh>();

			TargetY = Actuator.transform.localPosition.y;

			Actuator.OnInteract += delegate()
			{
				Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, buttonObject.transform);
				TargetY -= ButtonPressDistance;
				if (OnPress != null)
				{
					OnPress(this);
				}
				return false;
			};

			Actuator.OnInteractEnded += delegate()
			{
				Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, buttonObject.transform);
				TargetY += ButtonPressDistance;
			};
		}

		public void Update()
		{
			float distanceToTarget = TargetY - Actuator.transform.localPosition.y;
			Actuator.transform.localPosition += Vector3.up * distanceToTarget * 0.25f;
		}
	}

}
