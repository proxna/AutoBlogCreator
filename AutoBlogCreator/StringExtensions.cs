using System.Runtime.CompilerServices;
using System.Text;
using AutoBlogCreator.Models;

namespace AutoBlogCreator
{
    public static class StringExtensions
    {
        public static string RemoveWhitespaces(this string input) => string.Concat(input.Where(c => !Char.IsWhiteSpace(c)));

        public static string RemoveSpecialCharacters(this string input)
        {
            StringBuilder result = new StringBuilder(input.Length);
            foreach (char character in input)
            {
                if (Char.IsLetterOrDigit(character))
                {
                    result.Append(character);
                }
            }
            return result.ToString();
        }

        private static readonly Dictionary<char, char> PolishCharReplacements = new Dictionary<char, char>
        {
            {'ą', 'a'}, {'ć', 'c'}, {'ę', 'e'}, {'ł', 'l'}, {'ń', 'n'}, {'ó', 'o'}, {'ś', 's'}, {'ż', 'z'}, {'ź', 'z'},
            {'Ą', 'A'}, {'Ć', 'C'}, {'Ę', 'E'}, {'Ł', 'L'}, {'Ń', 'N'}, {'Ó', 'O'}, {'Ś', 'S'}, {'Ż', 'Z'}, {'Ź', 'Z'}
        };

        public static string RemovePolishCharacters(this string text)
        {
            var result = new StringBuilder(text.Length);
            foreach (var character in text)
            {
                result.Append(PolishCharReplacements.TryGetValue(character, out var replacement) ? replacement : character);
            }
            return result.ToString();
        }

        public static string GetLocalPath(this IConfiguration configuration)
        {
            RepositoryInfo repositoryInfo = new RepositoryInfo();
            configuration.GetSection("RepositoryInfo").Bind(repositoryInfo);
            return repositoryInfo.LocalPath;
        }
    }
}
