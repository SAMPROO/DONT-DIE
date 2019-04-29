using System;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class MenuInputSetter
{
	[Serializable]
	private class InputAxesNames
	{
		public string horizontal;
		public string vertical;
		public string submit;
		public string cancel;
	}

	[SerializeField] private InputAxesNames XBoxNames;
	[SerializeField] private InputAxesNames PS4Names;

	[SerializeField] private StandaloneInputModule inputModule;

	public void DetectAndSet()
	{
        var joystickNames = Input.GetJoystickNames();
        bool firstControllerPS4 = 
        	joystickNames.Length > 0 
        	&& joystickNames[0] == InputControllerManager.dualShockName;

		InputAxesNames names = firstControllerPS4 ? PS4Names : XBoxNames;

		inputModule.horizontalAxis = names.horizontal;
		inputModule.verticalAxis = names.vertical;
		inputModule.submitButton = names.submit;
		inputModule.cancelButton = names.cancel;

		CancelEvent.cancelButtonName = names.cancel;
	}
}