using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace WolfRPG.Core.CommandConsole
{
	[RequireComponent(typeof(UIDocument))]
	public class CommandConsole: MonoBehaviour
	{
		private static CommandConsole _instance;
		private CommandConsoleParser _parser;
		private UIDocument _uiDocument;
		private ScrollView _log;
		private TextField _inputField;

		private void Awake()
		{
			_instance = this;
			_parser = new();
		}

		private void OnEnable()
		{
			// UI elements stop existing when the object is disabled
			_uiDocument = GetComponent<UIDocument>();
			var root = _uiDocument.rootVisualElement;
			_log = root.Query<ScrollView>("Log").First();

			_inputField = root.Query<TextField>("TextInput").First();
			_inputField.RegisterCallback<KeyDownEvent>(OnSubmit);
			
			_inputField.Focus();
		}

		private void OnDisable()
		{
			_inputField.UnregisterCallback<KeyDownEvent>(OnSubmit);
		}

		private void OnSubmit(KeyDownEvent e)
		{
			if (e.keyCode == KeyCode.Return)
			{
				AddText(_inputField.value, Color.cyan);
				
				_parser.ExecuteCommand(_inputField.value);
				_inputField.SetValueWithoutNotify("");
				_inputField.Focus();
			}
		}

		private void AddText(string text, Color color)
		{
			var label = new Label(text);
			label.style.color = color;
			_log.Add(label);
		}

		public static void RegisterCommand(IConsoleCommand command)
		{
			_instance._parser.RegisterCommand(command);
		}

		public static void LogMessage(string message)
		{
			if (_instance == null) return;

			_instance.AddText(message, Color.white);
		}
		
		public static void LogError(string message)
		{
			if (_instance == null) return;

			_instance.AddText(message, Color.red);
		}
	}
}