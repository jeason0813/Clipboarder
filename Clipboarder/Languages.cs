﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Clipboarder.SyntaxHighlighting {
    public class Languages {
        public static List<ColoredKeyword> CompileFromFile(string language) {
            if (!doesLanguageExists(language))
                throw new ArgumentException("Language file for the specified language does not exist at default path");

            List<ColoredKeyword> coloredKeywords = new List<ColoredKeyword>();
            List<string> splitted = SplitIntoColors(language);

            splitted.ForEach(s => {
                coloredKeywords.AddRange(GetKeywords(s));
            });

            coloredKeywords.ForEach(coloredKeyword => {
                new Regex(coloredKeyword.keyword);
            });

            return coloredKeywords;
        }
        public static List<string> SplitIntoColors(string language) {
            List<string> splitted = new List<string>();

            Regex regex = new Regex(@"\$.+ {");
            string languageFileContent = LanguageFileAsString(language);
            MatchCollection matches = regex.Matches(languageFileContent);

            for (int i = 0; i < matches.Count - 1; i++) {
                splitted.Add(languageFileContent.Substring(matches[i].Index, matches[i + 1].Index - matches[i].Index));
            }

            splitted.Add(languageFileContent.Substring(matches[matches.Count - 1].Index));
            return splitted;
        }
        public static List<string> GetLanguages() {
            string filePath = System.IO.Path.Combine(Application.StartupPath, "SHLs");
            string[] languages = Directory.GetFiles(filePath, "*.shl");

            // Removes path and leaves names
            for (int i = 0; i < languages.Length; i++) {
                languages[i] = Path.GetFileNameWithoutExtension(languages[i]);
            }

            List<string> languagesToReturn = new List<string>();
            foreach (string language in languages) {
                languagesToReturn.Add(language);
            }

            return languagesToReturn;
        }
        public static bool doesLanguageExists(string language) {
            List<string> languages = GetLanguages();

            bool result = false;

            languages.ForEach(l => {
                if (l == language) result = true;
            });

            return result;
        }
        public static List<string> LanguageFileLines(string language) {
            string filePath = System.IO.Path.Combine(
                Application.StartupPath,
                "SHLs",
                language + ".shl");

            List<string> linesToReturn = new List<string>();

            foreach (string line in File.ReadLines(filePath, Encoding.UTF8)) {
                linesToReturn.Add(line);
            }
            return linesToReturn;
        }
        public static string LanguageFileAsString(string language) {
            if (!doesLanguageExists(language))
                throw new ArgumentException("Language file for the specified language does not exist at default path");
            return File.ReadAllText(getLanguagePath(language));
        }
        public static string getLanguagePath(String language) {
            return System.IO.Path.Combine(
                Application.StartupPath,
                "SHLs",
                language + ".shl");
        }
        public static List<ColoredKeyword> GetKeywords(string splittedByColor) {
            List<ColoredKeyword> keywordsToReturn = new List<ColoredKeyword>();
            string firstLine = GetLine(splittedByColor, 1);

            Color color = Color.FromName(
                RemoveWhiteSpaces(firstLine).Substring(1, RemoveWhiteSpaces(firstLine).Length - 2)
                );
            List<string> lines = splittedByColor.Replace("\r", "").Split('\n')
                .Cast<string>().ToList();
            lines.RemoveAt(0);
            lines.RemoveAt(lines.Count - 1);
            if (RemoveWhiteSpaces(lines.ElementAt(lines.Count - 1)) == "}") lines.RemoveAt(lines.Count - 1);

            for (int i = 0; i < lines.Count; i++) {
                lines[i] = RemoveWhiteSpaces(lines[i]);
                keywordsToReturn.Add(new ColoredKeyword(lines[i], color));
            }

            return keywordsToReturn;
        }
        public static string GetLine(string text, int lineNo) {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }
        public static string RemoveWhiteSpaces(string inputString) {
            return Regex.Replace(inputString, @"\s+", "");
        }
    }
    public class ColoredKeyword {
        public string keyword;
        public Color color;

        public ColoredKeyword(string keyword, Color color) {
            this.keyword = keyword;
            this.color = color;
        }
    }
}