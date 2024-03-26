﻿namespace UglyToad.Examples
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Welcome to the PdfPig examples gallery!");

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filesDirectory = Path.Combine(baseDirectory, "..", "..", "..", "..", "src", "UglyToad.PdfPig.Tests", "Integration", "Documents");

            var examples = new Dictionary<int, (string name, Action action)>
            {
                {1,
                    ("Extract Words with newline detection (example with algorithm)",
                    () => OpenDocumentAndExtractWords.Run(Path.Combine(filesDirectory, "Two Page Text Only - from libre office.pdf")))
                },
                {2,
                    ("Extract Text with newlines (using built-in content extractor)",
                    () => ExtractTextWithNewlines.Run(Path.Combine(filesDirectory, "Two Page Text Only - from libre office.pdf")))
                },
                {3,
                    ("Extract images",
                    () => ExtractImages.Run(Path.Combine(filesDirectory, "2006_Swedish_Touring_Car_Championship.pdf")))
                },
                {4,
                    ("Merge PDF Documents",
                    () => MergePdfDocuments.Run(Path.Combine(filesDirectory, "Two Page Text Only - from libre office.pdf"),
                        Path.Combine(filesDirectory, "2006_Swedish_Touring_Car_Championship.pdf"),
                        Path.Combine(filesDirectory, "Rotated Text Libre Office.pdf")))
                },
                {5,
                    ("Extract form contents",
                    () => GetFormContents.Run(Path.Combine(filesDirectory, "AcroFormsBasicFields.pdf")))
                },
                {6,
                    ("Generate PDF/A-2A compliant file",
                    () => GeneratePdfA2AFile.Run(Path.Combine(filesDirectory, "..", "..", "Fonts", "TrueType", "Roboto-Regular.ttf"),
                        Path.Combine(filesDirectory, "smile-250-by-160.jpg")))
                },
                {7,
                    ("Advance text extraction using layout analysis algorithms",
                    () => AdvancedTextExtraction.Run(Path.Combine(filesDirectory, "ICML03-081.pdf")))                
                },
                {
                8,
                    ("Extract Words with newline detection (example with algorithm). Issue 512",
                    () => OpenDocumentAndExtractWords.Run(Path.Combine(filesDirectory, "OPEN.RABBIT.ENGLISH.LOP.pdf")))
                } 
        };

            var choices = string.Join(Environment.NewLine, examples.Select(x => $"{x.Key}: {x.Value.name}"));

            // Console.WriteLine(choices);
            // Console.WriteLine();

            // do
            // {
                // Console.Write("Enter a number to pick an example (enter 'q' to exit):");

                // var val = "1";

                // if (!int.TryParse(val, out var opt) || !examples.TryGetValue(opt, out var act))
                // {
                //     if (string.Equals(val, "q", StringComparison.OrdinalIgnoreCase))
                //     {
                //         return;
                //     }
                //
                //     // Console.WriteLine($"No option with value: {val}.");
                //     // continue;
                // }

               examples.TryGetValue(1,out var act);
               act.action();

                Console.WriteLine();
                Console.WriteLine();

                // Console.WriteLine(choices);
                Console.WriteLine();
            // } while (true);
        }
    }
}
