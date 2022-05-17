using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentScissors
{
    public partial class MainForm : Form
    {
        public Point start, end,
        firstNode = new Point(int.MinValue, int.MinValue);
        public bool startBool = false,
                    endBool = false;
        public Dictionary<int, double>[] graph;
        string OpenedFilePath;
        Image buffer = null, buffer2 = null;
        List<int> liveWire;
        List<List<int>> paths = new List<List<int>>();
        bool[] taken;

        bool cropClicked = false;
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        RGBPixel[,] CroppedMatrix;
        private async void btnOpen_Click(object sender, EventArgs e)
        {
            startBool = false;
            endBool = false;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                buffer = pictureBox1.Image;
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();

            //Initializing Graph
            int width = 0, height = 0;
            width = ImageOperations.GetWidth(ImageMatrix);
            height = ImageOperations.GetHeight(ImageMatrix);

            // Converts image to greyscale.
            greyScale(height, width);

            // Construct graph for the opened image
            constructGraph(height, width);

            // Creating Output File.
            createOutputFile();

        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
            int width = 0, height = 0;
            width = ImageOperations.GetWidth(ImageMatrix);
            height = ImageOperations.GetHeight(ImageMatrix);
            graph = new Dictionary<int, double>[width * height];

            for (int i = 0; i < graph.Length; i++)
            {
                int x = i % width;
                int y = i / width;

                //y * width + x
                //left right top bottom
                int[] xSum = { -1, 1, 0, 0 };
                int[] ySum = { 0, 0, -1, 1 };
                char[] direction = { 'L', 'R', 'T', 'B' };

                graph[i] = new Dictionary<int, double>();

                for (int j = 0; j < 4; j++)
                {
                    if (x + xSum[j] >= width || x + xSum[j] < 0)
                        continue;
                    if (y + ySum[j] >= height || y + ySum[j] < 0)
                        continue;

                    int pointNum = (y + ySum[j]) * width + x + xSum[j];
                    double cost;
                    switch (direction[j])
                    {
                        case 'L':
                            cost = ImageOperations.CalculateLeftPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E+16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                        case 'R':
                            cost = ImageOperations.CalculateRightPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E+16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                        case 'T':
                            cost = ImageOperations.CalculateTopPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E+16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                        case 'B':
                            cost = ImageOperations.CalculateBottomPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E+16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                    }


                }


            }
        }


        public List<int> findShortestPath(int start, int end)
        {
            List<int> path = new List<int>();

            //Child , Parent , Cost 
            Dictionary<int, KeyValuePair<int, double>> childParent = new Dictionary<int, KeyValuePair<int, double>>();

            PriorityQueue<int, double> queue = new PriorityQueue<int, double>();

            bool[] visited = new bool[graph.Length];


            queue.Enqueue(start, 0);
            childParent.Add(start, new KeyValuePair<int, double>(-1, 0));

            while (queue.Count > 0)
            {
                int currentNode = queue.Dequeue();

                //Check if Visited Before
                if (visited[currentNode])
                    continue;

                //Update Visited Status
                visited[currentNode] = true;

                if (currentNode == end)
                    break;

                if (taken[currentNode])
                    continue;


                //MessageBox.Show("Current : " + currentNode + " with cost : " + childParent[currentNode].Value + " From Parent : " + childParent[currentNode].Key);
                double currentCost = childParent[currentNode].Value;
                foreach (KeyValuePair<int, double> child in graph[currentNode])
                {
                    // Skips the pixel if it's taken before.
                
                    if (childParent.ContainsKey(child.Key))
                    {

                        //Found A Better Path
                        if (childParent[child.Key].Value > child.Value + currentCost)
                        {
                            //MessageBox.Show("Child : "+child.Key +" Child Old Cost: " + childParent[child.Key].Value + " Child New Cost : " + child.Value + currentCost);
                            childParent[child.Key] = new KeyValuePair<int, double>(currentNode, child.Value + currentCost);
                            queue.Enqueue(child.Key, child.Value + currentCost);
                        }
                    }
                    else //New Node Explored
                    {
                        childParent.Add(child.Key, new KeyValuePair<int, double>(currentNode, child.Value + currentCost));
                        queue.Enqueue(child.Key, child.Value + currentCost);
                    }
                }



            }

            //Generate Path
            return getPath(childParent, end, start);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        public void Reset()
        {
            firstNode = new Point(int.MinValue, int.MinValue);
            startBool = false;
            endBool = false;
            cropClicked = false;
            pictureBox1.Image = buffer;

        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Image = buffer;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!startBool) return;

            buffer2 = new Bitmap(buffer);
            pictureBox1.Image = buffer2;

            Graphics g = Graphics.FromImage(pictureBox1.Image);


            int width = ImageOperations.GetWidth(ImageMatrix);
            int startNum;
            if (!endBool)
                startNum = start.Y * width + start.X;
            else startNum = end.Y * width + end.X;
            int endNum = e.Y * width + e.X;

            liveWire = findShortestPath(startNum, endNum);

            Pen LiveWirePen = new Pen(Color.Yellow, 0.5f);

            for (int i = 0; i < liveWire.Count; i++)
            {
                int xCoordinate, yCoordinate;

                xCoordinate = liveWire[i] % width;
                yCoordinate = liveWire[i] / width;

                g.DrawRectangle(LiveWirePen, xCoordinate, yCoordinate, 0.5f, 0.5f);

            }

            pictureBox1.Refresh();
        }


        public List<int> getPath(Dictionary<int, KeyValuePair<int, double>> childParent, int end, int start)
        {
            List<int> result = new List<int>();
            
            while (end != -1)
            {
                result.Add(end);
                end = childParent[end].Key;
                if (end == start)
                    break;
            }

            result.Add(end);

            return Enumerable.Reverse(result).ToList();
        }


        public void DrawPathInPictureBox(Pen Line1, Pen Line2, float size)
        {
            Graphics g = Graphics.FromImage(buffer);




            int width = ImageOperations.GetWidth(ImageMatrix);

            int counter = 2;
            bool whiteBool = false;

            for (int i = 0; i < liveWire.Count; i++)
            {
                if (i != 0 && i != liveWire.Count - 1)
                    taken[liveWire[i]] = true;
    
                int xCoordinate, yCoordinate;

                xCoordinate = liveWire[i] % width;
                yCoordinate = liveWire[i] / width;

                if (counter == 0)
                {
                    whiteBool = true;
                }
                else if (counter == 2)
                {
                    whiteBool = false;
                }

                if (whiteBool)
                    counter++;
                else
                    counter--;



                if (whiteBool)
                    g.DrawRectangle(Line1, xCoordinate, yCoordinate, size, size);
                else
                    g.DrawRectangle(Line2, xCoordinate, yCoordinate, size, size);



            }

            liveWire.Clear();
            pictureBox1.Refresh();
        }

        private void crop_Click(object sender, EventArgs e)
        {
            if (cropClicked)
                return;
            cropClicked = true;
            liveWire = findShortestPath(end.Y * ImageOperations.GetWidth(ImageMatrix) + end.X, firstNode.Y * ImageOperations.GetWidth(ImageMatrix) + firstNode.X);
            paths.Add(liveWire);
            //taken[paths[0][0]] = true;
            //taken[paths[0][paths.Count - 1]] = true;
            Pen LinePenBlack = new Pen(Color.Black, 0.5f);
            Pen LinePenWhite = new Pen(Color.White, 0.5f);

            pictureBox1.Image = buffer;

            DrawPathInPictureBox(LinePenBlack, LinePenWhite, 0.8f);
            genreateImage();
            Reset();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //buffer = new Bitmap(pictureBox1.Image);
            Point pixel = e.Location;
            if (firstNode == new Point(int.MinValue, int.MinValue))
                firstNode = pixel;

            if (!startBool)
            {
                start = pixel;
                startBool = true;
            }
            else
            {
                if (endBool)
                    start = end;

                end = pixel;
                endBool = true;
            }

            buffer = new Bitmap(pictureBox1.Image);
            Graphics g = Graphics.FromImage(buffer);
            Pen p = new Pen(Color.Black, 3);
            Pen p2 = new Pen(Color.White, 2);


            g.DrawRectangle(p, pixel.X, pixel.Y, 3, 3);
            g.DrawRectangle(p2, pixel.X + 1, pixel.Y + 1, 2, 2);



            /*if (buffer != null) pictureBox1.Image = buffer;*/

            if (!endBool)
                return;
            
            Pen LinePenBlack = new Pen(Color.Black, 0.5f);
            Pen LinePenWhite = new Pen(Color.White, 0.5f);
            paths.Add(liveWire);

            DrawPathInPictureBox(LinePenBlack, LinePenWhite, 0.8f);
        }

        private void greyScale(int height, int width)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double avg = (ImageMatrix[i, j].red + ImageMatrix[i, j].green + ImageMatrix[i, j].blue) / 3;
                    ImageMatrix[i, j].red = ImageMatrix[i, j].green = ImageMatrix[i, j].blue = Convert.ToByte(avg);
                }
            }
        }

        private void constructGraph(int height, int width)
        {
            graph = new Dictionary<int, double>[width * height];

            // Creating Adjacency List.
            for (int i = 0; i < graph.Length; i++)
            {
                int x = i % width;
                int y = i / width;


                // Left || Right || Top || Bottom
                int[] xSum = { -1, 1, 0, 0 };
                int[] ySum = { 0, 0, -1, 1 };
                char[] direction = { 'L', 'R', 'T', 'B' };

                graph[i] = new Dictionary<int, double>();

                for (int j = 0; j < 4; j++)
                {

                    if (x + xSum[j] >= width || x + xSum[j] < 0)
                        continue;
                    if (y + ySum[j] >= height || y + ySum[j] < 0)
                        continue;

                    // Pixel Number = y * width + x
                    int pointNum = (y + ySum[j]) * width + x + xSum[j];
                    double cost;
                    switch (direction[j])
                    {
                        case 'L':
                            cost = ImageOperations.CalculateLeftPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E+16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                        case 'R':
                            cost = ImageOperations.CalculateRightPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E+16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                        case 'T':
                            cost = ImageOperations.CalculateTopPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E+16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                        case 'B':
                            cost = ImageOperations.CalculateBottomPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E+16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                    }
                }
            }
            taken = new bool[graph.Length];
        }
        private void createOutputFile()
        {
            string s = "";

            for (int i = 0; i < graph.Length; i++)
            {
                s = s + "The  index node" + i + "\nEdges\n";
                foreach (KeyValuePair<int, double> x in graph[i])
                {
                    s = s + "edge from " + i + " To " + x.Key + " With Weights " + x.Value + "\n";

                    s = "";
                }
                s = s + "\n";
            }

            // ToDo: createfile.
        }
        private void genreateImage()
        {

            CroppedMatrix = ImageOperations.OpenImage(OpenedFilePath);
                
            int width = ImageOperations.GetWidth(ImageMatrix);
            int height = ImageOperations.GetHeight(ImageMatrix);

            int pixelNumber = 0;

            for(int i =0; i < height; i++)
            {
                int lastX = -2;
                bool inCropped = false;
                for(int j =0; j < width; j++)
                {
                    
                    if (taken[pixelNumber] && j - lastX > 1)
                        inCropped = !inCropped;

                    if (taken[pixelNumber])
                        lastX = j;

                    if (!inCropped)
                    {
                        // Setting the background to white.
                        CroppedMatrix[i, j].red = 0;
                        CroppedMatrix[i, j].green = 0;
                        CroppedMatrix[i, j].blue = 0;
                    }
                 
                    pixelNumber++;
                }

            }


            for (int i = 0; i < width; i++)
            {
                int lastY = -2;
                bool inCropped = false;
                for (int j = 0; j < height; j++)
                {

                    pixelNumber = j * width + i;
                    if (taken[pixelNumber] && j - lastY > 1)
                        inCropped = !inCropped;

                    if (taken[pixelNumber])
                        lastY = j;

                    if (!inCropped)
                    {
                        // Setting the background to white.
                        CroppedMatrix[j, i].red = 0;
                        CroppedMatrix[j, i].green = 0;
                        CroppedMatrix[j, i].blue = 0;
                    }
                    
                }

            }

            ImageOperations.DisplayImage(CroppedMatrix, pictureBox2);
        }
    }
}