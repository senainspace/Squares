using System;

namespace Stage1_PieceOnlyFunctions
{
    class Program
    {
        const int SIZE = 5;
        static Random rng = new Random();

        // === Bir hücre işaretli mi? ===
        static bool IsFilled(bool[,] g, int y, int x)
        {
            return g[y, x];
        }

        // === Hücreyi doldur ===
        static void Fill(bool[,] g, int y, int x)
        {
            g[y, x] = true;
        }

        // === Grid oluştur ===
        static bool[,] CreateEmptyGrid()
        {
            return new bool[SIZE, SIZE];
        }

        // === Grid'i yazdır ===
        static void PrintGrid(bool[,] g)
        {
            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                    Console.Write(g[y, x] ? "X" : ".");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        // === Normalize et: sola & yukarı yasla ===
        static void Normalize(bool[,] g)
        {
            int minX = SIZE, minY = SIZE;

            for (int y = 0; y < SIZE; y++)
                for (int x = 0; x < SIZE; x++)
                    if (g[y, x])
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                    }

            if (minX == SIZE && minY == SIZE) return; // hiç dolu hücre yoksa

            bool[,] newG = CreateEmptyGrid();

            for (int y = 0; y < SIZE; y++)
                for (int x = 0; x < SIZE; x++)
                    if (g[y, x])
                    {
                        newG[y - minY, x - minX] = true;
                    }

            // yeni grid'i geri yükle
            for (int y = 0; y < SIZE; y++)
                for (int x = 0; x < SIZE; x++)
                    g[y, x] = newG[y, x];
        }

        // === Rastgele 4-yönlü polyomino büyüt ===
        static bool[,] GeneratePiece(int target)
        {
            bool[,] g = CreateEmptyGrid();

            int[,] dirs = new int[,] { {0,-1}, {0,1}, {-1,0}, {1,0} };

            // başlama noktası (orta)
            int cx = 2, cy = 2;
            Fill(g, cy, cx);

            // büyüyen hücre listesi
            int count = 1;

            int[] listX = new int[target];
            int[] listY = new int[target];
            listX[0] = cx;
            listY[0] = cy;

            while (count < target)
            {
                int pick = rng.Next(count);
                int x = listX[pick];
                int y = listY[pick];

                // rastgele yön
                int dx = 0, dy = 0;
                int tries = 0;
                while (true)
                {
                    int dir = rng.Next(4);
                    dx = dirs[dir, 0];
                    dy = dirs[dir, 1];

                    int nx = x + dx;
                    int ny = y + dy;

                    tries++;

                    if (nx >= 0 && nx < SIZE && ny >= 0 && ny < SIZE)
                    {
                        if (!g[ny, nx])
                        {
                            g[ny, nx] = true;
                            listX[count] = nx;
                            listY[count] = ny;
                            count++;
                            break;
                        }
                    }

                    if (tries > 15) break;
                }
            }

            Normalize(g);
            return g;
        }

        static void Main(string[] args)
        {
            while (true)
            {Console.Write("Kaç kareli parça istiyorsun (2–12): ");
            int t = Convert.ToInt32(Console.ReadLine());

            bool[,] piece = GeneratePiece(t);

            Console.WriteLine("\nÜretilen Parça:");
            PrintGrid(piece);

            Console.ReadKey();}
        }
    }
}
