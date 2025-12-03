using System;

namespace PBL_Squares
{
    internal class Program
    {
        static Random random = new Random();
        static int[,,,] pieces = new int[13, 24, 5, 5];
        static char[,] puzzle;
        
        static void Main()
        {
            Console.Write("Enter number of pieces (1–20): ");
            int numberOfPieces = Convert.ToInt32(Console.ReadLine());

            int[] square = new int[numberOfPieces];
            int[] countBySquares = new int[13];

            // Oyuncudan parça boyutlarını al
            GetPieceSizesFromUser(numberOfPieces, square, countBySquares); 

            // Parçaları oluştur ve normalize et, unique kontrolü yap
            GenerateAndValidatePieces(numberOfPieces, square);

            // Tüm oryantasyonları göster ve oyun ekranını hazırla
            ShowAllPieceOrientations(numberOfPieces, square);
    
            // Oyun Alanını (Puzzle'ı) Oluştur ve Ekrana Bas
            
            // Puzzle oluşturma
            GeneratePuzzle(square, numberOfPieces); // GeneratePuzzle kullanıcıdan boyutu alır ve puzzle'ı doldurur.
            // Oyun Ekranını Printleme
            PrintGameScreen(square, numberOfPieces);
            Console.ReadLine();
        }
        static void GetPieceSizesFromUser(int numberOfPieces, int[] square, int[] countBySquares)
        {
            int[] MaxPolyomino = { 0, 0, 1, 2, 5, 12, 35, 108, 369, 1285, 4655, 17073, 63600 };

            for (int i = 0; i < numberOfPieces; i++)
            {
                bool isValid = false;

                while (!isValid)
                {
                    Console.Write($"Enter number of squares for piece {i + 1} (2–12): ");
                    int squares = Convert.ToInt32(Console.ReadLine());

                    if (squares < 2 || squares > 12)
                    {
                        Console.WriteLine("ERROR: Value must be between 2 and 12.");
                        continue;
                    }

                    if (countBySquares[squares] >= MaxPolyomino[squares])
                    {
                        Console.WriteLine(
                            $"ERROR: Only {MaxPolyomino[squares]} unique pieces exist with {squares} squares.\n" +
                            $"You already entered {countBySquares[squares]}.\n" +
                            $"Please enter another value."
                        );
                        continue;
                    }

                    square[i] = squares;
                    countBySquares[squares]++;
                    isValid = true;
                }
            }
        }
        static void GenerateAndValidatePieces(int numberOfPieces, int[] square)
        {
            for (int i = 0; i < numberOfPieces; i++)
            {
                bool isDuplicate;
                do
                {
                    GeneratePiece(square[i], i);
                    NormalizePieceIn4D(square[i], i); // Normalizasyon burada yapılıyor

                    isDuplicate = false;

                    for (int previous = 0; previous < i; previous++)
                    {
                        int[,] piece1 = LoadPiece(square[i], i);
                        int[,] piece2 = LoadPiece(square[previous], previous);

                        if (PiecesEqual(piece1, piece2))
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                } while (isDuplicate);

                Console.WriteLine($"Generated Piece {i + 1} (Size: {square[i]})");
                //printpiece fonksiyonunu sildim ve yerine PrintMatrix kullandım
                int[,] generatedPiece = LoadPiece(square[i], i); // Parçayı 4D'den 2D'ye yükle
                PrintMatrix(generatedPiece);                      // 2D matrisi bas
            }
        }
        static void ShowAllPieceOrientations(int numberOfPieces, int[] square)
        {
            Console.WriteLine("\n## Parça Oryantasyonları Kontrolü ##");

            for (int idx = 0; idx < numberOfPieces; idx++)
            {
                Console.WriteLine($"\n--- Parça {idx + 1} ({square[idx]} Kare) ---");
                int[,] originalPiece = LoadPiece(square[idx], idx);

                // Orijinal Parça (Normalize Edilmiş)
                Console.WriteLine("Orijinal Parça:");
                PrintMatrix(originalPiece);

                // 4 Rotasyon
                int[,] current = originalPiece;
                for (int r = 1; r < 4; r++)
                {
                    int[,] rotated = new int[5, 5];
                    Rotate90(current, rotated);
                    Normalize(rotated); // Her operasyondan sonra normalizasyon gerekli
                    Console.WriteLine($"{r * 90}° Rotasyon:");
                    PrintMatrix(rotated);
                    current = rotated;
                }

                // Yatay Ters Çevirme (Left-Right Reverse)
                int[,] leftRight = new int[5, 5];
                ReverseLR(originalPiece, leftRight);
                Normalize(leftRight);
                Console.WriteLine("Yatay Ters Çevirme (LR):");
                PrintMatrix(leftRight);

                // Dikey Ters Çevirme (Up-Down Reverse)
                int[,] upDown = new int[5, 5];
                ReverseUD(originalPiece, upDown);
                Normalize(upDown);
                Console.WriteLine("Dikey Ters Çevirme (UD):");
                PrintMatrix(upDown);
            }
            Console.WriteLine("\n" + new string('-', 50));
        }

        // PrintPiece fonksiyonunun 2D matrisi basan genel bir versiyonu
        static void PrintMatrix(int[,] piece)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Console.Write(piece[i, j] == 1 ? 'X' : '.');
                }
                Console.WriteLine();
            }
        }

        static void PrintGameScreen(int[] square, int numberOfPieces)
        {
            if (puzzle == null)
            {
                Console.WriteLine("Puzzle is not generated yet.");
                return;
            }

            int puzzleSize = puzzle.GetLength(0);
            int pieceColumnStart = puzzleSize + 2; 

            // Başlık
            Console.WriteLine($"Round: 1 - Puzzle Size: {puzzleSize}x{puzzleSize}");
            Console.WriteLine(new string('-', pieceColumnStart) + " | Parçalar (Maks. 5x5)");

            // Parça indekslerini ve harflerini hazırla
            char[] pieceNames = new char[numberOfPieces];
            for (int i = 0; i < numberOfPieces; i++)
            {
                pieceNames[i] = (char)('A' + i);
            }
            
            // Satır satır basma
            for (int row = 0; row < Math.Max(puzzleSize, 5); row++) // Max 5, çünkü parçalar 5x5
            {
                // Sol Kısım (Puzzle)
                for (int col = 0; col < puzzleSize; col++)
                {
                    if (row < puzzleSize)
                    {
                        Console.Write(puzzle[row, col]);
                    }
                    else
                    {
                        Console.Write(' '); // Puzzle boyutundan küçükse boşluk
                    }
                }

                Console.Write(" | "); // Ayırıcı

                // Sağ Kısım (Parçalar)
                for (int idx = 0; idx < Math.Min(numberOfPieces, 4); idx++) // İlk 4 parçayı bas
                {
                    if (row < 5) // Parçalar 5x5 olduğu için sadece ilk 5 satırı bas
                    {
                        int squares = square[idx];
                        for (int j = 0; j < 5; j++)
                        {
                            // Seçilen parçanın orijinal halini (oryantasyonsuz) bas
                            if (pieces[squares, idx, row, j] == 1) 
                            {
                                Console.Write(pieceNames[idx]);
                            }
                            else
                            {
                                Console.Write(".");
                            }
                        }
                        Console.Write("  "); // Parçalar arası boşluk
                    }
                }
                
                Console.WriteLine();
            }
            
            if (numberOfPieces > 4)
            {
                Console.WriteLine($"... ve {numberOfPieces - 4} adet daha parça...");
            }
        }
        static void GeneratePiece(int squares, int index)
        {
            int[,] piece = new int[5, 5];

            int row = random.Next(0, 5);
            int column = random.Next(0, 5);
            piece[row, column] = 1;

            int placedNo = 1;

            while (placedNo < squares)
            {
                int newPlaced = AddNeighbor(piece, placedNo);
                if (newPlaced != placedNo)
                {
                    placedNo = newPlaced;
                }
            }

            Normalize(piece);
            SaveTo4D(piece, squares, index);
        }

        static int AddNeighbor(int[,] piece, int counter)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        int dir = random.Next(-1, 3);

                        int newI = i;
                        int newJ = j;

                        if (dir == -1)
                        {
                            newI = i - 1;
                        }
                        if (dir == 1)
                        {
                            newI = i + 1;
                        }
                        if (dir == 0)
                        {
                            newJ = j - 1;
                        }
                        if (dir == 2)
                        {
                            newJ = j + 1;
                        }

                        if (newI >= 0 && newI < 5 && newJ >= 0 && newJ < 5)
                        {
                            if (piece[newI, newJ] == 0)
                            {
                                piece[newI, newJ] = 1;
                                return counter + 1;
                            }
                        }
                    }
                }
            }
            return counter;
        }

        static void Normalize(int[,] piece)
        {
            int minRow = 5, minColumn = 5;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        if (i < minRow)
                        {
                            minRow = i;
                        }
                        if (j < minColumn)
                        {
                            minColumn = j;
                        }
                    }
                }
            }

            int[,] tmp = new int[5, 5];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        tmp[i - minRow, j - minColumn] = 1;
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    piece[i, j] = tmp[i, j];
                }
            } 
        }

        static void SaveTo4D(int[,] piece, int squares, int index)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    pieces[squares, index, i, j] = piece[i, j];
                }
            }
        }

        static void NormalizePieceIn4D(int squares, int index)
        {
            int[,] piece = LoadPiece(squares, index);
            Normalize(piece);
            SaveTo4D(piece, squares, index);
        }
        
        static int[,] LoadPiece(int squares, int index)
        {
            int[,] piece = new int[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    piece[i, j] = pieces[squares, index, i, j];
                }
            }
            return piece;
        }

        static bool PiecesEqual(int[,] piece1, int[,] piece2)
        {
            int[,] rotate90 = new int[5, 5];
            int[,] rotate180 = new int[5, 5];
            int[,] rotate270 = new int[5, 5];
            int[,] leftRight = new int[5, 5];
            int[,] upDown = new int[5, 5];

            Rotate90(piece1, rotate90);
            Rotate180(piece1, rotate180);
            Rotate270(piece1, rotate270);
            ReverseLR(piece1, leftRight);
            ReverseUD(piece1, upDown);

            if (isSame(piece1, piece2))
            {
                return true;
            }
            if (isSame(rotate90, piece2))
            {
                return true;
            }
            if (isSame(rotate180, piece2))
            {
                return true;
            }
            if (isSame(rotate270, piece2))
            {
                return true;
            }
            if (isSame(leftRight, piece2))
            {
                return true;
            }
            if (isSame(upDown, piece2))
            {
                return true;
            }

            return false;
        }

        static bool isSame(int[,] piece1, int[,] piece2)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece1[i, j] != piece2[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        static void Rotate90(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    row[j, 4 - i] = piece[i, j];
                } 
            }
        }

        static void Rotate180(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    row[4 - i, 4 - j] = piece[i, j];
                }
            }
        }

        static void Rotate270(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    row[4 - j, i] = piece[i, j];
                }
            }
        }

        static void ReverseLR(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    row[i, 4 - j] = piece[i, j];
                }
            }
        }

        static void ReverseUD(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    row[4 - i, j] = piece[i, j];
                }
            } 
        }
        static void GeneratePuzzle(int[] square, int counter)
        {
            Console.Write("Enter puzzle size (10–25): ");
            int size = Convert.ToInt32(Console.ReadLine());

            puzzle = new char[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    puzzle[i, j] = '.';
                }
            }

            for (int idx = 0; idx < counter; idx++)
            {
                int[,] piece = LoadPiece(square[idx], idx);
                int[,] orientation = RandomOrientation(piece);
                Normalize(orientation);

                bool isPlaced = false;

                for (int t = 0; t < 8000 && !isPlaced; t++)
                {
                    int row = random.Next(0, size);
                    int col = random.Next(0, size);

                    if (!CanPlace(orientation, row, col, size))
                    {
                        continue;
                    }

                    if (idx == 0)
                    {
                        PlacePiece(orientation, row, col, (char)('A' + idx));
                        isPlaced = true;
                    }
                    else
                    {
                        if (TouchesExisting(orientation, row, col, size))
                        {
                            PlacePiece(orientation, row, col, (char)('A' + idx));
                            isPlaced = true;
                        }
                    }
                }

                if (!isPlaced)
                {
                    Console.WriteLine($"Could not place piece {(char)('A' + idx)}!");
                }
            }
            PrintPuzzle(size);
        }

        static int[,] RandomOrientation(int[,] piece)
        {
            int[,] t = new int[5, 5];

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    t[i, j] = piece[i, j];

            int rotate = random.Next(0, 4);

            for (int r = 0; r < rotate; r++)
            {
                int[,] temp = new int[5, 5];
                Rotate90(t, temp);
                t = temp;
            }

            int reverse = random.Next(0, 3);

            if (reverse == 1)
            {
                int[,] temp = new int[5, 5];
                ReverseLR(t, temp);
                t = temp;
            }
            else if (reverse == 2)
            {
                int[,] temp = new int[5, 5];
                ReverseUD(t, temp);
                t = temp;
            }

            return t;
        }

        static bool CanPlace(int[,] piece, int row, int column, int size)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        if (row + i >= size || column + j >= size)
                        {
                            return false;
                        }

                        if (puzzle[row + i, column + j] != '.')
                        {
                            return false;
                        }
                    }
                }
            } 
            return true;
        }

        static void PlacePiece(int[,] piece, int row, int column, char letter)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        puzzle[row + i, column + j] = letter;
                    }
                }
            }
        }

        static void PrintPuzzle(int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(puzzle[i, j]);
                }
                Console.WriteLine();
            }
        }

        static bool TouchesExisting(int[,] piece, int row, int column, int size)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        int r = row + i;
                        int c = column + j;

                        if (r > 0 && puzzle[r - 1, c] != '.')
                        {
                            return true;
                        }
                        if (r < size - 1 && puzzle[r + 1, c] != '.')
                        {
                            return true;
                        }
                        if (c > 0 && puzzle[r, c - 1] != '.')
                        {
                            return true;
                        }
                        if (c < size - 1 && puzzle[r, c + 1] != '.')
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
    }
}
