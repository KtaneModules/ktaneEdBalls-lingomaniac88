namespace KtaneEdBalls
{
	public interface PuzzleDescriptor
	{
		// Transforms a letter.
		char EncodeLetter(char letter);

		// Describes how a letter is transformed.
		string EncodingDescription(char letter);

		// Does this transform map every letter to itself?
		bool IsIdentity();

		// Describes this puzzle's transformation.
		string TransformationDescription();
	}
}