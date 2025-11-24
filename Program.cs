using System;

namespace SquaresProject
{
    internal class Program
    {
        const int SIZE = 5;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            char[,] piece = GeneratePiece(rnd.Next(2,12));
            PrintGrid(piece);              
        }
        static char[,] GeneratePiece(int squareCount)
        {
            char[,] grid = CreateEmptyGrid();
            int startX = rnd.Next(0, SIZE);
            int startY = rnd.Next(0, SIZE);
            grid[startX, startY] = 'X';

            for (int i = 1; i < squareCount; i++)
            {
                NeighborList neighbors = CollectNeighbors(grid);

                if (neighbors.count == 0)
                    break;

                int randomIndex = rnd.Next(0, neighbors.count);

                int neighborX = neighbors.data[randomIndex, 0];
                int neighborY = neighbors.data[randomIndex, 1];

                grid[neighborX, neighborY] = 'X';
            }

            grid = ShiftLeft(grid);

            return grid;
        }
        struct NeighborList
        {
            public int[,] data;
            public int count;

            public NeighborList(int maxSize)
            {
                data = new int[maxSize, 2];
                count = 0;
            }
        }
        static NeighborList CollectNeighbors(char[,] grid)
        {
            NeighborList neighbors = new NeighborList(100);

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++) 
                {
                    if (grid[x, y] == 'X')
                    {
                        neighbors = TryAddNeighbor(grid, x + 1, y, neighbors);
                        neighbors = TryAddNeighbor(grid, x - 1, y, neighbors);
                        neighbors = TryAddNeighbor(grid, x, y + 1, neighbors);
                        neighbors = TryAddNeighbor(grid, x, y - 1, neighbors);
                    }
                }
            }
            
            return neighbors;
        }
        static NeighborList TryAddNeighbor(char[,] grid, int x, int y, NeighborList neighbors)
        {
            if (x >= 0 && x < SIZE && y >= 0 && y < SIZE)
            {
                if (grid[x, y] == '.')
                {
                    neighbors.data[neighbors.count, 0] = x;
                    neighbors.data[neighbors.count, 1] = y;
                    neighbors.count++;
                }
            }
            return neighbors;
        }
        static char[,] CreateEmptyGrid()
        {
            char[,] dot = new char[SIZE, SIZE];

            for (int x = 0; x < SIZE; x++)
                for (int y = 0; y < SIZE; y++)
                    dot[x, y] = '.';

            return dot;
        }
        static char[,] ShiftLeft(char[,] grid)
        {
            int shift = 0;

            while (shift < SIZE && IsColumnEmpty(grid, shift))
                shift++;

            if (shift == 0)
                return grid;

            char[,] newGrid = CreateEmptyGrid();

            for (int x = shift; x < SIZE; x++)  
            {
                for (int y = 0; y < SIZE; y++)  
                {
                    newGrid[x - shift, y] = grid[x, y];
                }
            }

            return newGrid;
        }
        static bool IsColumnEmpty(char[,] grid, int col)
        {
            for (int y = 0; y < SIZE; y++)
            {
                if (grid[col, y] == 'X')
                    return false;
            }
            return true;
        }
        static void PrintGrid(char[,] grid)
        {
            int startLeft = Console.CursorLeft;
            int startTop = Console.CursorTop;

            for (int y = 0; y < SIZE; y++)
            {
                Console.SetCursorPosition(startLeft, startTop + y);
                for (int x = 0; x < SIZE; x++) 
                {
                    Console.Write(grid[x, y]);
                }
            }
        }

        



    }
}