using System;
using System.IO;
using System.Windows;

namespace Wallpaperomatic
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandLineArgs = new ParseCommandLine(Environment.CommandLine);
            if (TheHelpTextShouldBeShown(commandLineArgs))
            {
                ShowHelp();
                return;
            }

            var outputPath = commandLineArgs["output"][0];
            var texturesPath = commandLineArgs.GetFirstOrSubstitute("textures", "");
            var rows = commandLineArgs.GetFirstOrSubstitute("rows", 7);
            var cols = commandLineArgs.GetFirstOrSubstitute("cols", 7);
            var jiggleX = commandLineArgs.GetFirstOrSubstitute("jiggleX", 40);
            var jiggleY = commandLineArgs.GetFirstOrSubstitute("jiggleY", 40);
            var numberOfImages = commandLineArgs.GetFirstOrSubstitute("numberOfImages", 1);
            var colours = commandLineArgs.GetFirstOrSubstitute("colours", "#C5C9C7,#D4DAE1,#DEE1DD,#EDF0F1,#C7D8E4,#92ADD9,#FDE818,#A94637"); //colours inspired by Ben Nicholson's June 1937

            var screenHeight = (int)SystemParameters.PrimaryScreenHeight;
            var screenWidth = (int)SystemParameters.PrimaryScreenWidth;

            var triangleGrid = new TriangleGrid(texturesPath, colours, rows, cols, jiggleX, jiggleY, screenWidth, screenHeight);
            for (var i = 0; i <= numberOfImages; i++)
            {
                var fileName = string.Format("{0}.jpg", i);
                var outputFileName = Path.Combine(outputPath, fileName);
                triangleGrid.Draw(outputFileName);               
            }
        }

        private static bool TheHelpTextShouldBeShown(ParseCommandLine commandLineArgs)
        {
            if (commandLineArgs.ContainsKey("help") || commandLineArgs.ContainsKey("?")) return true;
            if (!commandLineArgs.ContainsKey("output")) return true;

            var outputFolder = commandLineArgs["output"][0];
            return !Directory.Exists(outputFolder);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Creates of images based on an irregular grid of triangles");
            Console.WriteLine();
            Console.WriteLine("{0} /output:pathForImages [[/textures:pathForTextures] [/colours:commaSeparatedListOfHexCodes]] [/rows:numberOfRowsInGrid] [/cols:numberOfColumnsInGrid] [/jigglex:HowMuchToVaryPositionsAlongXAxis] [/jiggley:HowMuchToVaryPositionsAlongXAxis] [/numberOfImages:numberOfImagesPerRun]", System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            Console.WriteLine();
            Console.WriteLine("/output");
            Console.WriteLine("/textures");
            Console.WriteLine("/colours");
            Console.WriteLine("/rows");
            Console.WriteLine("/cols");
            Console.WriteLine("/jigglex");
            Console.WriteLine("/jiggley");
            Console.WriteLine("/numberOfImages");
            Console.WriteLine();
            Console.WriteLine("Must specify an output folder");
        }
    }
}
