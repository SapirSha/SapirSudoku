﻿using SapirSudoku;
using SapirSudoku.src;
using SapirSudoku.src.Exceptions;
using SapirSudoku.src.IO;

namespace SapirSudokuTest
{
    [TestClass]
    public sealed class ValidStringToSudokuConvertionTest
    {
        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValidEmpty1X1StringBoard()
        {
            String stringBoard = "0";
            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {0 }
                }
            );
            

            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValidEmpty4X4StringBoard()
        {
            String stringBoard = "0000000000000000";
            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {0,0,  0,0 },
                    {0,0,  0,0 },

                    {0,0,  0,0 },
                    {0,0,  0,0 }
                }
            );


            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValidEmpty9X9StringBoard()
        {
            String stringBoard = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {0,0,0,  0,0,0,  0,0,0 },
                    {0,0,0,  0,0,0,  0,0,0 },
                    {0,0,0,  0,0,0,  0,0,0 },
                    
                    {0,0,0,  0,0,0,  0,0,0 },
                    {0,0,0,  0,0,0,  0,0,0 },
                    {0,0,0,  0,0,0,  0,0,0 },
                    
                    {0,0,0,  0,0,0,  0,0,0 },
                    {0,0,0,  0,0,0,  0,0,0 },
                    {0,0,0,  0,0,0,  0,0,0 },

                }
            );


            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValidEmpty16X16StringBoard()
        {
            String stringBoard = 
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" +
                "0000000000000000" ;

            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },

                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },

                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },

                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                    {0,0,0,0,  0,0,0,0,  0,0,0,0,  0,0,0,0 },
                }
            );


            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValidEmpty25X25StringBoard()
        {
            String stringBoard =
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000" +
                "0000000000000000000000000";

            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                }
            );


            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValidEmpty6X6StringBoard()
        {
            String stringBoard =
                "000000" +
                "000000" +
                "000000" +
                "000000" +
                "000000" +
                "000000";
            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {0,0,0, 0,0,0 },
                    {0,0,0, 0,0,0 },

                    {0,0,0, 0,0,0 },
                    {0,0,0, 0,0,0 },

                    {0,0,0, 0,0,0 },
                    {0,0,0, 0,0,0 },
                }
            );


            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValidEmpty15X15StringBoard()
        {
            String stringBoard =
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000" +
                "000000000000000";

            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                }
            );


            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValidEmpty20X20StringBoard()
        {
            String stringBoard =
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000" +
                "00000000000000000000";

            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },

                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                    {0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0, 0,0,0,0,0 },
                }
            );


            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValid9X9StringBoard()
        {
            String stringBoard =
                "507000030" +
                "000061000" +
                "108000000" +
                "620040000" +
                "000700080" +
                "000000000" +
                "010000604" +
                "300500000" +
                "000000200";

            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {5  ,0  ,7  ,  0  ,0  ,0  ,  0  ,3  ,0  ,},
                    {0  ,0  ,0  ,  0  ,6  ,1  ,  0  ,0  ,0  ,},
                    {1  ,0  ,8  ,  0  ,0  ,0  ,  0  ,0  ,0  ,},

                    {6  ,2  ,0  ,  0  ,4  ,0  ,  0  ,0  ,0  ,},
                    {0  ,0  ,0  ,  7  ,0  ,0  ,  0  ,8  ,0  ,},
                    {0  ,0  ,0  ,  0  ,0  ,0  ,  0  ,0  ,0  ,},

                    {0  ,1  ,0  ,  0  ,0  ,0  ,  6  ,0  ,4  ,},
                    {3  ,0  ,0  ,  5  ,0  ,0  ,  0  ,0  ,0  ,},
                    {0  ,0  ,0  ,  0  ,0  ,0  ,  2  ,0  ,0  ,},
                }
            );


            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValid16X16StringBoard()
        {
            String stringBoard = 
                "10023400<0600070" +
                "0080007003009:6;" +
                "0<00:0010=0;00>0" +
                "300?200>000900<0" +
                "=000800:0<201?00" +
                "0;76000@000?005=" +
                "000:05?0040800;0" +
                "@0059<0010000080" +
                "0200000=00<58003" +
                "0=00?0300>80@000" +
                "580010002000=9?0" +
                "00<406@0=0070005" +
                "0300<0006004;00@" +
                "0700@050>0010020" +
                ";1?900=002000>00" +
                "0>000;0200=3500<";

            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {1  ,0  ,0  ,2  ,  3  ,4  ,0  ,0  ,  12 ,0  ,6  ,0  ,  0  ,0  ,7  ,0  ,},
                    {0  ,0  ,8  ,0  ,  0  ,0  ,7  ,0  ,  0  ,3  ,0  ,0  ,  9  ,10 ,6  ,11 ,},
                    {0  ,12 ,0  ,0  ,  10 ,0  ,0  ,1  ,  0  ,13 ,0  ,11 ,  0  ,0  ,14 ,0  ,},
                    {3  ,0  ,0  ,15 ,  2  ,0  ,0  ,14 ,  0  ,0  ,0  ,9  ,  0  ,0  ,12 ,0  ,},
                    
                    {13 ,0  ,0  ,0  ,  8  ,0  ,0  ,10 ,  0  ,12 ,2  ,0  ,  1  ,15 ,0  ,0  ,},
                    {0  ,11 ,7  ,6  ,  0  ,0  ,0  ,16 ,  0  ,0  ,0  ,15 ,  0  ,0  ,5  ,13 ,},
                    {0  ,0  ,0  ,10 ,  0  ,5  ,15 ,0  ,  0  ,4  ,0  ,8  ,  0  ,0  ,11 ,0  ,},
                    {16 ,0  ,0  ,5  ,  9  ,12 ,0  ,0  ,  1  ,0  ,0  ,0  ,  0  ,0  ,8  ,0  ,},
                    
                    {0  ,2  ,0  ,0  ,  0  ,0  ,0  ,13 ,  0  ,0  ,12 ,5  ,  8  ,0  ,0  ,3  ,},
                    {0  ,13 ,0  ,0  ,  15 ,0  ,3  ,0  ,  0  ,14 ,8  ,0  ,  16 ,0  ,0  ,0  ,},
                    {5  ,8  ,0  ,0  ,  1  ,0  ,0  ,0  ,  2  ,0  ,0  ,0  ,  13 ,9  ,15 ,0  ,},
                    {0  ,0  ,12 ,4  ,  0  ,6  ,16 ,0  ,  13 ,0  ,0  ,7  ,  0  ,0  ,0  ,5  ,},
                    
                    {0  ,3  ,0  ,0  ,  12 ,0  ,0  ,0  ,  6  ,0  ,0  ,4  ,  11 ,0  ,0  ,16 ,},
                    {0  ,7  ,0  ,0  ,  16 ,0  ,5  ,0  ,  14 ,0  ,0  ,1  ,  0  ,0  ,2  ,0  ,},
                    {11 ,1  ,15 ,9  ,  0  ,0  ,13 ,0  ,  0  ,2  ,0  ,0  ,  0  ,14 ,0  ,0  ,},
                    {0  ,14 ,0  ,0  ,  0  ,11 ,0  ,2  ,  0  ,0  ,13 ,3  ,  5  ,0  ,0  ,12 ,},
                }
            );


            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        public void Should_CreateSudoku_When_RecievedAValid6X6StringBoard()
        {
            String stringBoard = 
                "000000" +
                "000506" +
                "200050" +
                "050002" +
                "603000" +
                "000613";

            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {0,0,0, 0,0,0},
                    {0,0,0, 5,0,6},
                    
                    {2,0,0, 0,5,0},
                    {0,5,0, 0,0,2},
                    
                    {6,0,3, 0,0,0},
                    {0,0,0, 6,1,3},
                }
            );


            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }
    }

}
