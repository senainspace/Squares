// ================================================================
// CME1251 PROJECT - SQUARES GAME
// .NET Framework Console Application
// Stage 1 & Stage 2 - Piece Generation / Normalization / Orientation
// ================================================================

using System;
using System.Collections.Generic;

namespace SquaresGame
{
    class Program
    {
        // GRID SIZES
        const int PIECE_SIZE = 5;
        const int PUZZLE_H = 20;
        const int PUZZLE_W = 30;

        // RANDOM
        static Random rng = new Random();

        // ====================================================================
        // SMALL HELPER STRUCT TO STORE (X,Y)
        // ====================================================================
        struct Point
        {
            public int X;
            public int Y;
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        // ====================================================================
        // CLASS: PIECE
        // A piece is a 5x5 boolean grid.
        // ====================================================================
        class Piece
        {
            public bool[,] grid = new bool[PIECE_SIZE, PIECE_SIZE];

            // Count how many squares exist inside this piece
            public int CountSquares()
            {
                int c = 0;
                for (int y = 0; y < PIECE_SIZE; y++)
                    for (int x = 0; x < PIECE_SIZE; x++)
                        if (grid[y, x]) c++;
                return c;
            }

            // Make a deep copy
            public Piece Clone()
            {
                Piece p = new Piece();
                for (int y = 0; y < PIECE_SIZE; y++)
                    for (int x = 0; x < PIECE_SIZE; x++)
                        p.grid[y, x] = this.grid[y, x];
                return p;
            }

            // Convert piece into string lines for debugging
            public string[] ToText()
            {
                string[] lines = new string[PIECE_SIZE];
                for (int y = 0; y < PIECE_SIZE; y++)
                {
                    string s = "";
                    for (int x = 0; x < PIECE_SIZE; x++)
                        s += grid[y, x] ? "X" : ".";
                    lines[y] = s;
                }
                return lines;
            }
        }

        // ====================================================================
        // NORMALIZE A PIECE (SHIFT TO LEFT AND TOP)
        // ====================================================================
        static void Normalize(Piece p)
        {
            int minX = PIECE_SIZE, minY = PIECE_SIZE;

            // find the smallest bounding-box start
            for (int y = 0; y < PIECE_SIZE; y++)
                for (int x = 0; x < PIECE_SIZE; x++)
                    if (p.grid[y, x])
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                    }

            if (minX == PIECE_SIZE) return; // empty piece

            bool[,] newGrid = new bool[PIECE_SIZE, PIECE_SIZE];

            for (int y = 0; y < PIECE_SIZE; y++)
                for (int x = 0; x < PIECE_SIZE; x++)
                {
                    if (p.grid[y, x])
                    {
                        int nx = x - minX;
                        int ny = y - minY;
                        if (nx >= 0 && nx < PIECE_SIZE && ny >= 0 && ny < PIECE_SIZE)
                            newGrid[ny, nx] = true;
                    }
                }

            p.grid = newGrid;
        }

        // ====================================================================
        // ROTATE PIECE 90° CLOCKWISE
        // ====================================================================
        static Piece Rotate90(Piece p)
        {
            Piece r = new Piece();

            for (int y = 0; y < PIECE_SIZE; y++)
                for (int x = 0; x < PIECE_SIZE; x++)
                {
                    int nx = PIECE_SIZE - 1 - y;
                    int ny = x;
                    r.grid[ny, nx] = p.grid[y, x];
                }

            Normalize(r);
            return r;
        }

        // ====================================================================
        // REVERSE PIECE HORIZONTALLY
        // ====================================================================
        static Piece ReverseHorizontal(Piece p)
        {
            Piece r = new Piece();

            for (int y = 0; y < PIECE_SIZE; y++)
                for (int x = 0; x < PIECE_SIZE; x++)
                {
                    int nx = PIECE_SIZE - 1 - x;
                    r.grid[y, nx] = p.grid[y, x];
                }

            Normalize(r);
            return r;
        }

