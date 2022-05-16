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
        public Point start, end;
        public bool startBool = false, endBool = false;
        public Dictionary<int, double>[] graph;
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;

        private async void btnOpen_Click(object sender, EventArgs e)
        {
            startBool = false;
            endBool = false;
           
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();

            //Initializing Graph
            int width = 0, height = 0;
            width = ImageOperations.GetWidth(ImageMatrix);
            height = ImageOperations.GetHeight(ImageMatrix);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double avg = (ImageMatrix[i, j].red + ImageMatrix[i, j].green + ImageMatrix[i, j].blue) / 3;
                    ImageMatrix[i, j].red = ImageMatrix[i, j].green = ImageMatrix[i, j].blue = Convert.ToByte(avg);
                }
            }

           
            graph = new Dictionary<int, double>[width*height];

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
                
                for(int j = 0; j < 4; j++)
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

            //MessageBox.Show("Adjacency List generated");

        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
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

            while(queue.Count > 0)
            {
                int currentNode = queue.Dequeue();

                //Check if Visited Before
                if (visited[currentNode])
                    continue;

                //Update Visited Status
                visited[currentNode] = true;

                if (currentNode == end)
                    break;


                //MessageBox.Show("Current : " + currentNode + " with cost : " + childParent[currentNode].Value + " From Parent : " + childParent[currentNode].Key);
                double currentCost = childParent[currentNode].Value;
                foreach(KeyValuePair<int, double> child in graph[currentNode])
                {
                    
                    if(childParent.ContainsKey(child.Key))
                    {
                        //Found A Better Path
                        if (childParent[child.Key].Value > child.Value + currentCost )
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
            return getPath(childParent, end,start);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            graph = new Dictionary<int, double>[5];
            for (int i = 0; i < 5; i++)
                graph[i] = new Dictionary<int, double>();

            graph[0].Add(1, 6);
            graph[0].Add(3, 1);
            graph[1].Add(4, 2);
            graph[1].Add(3, 2);
            graph[1].Add(2, 5);
            graph[2].Add(1, 5);
            graph[2].Add(4, 5);
            graph[3].Add(0, 1);
            graph[3].Add(1, 2);
            graph[3].Add(4, 1);
            graph[4].Add(2, 5);
            graph[4].Add(1, 2);
            graph[4].Add(3, 1);
            List<int> path = findShortestPath(0, 4);
            string s = "";
            foreach (int i in path)
                s += i + "->";
            MessageBox.Show(s);
        }

        public List<int> getPath(Dictionary<int, KeyValuePair<int, double>> childParent, int end, int start)
        {
            List<int> result = new List<int>();
           
            while (end != -1)
            {
                
               // MessageBox.Show(end + "->");
                result.Add(end);
                end = childParent[end].Key;
                
                if (end == start)
                    break;
            }
           
            result.Add(end);
            return Enumerable.Reverse(result).ToList();
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point pixel = e.Location;
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


            Graphics g = Graphics.FromImage(pictureBox1.Image);
            Pen p = new Pen(Color.Black, 3);
            Pen p2 = new Pen(Color.White, 2);
            var cursorPosition = pictureBox1.PointToClient(Cursor.Position);
            g.DrawRectangle(p, pixel.X, pixel.Y, 3, 3);
            g.DrawRectangle(p2, pixel.X + 1, pixel.Y + 1, 2, 2);
            pictureBox1.Refresh();

            if (!endBool)
                return;

            int width = ImageOperations.GetWidth(ImageMatrix);
            
            int startNum = start.Y * width + start.X;
            int endNum = end.Y * width + end.X;
            //MessageBox.Show("Test 1");
            List<int> path = findShortestPath(startNum, endNum);
           // MessageBox.Show("Generated Path");
            Pen whitePen = new Pen(Color.White, 2f);
            Pen blackPen = new Pen(Color.Black, 0.5f);

            Point first = new Point(0,0), second;
            bool f = true;
            for(int i = 0;i < path.Count;i++)
            {
                int xCoordinate, yCoordinate;

                xCoordinate = path[i] % width;
                yCoordinate = path[i] / width;

                g.DrawRectangle(p2, xCoordinate, yCoordinate, 1,1);
               
                //MessageBox.Show(path[i] + " : " + xCoordinate + ", " + yCoordinate);
                /* if (f)
                     first = new Point(xCoordinate, yCoordinate);
                 else
                 {
                     second = new Point(xCoordinate, yCoordinate);
                     g.DrawLine(whitePen, first, second);
                     g.DrawLine(blackPen, first, second);
                     first = second;

                 }*/

            }
            pictureBox1.Refresh();




            /*Console.WriteLine(pixel.X + ", " + pixel.Y);
            Console.WriteLine("Top : " + ImageOperations.CalculateTopPixelEnergy(pixel.X, pixel.Y, ImageMatrix));
            Console.WriteLine("Left : " + ImageOperations.CalculateLeftPixelEnergy(pixel.X, pixel.Y, ImageMatrix));
            Console.WriteLine("Right : " + ImageOperations.CalculateRightPixelEnergy(pixel.X, pixel.Y, ImageMatrix));
            Console.WriteLine("Bottom : " + ImageOperations.CalculateBottomPixelEnergy(pixel.X, pixel.Y, ImageMatrix));*/



        }
    }
}