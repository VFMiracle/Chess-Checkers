using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChessAndCheckers;

namespace CheckersAndChessGame
{
    /// <summary>
    /// Lógica interna para CheckerGame.xaml
    /// </summary>
    public partial class CheckerGame : Window
    {
        public CheckerGame()
        {
            InitializeComponent();

            KeyDown += KeyPresses;
            Loaded += SetupGame;
            restartButton.Click += RestartGame;
            mainMenuButton.Click += GoToMainMenu;
            quitButton.Click += QuitGame;
        }

        private void SetupGame(object sender, RoutedEventArgs e)
        {
            ChessAndCheckers.Table.SetTabSections(this);
            GameManager.SetTextBlocks(this);
            SelectSquare.SetPlayers(this);
            Piece.SetPieces(this);
        }

        private void KeyPresses(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    SelectSquare.SelectSection();
                    Vector qtds = Piece.FindQtdPieces();
                    if (qtds.X == 0 || qtds.Y == 0)
                    {
                        restartButton.Visibility = Visibility.Visible;
                        mainMenuButton.Visibility = Visibility.Visible;
                        quitButton.Visibility = Visibility.Visible;
                    }
                    break;
                case Key.Tab:
                    SelectSquare.DropPiece();
                    break;
                case Key.E:
                    break;
                default:
                    SelectSquare.Movement(e);
                    GameManager.ClearLabels();
                    break;
            }
        }

        private void RestartGame(object sender, RoutedEventArgs e)
        {
            restartButton.Visibility = Visibility.Hidden;
            quitButton.Visibility = Visibility.Hidden;
            mainMenuButton.Visibility = Visibility.Hidden;
            GameManager.ResetGame();
        }

        private void GoToMainMenu(object sender, RoutedEventArgs e)
        {
            Window mainMenu = new MainWindow();
            mainMenu.Show();
            Close();
        }

        private void QuitGame(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
