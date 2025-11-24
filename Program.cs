namespace puzzle
{
    internal class Program
    {



        static int[,] memory = new int[5, 5];

        static void Main(string[] args)
        {

            int[,] mat = new int[5, 5];
            int piece = RandomSquaresNumber();
            GeneratePiece(mat, piece);

            Console.WriteLine("Piece is getting generated: ");
            Printing(mat);

            NormalizingPieces(mat);

            Console.WriteLine(" Piece is getting shifted:");
            Printing(mat);



            SavingToMemory(mat);



        }

        static int RandomSquaresNumber() {

            Random rnd = new Random();
            return rnd.Next(0, 21);

        }



        static void GeneratePiece(int[,] matrice, int square)
        {

            Console.Write("Enter a piece's square number: ");
            square = Convert.ToInt16(Console.ReadLine());

            Console.Write("Enter regularity: ");
            int regularity = Convert.ToInt16(Console.ReadLine());




            int counter = 1;

            Random rnd = new Random();
            int i = rnd.Next(0, 5);
            int j = rnd.Next(0, 5);
            matrice[i, j] = 1;

            while (square > counter)
            {
                int x = rnd.Next(0, 5);


                int y = rnd.Next(0, 5);

                if (matrice[x, y] == 0 && SameEdgeControl(matrice, x, y))
                {

                    matrice[x, y] = 1;
                    counter++;

                }

            }

        }


        static bool SameEdgeControl(int[,] matrice, int i, int j)
        {

            if (i > 0 && matrice[i - 1, j] == 1)


            {
                return true;
            }

            if (i < 4 && matrice[i + 1, j] == 1)
            {
                return true;
            }

            if (j > 0 && matrice[i, j - 1] == 1)

            {
                return true;
            }

            if (j < 4 && matrice[i, j + 1] == 1)

            {
                return true;
            }
            return false;


        }

        static void NormalizingPieces(int[,] matrice)
        {

            int minimumaRowNumber = 5;
            int minimumColumnNumber = 5;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {


                    if (matrice[i, j] == 1 && i < minimumaRowNumber)
                    {
                        minimumaRowNumber = i;
                    }
                }

            }



            for (int j = 0; j < 5; j++)
            {
                {


                    for (int i = 0; i < 5; i++)
                    {

                        if (matrice[i, j] == 1 && j < minimumColumnNumber)
                        {

                            minimumColumnNumber = j;

                        }

                    }

                }

            }


            int[,] temporaryMatrice = new int[5, 5];

            for (int i = 0; i < 5; i++)
            {

                for (int j = 0; j < 5; j++)
                {

                    if (matrice[i, j] == 1)
                    {
                        int newi = i - minimumaRowNumber;
                        int newj = j - minimumColumnNumber;
                        temporaryMatrice[newi, newj] = 1;
                    }

                }

            }

            for (int i = 0; i < 5; i++)
            {

                for (int j = 0; j < 5; j++)
                {

                    matrice[i, j] = temporaryMatrice[i, j];

                }


            }


        }


        static void SavingToMemory(int[,] matrice)
        {

            for (int i = 0; i < 5; i++)
            {

                for (int j = 0; j < 5; j++)
                {
                    memory[i, j] = matrice[i, j];
                }

            }


        }

        static void Printing(int[,] matrice)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Console.Write(matrice[i, j] == 1 ? 'X' : '.');
                }
                Console.WriteLine();
            }
        }







        static void RotatePiece90(int[,] matrice)
        {

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (matrice[i, j] == 1)
                    {

                    }
                }

            }

        }

        static void RotatePiece180()
        {


        }
        



        static void RotatePiece270()
        {

        }



        static void ReversePiece()
        {


        }




    }
}


