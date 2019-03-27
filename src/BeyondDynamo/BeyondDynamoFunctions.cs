using System.IO;
using System.Xml;
using System.Collections.Generic;
using Dynamo.ViewModels;
using Dynamo.Graph.Workspaces;
using Dynamo.Graph.Annotations;
using Dynamo.Models;
using Dynamo.Graph.Notes;
using Dynamo.Graph.Connectors;
using Dynamo.Graph.Nodes;
using BeyondDynamo.UI;
using System.Windows;
using static Dynamo.Models.DynamoModel;
using System.Net.Http;
using Forms = System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BeyondDynamo
{
    /// <summary>
    /// The Core Functions Class for the Beyond Dynamo Extension
    /// </summary>
    public class BeyondDynamoFunctions
    {
        /// <summary>
        /// Checks in which Language the Core String of Dynamo Script is
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string DynamoCoreLanguage(string filePath)
        {
            string coreString = File.ReadAllText(filePath);
            if (coreString.StartsWith("<"))
            {
                return "XML";
            }
            else
            {
                return "Json";
            }
        }

        /// <summary>
        /// Checks if a given filePath is open as a current workspace
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsFileOpen(DynamoViewModel viewModel, string filePath)
        {
            WorkspaceModel model = viewModel.Model.CurrentWorkspace;
            if(model.FileName == filePath)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// This Returns all Selected Items to a Current Selection
        /// </summary>
        /// <param name="model">The Current Dynamo Model</param>
        public static void KeepSelection(DynamoModel model)
        {
            foreach(dynamic item in model.CurrentWorkspace.CurrentSelection)
            {
                model.AddToSelection(item);
            }
        }

        /// <summary>
        /// Removes Session Trace Data from a given Filepath. Only for Dynamo 1.3 scripts because of the XML Structure
        /// </summary>
        /// <param name="filePath">The Filepath to the Dynamo 1.3 File</param>
        /// <returns></returns>
        public static bool RemoveSessionTraceData(string filePath)
        {
            bool succes = false;
            if (DynamoCoreLanguage(filePath) == "XML")
            {
                string xmlPath = Path.ChangeExtension(filePath, ".xml");
                File.Move(filePath, xmlPath);
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);

                XmlNode sessionTraceDataNode = null;
                XmlNode parentNode = null;

                foreach (XmlNode xmlNode in doc.DocumentElement)
                {
                    if (xmlNode.Name == "SessionTraceData")
                    {
                        sessionTraceDataNode = xmlNode;
                        parentNode = sessionTraceDataNode.ParentNode;
                    }
                }
                try
                {
                    parentNode.RemoveChild(sessionTraceDataNode);
                    doc.Save(xmlPath);
                    succes = true;
                }
                catch
                {

                }
                finally
                {
                    string newFilePath = Path.ChangeExtension(xmlPath, ".dyn");
                    File.Move(xmlPath, newFilePath);
                    File.Delete(xmlPath);
                }
                return succes;
            }
            else
            {
                return succes;
            }
        }

        /// <summary>
        /// Removes the Bindings from a given Filepath. This one is made for Dynamo 2.0
        /// </summary>
        /// <param name="DynamoFilepath"></param>
        /// <returns></returns>
        public static bool RemoveBindings(string DynamoFilepath)
        {
            bool succes = false;

            //Convert the Dynamo File to a Json Text
            string jsonString = File.ReadAllText(DynamoFilepath);

            //Create a JObject from the Json Text
            JObject dynamoGraph = JObject.Parse(jsonString);

            //Loop over the Output Nodes and get their names
            JToken bindings = dynamoGraph.SelectToken("Bindings");
            JContainer bindingsParent = null;
            foreach (JToken child in bindings.Children())
            {
                bindingsParent = child.Parent;
            }
            if (bindingsParent != null)
            {
                bindingsParent.RemoveAll();
                succes = true;
            }
            //Write that string 
            File.WriteAllText(DynamoFilepath, dynamoGraph.ToString());
            return succes;
        }

        /// <summary>
        /// Import a Dynamo script into the current ViewModel By Selecting a Filepath. Only for Dynamo 1.3 scripts because of the XML Structure
        /// </summary>
        /// <param name="viewModel">The Current Dynamo ViewModel</param>
        public static void ImportFromScript(DynamoViewModel viewModel)
        {
            WorkspaceModel model = viewModel.Model.CurrentWorkspace;
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Filter = "Dynamo Files (*.dyn)|*.dyn";
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Get the selected filePath
                string DynamoFilepath = fileDialog.FileName;
                string DynamoString = File.ReadAllText(DynamoFilepath);
                if (DynamoCoreLanguage(DynamoFilepath)=="XML")
                {
                    ImportXMLDynamo(viewModel, DynamoFilepath);
                }
                else if (DynamoCoreLanguage(DynamoFilepath) == "Json")
                {
                    System.Windows.Forms.MessageBox.Show("This script was made in Dynamo 2.0 or later and cannot be imported", "Import Error");
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Imports a XML DynamoScript into the current model
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="DynamoFilePath"></param>
        public static void ImportXMLDynamo(DynamoViewModel viewModel, string DynamoFilePath)
        {
            //Set the Run Type on Manual
            viewModel.CurrentSpaceViewModel.RunSettingsViewModel.SelectedRunTypeItem = new Dynamo.Wpf.ViewModels.RunTypeItem(RunType.Manual);
            WorkspaceModel model = viewModel.Model.CurrentWorkspace;
            
            //Create two lists for the Selection of the imported model
            List<string> guidList = new List<string>();
            List<dynamic> selectionList = new List<dynamic>();
            
            //Load a XML Document from the Dynamo File
            XmlDocument doc = new XmlDocument();
            doc.Load(DynamoFilePath);
            
            //Loop over the XML Elements in the Document
            foreach (XmlElement node in doc.DocumentElement)
            {
                try
                {
                    foreach (XmlElement element in node.ChildNodes)
                    {
                        model.CreateModel(element);
                        XmlAttributeCollection attributes = element.Attributes;
                        foreach (XmlAttribute attribute in attributes)
                        {
                            string attributeName = attribute.Name;
                            string attributeValue = attribute.Value;
                            if (attributeName == "guid")
                            {
                                guidList.Add(attributeValue);
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            # region MAKE ALL IMPORTED ITEMS THE NEW SELECTION
            //Check Groups
            #region
            foreach (AnnotationModel group in model.Annotations)
            {
                string itemGuid = group.GUID.ToString();
                foreach (string guid in guidList)
                {
                    if (itemGuid == guid)
                    {
                        selectionList.Add(group);
                    }
                }
            }
            #endregion
            

            //Check Notes
            #region
            foreach (Dynamo.Graph.Notes.NoteModel note in model.Notes)
            {
                string itemGuid = note.GUID.ToString();
                foreach (string guid in guidList)
                {
                    if (itemGuid == guid)
                    {
                        selectionList.Add(note);
                    }
                }
            }
            #endregion
            

            //Check Nodes
            #region
            foreach (Dynamo.Graph.Nodes.NodeModel node in model.Nodes)
            {
                string itemGuid = node.GUID.ToString();
                foreach (string guid in guidList)
                {
                    if (itemGuid == guid)
                    {
                        selectionList.Add(node);
                    }
                }
            }
            #endregion
            

            foreach (dynamic item in selectionList)
            {
                viewModel.Model.AddToSelection(item);
            }
            #endregion
            
        }

        /// <summary>
        /// Freezes all the Selected Nodes in the Dynamo Model
        /// </summary>
        /// <param name="model">The Current Dynamo Model</param>
        public static void FreezeNodes(DynamoModel model)
        {
            WorkspaceModel workspace = model.CurrentWorkspace;
            foreach (Dynamo.Graph.Nodes.NodeModel node in workspace.Nodes)
            {
                if (node.IsSelected)
                {
                    if (node.IsFrozen == false)
                    {
                        node.IsFrozen = true;
                    }
                }
            }
            KeepSelection(model);
        }

        /// <summary>
        /// Unfreezes all the Selected Nodes in the Dynamo Model
        /// </summary>
        /// <param name="model">The Current Dynamo Model</param>
        public static void UnfreezeNodes(DynamoModel model)
        {
            WorkspaceModel workspace = model.CurrentWorkspace;
            foreach (Dynamo.Graph.Nodes.NodeModel node in workspace.Nodes)
            {
                if (node.IsSelected)
                {
                    if (node.IsFrozen)
                    {
                        node.IsFrozen = false;
                    }
                }
            }
            KeepSelection(model);


        }

        /// <summary>
        /// Changes the Colors of the Selected Groups in the Workspace Model by using a Color Picker UI
        /// </summary>
        /// <param name="model">The Current Dynamo Model</param>
        /// <param name="config">The Beyond Dynamo Settings</param>
        public static BeyondDynamoConfig ChangeGroupColor(WorkspaceModel model, BeyondDynamoConfig config)
        {
            List<AnnotationModel> selectedGroups = new List<AnnotationModel>();
            foreach (AnnotationModel group in model.Annotations)
            {
                if (group.IsSelected)
                {
                    selectedGroups.Add(group);
                }
            }
            if (selectedGroups.Count > 0)
            {
                System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
                if(config.customColors != null)
                {
                    colorDialog.CustomColors = config.customColors;
                }
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach(AnnotationModel group in selectedGroups)
                    {
                        string colorString = System.Drawing.ColorTranslator.ToHtml(colorDialog.Color);
                        group.Background = colorString;
                    }
                    config.customColors = colorDialog.CustomColors;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("No Groups selected");
            }

            return config;
        }
        
        /// <summary>
        /// This Functions Calls a Text Editor Window for Each Selected Note in the Dynamo Model.
        /// </summary>
        /// <param name="model">The Current Dynamo Model</param>
        public static IEnumerable<Dynamo.Graph.Notes.NoteModel> GetTextNotes(DynamoModel model)
        {
            WorkspaceModel workspaceModel = model.CurrentWorkspace;
            
            //Check if there are any Notes selected
            List<Dynamo.Graph.Notes.NoteModel> selectedNotes = new List<Dynamo.Graph.Notes.NoteModel>();
            foreach (Dynamo.Graph.Notes.NoteModel note in workspaceModel.Notes)
            {
                if (note.IsSelected)
                {
                    selectedNotes.Add(note);
                }
            }

            if (selectedNotes.Count == 0)
            {
                System.Windows.MessageBox.Show("No Notes Selected");
                KeepSelection(model);
                return null;
            }

            IEnumerable<Dynamo.Graph.Notes.NoteModel> notes = workspaceModel.Notes;
            return notes;


        }

        /// <summary>
        /// Returns all the input node names within a given Graph File Path
        /// </summary>
        /// <param name="filePath">The Filepath to the Dynamo 1.3 File</param>
        public static List<string> GetInputNodes(string filePath)
        {
            List<string> inputNodeNames = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            foreach (XmlElement child in doc.DocumentElement)
            {
                if (child.Name == "Elements")
                {
                    foreach (XmlElement Node in child.ChildNodes)
                    {
                        XmlAttributeCollection attributes = Node.Attributes;
                        try
                        {
                            if (attributes["isSelectedInput"].Value.ToString() == "True")
                            {
                                string nodeName = attributes["nickname"].Value.ToString();
                                inputNodeNames.Add(nodeName);
                            }
                        }
                        catch { }
                    }
                }
            }
            return inputNodeNames;
        }
    }
}
