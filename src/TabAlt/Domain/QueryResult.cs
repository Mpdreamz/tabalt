namespace Tabalt.Domain
{
	public struct QueryResult
	{
		public bool Matches { get; }
		public double Score { get; }

		public static QueryResult NoMatch { get; } = new QueryResult(false, 0.0);
		public static QueryResult Match(double score) => new QueryResult(score);

		internal QueryResult(double score)
		{
			this.Score = score;
			this.Matches = true;
		}

		internal QueryResult(bool matches, double score)
		{
			this.Score = score;
			this.Matches = matches;
		}
	}
}