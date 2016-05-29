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
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Start();
            timer.Tick += _timer_Tick;
            MessageBox.Show("Click to play Game");
        }

        static double initialSpeed = 5;
        static double initialPadSpeed = 10;
        static double acceleration = 1.1;
        static double padAcceleration = 1.02;

        private double _angle = 155;
        private double _speed = initialSpeed;
        private double _padSpeed = initialPadSpeed;
        void _timer_Tick(object sender, EventArgs e)

        {

            GameEnd();

            if ((Keyboard.IsKeyDown(Key.W)) && (_leftPad.YPosition >= 0))
                _leftPad.YPosition -= (int)_padSpeed;
            if ((Keyboard.IsKeyDown(Key.S)) && (_leftPad.YPosition < MainCanvas.ActualHeight - LeftPad.ActualHeight))
                _leftPad.YPosition += (int)_padSpeed;
            if ((Keyboard.IsKeyDown(Key.Up)) && (_rightPad.YPosition >= 0))
                _rightPad.YPosition -= (int)_padSpeed;
            if ((Keyboard.IsKeyDown(Key.Down)) && (_rightPad.YPosition < MainCanvas.ActualHeight - RightPad.ActualHeight))
                _rightPad.YPosition += (int)_padSpeed;


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
                GameReset();
            }
            if (_ball.X <= 0)
            {
                _ball.RightResult += 1;
                GameReset();
            }
        }
        private void GameReset()
        {
            _ball.Y = 210;
            _ball.X = 380;
            _speed = initialSpeed;
            _padSpeed = initialPadSpeed;
        }

        private void GameEnd()
        {
            if (_ball.LeftResult == 11 || _ball.RightResult == 11)
            {
                Scores();     

                if (_ball.LeftResult > _ball.RightResult)
                {
                    MessageBox.Show("Player 1 Won");
                    this.Close();
                }
                if (_ball.LeftResult < _ball.RightResult)
                {
                    MessageBox.Show("Player 2 Won");
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
            _ball.MovingRight = !_ball.MovingRight;
            _speed *= acceleration;
            _padSpeed *= padAcceleration;
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


        readonly Ball _ball = new Ball { X = 400, Y = 200, MovingRight = true };

        readonly Pad _leftPad = new Pad { YPosition = 200 };
        readonly Pad _rightPad = new Pad { YPosition = 200 };


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
           // switch(e.Key)
           // {
           //     case Key.W: _leftPad.MoveUp(_padSpeed); break;
           //     case Key.S: _leftPad.MoveDown(_padSpeed); break;

           //     case Key.Up: _rightPad.MoveUp(_padSpeed); break;
           //     case Key.Down: _rightPad.MoveDown(_padSpeed); break;
           // }
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
