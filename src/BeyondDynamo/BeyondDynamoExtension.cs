using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;
using BeyondDynamo.UI.About;
using BeyondDynamo.UI;
using Forms = System.Windows.Forms;
using System.Windows.Media.Imaging;
using Dynamo.Graph.Workspaces;
using Dynamo.Graph.Nodes;
using DynamoCore.UI.Controls;
using System.Windows.Media;
using System.IO;
using System.Windows.Markup;
using System.Xml;

namespace BeyondDynamo
{
    /// <summary>
    /// The Core Functions Class for the Beyond Dynamo Extension
    /// </summary>
    public class BeyondDynamoExtension : IViewExtension
    {
        /// <summary>
        /// Head Menu Item
        /// </summary>
        private MenuItem BDmenuItem;

        /// <summary>
        /// Remove Trace Data Menu Item
        /// </summary>
        private MenuItem RemoveTraceData;

        /// <summary>
        /// Batch Remove Data Menu Item
        /// </summary>
        private MenuItem BatchRemoveTraceData;

        /// <summary>
        /// Change Group Color Menu Item
        /// </summary>
        private MenuItem GroupColor;

        /// <summary>
        /// Import Script Menu Item
        /// </summary>
        private MenuItem ScriptImport;

        /// <summary>
        /// Edit Notes Menu Item
        /// </summary>
        private MenuItem EditNotes;

        /// <summary>
        /// Freeze Multiple Nodes Menu Item
        /// </summary>
        private MenuItem FreezeNodes;

        /// <summary>
        /// Unfreeze Multiple Nodes Menu Item
        /// </summary>
        private MenuItem UnfreezeNodes;

        /// <summary>
        /// Order Player Inputs Menu Item
        /// </summary>
        private MenuItem OrderPlayerInput;

        /// <summary>
        /// Change Node Colors Menuitem
        /// </summary>
        private MenuItem ChangeNodeColors;
        
        /// <summary>
        /// About Window Menu Item
        /// </summary>
        private MenuItem AboutItem;
        

        #region Functions which have to be inplemented for the IViewExtension interface
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// StartUp
        /// </summary>
        /// <param name="p"></param>
        public void Startup(ViewStartupParams p)
        {
        }
        /// <summary>
        /// CurrentSpaceViewModel_WorkspacePropertyEditRequested
        /// </summary>
        /// <param name="workspace"></param>
        private void CurrentSpaceViewModel_WorkspacePropertyEditRequested(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Shutdown
        /// </summary>
        public void Shutdown()
        {
        }
        /// <summary>
        /// UniqueId
        /// </summary>
        public string UniqueId
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                return "Remove Trace Data";
            }
        }
        #endregion


