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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Martingale
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var parameters = (SessionParameters)this.Resources["MyParameters"];
            var session = Player.Play(parameters);
            var comments = new List<string>();
            comments.Add(String.Format("Argent en arrivant: {0:C}", parameters.StartMoney));
            comments.Add(String.Format("Argent en partant: {0:C}", parameters.StartMoney + session.PL));
            comments.Add(String.Format("{0}: {1:C}", session.PL>0 ? "Gain":"Perte", session.PL));
            comments.Add(String.Format("{0} parties jouées", session.Games.Count));
            comments.Add(String.Format("Pourcentage effectif de tirages gagnés: {0:##0,0.0000} %", session.GetEffectiveWinPercent()));
            TextResults.Text = String.Join("\r\n", comments);
            GridMasterResults.ItemsSource = session.Games;
        }
    }
}
