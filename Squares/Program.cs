using System;

class Program
{
    // random piece üret
    static void GeneratePiece(int[,] grid, int size)
    {
        Random random = new Random();

        // gridi boşalt
        for (int x = 0; x < 5; x++)
        for (int y = 0; y < 5; y++)
            grid[x, y] = 0;

        // gridin içinde piecein ilk parçası için random başlangıç noktası
        int sx = random.Next(0, 5);
        int sy = random.Next(0, 5);
        grid[sx, sy] = 1;

        // mevcut karelerin listesi
        (int x, int y)[] cells = new (int, int)[12];
        int count = 0;
        cells[count++] = (sx, sy);

        // 4 yön
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        while (count < size)
        {
            int index = random.Next(0, count);
            int cx = cells[index].x;
            int cy = cells[index].y;

            int d = random.Next(0, 4);
            int nx = cx + dx[d];
            int ny = cy + dy[d];

            if (nx >= 0 && nx < 5 && ny >= 0 && ny < 5)
            {
                if (grid[nx, ny] == 0)
                {
                    grid[nx, ny] = 1;
                    cells[count++] = (nx, ny);
                }
            }
        }
    }
    // normalize et
    static void Normalize(int[,] grid)
    {
        int minX = 5;
        int minY = 5;

        // min koordinatları bul
        for (int x = 0; x < 5; x++)
        for (int y = 0; y < 5; y++)
            if (grid[x, y] == 1)
            {
                if (x < minX) minX = x;
                if (y < minY) minY = y;
            }

        int[,] newGrid = new int[5, 5];

        for (int x = 0; x < 5; x++)
        for (int y = 0; y < 5; y++)
            if (grid[x, y] == 1)
                newGrid[x - minX, y - minY] = 1;

        // geri kopyala
        for (int x = 0; x < 5; x++)
        for (int y = 0; y < 5; y++)
            grid[x, y] = newGrid[x, y];
    }
    // print piece
    static void PrintPieces(int[][,] pieces, int startX)
    {
        int piecesPerRow = 5; // her satırda 5 parça
        int cursorY = 2;      

        char letter = 'A';

        for (int rowStart = 0; rowStart < pieces.Length; rowStart += piecesPerRow)
        {
            int rowEnd = Math.Min(rowStart + piecesPerRow, pieces.Length);

            // 5 satırlık bir parça satırını yazıyoruz
            for (int y = 0; y < 5; y++)
            {
                int cursorX = startX;

                for (int p = rowStart; p < rowEnd; p++)
                {
                    int[,] grid = new int[5, 5];
                    grid = pieces[p];

                    Console.SetCursorPosition(cursorX, cursorY + y);

                    for (int x = 0; x < 5; x++)
                        Console.Write(grid[x, y] == 1 ? (char)('A' + p) : '.');

                    cursorX += 8; // her parçadan sonra boşluk
                }
            }

            cursorY += 6; // bir sonraki parça satır grubuna geç
        }
    }
    static int[][,] GenerateMultiplePieces(int pieceCount)
    {
        int[][,] pieces = new int[pieceCount][,];
        Random rnd = new Random();

        for (int i = 0; i < pieceCount; i++)
        {
            pieces[i] = new int[5, 5];
            int size = rnd.Next(2, 13);

            GeneratePiece(pieces[i], size);
            Normalize(pieces[i]);
        }
        return pieces;
    }
    static void Main()
    {   
        int[][,] pieces = GenerateMultiplePieces(15);
        Console.Clear();
        PrintPieces(pieces, 10);  
        Console.ReadLine();
    }
    
}
