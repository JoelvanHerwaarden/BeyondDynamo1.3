using System.Windows;

namespace BeyondDynamo.UI
{
    /// <summary>
    /// Interaction logic for TextBoxWindow.xaml
    /// </summary>
    public partial class TextBoxWindow : Window
    {
        /// <summary>
        /// The Start Text Propery of the Text Editor UI
        /// </summary>
        public string text { get; set; }
        
        /// <summary>
        /// The Constructor which gets called on initiation. Sets the Text from the Selected Text Note to the Text Editor UI
        /// </summary>
        /// <param name="startText"></param>
        public TextBoxWindow(string startText)
        {
            text = startText;
            InitializeComponent();
            textBox.Text = text;
        }

        /// <summary>
        /// Sets the Written Text from the UI to the Selected Text Note when the User presses OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            text = textBox.Text;
            this.Close();
        }
    }
}
