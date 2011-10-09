using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tabalt
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>
		 (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> knownKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				if (knownKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
		}
	}
}
