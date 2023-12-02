using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Maze_Forms.Views;

namespace Maze_Forms
{
    public partial class Form1 : Form
    {
        private Panel panel1;

        private int[,] maze;
        private const int cellSize = 20; // Adjust the cell size as needed
        private const int mazeSize = 20;

        public Form1()
        {
            InitializeComponent();
            panel1 = new Panel();


            // Load the initial UserControl
            LoadPartialForm(new MenuView());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void LoadPartialForm(UserControl partialForm)
        {
            // Clear the existing controls in the container
            panel1.Controls.Clear();

            // Dock the UserControl to fill the panel
            partialForm.Dock = DockStyle.Fill;

            // Add the UserControl to the panel
            panel1.Controls.Add(partialForm);
        }

        private void button_Run_Click(object sender, EventArgs e)
        {
            GenerateMaze();
        }

        private void DrawMaze()
        {
            // Create a bitmap as the canvas for drawing
            Bitmap mazeBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            using (Graphics g = Graphics.FromImage(mazeBitmap))
            {
                for (int i = 0; i < maze.GetLength(0); i++)
                {
                    for (int j = 0; j < maze.GetLength(1); j++)
                    {
                        Brush brush = maze[i, j] == 1 ? Brushes.Black : Brushes.White;
                        brush = maze[i, j] == 2 ? Brushes.Green : brush;
                        g.FillRectangle(brush, j * cellSize, i * cellSize, cellSize, cellSize);
                    }
                }
            }

            // Assign the bitmap to the PictureBox.Image
            pictureBox1.Image = mazeBitmap;
        }

        private void SolveMaze()
        {
            // Your maze solving logic goes here
            // This is just a placeholder for demonstration
            MessageBox.Show("Maze solved!");
        }

        private void ClearMaze()
        {
            // Clear the PictureBox content
            pictureBox1.Image = null;
        }

        private void GenerateMaze()
        {
            // Initialize maze with borders
            maze = new int[mazeSize * 2 + 1, mazeSize * 2 + 1];

            // Fill the maze with walls
            for (int i = 0; i < mazeSize * 2 + 1; i++)
            {
                for (int j = 0; j < mazeSize * 2 + 1; j++)
                {
                    maze[i, j] = 1;
                }
            }

            // Create openings on the edges (no border)
            for (int i = 0; i < mazeSize * 2 + 1; i++)
            {
                maze[i, 0] = 0;
                maze[i, mazeSize * 2] = 0;
                maze[0, i] = 0;
                maze[mazeSize * 2, i] = 0;
            }

            // Generate a solvable maze using randomized Prim's algorithm
            GenerateMazePrim();

            // Draw the maze
            DrawMaze();
        }

        private void GenerateMazePrim()
        {
            Random random = new Random();
            List<Tuple<int, int>> walls = new List<Tuple<int, int>>();

            // Initialize cells
            for (int i = 2; i < mazeSize * 2; i += 2)
            {
                for (int j = 2; j < mazeSize * 2; j += 2)
                {
                    maze[i, j] = 0;
                    walls.Add(new Tuple<int, int>(i, j + 1));
                    walls.Add(new Tuple<int, int>(i + 1, j));
                }
            }

            while (walls.Count > 0)
            {
                // Randomly choose a wall
                int index = random.Next(walls.Count);
                var wall = walls[index];
                walls.RemoveAt(index);

                int x = wall.Item1;
                int y = wall.Item2;

                // If exactly one of the neighboring cells is visited
                if (IsValid(x - 2, y) && IsValid(x + 2, y) && IsValid(x, y - 2) && IsValid(x, y + 2))
                {
                    if ((maze[x - 2, y] == 0 && maze[x + 2, y] == 1 && maze[x, y - 2] == 1 && maze[x, y + 2] == 1) ||
                        (maze[x - 2, y] == 1 && maze[x + 2, y] == 0 && maze[x, y - 2] == 1 && maze[x, y + 2] == 1) ||
                        (maze[x - 2, y] == 1 && maze[x + 2, y] == 1 && maze[x, y - 2] == 0 && maze[x, y + 2] == 1) ||
                        (maze[x - 2, y] == 1 && maze[x + 2, y] == 1 && maze[x, y - 2] == 1 && maze[x, y + 2] == 0))
                    {
                        // Carve a passage
                        maze[x, y] = 0;
                        maze[x - (x % 2), y - (y % 2)] = 0;

                        // Add neighboring walls
                        if (IsValid(x - 2, y)) walls.Add(new Tuple<int, int>(x - 2, y));
                        if (IsValid(x + 2, y)) walls.Add(new Tuple<int, int>(x + 2, y));
                        if (IsValid(x, y - 2)) walls.Add(new Tuple<int, int>(x, y - 2));
                        if (IsValid(x, y + 2)) walls.Add(new Tuple<int, int>(x, y + 2));
                    }
                }
            }

            // Generate the goal area within the maze
            GenerateMazeWithGoal();
        }

        private void GenerateMazeWithGoal()
        {
            // Set the size of the goal area (2x2 for even mazeSize, 3x3 for odd mazeSize)
            int goalSize = mazeSize % 2 == 0 ? 2 : 3;

            // Calculate the starting position for the goal area in the center of the maze
            int goalStartX = mazeSize - goalSize + 2; //+2 to accomodate for walls
            int goalStartY = mazeSize - goalSize + 2; 

            // Set the goal area in the center of the maze
            for (int i = goalStartX; i < goalStartX + goalSize; i++)
            {
                for (int j = goalStartY; j < goalStartY + goalSize; j++)
                {
                    maze[i, j] = 2; // 0 for open space in the goal area
                }
            }
        }

        private bool IsValid(int x, int y)
        {
            return x >= 0 && x < mazeSize * 2 + 1 && y >= 0 && y < mazeSize * 2 + 1;
        }
    }
}