        /// <summary>
        /// Function Which gets Called on Loading the Plug-In
        /// </summary>
        /// <param name="p">Parameters</param>
        public void Loaded(ViewLoadedParams p)
        {
            //The Title of the Plug-In
            BDmenuItem = new MenuItem { Header = "Beyond Dynamo" };
            DynamoViewModel VM = p.DynamoWindow.DataContext as DynamoViewModel;

            #region THE FUNCTIONS WHICH CAN RUN OUTSIDE AN ACTIVE GRAPH

            ChangeNodeColors = new MenuItem { Header = "Change Node Color" };
            ChangeNodeColors.Click += (sender, args) =>
            {
                //Make a Viewmodel for the Change Node Color Window
                var viewModel = new BeyondDynamo.UI.ChangeNodeColorsViewModel(p);

                //Get the current Node Color Template
                System.Windows.ResourceDictionary dynamoUI = Dynamo.UI.SharedDictionaryManager.DynamoColorsAndBrushesDictionary;

                //Initiate a new Change Node Color Window
                ChangeNodeColorsWindow colorWindow = new ChangeNodeColorsWindow(dynamoUI)
                {
                    // Set the data context for the main grid in the window.
                    MainGrid = { DataContext = viewModel },
                    // Set the owner of the window to the Dynamo window.
                    Owner = p.DynamoWindow,
                };
                colorWindow.Left = colorWindow.Owner.Left + 400;
                colorWindow.Top = colorWindow.Owner.Top + 200;

                //Show the Color window
                colorWindow.Show();
            };
            BDmenuItem.Items.Add(ChangeNodeColors);

            BatchRemoveTraceData = new MenuItem { Header = "Remove Session Trace Data from Dynamo Graphs" };
            BatchRemoveTraceData.Click += (sender, args) =>
            {
                //Make a ViewModel for the Remove Trace Data window
                var viewModel = new RemoveTraceDataViewModel(p);

                //Initiate a new Remove Trace Data window
                RemoveTraceDataWindow window = new RemoveTraceDataWindow()
                {
                    MainGrid = { DataContext = viewModel },
                    Owner = p.DynamoWindow,
                    viewModel = VM
                };
                window.Left = window.Owner.Left + 400;
                window.Top = window.Owner.Top + 200;

                //Show the window
                window.Show();
            };
            BDmenuItem.Items.Add(BatchRemoveTraceData);

            OrderPlayerInput = new MenuItem { Header = "Order Player Input" };
            OrderPlayerInput.Click += (sender, args) =>
            {
                //Open a FileBrowser Dialog so the user can select a Dynamo Graph
                System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                fileDialog.Filter = "Dynamo Files (*.dyn)|*.dyn";

                //Make an empty list for the input names
                List<string> inputNodeNames = new List<string>();

                //Open a File Dialog
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Make a viewModel for the Order player input window
                    var viewModel = new BeyondDynamo.UI.OrderPlayerInputViewModel(p);

                    //Check if the Selected FilePath is not open
                    if (BeyondDynamoFunctions.IsFileOpen(VM, fileDialog.FileName))
                    {
                        Forms.MessageBox.Show("Please close the File before opening it", "Close file");
                        return;
                    }

                    //Call the SortInputNodes Function
                    List<string> inputNodesNames = BeyondDynamoFunctions.GetInputNodes(fileDialog.FileName);
                    
                    //Check if there are any input nodes
                    if (inputNodeNames.Count <= 0)
                    {
                        System.Windows.MessageBox.Show("No input nodes found");
                        return;
                    }

                    //Initiate a Order Player Input Window
                    OrderPlayerInputWindow orderPlayerInput = new OrderPlayerInputWindow(inputNodeNames)
                    {
                        MainGrid = { DataContext = viewModel },
                        Owner = p.DynamoWindow
                    };
                    orderPlayerInput.Left = orderPlayerInput.Owner.Left + 400;
                    orderPlayerInput.Top = orderPlayerInput.Owner.Top + 200;

                    //Apply properties
                    orderPlayerInput.dynamoGraph = new XmlDocument();
                    orderPlayerInput.dynamoGraphPath = fileDialog.FileName;
                    orderPlayerInput.Show();
                }
            };
            BDmenuItem.Items.Add(OrderPlayerInput);

            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(new Separator());
            #endregion

            #region THE FUNCTIONS WHICH CAN RUN INSIDE AN ACTIVE GRAPH

