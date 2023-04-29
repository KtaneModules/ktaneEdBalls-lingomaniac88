using System;
using System.Collections.Generic;
using System.Linq;

namespace KtaneEdBalls
{
	enum SemaphoreDirection
	{
		North,
		Northeast,
		East,
		Southeast,
		South,
		Southwest,
		West,
		Northwest
	}

	static class SemaphoreDirectionExtensions
	{
		public static SemaphoreDirection RotateClockwise(this SemaphoreDirection direction, int amount = 1)
		{
			int newValue = ((int) direction + amount) % 8;
			if (newValue < 0)
			{
				newValue += 8;
			}

			return (SemaphoreDirection) newValue;
		}

		public static SemaphoreDirection RotateCounterClockwise(this SemaphoreDirection direction, int amount = 1)
		{
			return direction.RotateClockwise(-amount);
		}

		public static char ToArrow(this SemaphoreDirection direction)
		{
			return "↑↗→↘↓↙←↖"[(int) direction];
		}
	}

	public class SemaphoreLetter : IEquatable<SemaphoreLetter>
	{
		public static Dictionary<char, SemaphoreLetter> LookupTable = new Dictionary<char, SemaphoreLetter> {
			{'A', new SemaphoreLetter(SemaphoreDirection.Southwest, SemaphoreDirection.South)},
			{'B', new SemaphoreLetter(SemaphoreDirection.West, SemaphoreDirection.South)},
			{'C', new SemaphoreLetter(SemaphoreDirection.Northwest, SemaphoreDirection.South)},
			{'D', new SemaphoreLetter(SemaphoreDirection.South, SemaphoreDirection.North)},
			{'E', new SemaphoreLetter(SemaphoreDirection.South, SemaphoreDirection.Northeast)},
			{'F', new SemaphoreLetter(SemaphoreDirection.South, SemaphoreDirection.East)},
			{'G', new SemaphoreLetter(SemaphoreDirection.South, SemaphoreDirection.Southeast)},
			{'H', new SemaphoreLetter(SemaphoreDirection.West, SemaphoreDirection.Southwest)},
			{'I', new SemaphoreLetter(SemaphoreDirection.Southwest, SemaphoreDirection.Northwest)},
			{'J', new SemaphoreLetter(SemaphoreDirection.North, SemaphoreDirection.East)},
			{'K', new SemaphoreLetter(SemaphoreDirection.Southwest, SemaphoreDirection.North)},
			{'L', new SemaphoreLetter(SemaphoreDirection.Southwest, SemaphoreDirection.Northeast)},
			{'M', new SemaphoreLetter(SemaphoreDirection.Southwest, SemaphoreDirection.East)},
			{'N', new SemaphoreLetter(SemaphoreDirection.Southwest, SemaphoreDirection.Southeast)},
			{'O', new SemaphoreLetter(SemaphoreDirection.West, SemaphoreDirection.Northwest)},
			{'P', new SemaphoreLetter(SemaphoreDirection.West, SemaphoreDirection.North)},
			{'Q', new SemaphoreLetter(SemaphoreDirection.West, SemaphoreDirection.Northeast)},
			{'R', new SemaphoreLetter(SemaphoreDirection.West, SemaphoreDirection.East)},
			{'S', new SemaphoreLetter(SemaphoreDirection.West, SemaphoreDirection.Southeast)},
			{'T', new SemaphoreLetter(SemaphoreDirection.Northwest, SemaphoreDirection.North)},
			{'U', new SemaphoreLetter(SemaphoreDirection.Northwest, SemaphoreDirection.Northeast)},
			{'V', new SemaphoreLetter(SemaphoreDirection.North, SemaphoreDirection.Southeast)},
			{'W', new SemaphoreLetter(SemaphoreDirection.Northeast, SemaphoreDirection.East)},
			{'X', new SemaphoreLetter(SemaphoreDirection.Northeast, SemaphoreDirection.Southeast)},
			{'Y', new SemaphoreLetter(SemaphoreDirection.Northwest, SemaphoreDirection.East)},
			{'Z', new SemaphoreLetter(SemaphoreDirection.Southeast, SemaphoreDirection.East)},
			{'-', new SemaphoreLetter(SemaphoreDirection.Northwest, SemaphoreDirection.Southeast)},
			{'#', new SemaphoreLetter(SemaphoreDirection.North, SemaphoreDirection.Northeast)}
		};

		private static Dictionary<SemaphoreDirection, int> HandPriority = new Dictionary<SemaphoreDirection, int>
		{
			{SemaphoreDirection.West, 0},
			{SemaphoreDirection.Southwest, 1},
			{SemaphoreDirection.Northwest, 2},
			{SemaphoreDirection.South, 3},
			{SemaphoreDirection.North, 4},
			{SemaphoreDirection.Northeast, 5},
			{SemaphoreDirection.Southeast, 6},
			{SemaphoreDirection.East, 7}
		};

		private static Dictionary<SemaphoreLetter, char> TranslationTable = LookupTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

		readonly SemaphoreDirection First;
		readonly SemaphoreDirection Second;

		private SemaphoreLetter(SemaphoreDirection first, SemaphoreDirection second)
		{
			First = first;
			Second = second;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SemaphoreLetter);
		}

		public bool Equals(SemaphoreLetter otherLetter)
		{
			if (Object.ReferenceEquals(otherLetter, null))
			{
				return false;
			}

			if (Object.ReferenceEquals(this, otherLetter))
			{
				return true;
			}

			if (GetType() != otherLetter.GetType())
			{
				return false;
			}

			return (First == otherLetter.First && Second == otherLetter.Second) || (First == otherLetter.Second && Second == otherLetter.First);
		}

		public override int GetHashCode()
		{
			int a = (int) First;
			int b = (int) Second;
			return (a + b) << 8 + a * b;
		}

		public SemaphoreLetter RotateClockwise(int amount = 1)
		{
			return new SemaphoreLetter(First.RotateClockwise(amount), Second.RotateClockwise(amount));
		}

		public SemaphoreLetter RotateCounterClockwise(int amount = 1)
		{
			return RotateClockwise(-amount);
		}

		public SemaphoreLetter ShiftBackward(int amount = 1)
		{
			return ShiftForward(-(amount % 26));
		}

		public SemaphoreLetter ShiftForward(int amount = 1)
		{
			if (!Char.IsLetter(ToLetter()))
			{
				throw new ArgumentException(string.Format("Semaphore {0} [{1}] is not a letter", ToLetter(), ToArrows()));
			}

			amount %= 26;
			if (amount < 0)
			{
				amount += 26;
			}

			int newIndex = ToLetter() + amount;
			if (newIndex > 'Z')
			{
				newIndex -= 26;
			}

			return LookupTable[(char) newIndex];
		}

		public string ToArrows()
		{
			var answer = "" + First.ToArrow() + Second.ToArrow();
			if (HandPriority[First] > HandPriority[Second])
			{
				return answer.Reverse().Join("");
			}
			else
			{
				return answer;
			}
		}

		public char ToLetter()
		{
			return TranslationTable[this];
		}

		public static bool operator ==(SemaphoreLetter lhs, SemaphoreLetter rhs)
		{
			if (Object.ReferenceEquals(lhs, null))
			{
				return Object.ReferenceEquals(rhs, null);
			}
			else
			{
				return lhs.Equals(rhs);
			}
		}

		public static bool operator !=(SemaphoreLetter lhs, SemaphoreLetter rhs)
		{
			return !(lhs == rhs);
		}
	}
}