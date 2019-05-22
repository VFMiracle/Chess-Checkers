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
    /// Lógica interna para ChessGame.xaml
    /// </summary>
    public partial class ChessGame : Window
    {
        public ChessGame()
        {
            InitializeComponent();

            KeyDown += KeyPresses;
            KeyDown += PawnChange;
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
            if (!GameManager.GameEnded)
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        SelectSquare.SelectSection();
                        break;
                    case Key.Tab:
                        SelectSquare.DropPiece();
                        break;
                    case Key.E:
                        break;
                    default:
                        SelectSquare.Movement(e);
                        break;
                }
            }
            else
            {
                restartButton.Visibility = Visibility.Visible;
                mainMenuButton.Visibility = Visibility.Visible;
                quitButton.Visibility = Visibility.Visible;
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

        private void PawnChange(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R || e.Key == Key.Q || e.Key == Key.K || e.Key == Key.B)
                GameManager.ChangePawn(e.Key);
        }
    }
}
