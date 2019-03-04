using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BeyondDynamo.UI
{
    /// <summary>
    /// Interaction logic for TextBoxWindow.xaml
    /// </summary>
    public partial class SearchWorkspaceWindow : Window
    {
        /// <summary>
        /// Current Workspace
        /// </summary>
        public WorkspaceModel currentWorkspace { get; set; }

        /// <summary>
        /// List with all found nodes
        /// </summary>
        public List<NodeModel> FoundNodes { get; set; }

        /// <summary>
        /// List with all found nodes
        /// </summary>
        public List<NodeModel> nodes { get; set; }

        /// <summary>
        /// Constructor for the Class
        /// </summary>
        /// <param name="model"></param>
        public SearchWorkspaceWindow(WorkspaceModel model)
        {
            InitializeComponent();
            currentWorkspace = model;
            FoundNodes = new List<NodeModel>();
            this.searchBar.Text = "";
            foreach(NodeModel node in model.Nodes)
            {
                FoundNodes.Add(node);
                this.FoundNodesListbox.Items.Add(node.Name);
            }
        }

        private void FoundNodesListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchModel();
            int ind = this.FoundNodesListbox.SelectedIndex;
            if(ind != -1)
            {
                NodeModel selectedNode = FoundNodes[ind];
                currentWorkspace.CenterX = selectedNode.CenterX;
                currentWorkspace.CenterY = selectedNode.CenterY;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.FoundNodesListbox.SelectedIndex = -1;
            searchModel();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            searchModel();
        }
        
        private void searchModel()
        {
            //Clear the Search Results
            this.FoundNodesListbox.Items.Clear();
            //Clear the SearchSearch and Find List
            this.FoundNodes.Clear();

            //Check what the Search Condition is
            if (this.searchBar.Text == "")
            {
                // If Empty, add all nodes to the Search and Find List
                foreach (NodeModel node in currentWorkspace.Nodes)
                {
                    FoundNodesListbox.Items.Add(node.Name);
                    FoundNodes.Add(node);
                }
            }
            else
            {
                // If not empty, Check which nodes match the Criteria
                foreach (NodeModel node in currentWorkspace.Nodes)
                {
                    //Check if the Node matches the Criteria. Name and Search Text both in UPPERCASE
                    if (node.Name.ToUpper().Contains(this.searchBar.Text.ToUpper()))
                    {
                        FoundNodesListbox.Items.Add(node.Name);
                        FoundNodes.Add(node);
                    }
                }
            }
        }
        
    }
}
