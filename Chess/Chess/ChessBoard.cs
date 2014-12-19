//-----------------------------------------------------------------------------------------------------
// <copyright file="BattleshipPanel.cs" company="None">
//      Copyright (c) Andreas Andersson, Henrik Ottehall, Victor Ström Nilsson & Torbjörn Widström 2014
// </copyright>
// <author>Henrik Ottehall</author>
//-----------------------------------------------------------------------------------------------------

namespace Chess
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Class for drawing the playing field and handling events in it.
    /// </summary>
    public class ChessBoard : Panel
    {
        public enum PieceType { None, Pawn, Rook, Knight, Bishop, King, Queen }
        public enum PieceColor { None, Black, White }

        public struct Piece
        {
            public PieceColor color;
            public PieceType type;
        }

        /// <summary>
        /// Constants for the images for the black pieces that is drawn.
        /// </summary>
        private const string IMAGEPATH = @"images\", BLACKPAWNIMAGE = "RedCross.png", BLACKROOKIMAGE = "RedCross.png", BLACKKNIGHTIMAGE = "RedCross.png",
            BLACKBISHOPIMAGE = "RedCross.png", BLACKQUEENIMAGE = "RedCross.png", BLACKKINGIMAGE = "RedCross.png";

        /// <summary>
        /// Constants for the images for the white pieces that is drawn.
        /// </summary>
        private const string WHITEPAWNIMAGE = "RedCross.png", WHITEROOKIMAGE = "RedCross.png", WHITEKNIGHTIMAGE = "RedCross.png",
            WHITEBISHOPIMAGE = "RedCross.png", WHITEQUEENIMAGE = "RedCross.png", WHITEKINGIMAGE = "RedCross.png";


        /// <summary>
        /// Number of rows and columns in the play field and the row and column that a ship is being placed at.
        /// </summary>
        private int rows = 8, columns = 8;

        /// <summary>
        /// The play field.
        /// </summary>
        private Piece[,] boardArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChessBoard"/> class.
        /// Defaults to a 10x10 grid.
        /// </summary>
        public ChessBoard()
        {
            // Force it to redraw the field if the size of the panel changes
            this.ResizeRedraw = true;
            /*
            * Use OptimizedDoubleBuffer to make the panel paint in a buffer before outputting it to screen
            * Use UserPaint to make the panel paint itself instead of having the operating system do it
            * Use AllPaintingInWmPaint to make the control ignore the WM_ERASEBKGND window message
            * This reduces flicker when repainting the panel
            */
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            
            this.boardArray = new Piece[this.rows, this.columns];

            this.SquareHeight = this.ClientSize.Height / this.rows;
            this.SquareWidth = this.ClientSize.Width / this.columns;
            ResetBoard();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChessBoard"/> class.
        /// This constructor that can be used to load a preset playing field
        /// </summary>
        /// <param name="fieldArray">An array of Piecess.</param>
        /// <param name="showingShips">Whether to show ships in the field or not</param>
        public ChessBoard(Piece[,] fieldArray, bool showingShips)
        {
            // Force it to redraw the field if the size of the panel changes
            this.ResizeRedraw = true;
            /*
            * Use OptimizedDoubleBuffer to make the panel paint in a buffer before outputting it to screen
            * Use UserPaint to make the panel paint itself instead of having the operating system do it
            * Use AllPaintingInWmPaint to make the control ignore the WM_ERASEBKGND window message
            * This reduces flicker when repainting the panel
            */
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            this.rows = fieldArray.GetLength(0);
            this.columns = fieldArray.GetLength(1);
            this.boardArray = fieldArray;

            this.SquareHeight = this.ClientSize.Height / this.rows;
            this.SquareWidth = this.ClientSize.Width / this.columns;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChessBoard"/> class.
        /// This constructor can be used to create a new field with a variable number of columns and rows.
        /// </summary>
        /// <param name="numColumns">Number of columns</param>
        /// <param name="numRows">Number of rows</param>
        /// <param name="showingShips">Whether to show ships in the field or not</param>
        public ChessBoard(int numColumns, int numRows, bool showingShips)
        {
            // Force it to redraw the field if the size of the panel changes
            this.ResizeRedraw = true;
            /*
            * Use OptimizedDoubleBuffer to make the panel paint in a buffer before outputting it to screen
            * Use UserPaint to make the panel paint itself instead of having the operating system do it
            * Use AllPaintingInWmPaint to make the control ignore the WM_ERASEBKGND window message
            * This reduces flicker when repainting the panel
            */
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            this.rows = numColumns;
            this.columns = numRows;
            this.boardArray = new Piece[this.rows, this.columns];

            this.SquareHeight = this.ClientSize.Height / this.rows;
            this.SquareWidth = this.ClientSize.Width / this.columns;
        }

        /// <summary>
        /// Used by PlayerHandleShip as a return value.
        /// </summary>
        public enum ShipHandleReturn
        {
            /// <summary>
            /// A ship has been placed
            /// </summary>
            ShipSet,

            /// <summary>
            /// Starting point for a ship has been set
            /// </summary>
            PointSet,

            /// <summary>
            /// A ship has been removed
            /// </summary>
            ShipRemoved,

            /// <summary>
            /// The method failed to set a starting point, place or remove a ship
            /// </summary>
            Failed
        }

        /// <summary>
        /// Gets or sets the SquareHeight variable.
        /// </summary>
        public int SquareHeight { get; set; }

        /// <summary>
        /// Gets or sets the SquareWidth variable.
        /// </summary>
        public int SquareWidth { get; set; }

        private void ResetBoard()
        {
            // Place pawns
            for (int row = 1; row < rows; row += 6)
            {
                for (int column = 0; column < columns; column++)
                {
                    boardArray[row, column] = new Piece { color = (row == 1) ?  PieceColor.Black : PieceColor.White, type = PieceType.Pawn };
                }
            }

            boardArray[0, 0] = new Piece { color = PieceColor.Black, type = PieceType.Rook };
            boardArray[0, 7] = new Piece { color = PieceColor.Black, type = PieceType.Rook };
            boardArray[0, 1] = new Piece { color = PieceColor.Black, type = PieceType.Knight };
            boardArray[0, 6] = new Piece { color = PieceColor.Black, type = PieceType.Knight };
            boardArray[0, 2] = new Piece { color = PieceColor.Black, type = PieceType.Bishop };
            boardArray[0, 5] = new Piece { color = PieceColor.Black, type = PieceType.Bishop };
            boardArray[0, 3] = new Piece { color = PieceColor.Black, type = PieceType.Queen };
            boardArray[0, 4] = new Piece { color = PieceColor.Black, type = PieceType.King };

            boardArray[7, 0] = new Piece { color = PieceColor.White, type = PieceType.Rook };
            boardArray[7, 7] = new Piece { color = PieceColor.White, type = PieceType.Rook };
            boardArray[7, 1] = new Piece { color = PieceColor.White, type = PieceType.Knight };
            boardArray[7, 6] = new Piece { color = PieceColor.White, type = PieceType.Knight };
            boardArray[7, 2] = new Piece { color = PieceColor.White, type = PieceType.Bishop };
            boardArray[7, 5] = new Piece { color = PieceColor.White, type = PieceType.Bishop };
            boardArray[7, 3] = new Piece { color = PieceColor.White, type = PieceType.Queen };
            boardArray[7, 4] = new Piece { color = PieceColor.White, type = PieceType.King };
        }

        /// <summary>
        /// An event that fires when the panel is painted.
        /// </summary>
        /// <param name="paintEvent">A System.Windows.Form.PaintEventArgs that contain the event data.</param>
        protected override void OnPaint(PaintEventArgs paintEvent)
        {
            int rowPlace, columnPlace;
            SolidBrush squareColor;
            
            for (int row = 0; row < this.rows; row++)
            {
                // Calculate where along the rows the image is placed
                rowPlace = row * this.SquareHeight;
                for (int column = 0; column < this.columns; column++)
                {
                    // Calculate where along the columns the image is placed
                    columnPlace = column * this.SquareWidth;
                    
                    if ((column + row) % 2 == 0)
                    {
                        // column number plus row number is an even number, therefore a black square
                        squareColor = new SolidBrush(Color.Black);
                        paintEvent.Graphics.FillRectangle(squareColor, columnPlace, rowPlace, SquareWidth, SquareHeight);
                    }
                    else
                    {
                        // column number plus row number is an odd number, therefore a white square
                        squareColor = new SolidBrush(Color.White);
                        paintEvent.Graphics.FillRectangle(squareColor, columnPlace, rowPlace, SquareWidth, SquareHeight);
                    }

                    // Paint the correct image depending on what kind of Pieces is at the current coordinates
                    if (this.boardArray[row, column].type == PieceType.Pawn)
                    {
                        paintEvent.Graphics.DrawImage(Image.FromFile(IMAGEPATH + BLACKPAWNIMAGE), columnPlace, rowPlace, SquareWidth, SquareHeight);
                    }
                    else if (this.boardArray[row, column].type == PieceType.Rook)
                    {
                        //paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(IMAGEPATH + MISSIMAGE), columnPlace, rowPlace);
                    }
                    else if (this.boardArray[row, column].type == PieceType.Bishop)
                    {
                        //paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(IMAGEPATH + SUNKIMAGE), columnPlace, rowPlace);
                    }
                    else if (this.boardArray[row, column].type == PieceType.Knight)
                    {
                        //paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(IMAGEPATH + RIGHTARROWIMAGE), columnPlace, rowPlace);
                    }
                    else if (this.boardArray[row, column].type == PieceType.King)
                    {
                        //paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(IMAGEPATH + DOWNARROWIMAGE), columnPlace, rowPlace);
                    }
                    else if (this.boardArray[row, column].type == PieceType.Queen)
                    {
                        //paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(IMAGEPATH + LEFTARROWIMAGE), columnPlace, rowPlace);
                    }
                }
            }

            base.OnPaint(paintEvent);
        }

        /// <summary>
        /// An event that fires when the panel is resized.
        /// </summary>
        /// <param name="e">An System.EventArgs that contain the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            this.SquareHeight = this.ClientSize.Height / this.columns;
            this.SquareWidth = this.ClientSize.Width / this.rows;

            base.OnResize(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {

            base.OnMouseClick(e);
        }
    }
}
