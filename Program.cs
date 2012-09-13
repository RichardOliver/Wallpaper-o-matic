using System.IO;
using System.Windows;

namespace GeneratePicture
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandLineArgs = new ParseCommandLine(System.Environment.CommandLine);
            if (commandLineArgs.ContainsKey("help") || commandLineArgs.ContainsKey("?"))
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

        private static void ShowHelp()
        {
            System.Console.WriteLine("Creates of images based on an iregular grid of triangles");
            System.Console.WriteLine();
            System.Console.WriteLine("{0} /output:pathForImages [[/textures:pathForTextures] [/colours:commaSeparatedListOfHexCodes]] [/rows:numberOfRowsInGrid] [/cols:numberOfColumnsInGrid] [/jigglex:HowMuchToVaryPositionsAlongXAxis] [/jiggley:HowMuchToVaryPositionsAlongXAxis] [/numberOfImages:numberOfImagesPerRun]", System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            System.Console.WriteLine();
            System.Console.WriteLine("/output");
            System.Console.WriteLine("/textures");
            System.Console.WriteLine("/colours");
            System.Console.WriteLine("/rows");
            System.Console.WriteLine("/cols");
            System.Console.WriteLine("/jigglex");
            System.Console.WriteLine("/jiggley");
            System.Console.WriteLine("/numberOfImages");
            System.Console.WriteLine();
            System.Console.WriteLine("Must specify an output folder");
        }
    }
}
