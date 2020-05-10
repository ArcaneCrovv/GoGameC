using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using WindowsFormsApp2.Domains;
using NUnit.Framework;
using FluentAssertions;
using NUnit.Framework.Internal;

namespace GoTests
{
    [TestFixture]
    public class Tests
    {
        private GameBoard board;
        private Point _testPointOutBounds;
        private Point _testPointZeroZero;
        private Point _testPointMiddleOfBoard;
        private Point _testPointRightLower;
        private Point _testPointUpMiddle;
        private Point _testPointOutSize;

        [SetUp]
        public void SetUp()
        {
            board = new GameBoard(4);
            _testPointOutBounds = new Point(-1, -1);
            _testPointZeroZero = new Point(0, 0);
            _testPointUpMiddle = new Point(0, 1);
            _testPointMiddleOfBoard = new Point(1, 1);
            _testPointRightLower = new Point(3, 3);
            _testPointOutSize = new Point(4, 4);
        }

        [Test]
        public void Test_ForceChangeColor()
        {
            board.ChangeColorOn(_testPointZeroZero, BoardColor.Black);
            board.StoneColorOn(_testPointZeroZero).Should().Be(BoardColor.Black);
        }
        
        [Test]
        public void Test_PointOutIfBounds()
        {
            board.IsPointInBordBounds(_testPointOutSize).Should().Be(false);
        }

        [Test]
        public void Test_PointOutOfSize()
        {
            board.IsPointInBordBounds(_testPointOutSize).Should().Be(false);
        }

        [Test]
        public void Test_PointZeroZer()
        {
            board.IsPointInBordBounds(_testPointZeroZero).Should().Be(true);
        }

        [Test]
        public void Test_PointMiddleTop()
        {
            board.IsPointInBordBounds(_testPointUpMiddle).Should().Be(true);
        }
        
        [Test]
        public void Test_PointInBoardMiddle()
        {
            board.IsPointInBordBounds(_testPointMiddleOfBoard).Should().Be(true);
        }
        
        [Test]
        public void Test_NearPoints_OutBounds()
        {
            board.NearPoints(_testPointOutBounds).Count().Should().Be(0);
        }

        [Test]
        public void Test_NearPoints_Angle()
        {
            board.NearPoints(_testPointZeroZero).Count().Should().Be(2);
        }

        [Test]
        public void Test_NearPoints_OnSide()
        {
            board.NearPoints(_testPointUpMiddle).Count().Should().Be(3);
        }

        [Test]
        public void Test_NearPoints_BoardMiddle()
        {
            board.NearPoints(_testPointMiddleOfBoard).Count().Should().Be(4);
        }

        [Test]
        public void Test_StoneGroupWithPoint_NoGroups()
        {
            board.StoneGroupWithPoint(_testPointZeroZero, BoardColor.Black)
                .ToList().Should().Equal(_testPointZeroZero);
        }
        
        [Test]
        public void Test_StoneGroupWithPoint_LittleGroup()
        {
            board.Board[0, 1] = BoardColor.Black;
            board.Board[0, 2] = BoardColor.Black;
            board.StoneGroupWithPoint(_testPointZeroZero, BoardColor.Black)
                .ToList().Should().Equal(_testPointZeroZero, new Point(0, 1), new Point(0, 2));
        }

        [Test]
        public void Test_StoneGroupWithPoint_Cross()
        {
            board.Board[0, 1] = BoardColor.Black;
            board.Board[1, 0] = BoardColor.Black;
            board.Board[1, 2] = BoardColor.Black;
            board.Board[2, 1] = BoardColor.Black;
            board.StoneGroupWithPoint(_testPointMiddleOfBoard, BoardColor.Black)
                .Count().Should().Be(5);
        }

