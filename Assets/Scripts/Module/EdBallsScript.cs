using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdBallsScript : MonoBehaviour
{
	private static Dictionary<KtaneEdBalls.PuzzleDescriptor, KtaneEdBalls.SemaphoreLetter[]> AllAffinePuzzles = KtaneEdBalls.AffinePuzzleDescriptor.GenerateAllLegalPuzzles();
	private static Dictionary<KtaneEdBalls.PuzzleDescriptor, KtaneEdBalls.SemaphoreLetter[]> AllCruelPuzzles = KtaneEdBalls.CruelPuzzleDescriptor.GenerateAllLegalPuzzles();

	public KMAudio Audio;
	public KMBombModule Module;

	static int ModuleIdCounter = 1;
	int ModuleId;

	public KtaneEdBalls.Button[] Buttons;

	public TextMesh VictoryText;
	public TextMesh VictoryShadow;

	public bool IsCruel;

	char[] CorrectAnswer;
	int CharactersEnteredSoFar;

	void Awake()
	{
		ModuleId = ModuleIdCounter++;
	}

	// Use this for initialization
	void Start()
	{
		// BombModule.OnActivate += FunctionToCallWhenTheLightsTurnOn;

		// Select a puzzle
		var puzzleInfo = (IsCruel ? AllCruelPuzzles : AllAffinePuzzles).PickRandom();

		CorrectAnswer = (new int[] {0, 1, 2, 3, 4, 4, 5}).Select(i => puzzleInfo.Value[i].ToLetter()).ToArray();
		
		CharactersEnteredSoFar = 0;

		Buttons = GetComponent<KMSelectable>().Children.Select(selectable => new KtaneEdBalls.Button(selectable.gameObject)).ToArray();

		// Label the buttons and register their events
		var labels = puzzleInfo.Value.ToList().Shuffle();
		for (int i = 0; i < Buttons.Length; i++)
		{
			var button = Buttons[i];
			button.Audio = Audio;
			button.OnPress = OnButtonPressed;
			button.Label.text = "" + labels[i].ToLetter();
		}

		// Log the solution
		ModuleLog("Buttons labels are {0}", labels.Select(l => l.ToLetter()).Join(", "));
		ModuleLog(puzzleInfo.Key.TransformationDescription());
		foreach (var letter in "EDBALS")
		{
			ModuleLog(puzzleInfo.Key.EncodingDescription(letter));
		}
		
		/*if (puzzleInfo.Key.CaesarShifts == 0 && puzzleInfo.Key.SemaphoreRotations == 0)
		{
			ModuleLog("Encoding transformation: shift 0 forward, rotate 0° clockwise. Lucky you!");
		}
		else if (puzzleInfo.Key.CaesarShifts == 0)
		{
			ModuleLog("Encoding transformation: rotate {0}° clockwise, shift 0 forward", 45 * puzzleInfo.Key.SemaphoreRotations);
			foreach (var letter in puzzleInfo.Value)
			{
				var originalLetter = letter.RotateCounterClockwise(puzzleInfo.Key.SemaphoreRotations);
				ModuleLog("{0} [{1}] ⇒ [{2}] {3}", originalLetter.ToLetter(), originalLetter.ToArrows(), letter.ToArrows(), letter.ToLetter());
			}
		}
		else if (puzzleInfo.Key.SemaphoreRotations == 0)
		{
			ModuleLog("Encoding transformation: shift {0} forward, rotate 0° clockwise", puzzleInfo.Key.CaesarShifts);
			foreach (var letter in puzzleInfo.Value)
			{
				var originalLetter = letter.ShiftBackward(puzzleInfo.Key.CaesarShifts);
				ModuleLog("{0} ⇒ {1}", originalLetter.ToLetter(), letter.ToLetter());
			}
		}
		else if (puzzleInfo.Key.ShiftFirst)
		{
			ModuleLog("Encoding transformation: shift {0} forward, rotate {1}° clockwise", puzzleInfo.Key.CaesarShifts, 45 * puzzleInfo.Key.SemaphoreRotations);
			foreach (var letter in puzzleInfo.Value)
			{
				var intermediateLetter = letter.RotateCounterClockwise(puzzleInfo.Key.SemaphoreRotations);
				var originalLetter = intermediateLetter.ShiftBackward(puzzleInfo.Key.CaesarShifts);
				ModuleLog("{0} ⇒ {1} [{2}] ⇒ [{3}] {4}", originalLetter.ToLetter(), intermediateLetter.ToLetter(), intermediateLetter.ToArrows(), letter.ToArrows(), letter.ToLetter());
			}
		}
		else
		{
			ModuleLog("Encoding transformation: rotate {0}° clockwise, shift {1} forward", 45 * puzzleInfo.Key.SemaphoreRotations, puzzleInfo.Key.CaesarShifts);
			foreach (var letter in puzzleInfo.Value)
			{
				var intermediateLetter = letter.ShiftBackward(puzzleInfo.Key.CaesarShifts);
				var originalLetter = intermediateLetter.RotateCounterClockwise(puzzleInfo.Key.SemaphoreRotations);
				ModuleLog("{0} [{1}] ⇒ [{2}] {3} ⇒ {4}", originalLetter.ToLetter(), originalLetter.ToArrows(), intermediateLetter.ToArrows(), intermediateLetter.ToLetter(), letter.ToLetter());
			}
		}*/

		var answer = puzzleInfo.Value.Select(l => l.ToLetter()).Join("");
		answer = answer.Insert(5, "" + answer[4]).Insert(2, " ");
		ModuleLog("Correct answer: {0}", answer);

		VictoryText.GetComponent<Renderer>().sortingOrder = 2;
		VictoryShadow.GetComponent<Renderer>().sortingOrder = 1;
	}

	void Update()
	{
		foreach (var button in Buttons)
		{
			button.Update();
		}
	}

	bool IsSolved()
	{
		return CharactersEnteredSoFar == CorrectAnswer.Length;
	}
	
	void ModuleLog(string format, params object[] args)
	{
		var prefix = string.Format("[Ed Balls #{0}] ", ModuleId);
		Debug.LogFormat(prefix + format, args);
	}

	void OnButtonPressed(KtaneEdBalls.Button button)
	{
		if (IsSolved())
		{
			return;
		}

		ModuleLog("Pressed button \"{0}\"", button.Label.text);

		if (CorrectAnswer[CharactersEnteredSoFar] == button.Label.text[0])
		{
			CharactersEnteredSoFar++;

			if (IsSolved())
			{
				ModuleLog("ED BALLS! Module disarmed!");
				Audio.PlaySoundAtTransform("solve", transform);
				Module.HandlePass();

				VictoryText.text = "ED BALLS!";
				VictoryShadow.text = "ED BALLS!";
			}
		}
		else
		{
			ModuleLog("Strike! That last button was wrong. Resetting input.");
			Module.HandleStrike();
			CharactersEnteredSoFar = 0;
		}
	}
}
