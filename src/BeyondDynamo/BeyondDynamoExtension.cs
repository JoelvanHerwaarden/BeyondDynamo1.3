using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;
using BeyondDynamo.UI.About;
using Forms = System.Windows.Forms;
using System.Windows.Media.Imaging;
using Dynamo.Graph.Workspaces;
using Dynamo.Graph.Nodes;

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
            BatchRemoveTraceData = new MenuItem { Header = "Remove Session Trace Data from Dynamo Graphs" };
            BatchRemoveTraceData.Click += (sender, args) =>
            {
                var viewModel = new RemoveTraceDataViewModel(p);
                var window = new RemoveTraceDataWindow()
                {
                    // Set the data context for the main grid in the window.
                    MainGrid = { DataContext = viewModel },
                    // Set the owner of the window to the Dynamo window.
                    Owner = p.DynamoWindow,
                    viewModel = VM
                };
                window.Left = window.Owner.Left + 400;
                window.Top = window.Owner.Top + 200;
                window.Show();
            };
            
            OrderPlayerInput = new MenuItem { Header = "Order Player Input" };
            OrderPlayerInput.Click += (sender, args) =>
            {
                //Open a FileBrowser Dialog so the user can select a Dynamo Graph
                System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                fileDialog.Filter = "Dynamo Files (*.dyn)|*.dyn";
                List<string> inputNodeNames = new List<string>();
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Check if the Selected FilePath is not open
                    if(BeyondDynamoFunctions.IsFileOpen(VM, fileDialog.FileName))
                    {
                        Forms.MessageBox.Show("Please close the File before opening it", "Close file");
                        return;
                    }
                    //Call the SortInputNodes Function
                    BeyondDynamoFunctions.SortInputNodes(fileDialog.FileName);
                }
            };

            AboutItem = new MenuItem { Header = "About Beyond Dynamo" };
            AboutItem.Click += (sender, args) =>
            {
                //Show the About dialog
                About about = new About();
                about.Show();
            };
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

            GroupColor = new MenuItem { Header = "Change Group Color" };
            GroupColor.Click += (sender, args) =>
            {
                BeyondDynamo.BeyondDynamoFunctions.ChangeGroupColor(VM.CurrentSpaceViewModel.Model);
            };

            ScriptImport = new MenuItem { Header = "Import From Script" };
            ScriptImport.Click +=(sender, args)=>
            {

                BeyondDynamoFunctions.ImportFromScript(VM);
            };

            EditNotes = new MenuItem { Header = "Edit Note Text" };
            EditNotes.Click += (sender, args) =>
            {
                //Check if we are in an active graph
                if (VM.Workspaces.Count < 1)
                {
                    Forms.MessageBox.Show("This command can only run in an active graph.\nPlease open a Dynamo Graph to use this function.");
                    return;
                }

                BeyondDynamoFunctions.CallTextEditor(VM.Model);
            };

            FreezeNodes = new MenuItem { Header = "Freeze Multiple Nodes" };
            FreezeNodes.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.FreezeNodes(VM.Model);
            };

            UnfreezeNodes = new MenuItem { Header = "Unfreeze Multiple Nodes" };
            UnfreezeNodes.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.UnfreezeNodes(VM.Model);
            };

            #endregion THE FUNCTIONS WHICH CAN RUN INSIDE AN ACTIVE GRAPH
        
            #region  ADD ALL MENU ITEMS TO THE EXTENSION
            //App Extensions
            BDmenuItem.Items.Add(BatchRemoveTraceData);
            BDmenuItem.Items.Add(OrderPlayerInput);
            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(new Separator());

            //Graph Extensions
            BDmenuItem.Items.Add(RemoveTraceData);
            BDmenuItem.Items.Add(GroupColor);
            BDmenuItem.Items.Add(ScriptImport);
            BDmenuItem.Items.Add(EditNotes);
            BDmenuItem.Items.Add(FreezeNodes);
            BDmenuItem.Items.Add(UnfreezeNodes);

            //Main Extension
            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(AboutItem);
            p.dynamoMenu.Items.Add(BDmenuItem);
            #endregion THE FUNCTIONS WHICH CAN RUN OUTSIDE AN ACTIVE GRAPH
        }
    }
}