        [Test]
        public void Test_IsGroupAlive_CheckOfLiberty()
        {
            Action test = () => board.IsGroupAlive(_testPointZeroZero);
            test.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Test_IsGroupAlive_AngleStone()
        {
            board.Board[0, 0] = BoardColor.Black;
            board.IsGroupAlive(_testPointZeroZero).Should().BeTrue();
        }

        [Test]
        public void Test_IsGroupAlive_BlockedAngleStone()
        {
            board.Board[0, 0] = BoardColor.Black;
            board.Board[0, 1] = BoardColor.White;
            board.Board[1, 0] = BoardColor.White;
            board.IsGroupAlive(_testPointZeroZero).Should().BeFalse();
        }

        [Test]
        public void Test_IsGroupAlive_MiddleBoardStone()
        {
            board.ChangeColorOn(_testPointMiddleOfBoard, BoardColor.Black);
            board.IsGroupAlive(_testPointMiddleOfBoard).Should().BeTrue();
        }

        [Test]
        public void Test_isGroupAlive_BlockedMiddleBoardStone()
        {
            board.ChangeColorOn(_testPointUpMiddle, BoardColor.Black);
            foreach (var point in board.NearPoints(_testPointUpMiddle))
                board.ChangeColorOn(point, BoardColor.White);

            board.IsGroupAlive(_testPointUpMiddle).Should().BeFalse();
        }

        [Test]
        public void Test_isGroupAlive_BlockedLongGroup()
        {
            board.ChangeColorOn(0, 1, BoardColor.Black);
            board.ChangeColorOn(_testPointMiddleOfBoard, BoardColor.Black);
            board.ChangeColorOn(2, 1, BoardColor.Black);
            for (var i = 0; i < 4; i++)
            {
                board.ChangeColorOn(i, 0, BoardColor.White);
                board.ChangeColorOn(i, 2, BoardColor.White);
            }
            board.ChangeColorOn(3, 1, BoardColor.White);
            board.IsGroupAlive(_testPointMiddleOfBoard).Should().BeFalse();
        }
        
        [Test]
        public void Test_isGroupAlive_LongGroup()
        {
            board.ChangeColorOn(0, 1, BoardColor.Black);
            board.ChangeColorOn(_testPointMiddleOfBoard, BoardColor.Black);
            board.ChangeColorOn(2, 1, BoardColor.Black);
            for (var i = 0; i < 4; i++)
            {
                board.ChangeColorOn(i, 0, BoardColor.White);
                board.ChangeColorOn(i, 2, BoardColor.White);
            }
            
            board.IsGroupAlive(_testPointMiddleOfBoard).Should().BeTrue();
        }

        [Test]
        public void Test_KillGroupReturnScore_OneStone()
        {
            board.ChangeColorOn(_testPointZeroZero, BoardColor.Black);
            board.KillGroupReturnScore(_testPointZeroZero).Should().Be(1);
            board.StoneColorOn(_testPointZeroZero).Should().Be(BoardColor.Liberty);
        }

        [Test]
        public void Test_KillGroupReturnScore_AngleGroup()
        {
            board.ChangeColorOn(_testPointZeroZero, BoardColor.Black);
            board.ChangeColorOn(0, 1, BoardColor.Black);
            board.ChangeColorOn(1, 1, BoardColor.Black);
            board.KillGroupReturnScore(_testPointZeroZero).Should().Be(3);
            board.StoneColorOn(_testPointZeroZero).Should().Be(BoardColor.Liberty);
            board.StoneColorOn(0, 1).Should().Be(BoardColor.Liberty);
            board.StoneColorOn(1, 1).Should().Be(BoardColor.Liberty);
        }

        [Test]
        public void Test_KillGroupReturnScore_AllField()
        {
            for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    board.ChangeColorOn(i, j, BoardColor.Black);

            board.StoneGroupWithPoint(_testPointZeroZero).Count().Should().Be(16);
            board.KillGroupReturnScore(_testPointZeroZero).Should().Be(16);
            
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                board.StoneColorOn(i, j).Should().Be(BoardColor.Liberty);
        }

        [Test]
        public void Test_EnemyStonesNear_OneStoneNear()
        {
            board.ChangeColorOn(_testPointMiddleOfBoard, BoardColor.Black);
            board.ChangeColorOn(0, 1, BoardColor.White);
            board.EnemyStonesNear(_testPointMiddleOfBoard)
                .ToList().Should().Equal(new Point(0, 1));
        }

        [Test]
        public void Test_EnemyStonesNear_TwoStones()
        {
            board.ChangeColorOn(_testPointMiddleOfBoard, BoardColor.Black);
            board.ChangeColorOn(0, 1, BoardColor.White);
            board.ChangeColorOn(1, 0, BoardColor.White);
            board.EnemyStonesNear(_testPointMiddleOfBoard)
                .ToList().Should().Equal(new Point(1, 0), new Point(0, 1));
        }

        [Test]
        public void Test_EnemyStonesNear_FourPoints()
        {
            board.ChangeColorOn(_testPointMiddleOfBoard, BoardColor.Black);
            foreach (var point in board.NearPoints(_testPointMiddleOfBoard))
            {
                board.ChangeColorOn(point, BoardColor.White);
            }

            board.EnemyStonesNear(_testPointMiddleOfBoard)
                .ToList().Should().BeEquivalentTo(board.NearPoints(_testPointMiddleOfBoard).ToList());
        }

        [Test]
        public void Test_TryKillAround_AliveGroups()
        {
            board.ChangeColorOn(_testPointZeroZero, BoardColor.Black);
            board.ChangeColorOn(1, 1, BoardColor.Black);
            board.ChangeColorOn(0, 1, BoardColor.White);
            board.ChangeColorOn(1, 0, BoardColor.White);
            board.TryKillAround(_testPointMiddleOfBoard).Should().Be(0);
            board.StoneColorOn(0, 1).Should().Be(BoardColor.White);
            board.StoneColorOn(1, 0).Should().Be(BoardColor.White);
        }
        
        [Test]
        public void Test_TryKillAround_DeadGroups()
        {
            board.ChangeColorOn(_testPointZeroZero, BoardColor.Black);
            board.ChangeColorOn(1, 1, BoardColor.Black);
            board.ChangeColorOn(0, 1, BoardColor.White);
            board.ChangeColorOn(0, 2, BoardColor.Black);
            board.ChangeColorOn(1, 0, BoardColor.White);
            board.ChangeColorOn(2, 0, BoardColor.Black);
            board.TryKillAround(_testPointMiddleOfBoard).Should().Be(2);
            board.StoneColorOn(0, 1).Should().Be(BoardColor.Liberty);
            board.StoneColorOn(1, 0).Should().Be(BoardColor.Liberty);
        }
        
        [Test]
        public void Test_TryKillAround_DeadField()
        {
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                board.ChangeColorOn(i, j, BoardColor.Black);
            
            board.ChangeColorOn(3, 3, BoardColor.White);

            board.TryKillAround(_testPointRightLower).Should().Be(15);
            board.ChangeColorOn(3, 3, BoardColor.Liberty);
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                board.StoneColorOn(i, j).Should().Be(BoardColor.Liberty);
        }
        
        [Test]
        public void Test_TryKillAround_FullBlackField()
        {
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                board.ChangeColorOn(i, j, BoardColor.Black);

            board.TryKillAround(_testPointRightLower).Should().Be(0);
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                board.StoneColorOn(i, j).Should().Be(BoardColor.Black);
        }

        [Test]
        public void Test_SetStoneNullIfCant_AloneAngleStone()
        {
            board.SetStoneNullIfCant(_testPointZeroZero, BoardColor.Black).Should().Be(0);
        }
        
        [Test]
        public void Test_SetStoneNullIfCant_AloneMiddleStone()
        {
            board.SetStoneNullIfCant(_testPointMiddleOfBoard, BoardColor.Black).Should().Be(0);
        }
        
        [Test]
        public void Test_SetStoneNullIfCant_AloneMiddleSideStone()
        {
            board.SetStoneNullIfCant(_testPointUpMiddle, BoardColor.Black).Should().Be(0);
        }

        [Test]
        public void Test_SetStoneNullIfCant_KillStone()
        {
            board.ChangeColorOn(_testPointZeroZero, BoardColor.Black);
            board.ChangeColorOn(0, 1, BoardColor.White);
            board.SetStoneNullIfCant(new Point(1, 0), BoardColor.White).Should().Be(1);
            board.StoneColorOn(_testPointZeroZero).Should().Be(BoardColor.Liberty);
        }

        [Test]
        public void Test_SetStoneNullIfCant_KillAllField()
        {
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                board.ChangeColorOn(i, j, BoardColor.Black);
            board.ChangeColorOn(3, 3, BoardColor.Liberty);

            board.SetStoneNullIfCant(new Point(3, 3), BoardColor.Black).Should().BeNull();
            board.StoneColorOn(3, 3).Should().Be(BoardColor.Liberty);
            board.SetStoneNullIfCant(new Point(3, 3), BoardColor.White).Should().Be(15);
            
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                if (i != 3 || j !=3)
                    board.StoneColorOn(i, j).Should().Be(BoardColor.Liberty);

            board.StoneColorOn(3, 3).Should().Be(BoardColor.White);
        }

        [Test]
        public void Test_TerritoryOwnerAndScore_AllField()
        {
            var result = board.TerritoryOwnerAndScore(_testPointZeroZero);
            result.Item1.Should().Be(BoardColor.Liberty);
            result.Item2.Count().Should().Be(16);
        }

        [Test]
        public void Test_TerritoryOwnerAndScore_OneScoreOneOwner()
        {
            board.ChangeColorOn(0, 1, BoardColor.Black);
            board.ChangeColorOn(1, 0, BoardColor.Black);
            var result = board.TerritoryOwnerAndScore(_testPointZeroZero);
            result.Item1.Should().Be(BoardColor.Black);
            result.Item2.Count.Should().Be(1);
        }

        [Test]
        public void Test_TerritoryOwnerAndScore_OneScoreNoOwner()
        {
            board.ChangeColorOn(0, 1, BoardColor.Black);
            board.ChangeColorOn(1, 0, BoardColor.White);
            var result = board.TerritoryOwnerAndScore(_testPointZeroZero);
            result.Item1.Should().Be(BoardColor.Liberty);
            result.Item2.Count.Should().Be(1);
        }
        
        [Test]
        public void Test_TerritoryOwnerAndScore_ManyScoreNoOwner()
        {
            board.ChangeColorOn(0, 1, BoardColor.Black);
            board.ChangeColorOn(1, 0, BoardColor.White);
            var result = board.TerritoryOwnerAndScore(_testPointMiddleOfBoard);
            result.Item1.Should().Be(BoardColor.Liberty);
            result.Item2.Count.Should().Be(13);
        }

        [Test]
        public void Test_CountTerritories_AllFieldNoOwner()
        {
            var result = board.CountTerritories();
            result[BoardColor.Black].Should().Be(0);
            result[BoardColor.White].Should().Be(0);
        }

        [Test]
        public void Test_CountTerritories_AllFieldToOnePlayer()
        {
            board.ChangeColorOn(1, 1, BoardColor.Black);
            var result = board.CountTerritories();
            result[BoardColor.Black].Should().Be(15);
            result[BoardColor.White].Should().Be(0);
        }

        [Test]
        public void Test_CountTerritories_AngleWhiteBigBlack()
        {
            board.ChangeColorOn(0, 1, BoardColor.White);
            board.ChangeColorOn(1, 0, BoardColor.White);
            board.ChangeColorOn(2, 0, BoardColor.Black);
            board.ChangeColorOn(1, 1, BoardColor.Black);
            board.ChangeColorOn(0, 2, BoardColor.Black);
            var result = board.CountTerritories();
            result[BoardColor.Black].Should().Be(10);
            result[BoardColor.White].Should().Be(1);
        }
    }
}