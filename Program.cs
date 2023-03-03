using OpenCvSharp;
using System.IO;

namespace Dot_Plotter {
    internal class Program {
        static void Main(string[] args) {
            string inputDirectory = $"{Directory.GetCurrentDirectory()}\\input";
            bool isExistingInputDirectory = Directory.Exists(inputDirectory);
            if (!isExistingInputDirectory) Directory.CreateDirectory(inputDirectory);

            string outputDirectory = $"{Directory.GetCurrentDirectory()}\\output";
            bool isExistingOutputDirectory = Directory.Exists(outputDirectory);
            if (!isExistingOutputDirectory) Directory.CreateDirectory(outputDirectory);

            IEnumerable<string> inputFilePaths = Directory.GetFiles(inputDirectory).Where(x => x.Contains(".png"));

            if (!inputFilePaths.Any()) {
                Console.WriteLine($"No .png files were found in \"{Directory.GetCurrentDirectory()}\\input\\\"! The program has been terminated.");
                return;
            }

            foreach(string inputFilePath in inputFilePaths) {
                // Load image
                Mat src = Cv2.ImRead(inputFilePath, ImreadModes.Grayscale);
                Mat colorSrc = Cv2.ImRead(inputFilePath, ImreadModes.Color);

                // Set up blob detector parameters
                SimpleBlobDetector.Params detectorParams = new SimpleBlobDetector.Params {
                    FilterByColor = true,
                    BlobColor = 0,
                    FilterByArea = true,
                    MinArea = 20,
                    MaxArea = 100,
                    FilterByCircularity = false,
                    FilterByConvexity = false,
                    FilterByInertia = false
                };

                // Create blob detector with the specified parameters
                SimpleBlobDetector detector = SimpleBlobDetector.Create(detectorParams);

                // Detect blobs
                KeyPoint[] keypoints = detector.Detect(src);

                Console.WriteLine($"{keypoints.Length} blob{(keypoints.Length != 1 ? "s were": "was")} found for \".\\input\\{Path.GetFileName(inputFilePath)}\\\"!");

                string output = string.Empty;

                // Draw detected blobs on the color image
                foreach (KeyPoint keypoint in keypoints) {
                    Cv2.Circle(colorSrc, (int)keypoint.Pt.X, (int)keypoint.Pt.Y, (int)keypoint.Size, Scalar.Red, 2);
                    output += $"{(int)keypoint.Pt.X},{(int)keypoint.Pt.Y}\n";
                }

                // Save output image
                Cv2.ImWrite($"{outputDirectory}\\{Path.GetFileName(inputFilePath)}", colorSrc);
                Console.WriteLine($"{keypoints.Length} blob{(keypoints.Length != 1 ? "s were" : " was")} saved to \".\\output\\{Path.GetFileName(inputFilePath)}\\\"!");

                File.WriteAllText($"{outputDirectory}\\{Path.GetFileNameWithoutExtension(inputFilePath)}.txt", output);
                Console.WriteLine($"{keypoints.Length} blob{(keypoints.Length != 1 ? "s were" : " was")} saved to \".\\output\\{Path.GetFileNameWithoutExtension(inputFilePath)}.txt\\\"!");
            }
        }
    }
}