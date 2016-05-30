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
            windowHeight = MainCanvas.ActualHeight;
            windowWidth = MainCanvas.ActualWidth;
            
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Start();
            timer.Tick += _timer_Tick;
            MessageBox.Show("Click to play Game");
        }
        
        static int padWidth = 20;
        static int ballSize = 20;
        static double initialSpeed = 5;
        static double initialPadSpeed = 10;
        static double acceleration = 1.1;
        static double padAcceleration = 1.02;
        static double windowWidth;
        static double windowHeight;

        private double _angle = 155;
        private double _speed = initialSpeed;
        private double _padSpeed = initialPadSpeed;
        void _timer_Tick(object sender, EventArgs e)

        {

            GameEnd();

            //Irányítás W S FEL LE
            if ((Keyboard.IsKeyDown(Key.W)) && (_leftPad.YPosition >= 0))
                _leftPad.YPosition -= (int)_padSpeed;
            if ((Keyboard.IsKeyDown(Key.S)) && (_leftPad.YPosition < MainCanvas.ActualHeight - LeftPad.ActualHeight))
                _leftPad.YPosition += (int)_padSpeed;
            if ((Keyboard.IsKeyDown(Key.Up)) && (_rightPad.YPosition >= 0))
                _rightPad.YPosition -= (int)_padSpeed;
            if ((Keyboard.IsKeyDown(Key.Down)) && (_rightPad.YPosition < MainCanvas.ActualHeight - RightPad.ActualHeight))
                _rightPad.YPosition += (int)_padSpeed;

            //Labda pattanása a felső és az alsó falon
            if (_ball.Y <= 0) _angle = _angle + (180 - 2 * _angle);
            if (_ball.Y >= MainCanvas.ActualHeight - Ball.Width) _angle = _angle + (180 - 2 * _angle);

            if (CheckCollision() == true) //ha ütközik a pad a labdával
            {
                ChangeAngle();
                ChangeDirection();
            }

            double radians = (Math.PI / 180) * _angle;
            Vector vector = new Vector { X = Math.Sin(radians), Y = -Math.Cos(radians) };
            _ball.X += vector.X * _speed;
            _ball.Y += vector.Y * _speed;

            //új kör, ha a labda kimegy a pályáról + pontszerzés
            if (_ball.X >= MainCanvas.ActualWidth)
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
        private void GameReset() //új kör
        {
            _ball.Y = MainCanvas.ActualHeight / 2;
            _ball.X = MainCanvas.ActualWidth / 2;
            _speed = initialSpeed;
            _padSpeed = initialPadSpeed;
        }

        private void GameEnd() //játék vége
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

        private void ChangeAngle() //szögváltás - Visszapattanási szög attól függően, hogy hol éri a padot a labda
        {
            if (_ball.MovingRight == true) _angle = 270 - ((_ball.Y + ballSize) - (_rightPad.YPosition + RightPad.ActualHeight / 2));
            else if (_ball.MovingRight == false) _angle = 90 + ((_ball.Y + ballSize) - (_leftPad.YPosition + LeftPad.ActualHeight / 2));
        }

        private void ChangeDirection() //irányváltás - amikor jobb vagy baloldalt pattan a labda a padon
        {
            _ball.MovingRight = !_ball.MovingRight;
            _speed *= acceleration; //növeli a labda sebességét
            _padSpeed *= padAcceleration; //növeli a pad mozgatásának sebességét
        }

        private bool CheckCollision() //ütközik -e a labda a paddal?
        {
            bool collisionResult = false;
            if (_ball.MovingRight == true)
                collisionResult = _ball.X >= MainCanvas.ActualWidth - padWidth - ballSize && (_ball.Y > _rightPad.YPosition - ballSize  && _ball.Y < _rightPad.YPosition + RightPad.ActualHeight );
            if (_ball.MovingRight == false)
                collisionResult = _ball.X <= padWidth && (_ball.Y > _leftPad.YPosition - ballSize && _ball.Y < _leftPad.YPosition + LeftPad.ActualHeight );
            return collisionResult;
        }
        
        readonly Ball _ball = new Ball { X = windowWidth / 2, Y = windowHeight / 2, MovingRight = true };
        readonly Pad _leftPad = new Pad { YPosition = (int)windowHeight / 2 };
        readonly Pad _rightPad = new Pad { YPosition = (int)windowHeight / 2 };

        //pontok mentése
        private void Scores()
        {
            FileStream fs = new FileStream("Pontokjo.txt", FileMode.Append);
            StreamWriter sr = new StreamWriter(fs);
            sr.WriteLine(" játék eredménye : {0} - {1}", _ball.LeftResult, _ball.RightResult);
            sr.Close();
            fs.Close();
        }

        //labda középről indulásához és a pontok 0-ról indulásához
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            _ball.LeftResult = 0;
            _ball.RightResult = 0;
            GameReset();
        }
    }
}
