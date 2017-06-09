using MovieServiceClient.MovieService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        ClientServiceClient cs;
        MainWindow Parent { get; set; }
        public LoginWindow()
        {
            InitializeComponent();
        }
        public LoginWindow(MainWindow parent):this()
        {
            this.Parent = parent;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cs = new ClientServiceClient();
                Parent.loggedUser = cs.Login(this.LoginTextBox.Text, this.PasswordBox.Password);
                Parent.cs = cs;
                if (Parent.loggedUser != null)
                {
                    
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Nie udało się zalogować");
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Nie udało się zalogować");
            }
        }


    }
}
