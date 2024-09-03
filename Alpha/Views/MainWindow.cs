using System.Windows;

namespace Alpha
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AlphabetViewModel();
        }
    }
}
