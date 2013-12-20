using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BulgarianStemmer
{
    public class BulgarianStemmer
    {
        private const int MinRuleFreq = 2;

        private static readonly CultureInfo BulgarianCultureInfo = CultureInfo.GetCultureInfo("bg-BG");

        private static readonly Regex BulgarianVowel = new Regex(@"[аъоуеияю]", RegexOptions.Compiled);
        private static readonly Regex Pattern = new Regex(@"(\w+)\s==>\s(\w+)\s(\d+)", RegexOptions.Compiled);

        private readonly Dictionary<string, string> _stemmingRules = new Dictionary<string, string>();

        public BulgarianStemmer()
        {
            LoadEmbeddedRules();
        }

        private void LoadEmbeddedRules(int minRuleFreq = MinRuleFreq)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(BulgarianStemmer));
            Encoding encoding = Encoding.GetEncoding(1251);

            Stream stemRulesContext1Stream = assembly.GetManifestResourceStream(@"BulgarianStemmer.Rules.stem_rules_context_1.txt");
            Stream stemRulesContext2Stream = assembly.GetManifestResourceStream(@"BulgarianStemmer.Rules.stem_rules_context_2.txt");
            Stream stemRulesContext3Stream = assembly.GetManifestResourceStream(@"BulgarianStemmer.Rules.stem_rules_context_3.txt");

            if (stemRulesContext1Stream != null)
                using (var reader = new StreamReader(stemRulesContext1Stream, encoding))
                    LoadStemmingRules(reader, minRuleFreq);

            if (stemRulesContext2Stream != null)
                using (var reader = new StreamReader(stemRulesContext2Stream, encoding))
                    LoadStemmingRules(reader, minRuleFreq);

            if (stemRulesContext3Stream != null)
                using (var reader = new StreamReader(stemRulesContext3Stream, encoding))
                    LoadStemmingRules(reader, minRuleFreq);
        }

        public void LoadStemmingRules(TextReader reader, int minRuleFreq = MinRuleFreq)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (String.IsNullOrWhiteSpace(line))
                    continue;

                Match m = Pattern.Match(line);
                if (m.Success && Int32.Parse(m.Groups[3].Value) > MinRuleFreq)
                {
                    _stemmingRules[m.Groups[1].Value] = m.Groups[2].Value;
                }
            }
        }

        public void ClearStemmingRules()
        {
            _stemmingRules.Clear();
        }

        public string Stem(String word)
        {
            //Convert to lower case
            string lowerWord = word.ToLower(BulgarianCultureInfo);

            Match m = BulgarianVowel.Match(lowerWord);
            if (!m.Success) // No Bulgarian vowel -> not a stemmable word
                return lowerWord;

            for (int i = m.Index + m.Length + 1; i < lowerWord.Length; i++)
            {
                string suffix = lowerWord.Substring(i);
                string value;
                //Check for stem suffix
                if (_stemmingRules.TryGetValue(suffix, out value))
                {
                    return lowerWord.Substring(0, i) + value;
                }
            }
            return lowerWord;
        }
    }
}