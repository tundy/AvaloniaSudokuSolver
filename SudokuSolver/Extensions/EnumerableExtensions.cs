using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Generates all permutations of a specified size from the given source collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the source collection.</typeparam>
        /// <param name="source">The source collection from which to generate permutations.</param>
        /// <param name="size">The size of each permutation.</param>
        /// <returns>An enumerable of all permutations of the specified size.</returns>
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