        // ====================================================================
        // GET ALL ORIENTATIONS (ROTATIONS + REVERSE)
        // ====================================================================
        static List<Piece> AllOrientations(Piece original)
        {
            List<Piece> list = new List<Piece>();

            Piece p = original.Clone();
            Normalize(p);

            // 4 rotations
            for (int i = 0; i < 4; i++)
            {
                Piece r = p.Clone();
                Normalize(r);
                list.Add(r);

                // reversed version of each rotation
                Piece rev = ReverseHorizontal(r);
                Normalize(rev);
                list.Add(rev);

                p = Rotate90(p);
            }
            return list;
        }

        // ====================================================================
        // COMPARE TWO PIECES (TRUE IF EQUIVALENT UNDER ANY ORIENTATION)
        // ====================================================================
        static bool PiecesEqual(Piece a, Piece b)
        {
            var allB = AllOrientations(b);

            foreach (var bb in allB)
            {
                bool same = true;
                for (int y = 0; y < PIECE_SIZE; y++)
                {
                    for (int x = 0; x < PIECE_SIZE; x++)
                    {
                        if (a.grid[y, x] != bb.grid[y, x])
                        {
                            same = false;
                            break;
                        }
                    }
                    if (!same) break;
                }
                if (same) return true;
            }
            return false;
        }

        // ====================================================================
        // STAGE 1: GENERATE RANDOM POLYOMINO (2–12 CELLS)
        // ====================================================================
        static Piece GenerateRandomPiece(int targetCells)
        {
            Piece p = new Piece();

            // BFS-like growth
            List<Point> cells = new List<Point>();

            // start at center
            Point start = new Point(2, 2);
            cells.Add(start);
            p.grid[start.Y, start.X] = true;

            while (cells.Count < targetCells)
            {
                // pick a random existing cell
                Point c = cells[rng.Next(cells.Count)];

                // pick a random direction
                List<Point> neighbors = new List<Point>();
                if (c.Y > 0) neighbors.Add(new Point(c.X, c.Y - 1));
                if (c.Y < PIECE_SIZE - 1) neighbors.Add(new Point(c.X, c.Y + 1));
                if (c.X > 0) neighbors.Add(new Point(c.X - 1, c.Y));
                if (c.X < PIECE_SIZE - 1) neighbors.Add(new Point(c.X + 1, c.Y));

                // choose a random neighbor
                Point n = neighbors[rng.Next(neighbors.Count)];

                if (!p.grid[n.Y, n.X])
                {
                    p.grid[n.Y, n.X] = true;
                    cells.Add(n);
                }
            }

            Normalize(p);
            return p;
        }

        // ====================================================================
        // STAGE 2.5: PIECE UNIQUENESS — ADD PIECE IF NOT DUPLICATE
        // ====================================================================
        static bool ContainsEquivalent(List<Piece> list, Piece p)
        {
            foreach (var q in list)
                if (PiecesEqual(q, p))
                    return true;
            return false;
        }

        // ====================================================================
        // STAGE 3: FORM PUZZLE (20x30) WITH RANDOM PIECES
        // ====================================================================
        static char[,] CreateEmptyPuzzle()
        {
            char[,] map = new char[PUZZLE_H, PUZZLE_W];
            for (int y = 0; y < PUZZLE_H; y++)
                for (int x = 0; x < PUZZLE_W; x++)
                    map[y, x] = '.';     // empty

            return map;
        }

        // ====================================================================
        // PLACE A PIECE INTO PUZZLE AT (ox, oy) USING A GIVEN LETTER SYMBOL
        // ====================================================================
        static void PlacePieceOnPuzzle(char[,] puzzle, Piece p, int ox, int oy, char symbol)
        {
            for (int y = 0; y < PIECE_SIZE; y++)
            {
                for (int x = 0; x < PIECE_SIZE; x++)
                {
                    if (p.grid[y, x])
                    {
                        int px = ox + x;
                        int py = oy + y;

                        if (px >= 0 && px < PUZZLE_W && py >= 0 && py < PUZZLE_H)
                        {
                            puzzle[py, px] = symbol;
                        }
                    }
                }
            }
        }

