using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        public SortedDictionary<int, double>[] graph;
        string OpenedFilePath;
        Image buffer = null, buffer2 = null;
        List<int> liveWire;
        List<List<int>> paths = new List<List<int>>();
        bool[] taken;
        bool completedShape = false;

        bool cropClicked = false;
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        RGBPixel[,] CroppedMatrix;
        private async void btnOpen_Click(object sender, EventArgs e)
        {
            try
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
            }
            catch(Exception ex)
            {
                MessageBox.Show("Loading image failed. Please try to reload the image");
            }

            // Creating Output File.
            

        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            try
            {
                double sigma = double.Parse(txtGaussSigma.Text);
                int maskSize = (int)nudMaskSize.Value;
                ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
                int width = 0, height = 0;
                width = ImageOperations.GetWidth(ImageMatrix);
                height = ImageOperations.GetHeight(ImageMatrix);
                graph = new SortedDictionary<int, double>[width * height];

                for (int i = 0; i < graph.Length; i++)
                {
                    int x = i % width;
                    int y = i / width;

                    //y * width + x
                    //left right top bottom
                    int[] xSum = { -1, 1, 0, 0 };
                    int[] ySum = { 0, 0, -1, 1 };
                    char[] direction = { 'L', 'R', 'T', 'B' };

                    graph[i] = new SortedDictionary<int, double>();

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
            catch(Exception ex)
            {
                MessageBox.Show("Please Load the Image First");
            }
        }


        public List<int> findShortestPath(int start, int end)
        {
            List<int> path = new List<int>();

            //Child , Parent , Cost 
            SortedDictionary<int, KeyValuePair<int, double>> childParent = new SortedDictionary<int, KeyValuePair<int, double>>();

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
            completedShape = false;
            for (int i = 0; i < paths.Count; i++)
            {
                for (int j = 0; j < paths[i].Count; j++)
                    taken[paths[i][j]] = false;
            }
            paths.Clear();
            ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            buffer = pictureBox1.Image ;

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


        public List<int> getPath(SortedDictionary<int, KeyValuePair<int, double>> childParent, int end, int start)
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

            liveWire = null;
            pictureBox1.Refresh();
        }

        private void crop_Click(object sender, EventArgs e)
        {
            if (!completedShape)
            {
                MessageBox.Show("You have to complete the shape first");
                return;
            }
            try
            {
                
                genreateImage();

                Reset();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please Load the image First");
            }
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

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void gnerateGraphFile_Click(object sender, EventArgs e)
        {
            try
            {
                createOutputFile();
                Stopwatch time = new Stopwatch();
                time.Start();
                string fileName = @"ConstructedGraphFormat2.txt";
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                //MessageBox.Show("I'm here");
                using (FileStream fs = File.Create(fileName))
                {// Create a new file     

                    string s = "Constructed Graph: (Format: node_index|edges:(from, to, weight)... )\n";
                    // Add some text to file    
                    Byte[] info = new UTF8Encoding(true).GetBytes(s);
                    fs.Write(info, 0, info.Length);

                    for (int i = 0; i < graph.Length; i++)
                    {

                        s = i.ToString() + "|edges:";
                        info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                        foreach (KeyValuePair<int, double> x in graph[i])
                        {
                            s += "("+i.ToString() + "," + x.Key +","+ x.Value + ")";
                            
                        }
                        s += "\n";
                        info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                    time.Stop();
                    s = "Graph construction took: "+time.Elapsed.Seconds + "."+time.ElapsedMilliseconds+"seconds.";
                    info = new UTF8Encoding(true).GetBytes(s);
                    fs.Write(info, 0, info.Length);

                }

            }
            catch(Exception exception)
            {
                MessageBox.Show("Please load the image first");
            }
        }

        private void gnerateShortestPathFile_Click(object sender, EventArgs e)
        {
            try
            {

                StartAndEndPointChooser chooser = new StartAndEndPointChooser();
                var result = chooser.ShowDialog();
               
                int width = ImageOperations.GetWidth(ImageMatrix);
                int startNum = Program.startY * width + Program.startX;
                int endNum = Program.endY * width + Program.endX;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                List<int> ShortestPath = findShortestPath(startNum , endNum);
                stopwatch.Stop();
                
                string fileName = @"ThePath.txt";
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file    
                    


                    string s = "The Shortest path from Node ";
                    Byte[] info = new UTF8Encoding(true).GetBytes(s);
                    fs.Write(info, 0, info.Length);

                    s = startNum.ToString()+" at (" + Program.startX.ToString() + ", " + Program.startY.ToString() + ") to Node " + endNum.ToString();
                    s += " at (" + Program.endX.ToString() + ", " + Program.endY + ")\n";
                    info = new UTF8Encoding(true).GetBytes(s);
                    fs.Write(info, 0, info.Length);

                    s = "Format: (node_index, x, y)\n";
                    info = new UTF8Encoding(true).GetBytes(s);
                    fs.Write(info, 0, info.Length);
                    for (int i = 0; i < ShortestPath.Count; i++)
                    {
                        int xCoordinate, yCoordinate;
                        xCoordinate = ShortestPath[i] % width;
                        yCoordinate = ShortestPath[i] / width;
                        s = "{X=" + xCoordinate.ToString() + ",Y=" + yCoordinate.ToString() + "}," + xCoordinate.ToString() + "," + yCoordinate.ToString() + ")\n";
                        info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);

                    }
                    s = "Path construction took:" + stopwatch.Elapsed.Seconds.ToString() + "." + stopwatch.Elapsed.Milliseconds.ToString() + "seconds.";
                    info = new UTF8Encoding(true).GetBytes(s);
                    fs.Write(info, 0, info.Length);
                }

                    // Create a new file     
   
            }
            catch(Exception ex)
            {
                MessageBox.Show("something went wrong maybe there is no image loaded or the start point or the end point are out of range");
            }
        }

        private void txtHeight_TextChanged(object sender, EventArgs e)
        {

        }

        //Complete Shape Button
        private void button1_Click(object sender, EventArgs e)
        {
            try 
            {
                liveWire = findShortestPath(end.Y * ImageOperations.GetWidth(ImageMatrix) + end.X, firstNode.Y * ImageOperations.GetWidth(ImageMatrix) + firstNode.X);
                paths.Add(liveWire);

                Pen LinePenBlack = new Pen(Color.Black, 0.5f);
                Pen LinePenWhite = new Pen(Color.White, 0.5f);

                pictureBox1.Image = buffer;

                DrawPathInPictureBox(LinePenBlack, LinePenWhite, 0.8f);
                completedShape = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("something went wrong maybe there is no image loaded or there is no shape to complete it ");
            }

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
            graph = new SortedDictionary<int, double>[width * height];

            // Creating Adjacency List.
            for (int i = 0; i < graph.Length; i++)
            {
                int x = i % width;
                int y = i / width;


                // Right || Bottom || Top || Left

                int[] xSum = { 1, 0, 0, -1 };
                int[] ySum = { 0, 1, -1, 0 };
                char[] direction = { 'R', 'B', 'T', 'L' };

                graph[i] = new SortedDictionary<int, double>();

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
                                cost = 1E16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                        case 'R':
                            cost = ImageOperations.CalculateRightPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                        case 'T':
                            cost = ImageOperations.CalculateTopPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E16;
                            else
                                cost = 1 / cost;
                            graph[i].Add(pointNum, cost);
                            break;
                        case 'B':
                            cost = ImageOperations.CalculateBottomPixelEnergy(x, y, ImageMatrix);
                            if (cost == 0)
                                cost = 1E16;
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
            try
            {
                // ToDo: createfile.

                string fileName = @"Result.txt";
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                MessageBox.Show("I'm here");
                using (FileStream fs = File.Create(fileName))
                {// Create a new file     

                    string s = "The constructed graph\n\n";
                    // Add some text to file    
                    Byte[] info = new UTF8Encoding(true).GetBytes(s);
                    fs.Write(info, 0, info.Length);
                    
                    for (int i = 0; i < graph.Length; i++)
                    {
                        
                        s = " The  index node" + i + "\nEdges\n";
                        info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                        foreach (KeyValuePair<int, double> x in graph[i])
                        {
                            s = "edge from   " + i + "  To  " + x.Key + "  With Weights  " + x.Value + "\n";
                            info = new UTF8Encoding(true).GetBytes(s);
                            fs.Write(info, 0, info.Length);
                        }
                        s ="\n\n\n";
                        info = new UTF8Encoding(true).GetBytes(s);
                        fs.Write(info, 0, info.Length);
                    }
                }
               
                //MessageBox.Show("I have fininshed");

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }
        private void genreateImage()
        {

            CroppedMatrix = ImageOperations.OpenImage(OpenedFilePath);
                
            int width = ImageOperations.GetWidth(ImageMatrix);
            int height = ImageOperations.GetHeight(ImageMatrix);

            for (int i = 0; i < paths.Count; i++)
            {
                for (int j = 0; j < paths[i].Count; j++)
                {
                    taken[paths[i][j]] = true;
                }
            }
            
            
            for(int i =0; i < height; i++)
            {
                
                bool crop = true;
                int points = 0;
                for (int j = 0; j < width; j++)
                {
                    if (taken[i * width + j])
                        points++;
                }
                for (int j =0; j < width; j++)
                {
                    if (taken[i*width + j])
                    {
                        points--;
                        while(j < width)
                        {
                            if(j+1 < width)
                            {
                                j++;
                                if (taken[i * width + j])
                                {
                                    points--;
                                    continue;
                                }
                                else
                                    break;
                            }
                        }
                        
                        crop = !crop;

                        if (points == 0)
                            crop = true;
                        else if (points == 1)
                            crop = false;
                    }


                    if(crop)
                    {
                        CroppedMatrix[i, j].red =222;
                        CroppedMatrix[i, j].green = 222;
                        CroppedMatrix[i, j].blue = 222;
                    }

                    
                }
            }



            ImageOperations.DisplayImage(CroppedMatrix, pictureBox2);
        }
    }
}