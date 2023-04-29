using System.Collections.Generic;
using System.Linq;

namespace KtaneEdBalls
{
	public struct AffinePuzzleDescriptor : PuzzleDescriptor
	{
		// It's silly to use semaphore representation here, but it makes this more compatible with the cruel variant
		public static Dictionary<PuzzleDescriptor, SemaphoreLetter[]> GenerateAllLegalPuzzles()
		{
			var allResults = new Dictionary<AffinePuzzleDescriptor, string>();

			for (int multiplier = 1; multiplier < 26; multiplier += 2)
			{
				// Make sure it's invertible
				if (26 % multiplier == 0)
				{
					continue;
				}
				for (int offset = 0; offset < 26; offset++)
				{
					var transform = new AffinePuzzleDescriptor(multiplier, offset);
					var transformedLetters = "EDBALS".Select(transform.EncodeLetter);
					if (transformedLetters.Distinct().Count() != transformedLetters.Count())
					{
						continue;
					}
					allResults[transform] = transformedLetters.Join("");
				}
			}

			// Filter out duplicates
			var counts = allResults.ToLookup(kvp => kvp.Value.OrderBy(c => c).Join(""), kvp => kvp.Key);
			var uniqueLetterGroups = counts.Where(group => group.Count() == 1);

			return uniqueLetterGroups.ToDictionary(group => (PuzzleDescriptor) group.First(), group => allResults[group.First()].Select(c => SemaphoreLetter.LookupTable[c]).ToArray());
		}

		public readonly int Multiplier;
		public readonly int Offset;

		public AffinePuzzleDescriptor(int multiplier, int offset)
		{
			Multiplier = multiplier;
			Offset = offset;
		}

		public char EncodeLetter(char letter)
		{
			int index = letter - 'A' + 1;
			int transformedIndex = (Multiplier * index + Offset) % 26;
			while (transformedIndex <= 0)
			{
				transformedIndex += 26;
			}
			return (char) ('A' + transformedIndex - 1);
		}

		public string EncodingDescription(char letter)
		{
			int index = letter - 'A' + 1;
			int transformedIndex = Multiplier * index + Offset;
			int modulatedIndex = transformedIndex % 26;
			while (modulatedIndex <= 0)
			{
				modulatedIndex += 26;
			}
			char transformedLetter = (char) ('A' + modulatedIndex - 1);

			string modulation = "";
			if (transformedIndex != modulatedIndex)
			{
				modulation = " ⇒ " + modulatedIndex;
			}
			return string.Format("{0} {1} ⇒ {2}{3} {4}", letter, index, transformedIndex, modulation, transformedLetter);
		}

		public bool IsIdentity()
		{
			return Multiplier == 1 && Offset == 0;
		}

		public string TransformationDescription()
		{
			string luckyMessage = IsIdentity() ? ". Lucky you!" : "";
			return string.Format("Index transformation: multiply by {0} and add {1}{2}", Multiplier, Offset, luckyMessage);
		}
	}
}