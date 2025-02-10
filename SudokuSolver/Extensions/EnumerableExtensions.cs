using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<ImmutableList<T>> Permutations<T>(this IReadOnlyList<T> source, int size)
        {
            if (size < 0 || size > source.Count)
            {
                return [];
            }
            return GetPermutations(source, size);
        }

        private static IEnumerable<ImmutableList<T>> GetPermutations<T>(IReadOnlyList<T> source, int size)
        {
            if (size == 0)
            {
                yield return [];
                yield break;
            }
            for (int i = 0; i < source.Count; i++)
            {
                var item = source[i];
                var remaining = source.Where((_, index) => index != i).ToImmutableList();
                foreach (var perm in GetPermutations(remaining, size - 1))
                {
                    yield return new[] { item }.Concat(perm).ToImmutableList();
                }
            }
        }
    }
}
