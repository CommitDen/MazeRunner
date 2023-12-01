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
            DrawMaze();
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
            maze = new int[mazeSize + 2, mazeSize + 2];

            // Fill the maze with walls
            for (int i = 0; i < mazeSize + 2; i++)
            {
                for (int j = 0; j < mazeSize + 2; j++)
                {
                    maze[i, j] = 1;
                }
            }

            // Create openings on the edges (no border)
            for (int i = 1; i <= mazeSize; i++)
            {
                maze[i, 0] = 0;
                maze[i, mazeSize + 1] = 0;
                maze[0, i] = 0;
                maze[mazeSize + 1, i] = 0;
            }

            // Generate a solvable maze starting from the center
            GenerateMazeRecursive(mazeSize / 2, mazeSize / 2);

            // Draw the maze
            DrawMaze();
        }

        private void GenerateMazeRecursive(int x, int y)
        {
            // Shuffle the directions (N, E, S, W)
            int[] directions = { 0, 1, 2, 3 };
            Shuffle(directions);

            foreach (int direction in directions)
            {
                int nx = x + dx[direction];
                int ny = y + dy[direction];

                if (IsInside(nx, ny) && maze[nx, ny] == 1)
                {
                    // Carve a passage
                    maze[x + dx[direction] * 2, y + dy[direction] * 2] = 0;
                    maze[nx, ny] = 0;

                    // Recur
                    GenerateMazeRecursive(nx, ny);
                }
            }
        }

        private bool IsInside(int x, int y)
        {
            return x > 0 && x <= mazeSize && y > 0 && y <= mazeSize;
        }

        private void Shuffle<T>(T[] array)
        {
            Random random = new Random();
            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        private readonly int[] dx = { 0, 1, 0, -1 }; // Directions (N, E, S, W)
        private readonly int[] dy = { -1, 0, 1, 0 };
    }
}
