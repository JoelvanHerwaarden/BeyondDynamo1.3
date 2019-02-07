using System.Windows;
using System.Xml;
using System.Collections.Generic;


namespace BeyondDynamo.UI
{
    /// <summary>
    /// Interaction logic for RemoveTraceDataWindow.xaml
    /// </summary>
    public partial class OrderPlayerInputWindow : Window
    {
        #region PROPERTIES
        /// <summary>
        /// The Dynamo Model as an XML Document for the OrderPlayerWindow 
        /// </summary>
        public XmlDocument dynamoGraph { get; set; }

        /// <summary>
        /// The Filepath for the Selected Dynamo Graph
        /// </summary>
        public string dynamoGraphPath { get; set; }

        /// <summary>
        /// The List with all the Input Nodes
        /// </summary>
        public List<string> InputNodeNames { get; set; }
        #endregion
        
        /// <summary>
        /// The OrderPlayerInputWindow Construtor. Sets the Input Nodes Names to the InputNodeNames property
        /// </summary>
        /// <param name="nodeNames"></param>
        public OrderPlayerInputWindow(List<string> nodeNames)
        {
            InitializeComponent();
            InputNodeNames = nodeNames;

            foreach (string nodeName in InputNodeNames)
            {
                InputNodesListBox.Items.Add(nodeName);
            }
        }
        
        /// <summary>
        /// Moves a the Selected Item in the window class Up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void up_click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this.InputNodesListBox.SelectedIndex;

            if (selectedIndex > 0)
            {
                var itemToMoveUp = this.InputNodeNames[selectedIndex];
                this.InputNodeNames.RemoveAt(selectedIndex);
                this.InputNodeNames.Insert(selectedIndex - 1, itemToMoveUp);
                this.InputNodesListBox.Items.Clear();
                foreach (string name in InputNodeNames)
                {
                    this.InputNodesListBox.Items.Add(name);
                }
                this.InputNodesListBox.SelectedIndex = selectedIndex - 1;
            }
        }

        /// <summary>
        /// Moves a the Selected Item in the window class Down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void down_click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this.InputNodesListBox.SelectedIndex;

            if (selectedIndex + 1 < this.InputNodeNames.Count)
            {
                var itemToMoveDown = this.InputNodeNames[selectedIndex];
                this.InputNodeNames.RemoveAt(selectedIndex);
                this.InputNodeNames.Insert(selectedIndex + 1, itemToMoveDown);
                this.InputNodesListBox.Items.Clear();
                foreach (string name in InputNodeNames)
                {
                    this.InputNodesListBox.Items.Add(name);
                }
                this.InputNodesListBox.SelectedIndex = selectedIndex + 1;
            }
        }

        /// <summary>
        /// Applies the current Order to the XML Document in the Background and Saves the XML Document in the Selected Filepath as a .Dyn File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            InputNodeNames.Clear();
            foreach (string name in InputNodesListBox.Items)
            {
                InputNodeNames.Add(name);
            }

            foreach (XmlElement child in dynamoGraph.DocumentElement)
            {
                if (child.Name == "Elements")
                {
                    foreach (string name in this.InputNodeNames)
                    {
                        foreach (XmlElement Node in child.ChildNodes)
                        {
                            if (Node.Attributes["nickname"].Value == name)
                            {
                                child.RemoveChild(Node);
                                child.AppendChild(Node);
                            }
                        }
                    }
                }
            }
            dynamoGraph.Save(dynamoGraphPath);
            this.Close();
        }
        
    }
}
