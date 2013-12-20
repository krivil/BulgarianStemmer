using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace BulgarianStemmer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            QuickTest();

            System.Console.WriteLine();
            
            TimeTest();

            System.Console.WriteLine("Press any key to continue . . .");
            System.Console.ReadKey(true);
        }

        private static void QuickTest()
        {
            var words = new[] { "Управляващите", "ПРОТЕСТИРАЩИТЕ" };

            var stemmer = new BulgarianStemmer();

            foreach (string word in words)
            {
                string stem = stemmer.Stem(word);
                System.Console.WriteLine("{0} => {1}", word, stem);
            }
        }

        private static void TimeTest()
        {
            var stemmer = new BulgarianStemmer();

            var wordsRegex = new Regex(@"\w+", RegexOptions.Compiled);

            const string text = @"Богдан Милчев, който оглавяваше КАТ-София, е преместен в резервния щат на МВР. Това означава, че той не е уволнен като служител, просто занапред няма да оглавява службата. Информацията беше потвърдена от вътрешния министър Цветлин Йовчев след церемонията за „Полицай на годината”.
Йовчев мотивира решението си с липса на добри резултати от работата на Милчев и начина, по който ръководи КАТ-София.
За временно изпълняващ длъжността директор е назначен заместникът на Милчев Иво Писанов. Пред dnevnik.bg Милчев заяви, че няма да коментира решението на Йовчев без разрешение на пресцентъра на МВР. 
Милчев беше назначен за шеф на столичния КАТ през май 2011 г., след като МВР пенсионира цялото ръководство на звеното. Милчев обеща да е безкомпромисен към подкупите и заяви, че ще има коренна промяна в поведението на пътните полицаи.";

            System.Console.WriteLine(text);

            System.Console.WriteLine();
            System.Console.WriteLine("=====");
            System.Console.WriteLine();

            Stopwatch tokenStopwatch = Stopwatch.StartNew();
            var words = wordsRegex.Matches(text).OfType<Match>().Where(m => m.Success).Select(m => m.Value).ToArray();
            tokenStopwatch.Stop();

            Stopwatch stemStopwatch = Stopwatch.StartNew();
            var stems = words.Select(stemmer.Stem).ToArray();
            stemStopwatch.Stop();

            foreach (string word in words)
            {
                System.Console.Write(word);
                System.Console.Write(" ");
            }

            System.Console.WriteLine();
            System.Console.WriteLine("=====");
            System.Console.WriteLine();

            foreach (string stem in stems)
            {
                System.Console.Write(stem);
                System.Console.Write(" ");
            }

            System.Console.WriteLine();
            System.Console.WriteLine("=====");
            System.Console.WriteLine();


            System.Console.WriteLine("Tokenization: {0}", tokenStopwatch.Elapsed);
            System.Console.WriteLine("Stemming: {0}", stemStopwatch.Elapsed);
        }
    }
}