            RemoveTraceData = new MenuItem { Header = "Remove Session Trace Data from Current Graph" };
            RemoveTraceData.Click += (sender, args) =>
            {
                //Check if the Graph is saved somewhere
                Dynamo.Graph.Workspaces.WorkspaceModel workspaceModel = VM.Model.CurrentWorkspace;
                string filePath = workspaceModel.FileName;
                bool succes = false;
                if(filePath == string.Empty)
                {
                    Forms.MessageBox.Show("Save the File before running this command");
                    return;
                }
                
                //Warn the user for the Restart
                Forms.DialogResult warningBox = Forms.MessageBox.Show("The Dynamo Graph will restart without Session Trace Data. \n\n The current graph will be saved", "Dynamo Graph Restart", System.Windows.Forms.MessageBoxButtons.OKCancel);
                if (warningBox == Forms.DialogResult.Cancel)
                {
                    return;
                }

                //Save the Graph, Close the Graph, Try to Remove Trace Data, Open the Graph again.
                VM.Model.CurrentWorkspace.Save(VM.Model.EngineController.LiveRunnerRuntimeCore);
                VM.Model.RemoveWorkspace(VM.Model.CurrentWorkspace);
                succes = BeyondDynamoFunctions.RemoveSessionTraceData(filePath);
                if (succes)
                {
                    Forms.MessageBox.Show("Session Trace Data removed", "Remove Session Trace Data");
                }
                else
                {
                    Forms.MessageBox.Show("The current graph doesn't contain Session Trace Data", "Remove Session Trace Data");
                }

                //Open workspace again
                VM.Model.OpenFileFromPath(filePath);
            };
            BDmenuItem.Items.Add(RemoveTraceData);

            GroupColor = new MenuItem { Header = "Change Group Color" };
            GroupColor.Click += (sender, args) =>
            {
                BeyondDynamo.BeyondDynamoFunctions.ChangeGroupColor(VM.CurrentSpaceViewModel.Model);
            };
            BDmenuItem.Items.Add(GroupColor);

            ScriptImport = new MenuItem { Header = "Import From Script" };
            ScriptImport.Click +=(sender, args)=>
            {

                BeyondDynamoFunctions.ImportFromScript(VM);
            };
            BDmenuItem.Items.Add(ScriptImport);

            EditNotes = new MenuItem { Header = "Edit Note Text" };
            EditNotes.Click += (sender, args) =>
            {
                //Check if we are in an active graph
                if (VM.Workspaces.Count < 1)
                {
                    Forms.MessageBox.Show("This command can only run in an active graph.\nPlease open a Dynamo Graph to use this function.");
                    return;
                }

                IEnumerable<Dynamo.Graph.Notes.NoteModel> notes = BeyondDynamoFunctions.GetTextNotes(VM.Model);
                if (notes != null)
                {
                    foreach (Dynamo.Graph.Notes.NoteModel note in notes)
                    {
                        if (note.IsSelected)
                        {
                            var viewModel = new TextBoxViewModel(p);
                            TextBoxWindow textBox = new TextBoxWindow(note.Text)
                            {
                                MainGrid = { DataContext = viewModel },
                                Owner = p.DynamoWindow
                            };
                            textBox.Left = textBox.Owner.Left + 400;
                            textBox.Top = textBox.Owner.Top + 200;
                            textBox.Show();
                            textBox.Closed += (send, arg) =>
                            {
                                note.Text = textBox.text;
                            };
                        }
                    }
                    BeyondDynamoFunctions.KeepSelection(VM.Model);
                }
            };
            BDmenuItem.Items.Add(EditNotes);

            FreezeNodes = new MenuItem { Header = "Freeze Multiple Nodes" };
            FreezeNodes.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.FreezeNodes(VM.Model);
            };
            BDmenuItem.Items.Add(FreezeNodes);

            UnfreezeNodes = new MenuItem { Header = "Unfreeze Multiple Nodes" };
            UnfreezeNodes.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.UnfreezeNodes(VM.Model);
            };
            BDmenuItem.Items.Add(UnfreezeNodes);
            #endregion THE FUNCTIONS WHICH CAN RUN INSIDE AN ACTIVE GRAPH
            
            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(new Separator());
            AboutItem = new MenuItem { Header = "About Beyond Dynamo" };
            AboutItem.Click += (sender, args) =>
            {
                //Show the About dialog
                About about = new About();
                about.Show();
            };
            BDmenuItem.Items.Add(AboutItem);

            p.dynamoMenu.Items.Add(BDmenuItem);
        }
    }
}