        // ====================================================================
        // CHECK IF PIECE CAN BE PLACED (NOT COLLIDING & INSIDE BOUNDS)
        // ====================================================================
        static bool CanPlacePiece(char[,] puzzle, Piece p, int ox, int oy)
        {
            for (int y = 0; y < PIECE_SIZE; y++)
            {
                for (int x = 0; x < PIECE_SIZE; x++)
                {
                    if (p.grid[y, x])
                    {
                        int px = ox + x;
                        int py = oy + y;

                        if (px < 0 || px >= PUZZLE_W || py < 0 || py >= PUZZLE_H)
                            return false; // out of bounds

                        if (puzzle[py, px] != '.')
                            return false; // collision
                    }
                }
            }
            return true;
        }

        // ====================================================================
        // COMPUTE PERIMETER OF THE PUZZLE SHAPE
        // ====================================================================
        static int ComputePerimeter(char[,] puzzle)
        {
            int per = 0;

            for (int y = 0; y < PUZZLE_H; y++)
            {
                for (int x = 0; x < PUZZLE_W; x++)
                {
                    if (puzzle[y, x] != '.')
                    {
                        // check 4 neighbors
                        if (y == 0 || puzzle[y - 1, x] == '.') per++;
                        if (y == PUZZLE_H - 1 || puzzle[y + 1, x] == '.') per++;
                        if (x == 0 || puzzle[y, x - 1] == '.') per++;
                        if (x == PUZZLE_W - 1 || puzzle[y, x + 1] == '.') per++;
                    }
                }
            }
            return per;
        }

        // ====================================================================
        // COUNT TOTAL FILLED SQUARES IN PUZZLE
        // ====================================================================
        static int PuzzleCountSquares(char[,] puzzle)
        {
            int c = 0;
            for (int y = 0; y < PUZZLE_H; y++)
                for (int x = 0; x < PUZZLE_W; x++)
                    if (puzzle[y, x] != '.') c++;
            return c;
        }

        // ====================================================================
        // COMPUTE REGULARITY = squares / ((perimeter/4)^2)
        // ====================================================================
        static double ComputeRegularity(char[,] puzzle)
        {
            int sq = PuzzleCountSquares(puzzle);
            int per = ComputePerimeter(puzzle);
            if (per == 0) return 0;

            double denom = Math.Pow(per / 4.0, 2);
            if (denom == 0) return 0;

            return sq / denom;
        }

        // ====================================================================
        // GENERATE COMPLETE PUZZLE
        // pieces: list of piece sizes (like 4,4,5,8,9...)
        // ====================================================================
        static char[,] GeneratePuzzle(List<Piece> pieceBank, List<int> pieceSizes,
                                      double minR, double maxR)
        {
            char[,] puzzle;
            double reg = 0;

            // Attempt many times until a puzzle fits regularity bounds
            for (int attempt = 0; attempt < 200; attempt++)
            {
                puzzle = CreateEmptyPuzzle();

                char sym = 'A';
                foreach (int sz in pieceSizes)
                {
                    // find a piece with EXACT sz cells
                    List<Piece> candidates = new List<Piece>();
                    foreach (var pc in pieceBank)
                        if (pc.CountSquares() == sz)
                            candidates.Add(pc);

                    if (candidates.Count == 0)
                        continue; // skip if none found (rare)

                    Piece basePiece = candidates[rng.Next(candidates.Count)];
                    Piece p = basePiece.Clone();

                    // Random orientation
                    var ori = AllOrientations(p);
                    p = ori[rng.Next(ori.Count)];

                    // Random position until find a valid slot
                    bool placed = false;
                    for (int tries = 0; tries < 100; tries++)
                    {
                        int ox = rng.Next(0, PUZZLE_W - PIECE_SIZE);
                        int oy = rng.Next(0, PUZZLE_H - PIECE_SIZE);

                        if (CanPlacePiece(puzzle, p, ox, oy))
                        {
                            PlacePieceOnPuzzle(puzzle, p, ox, oy, sym);
                            placed = true;
                            break;
                        }
                    }

                    sym++;
                    if (sym > 'Z') sym = 'a';
                    if (!placed)
                    {
                        // failed to place all pieces — restart puzzle attempt
                        goto NEXT_ATTEMPT;
                    }
                }

                // compute regularity
                reg = ComputeRegularity(puzzle);
                if (reg >= minR && reg <= maxR)
                {
                    return puzzle; // SUCCESS
                }

            NEXT_ATTEMPT:;
            }

            // Fail-safe (should rarely happen)
            return CreateEmptyPuzzle();
        }

