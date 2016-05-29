using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;

namespace PongGame
{

    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _ball;
            RightPad.DataContext = _rightPad;
            LeftPad.DataContext = _leftPad;
            Ball.DataContext = _ball;
            

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Start();
            timer.Tick += _timer_Tick;
            MessageBox.Show("Click to play Game");
        }

        private double _angle = 155;
        private double _speed = 5;
        private int _padSpeed = 60;
        void _timer_Tick(object sender, EventArgs e)
        {

            GameEnd();
            if (_ball.Y <= 0) _angle = _angle + (180 - 2 * _angle);
            if (_ball.Y >= MainCanvas.ActualHeight - 20) _angle = _angle + (180 - 2 * _angle);

            if (CheckCollision() == true)
            {
                ChangeAngle();
                ChangeDirection();
            }
            double radians = (Math.PI / 180) * _angle;
            Vector vector = new Vector { X = Math.Sin(radians), Y = -Math.Cos(radians) };
            _ball.X += vector.X * _speed;
            _ball.Y += vector.Y * _speed;

            if (_ball.X >= 800)
            {
                _ball.LeftResult += 1;
                _speed += 0.5;
                GameReset();
            }
            if (_ball.X <= 0)
            {
                _ball.RightResult += 1;
                _speed += 0.5;
                GameReset();
            }
        }
        private void GameReset()
        {
            _ball.Y = 210;
            _ball.X = 380;
        }
        private void GameEnd()
        {
            if (_ball.LeftResult == 11 || _ball.RightResult == 11)
            {
                Scores();     

                if (_ball.LeftResult > _ball.RightResult)
                {
                    MessageBox.Show("Player 1 Win");
                    this.Close();
                }
                if (_ball.LeftResult < _ball.RightResult)
                {
                    MessageBox.Show("Player 2 Win");
                    this.Close();
                }
            }

        }
        private void ChangeAngle()
        {
            if (_ball.MovingRight == true) _angle = 270 - ((_ball.Y + 20) - (_rightPad.YPosition + 60));
            else if (_ball.MovingRight == false) _angle = 90 + ((_ball.Y + 20) - (_leftPad.YPosition + 60));
        }

        private void ChangeDirection()
        {
            if (_ball.MovingRight == true) _ball.MovingRight = false;
            else if (_ball.MovingRight == false) _ball.MovingRight = true;
        }

        private bool CheckCollision()
        {
            bool collisionResult = false;
            if (_ball.MovingRight == true)
                collisionResult = _ball.X >= 760 && (_ball.Y > _rightPad.YPosition - 40 && _ball.Y < _rightPad.YPosition + 120);

            if (_ball.MovingRight == false)
                collisionResult = _ball.X <= 20 && (_ball.Y > _leftPad.YPosition - 40 && _ball.Y < _leftPad.YPosition + 120);

            return collisionResult;
        }


        readonly Ball _ball = new Ball { X = 400, Y = 235, MovingRight = true };

        readonly Pad _leftPad = new Pad { YPosition = 90 };
        readonly Pad _rightPad = new Pad { YPosition = 70 };


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.W: _leftPad.MoveUp(_padSpeed); break;
                case Key.S: _leftPad.MoveDown(_padSpeed); break;

                case Key.Up: _rightPad.MoveUp(_padSpeed); break;
                case Key.Down: _rightPad.MoveDown(_padSpeed); break;
            }
        }
        private void Scores()
        {
            FileStream fs = new FileStream("Pontokjo.txt", FileMode.Append);
            StreamWriter sr = new StreamWriter(fs);
            sr.WriteLine(" játék eredménye : {0} - {1}", _ball.LeftResult, _ball.RightResult);
            sr.Close();
            fs.Close();
        }


    }
}
