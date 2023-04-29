using System;
using System.Collections.Generic;
using System.Linq;

namespace KtaneEdBalls
{
	public struct CruelPuzzleDescriptor : PuzzleDescriptor
	{
		// Generates all legal puzzles as a dictionary whose values consist of the transformed letters "EDBALS".
		public static Dictionary<PuzzleDescriptor, SemaphoreLetter[]> GenerateAllLegalPuzzles()
		{
			var allResults = new Dictionary<CruelPuzzleDescriptor, SemaphoreLetter[]>();

			var originalLetters = "EDBALS".Select(c => SemaphoreLetter.LookupTable[c]);
			
			// Case 1: shift first, then rotate
			for (int shift = 0; shift < 26; shift++)
			{
				var intermediateLetters = originalLetters.Select(l => l.ShiftForward(shift));
				for (int rotate = 0; rotate < 8; rotate++)
				{
					var finalLetters = intermediateLetters.Select(l => l.RotateClockwise(rotate));
					if (finalLetters.All(l => Char.IsLetter(l.ToLetter())))
					{
						allResults[new CruelPuzzleDescriptor(shift, rotate, true)] = finalLetters.ToArray();
					}
				}
			}

			// Case 2: rotate first, then shift
			// Only consider cases where we both rotate and shift, since the others will be handled above
			for (int rotate = 1; rotate < 8; rotate++)
			{
				var intermediateLetters = originalLetters.Select(l => l.RotateClockwise(rotate));
				if (intermediateLetters.All(l => Char.IsLetter(l.ToLetter())))
				{
					for (int shift = 1; shift < 26; shift++)
					{
						var finalLetters = intermediateLetters.Select(l => l.ShiftForward(shift));
						allResults[new CruelPuzzleDescriptor(shift, rotate, false)] = finalLetters.ToArray();
					}
				}
			}

			// Filter out duplicates
			var counts = allResults.ToLookup(kvp => kvp.Value.Select(l => l.ToLetter()).OrderBy(c => c).Join(""), kvp => kvp.Key);
			var uniqueLetterGroups = counts.Where(group => group.Count() == 1);

			return uniqueLetterGroups.ToDictionary(group => (PuzzleDescriptor) group.First(), group => allResults[group.First()]);
		}

		public readonly int CaesarShifts;
		public readonly int SemaphoreRotations;
		public readonly bool ShiftFirst;

		public CruelPuzzleDescriptor(int shifts, int rotations, bool shiftFirst)
		{
			CaesarShifts = shifts;
			SemaphoreRotations = rotations;
			ShiftFirst = shiftFirst;
		}

		public char EncodeLetter(char letter)
		{
			var semaphoreLetter = SemaphoreLetter.LookupTable[letter];
			if (ShiftFirst)
			{
				return semaphoreLetter.ShiftForward(CaesarShifts).RotateClockwise(SemaphoreRotations).ToLetter();
			}
			else
			{
				return semaphoreLetter.RotateClockwise(SemaphoreRotations).ShiftForward(CaesarShifts).ToLetter();
			}
		}

		public string EncodingDescription(char letter)
		{
			if (IsIdentity())
			{
				// Don't return 
				return "";
			}

			var originalLetter = SemaphoreLetter.LookupTable[letter];
			var transformedLetter = SemaphoreLetter.LookupTable[EncodeLetter(letter)];

			if (CaesarShifts == 0)
			{
				return string.Format("{0} [{1}] ⇒ [{2}] {3}", originalLetter.ToLetter(), originalLetter.ToArrows(), transformedLetter.ToArrows(), transformedLetter.ToLetter());
			}
			else if (SemaphoreRotations == 0)
			{
				return string.Format("{0} ⇒ {1}", originalLetter.ToLetter(), transformedLetter.ToLetter());
			}
			else if (ShiftFirst)
			{
				var intermediateLetter = originalLetter.ShiftForward(CaesarShifts);
				return string.Format("{0} ⇒ {1} [{2}] ⇒ [{3}] {4}", originalLetter.ToLetter(), intermediateLetter.ToLetter(), intermediateLetter.ToArrows(), transformedLetter.ToArrows(), transformedLetter.ToLetter());
			}
			else
			{
				var intermediateLetter = originalLetter.RotateClockwise(SemaphoreRotations);
				return string.Format("{0} [{1}] ⇒ [{2}] {3} ⇒ {4}", originalLetter.ToLetter(), originalLetter.ToArrows(), intermediateLetter.ToArrows(), intermediateLetter.ToLetter(), transformedLetter.ToLetter());
			}
		}

		public bool IsIdentity()
		{
			return CaesarShifts == 0 && SemaphoreRotations == 0;
		}

		public string TransformationDescription()
		{
			string shiftDescription = string.Format("shift {0} forward", CaesarShifts);
			string rotateDescription = string.Format("rotate {0}° clockwise", 45 * SemaphoreRotations);

			string transformation = string.Format("{0}, {1}", ShiftFirst ? shiftDescription : rotateDescription, ShiftFirst ? rotateDescription : shiftDescription);
			string luckyMessage = IsIdentity() ? ". Lucky you!" : "";

			return string.Format("Encoding transformation: {0}{1}", transformation, luckyMessage);
		}
	}
}