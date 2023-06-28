using Yolov8Net;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp;

namespace ParkSpot
{
    public partial class Form1 : Form
    {
        private readonly SixLabors.Fonts.Font font;
        public Form1()
        {
            InitializeComponent();
            var fontCollection = new FontCollection();
            var fontFamily = fontCollection.Add("assets/CONSOLA.TTF");
            font = fontFamily.CreateFont(11, SixLabors.Fonts.FontStyle.Bold);
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string outputPath = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);

            using var yolo = YoloV8Predictor.Create("assets/yolov8m.onnx");

            using var image = SixLabors.ImageSharp.Image.Load(textBox1.Text);
            var predictions = yolo.Predict(image);


            string[] arr = { "car", "truck" };
            DrawBoxes(yolo.ModelInputHeight, yolo.ModelInputWidth, image, predictions);

            var filename = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
            if (File.Exists(outputPath + @"\" + filename + "_result.jpg"))
            {
                File.Delete(outputPath + @"\" + filename + "_result.jpg");
            }
            image.SaveAsJpeg(Path.Combine(outputPath + @"\" + filename + "_result.jpg"));
            pictureBox1.Image = System.Drawing.Image.FromFile(outputPath + @"\" + filename + "_result.jpg");
            label2.Text = predictions.Where(x => arr.Contains(x.Label.Name.ToLower())).Count().ToString();
            label1.Text = "Total Cars: ";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                textBox1.Text = openFileDialog1.FileName;
            }
            pictureBox2.Image = System.Drawing.Image.FromFile(openFileDialog1.FileName);
        }

        private void DrawBoxes(int modelInputHeight, int modelInputWidth, SixLabors.ImageSharp.Image image, Prediction[] predictions)
        {
            foreach (var pred in predictions)
            {
                var originalImageHeight = image.Height;
                var originalImageWidth = image.Width;

                var x = (int)Math.Max(pred.Rectangle.X, 0);
                var y = (int)Math.Max(pred.Rectangle.Y, 0);
                var width = (int)Math.Min(originalImageWidth - x, pred.Rectangle.Width);
                var height = (int)Math.Min(originalImageHeight - y, pred.Rectangle.Height);

                //Note that the output is already scaled to the original image height and width.

                // Bounding Box Text
                string text = $"{pred.Label.Name} [{pred.Score}]";
                var size = TextMeasurer.Measure(text, new TextOptions(font));

                image.Mutate(d => d.Draw(SixLabors.ImageSharp.Drawing.Processing.Pens.Solid(SixLabors.ImageSharp.Color.Yellow, 2),
                    new SixLabors.ImageSharp.Rectangle(x, y, width, height)));


                image.Mutate(d => d.DrawText(
                    new TextOptions(font)
                    {
                        Origin = new SixLabors.ImageSharp.Point(x, (int)(y - size.Height - 1))
                    },
                    text, SixLabors.ImageSharp.Color.Yellow)); ;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}