        // ====================================================================
        // PRINT PUZZLE TO SCREEN
        // ====================================================================
        static void PrintPuzzle(char[,] puzzle)
        {
            Console.WriteLine();
            for (int y = 0; y < PUZZLE_H; y++)
            {
                for (int x = 0; x < PUZZLE_W; x++)
                    Console.Write(puzzle[y, x]);
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        // ====================================================================
        // STAGE 4: GAMEPLAY — PLAYER PLACES PIECES USING W A S D / R / F / SPACE
        // ====================================================================

        static char[,] puzzleForPlayer;
        static bool[,] buildGrid = new bool[PUZZLE_H, PUZZLE_W];

        // print player's building area
        static void PrintBuildArea()
        {
            Console.Clear();
            for (int y = 0; y < PUZZLE_H; y++)
            {
                for (int x = 0; x < PUZZLE_W; x++)
                {
                    Console.Write(buildGrid[y, x] ? 'X' : '.');
                }
                Console.WriteLine();
            }
        }

        // place piece in the player area
        static bool TryPlaceInBuildArea(Piece p, int ox, int oy)
        {
            if (!CanPlacePiece_Player(p, ox, oy)) return false;

            for (int y = 0; y < PIECE_SIZE; y++)
                for (int x = 0; x < PIECE_SIZE; x++)
                    if (p.grid[y, x])
                    {
                        buildGrid[oy + y, ox + x] = true;
                    }
            return true;
        }

        // check if a piece can be placed in player's build area
        static bool CanPlacePiece_Player(Piece p, int ox, int oy)
        {
            for (int y = 0; y < PIECE_SIZE; y++)
                for (int x = 0; x < PIECE_SIZE; x++)
                    if (p.grid[y, x])
                    {
                        int px = ox + x;
                        int py = oy + y;

                        if (px < 0 || px >= PUZZLE_W || py < 0 || py >= PUZZLE_H)
                            return false;

                        if (buildGrid[py, px])
                            return false;
                    }
            return true;
        }

        // check if final build matches puzzle
        static bool CompareBuildToPuzzle(char[,] puzzle)
        {
            for (int y = 0; y < PUZZLE_H; y++)
            {
                for (int x = 0; x < PUZZLE_W; x++)
                {
                    bool target = puzzle[y, x] != '.';
                    bool player = buildGrid[y, x];

                    if (target != player) return false;
                }
            }
            return true;
        }

        // ====================================================================
        // GAME ROUND LOGIC
        // ====================================================================
        static int PlayRound(List<Piece> pieceBank, int round)
        {
            Console.Clear();
            Console.WriteLine("=== ROUND " + round + " ===");

            // ask number of pieces
            Console.Write("How many pieces this round? ");
            int n = int.Parse(Console.ReadLine());

            List<int> sizes = new List<int>();
            Console.WriteLine("Enter piece sizes (2 to 12 squares):");

            for (int i = 0; i < n; i++)
            {
                Console.Write("Piece " + (i + 1) + ": ");
                sizes.Add(int.Parse(Console.ReadLine()));
            }

            Console.Write("Min Regularity: ");
            double minR = double.Parse(Console.ReadLine());
            Console.Write("Max Regularity: ");
            double maxR = double.Parse(Console.ReadLine());

            Console.Clear();
            Console.WriteLine("Generating puzzle...");
            System.Threading.Thread.Sleep(500);

            // create puzzle
            char[,] puzzle = GeneratePuzzle(pieceBank, sizes, minR, maxR);
            puzzleForPlayer = puzzle;

            Console.WriteLine("Puzzle generated. Regularity = " + ComputeRegularity(puzzle).ToString("0.0000"));
            PrintPuzzle(puzzle); // for debugging/hint

            // reset build area
            for (int y = 0; y < PUZZLE_H; y++)
                for (int x = 0; x < PUZZLE_W; x++)
                    buildGrid[y, x] = false;

            // PLAY
            foreach (int sz in sizes)
            {
                // pick random piece with correct size
                List<Piece> cand = new List<Piece>();
                foreach (var pc in pieceBank)
                    if (pc.CountSquares() == sz)
                        cand.Add(pc);

                if (cand.Count == 0) continue;

                Piece basePiece = cand[rng.Next(cand.Count)];
                Piece cur = basePiece.Clone();

                int cx = 0, cy = 0;
                bool placed = false;

                while (!placed)
                {
                    PrintBuildArea();
                    Console.WriteLine();
                    Console.WriteLine("Placing piece with " + sz + " squares");
                    Console.WriteLine("Use W A S D to move, R rotate, F flip, SPACE to place, Q to quit");

                    // draw current piece (overlay)
                    DrawGhostPiece(cur, cx, cy);

                    ConsoleKey key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.W) cy--;
                    if (key == ConsoleKey.S) cy++;
                    if (key == ConsoleKey.A) cx--;
                    if (key == ConsoleKey.D) cx++;

                    if (key == ConsoleKey.R)
                        cur = Rotate90(cur);

                    if (key == ConsoleKey.F)
                        cur = ReverseHorizontal(cur);

                    if (key == ConsoleKey.Q)
                        return -1; // quit

                    if (key == ConsoleKey.Spacebar)
                    {
                        if (TryPlaceInBuildArea(cur, cx, cy))
                            placed = true;
                    }
                }
            }

            // compare final result
            if (CompareBuildToPuzzle(puzzle))
            {
                Console.Clear();
                Console.WriteLine("Puzzle completed successfully!");

                int squares = PuzzleCountSquares(puzzle);
                double R = ComputeRegularity(puzzle);
                int score = (int)(squares * Math.Pow(4 * R, 4));

                Console.WriteLine("Round Score = " + score);
                Console.ReadKey();
                return score;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Incorrect shape! GAME OVER.");
                Console.ReadKey();
                return -1;
            }
        }

        // draw current floating piece on build area view
        static void DrawGhostPiece(Piece p, int ox, int oy)
        {
            Console.WriteLine("\nCurrent Piece Preview:");

            for (int y = 0; y < PIECE_SIZE; y++)
            {
                for (int x = 0; x < PIECE_SIZE; x++)
                {
                    Console.Write(p.grid[y, x] ? "X" : ".");
                }
                Console.WriteLine();
            }
        }


        // ====================================================================
        // MAIN ENTRY
        // ====================================================================
        static void Main(string[] args)
        {
            Console.Title = "CME1251 - Squares Game";

            // Create piece bank (unique pieces)
            Console.WriteLine("Generating unique piece bank. Please wait...");
            List<Piece> pieceBank = new List<Piece>();

            // generate many random pieces 2–12 squares
            for (int sz = 2; sz <= 12; sz++)
            {
                int attempts = 0;

                while (true)
                {
                    attempts++;
                    if (attempts > 300) break;

                    Piece p = GenerateRandomPiece(sz);
                    if (!ContainsEquivalent(pieceBank, p))
                        pieceBank.Add(p);

                    if (pieceBank.Count > 200) break; // safety limit
                }
            }

            Console.WriteLine("Piece bank ready. Total unique pieces: " + pieceBank.Count);
            Console.WriteLine("Press a key to start game.");
            Console.ReadKey();

            int totalScore = 0;
            int round = 1;

            while (true)
            {
                int rscore = PlayRound(pieceBank, round);
                if (rscore < 0)
                {
                    Console.Clear();
                    Console.WriteLine("Final Score: " + totalScore);
                    Console.WriteLine("Thanks for playing!");
                    Console.ReadKey();
                    break;
                }

                totalScore += rscore;
                round++;
            }
        }
    }
}
