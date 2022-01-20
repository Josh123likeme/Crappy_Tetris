using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Game
    {

        Random random = new Random();

        private SpotType[,] board;

        private Block activeBlock;
        private BlockType inactiveBlock;

        public int width;
        public int height;

        public Game(int boardWidth, int boardHeight)
        {

            width = boardWidth;
            height = boardHeight;

            board = new SpotType[height, width];

            for (int y = 0; y < height; y++)
            {

                for (int x = 0; x < width; x++)
                {

                    board[y, x] = SpotType.EMPTY;

                }

            }

        }

        public void RunGame()
        {

            bool blockOut = false;

            bool newRound = true;

            while (blockOut == false)
            {

                RemoveDisplayedBlocks();

                if (newRound == true)
                {

                    activeBlock = SpawnNewBlock(BlockType.L);

                    newRound = false;

                }

                foreach (Vector vec in activeBlock.segmentLocations)
                {

                    SetSpotState(vec.y, vec.x, SpotType.DISPLAYED);

                }

                DisplayBoard();

                ConsoleKey key = GetInput();

                switch (key)
                {

                    case ConsoleKey.LeftArrow:

                        attemptMoveBlock(key);

                        break;

                    case ConsoleKey.RightArrow:

                        attemptMoveBlock(key);

                        break;

                    case ConsoleKey.DownArrow:

                        //slam

                        break;

                    case ConsoleKey.UpArrow:

                        //swap pieces

                        break;

                    case ConsoleKey.Spacebar:

                        //rotate piece

                        break;

                }

            }

        }

        public Block SpawnNewBlock(BlockType blockType)
        {

            return new Block(width / 2 - 1, 1, blockType);

        }

        public BlockType GetRandomBlock()
        {

            return (BlockType)Enum.GetValues(typeof(BlockType)).GetValue(random.Next(Enum.GetValues(typeof(BlockType)).Length));

        }

        public bool attemptMoveBlock(ConsoleKey key)
        {

            Vector[] newVecs = new Vector[4];

            switch (key)
            {

                case ConsoleKey.LeftArrow:

                    

                    for (int i = 0; i < 4; i++)
                    {

                        newVecs[i].x = activeBlock.segmentLocations[i].x - 1;
                        newVecs[i].y = activeBlock.segmentLocations[i].y;

                        if (newVecs[i].x < 0) return false;

                    }

                    break;

                case ConsoleKey.RightArrow:

                    for (int i = 0; i < 4; i++)
                    {

                        newVecs[i].x = activeBlock.segmentLocations[i].x + 1;
                        newVecs[i].y = activeBlock.segmentLocations[i].y;

                        if (newVecs[i].x > width - 1) return false;

                    }

                    break;

            }

            activeBlock.segmentLocations = newVecs;

            return true;

        }

        public void DisplayBoard()
        {   

            string boardDisplay = "";

            string border = "";

            for (int i = 0; i < width + 2; i++)
            {

                border += "██";

            }

            boardDisplay += border + "\n";

            for (int y = 0; y < height; y++)
            {

                boardDisplay += "██";

                for (int x = 0; x < width; x++)
                {

                    switch (board[y, x])
                    {

                        case SpotType.EMPTY:

                            boardDisplay += "  ";
                            break;

                        case SpotType.SET:

                            boardDisplay += "██";
                            break;

                        case SpotType.DISPLAYED:

                            boardDisplay += "██";
                            break;

                        default:

                            boardDisplay += "  ";
                            break;

                    }

                }

                boardDisplay += "██\n";

            }

            border = "";

            for (int i = 0; i < width + 2; i++)
            {

                border += "██";

            }

            boardDisplay += border + "\n";

            Console.Clear();

            Console.WriteLine(boardDisplay);

        }

        public void SetSpotState(int row, int column, SpotType type)
        {

            board[row, column] = type;

        }

        public SpotType GetSpotState(int row, int column)
        {

            return board[row, column];

        }

        public void RemoveDisplayedBlocks()
        {

            for (int y = 0; y < height; y++)
            {

                for (int x = 0; x < width; x++)
                {

                    if (GetSpotState(y, x) == SpotType.DISPLAYED) SetSpotState(y, x, SpotType.EMPTY);

                }

            }

        }

        public ConsoleKey GetInput()
        {

            return Console.ReadKey().Key;

        }
    }

    public class Block
    {

        public Vector[] segmentLocations;

        public BlockType blockType;

        public Block(int originX, int originY, BlockType blockType)
        {

            segmentLocations = new Vector[4];

            this.blockType = blockType;

            switch (blockType)
            {

                case BlockType.L:

                    segmentLocations[0] = new Vector { x = -1 , y = 1 };
                    segmentLocations[1] = new Vector { x = -1, y = 0 };
                    segmentLocations[2] = new Vector { x = 0, y = 0 };
                    segmentLocations[3] = new Vector { x = 1, y = 0 };

                    break;

                case BlockType.O:

                    segmentLocations[0] = new Vector { x = 0, y = 0 };
                    segmentLocations[1] = new Vector { x = 1, y = 0 };
                    segmentLocations[2] = new Vector { x = 0, y = 1 };
                    segmentLocations[3] = new Vector { x = 1, y = 1 };

                    break;
            }

            for (int i = 0; i < 4; i++)
            {

                segmentLocations[i].x += originX;
                segmentLocations[i].y += originY;

            }

        }

    }

    public enum BlockType
    {

        L,
        L_BACKWARDS,
        S,
        Z,
        I,
        O,
        T,

    }

    public enum SpotType
    {

        SET,
        DISPLAYED,
        EMPTY,

    }

    public struct Vector
    {

        public int x;
        public int y;

    }

}
