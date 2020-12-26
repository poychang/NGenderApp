using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NGenderApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.GetEncoding(950);

            var data = ReadCharFrequence(Path.Combine(Directory.GetCurrentDirectory(), @"Data\charfreq.csv"));

            // 測試單一筆
            var firstname = "瑋芳";
            var (gender, probability) = GuessGender(data, firstname);
            Console.WriteLine($"{firstname} 是{gender}生的機率 {probability:P2}");

            // 測試多筆資料
            var names = new List<string> { "瑋芳", "瑋芳" };
            var girlCount50 = 0;
            var girlCount60 = 0;
            var girlCount70 = 0;
            var girlCount80 = 0;
            var girlCount90 = 0;
            var girlCount95 = 0;
            names.ForEach(name =>
            {
                var (gender, probability) = GuessGender(data, name);
                if (gender == "女" && probability > 0.5) girlCount50++;
                if (gender == "女" && probability > 0.6) girlCount60++;
                if (gender == "女" && probability > 0.7) girlCount70++;
                if (gender == "女" && probability > 0.8) girlCount80++;
                if (gender == "女" && probability > 0.9) girlCount90++;
                if (gender == "女" && probability > 0.95) girlCount95++;
            });
            Console.WriteLine($"名單中(有50%↑機率是)女生有 {girlCount50} 位");
            Console.WriteLine($"名單中(有60%↑機率是)女生有 {girlCount60} 位");
            Console.WriteLine($"名單中(有70%↑機率是)女生有 {girlCount70} 位");
            Console.WriteLine($"名單中(有80%↑機率是)女生有 {girlCount80} 位");
            Console.WriteLine($"名單中(有90%↑機率是)女生有 {girlCount90} 位");
            Console.WriteLine($"名單中(有95%↑機率是)女生有 {girlCount95} 位");
        }

        public static (string gender, double probability) GuessGender(IEnumerable<CharFrequence> data, string firstname)
        {
            var charCount = CalCharCount(data);
            var charPercentage = CalCharPercentage(data, charCount);
            var pm = ProbForGender(charPercentage, charCount, firstname, true);
            var pf = ProbForGender(charPercentage, charCount, firstname, false);

            return pm > pf
                ? ("男", pm / (pm + pf))
                : ("女", pf / (pf + pm));
        }

        public static IEnumerable<CharFrequence> ReadCharFrequence(string filepath)
        {
            return File.ReadAllLines(filepath)
                   .Skip(1)
                   .Select(x => x.Split(','))
                   .Select(x => new CharFrequence
                   {
                       Char = x[0],
                       MaleFrequence = int.Parse(x[1]),
                       FemaleFrequence = int.Parse(x[2]),
                   });
        }

        public static CharCount CalCharCount(IEnumerable<CharFrequence> charFrequences)
        {
            return new CharCount
            {
                MaleTotal = charFrequences.Sum(p => p.MaleFrequence),
                FemaleTotal = charFrequences.Sum(p => p.FemaleFrequence),
            };
        }

        public static IEnumerable<CharPercentage> CalCharPercentage(IEnumerable<CharFrequence> charFrequences, CharCount charCount)
        {
            return charFrequences.Select(record =>
                new CharPercentage
                {
                    Char = record.Char,
                    MalePercentage = record.MaleFrequence / charCount.MaleTotal,
                    FemalePercentage = record.FemaleFrequence / charCount.FemaleTotal,
                }
            );
        }

        public static double ProbForGender(IEnumerable<CharPercentage> charFrequences, CharCount charCount, string firstname, bool assumeGender)
        {
            // assumeGender = true is boy, assumeGender = false is girl
            return charFrequences
                .Where(p => firstname.Contains(p.Char))
                .Select(p => assumeGender ? p.MalePercentage : p.FemalePercentage)
                .Aggregate((pre, nxt) => pre * nxt) * (charCount.FemaleTotal / charCount.Total);
        }
    }

    public class CharFrequence
    {
        public string Char { get; set; }
        public double MaleFrequence { get; set; }
        public double FemaleFrequence { get; set; }
    }

    public class CharCount
    {
        public double MaleTotal { get; set; }
        public double FemaleTotal { get; set; }
        public double Total { get { return MaleTotal + FemaleTotal; } }
    }

    public class CharPercentage
    {
        public string Char { get; set; }
        public double MalePercentage { get; set; }
        public double FemalePercentage { get; set; }
    }
}
