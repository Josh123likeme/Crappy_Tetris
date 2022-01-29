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

        private BlockType[] pendingBlocks = new BlockType[3];

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

            inactiveBlock = BlockType.NULL;

            for (int i = 0; i < pendingBlocks.Length; i++)
            {

                pendingBlocks[i] = GetRandomBlockType();

            }

            bool newRound = true;

            bool hasSwapped = false;

            while (!blockOut)
            {

                RemoveDisplayedBlocks();

                if (newRound)
                {

                    hasSwapped = false;

                    InitiateNewRound();

                    newRound = false;

                }

                foreach (Vector vec in activeBlock.segmentLocations)
                {

                    SetSpotState(vec.y, vec.x, SpotType.DISPLAYED);

                }

                DisplayBoard();       

                ButtonType key = GetInput();

                switch (key)
                {

                    case ButtonType.LEFT:

                        //move left

                        AttemptMoveBlock(key);

                        break;

                    case ButtonType.RIGHT:

                        //move right

                        AttemptMoveBlock(key);

                        break;

                    case ButtonType.SLAM:

                        //slam

                        newRound = true; //TODO remove

                        break;

                    case ButtonType.SWAP:

                        //swap pieces

                        if (hasSwapped) break;

                        bool actualSwap = SwapBlock();

                        if (!actualSwap)
                        {

                            InitiateNewRound();

                        }

                        hasSwapped = true;

                        break;

                    case ButtonType.ROTATE:

                        //rotate block

                        AttemptRotateBlock();

                        break;

                }

            }

        }

        public void InitiateNewRound()
        {

            activeBlock = SpawnNewBlock(pendingBlocks[0]);

            for (int i = 0; i < pendingBlocks.Length - 1; i++)
            {

                pendingBlocks[i] = pendingBlocks[i + 1];

            }

            pendingBlocks[pendingBlocks.Length - 1] = GetRandomBlockType();

        }

        public Block SpawnNewBlock(BlockType blockType)
        {

            return new Block(width / 2 - 1, 2, blockType);

        }

        public BlockType GetRandomBlockType()
        {

            BlockType blockType;

            do
            {

                blockType = (BlockType)Enum.GetValues(typeof(BlockType)).GetValue(random.Next(Enum.GetValues(typeof(BlockType)).Length));

            }
            while (blockType == BlockType.NULL);

            return blockType;

        }

        public bool IsWithinBounds(Vector vec)
        {

            if (vec.x < 0) return false;
            if (vec.x > width - 1) return false;

            if (vec.y < 0) return false;

            return true;

        }

        public bool AttemptMoveBlock(ButtonType key)
        {

            Vector[] newVecs = new Vector[4];

            Vector newOrigin = activeBlock.originLocation;

            switch (key)
            {

                case ButtonType.LEFT:

                    newOrigin.x += -1;

                    for (int i = 0; i < 4; i++)
                    {

                        newVecs[i].x = activeBlock.segmentLocations[i].x - 1;
                        newVecs[i].y = activeBlock.segmentLocations[i].y;

                        if (!IsWithinBounds(newVecs[i])) return false;

                    }

                    break;

                case ButtonType.RIGHT:

                    newOrigin.x += 1;

                    for (int i = 0; i < 4; i++)
                    {

                        newVecs[i].x = activeBlock.segmentLocations[i].x + 1;
                        newVecs[i].y = activeBlock.segmentLocations[i].y;

                        if (!IsWithinBounds(newVecs[i])) return false;

                    }

                    break;

            }

            activeBlock.originLocation = newOrigin;

            activeBlock.segmentLocations = newVecs;

            return true;

        }

        public bool AttemptRotateBlock()
        {

            if (!activeBlock.blockType.IsRotatable()) return false;

            Vector[] newVecs = new Vector[4];

            for (int i = 0; i < 4; i++)
            {

                newVecs[i].x = activeBlock.segmentLocations[i].x;
                newVecs[i].y = activeBlock.segmentLocations[i].y;

            }

            for (int i = 0; i < 4; i++)
            {

                newVecs[i].x -= activeBlock.originLocation.x;
                newVecs[i].y -= activeBlock.originLocation.y;

                int x = newVecs[i].x;
                int y = newVecs[i].y;

                newVecs[i].x = y;
                newVecs[i].y = -x;

                newVecs[i].x += activeBlock.originLocation.x;
                newVecs[i].y += activeBlock.originLocation.y;

                if (!IsWithinBounds(newVecs[i])) return false;

            }

            activeBlock.segmentLocations = newVecs;

            return true;

        }

        public bool SwapBlock()
        {

            if (inactiveBlock == BlockType.NULL)
            {

                inactiveBlock = activeBlock.blockType;

                return false;

            }

            else
            {

                BlockType oldActive = activeBlock.blockType;

                activeBlock = SpawnNewBlock(inactiveBlock);

                inactiveBlock = oldActive;

                return true;

            }

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

            border += "\n";

            for (int i = 0; i < width + 2; i++)
            {

                border += "██";

            }

            boardDisplay += border + "\n";

            string pendingDisplay = "██   " + ((inactiveBlock != BlockType.NULL) ? Enum.GetName(typeof(BlockType), inactiveBlock) + "   ██  " : "    ██  ");

            foreach (BlockType blockType in pendingBlocks)
            {

                pendingDisplay += Enum.GetName(typeof(BlockType), blockType) + "  ";

            }

            pendingDisplay += "██\n";

            for (int i = 0; i < 12; i++)
            {

                pendingDisplay += "██";

            }

            boardDisplay += pendingDisplay;

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

        public ButtonType GetInput()
        {

            try
            {

                return keybinds[Console.ReadKey().Key];

            }
            catch (KeyNotFoundException)
            {

                return ButtonType.NULL;

            }

        }

        public enum ButtonType
        {
            NULL,
            LEFT,
            RIGHT,
            SLAM,
            SWAP,
            ROTATE,

        }

        public static Dictionary<ConsoleKey, ButtonType> keybinds = new Dictionary<ConsoleKey, ButtonType>
        {
            {ConsoleKey.LeftArrow, ButtonType.LEFT },
            {ConsoleKey.RightArrow, ButtonType.RIGHT },
            {ConsoleKey.DownArrow, ButtonType.SLAM },
            {ConsoleKey.UpArrow, ButtonType.SWAP },
            {ConsoleKey.Spacebar, ButtonType.ROTATE },

        };

    }

    public class Block
    {

        public Vector[] segmentLocations;

        public Vector originLocation;

        public BlockType blockType;

        public Block(int originX, int originY, BlockType blockType)
        {

            segmentLocations = new Vector[4];

            this.blockType = blockType;

            this.originLocation = new Vector { x = 0, y = 0 };

            switch (blockType)
            {

                case BlockType.L:

                    segmentLocations[0] = new Vector { x = -1, y = 0 };
                    segmentLocations[1] = new Vector { x = -1, y = -1 };
                    segmentLocations[2] = new Vector { x = 0, y = -1 };
                    segmentLocations[3] = new Vector { x = 1, y = -1 };

                    break;

                case BlockType.J:

                    segmentLocations[0] = new Vector { x = 1, y = 0 };
                    segmentLocations[1] = new Vector { x = 1, y = -1 };
                    segmentLocations[2] = new Vector { x = 0, y = -1 };
                    segmentLocations[3] = new Vector { x = -1, y = -1 };

                    break;

                case BlockType.S:

                    segmentLocations[0] = new Vector { x = -1, y = 1 };
                    segmentLocations[1] = new Vector { x = 0, y = 1 };
                    segmentLocations[2] = new Vector { x = 0, y = 0 };
                    segmentLocations[3] = new Vector { x = 1, y = 0 };

                    break;

                case BlockType.Z:

                    segmentLocations[0] = new Vector { x = 1, y = 1 };
                    segmentLocations[1] = new Vector { x = 0, y = 1 };
                    segmentLocations[2] = new Vector { x = 0, y = 0 };
                    segmentLocations[3] = new Vector { x = -1, y = 0 };

                    break;

                case BlockType.I:

                    segmentLocations[0] = new Vector { x = -1, y = 0 };
                    segmentLocations[1] = new Vector { x = 0, y = 0 };
                    segmentLocations[2] = new Vector { x = 1, y = 0 };
                    segmentLocations[3] = new Vector { x = 2, y = 0 };

                    break;

                case BlockType.O:

                    segmentLocations[0] = new Vector { x = 0, y = 0 };
                    segmentLocations[1] = new Vector { x = 1, y = 0 };
                    segmentLocations[2] = new Vector { x = 0, y = 1 };
                    segmentLocations[3] = new Vector { x = 1, y = 1 };

                    break;

                case BlockType.T:

                    segmentLocations[0] = new Vector { x = 0, y = 1 };
                    segmentLocations[1] = new Vector { x = -1, y = 0 };
                    segmentLocations[2] = new Vector { x = 0, y = 0 };
                    segmentLocations[3] = new Vector { x = 1, y = 0 };

                    break;

            }

            for (int i = 0; i < 4; i++)
            {

                segmentLocations[i].x += originX;
                segmentLocations[i].y += originY;

                

            }

            originLocation.x += originX;
            originLocation.y += originY;

        }

    }

    public enum BlockType
    {
        NULL,
        L,
        J,
        S,
        Z,
        I,
        O,
        T,

    }

    public static class EnumExtensions
    {

        public static bool IsRotatable(this BlockType blockType)
        {

            if (blockType == BlockType.O) return false;

            return true;

        }

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
