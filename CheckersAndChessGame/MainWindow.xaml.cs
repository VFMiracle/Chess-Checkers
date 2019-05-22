using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CheckersAndChessGame
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            startCheckersButton.Click += OpenCheckersWindow;
            startChessButton.Click += OpenChessWindow;
            quitButton.Click += QuitGame;
        }

        private void OpenChessWindow(object sender, RoutedEventArgs e)
        {
            Window chessWindow = new ChessGame();
            chessWindow.Show();
            Close();
        }

        private void OpenCheckersWindow(object sender, RoutedEventArgs e)
        {
            Window checkerWindow = new CheckerGame();
            checkerWindow.Show();
            Close();
        }

        private void QuitGame(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
