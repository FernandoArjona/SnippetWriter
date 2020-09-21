using Microsoft.Win32;
using Newtonsoft.Json;
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

namespace SnipetWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string loadDirectory = ($"{System.IO.Directory.GetCurrentDirectory()}" +
            $"\\Directory.json");
        public MainWindow()
        {
            //INITIALIZATION
            InitializeComponent();

            try
            {
                File.ReadAllText($"{System.IO.Directory.GetCurrentDirectory()}\\Directory.json");
                Update_Projects();
            }
            catch
            {
                CreateDirectory();
            }
        }

        #region Save and Load
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            /*
             *Takes the values from the xaml and
             *saves them into a a project file.
            */
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = ".json";

            if (saveFile.ShowDialog() == true)
            {
                Project_File project    = new Project_File();
                bool isDocumentFound    = false;
                string projectFile      = File.ReadAllText(saveFile.FileName);
                project                 = JsonConvert.DeserializeObject<Project_File>(projectFile);

                #region Matching Snippet search
                int i = 0;
                foreach (snippet Snippet in project.snippets)
                {
                    if (Snippet.Name == this.Name_Text.Text)
                    {
                        //Matching document IS found
                        isDocumentFound = true;
                        project.snippets[i].Name            = this.Name_Text.Text;
                        project.snippets[i].Text            = this.Writer_Main_Text.Text;
                        project.snippets[i].CharacterCount  = Convert.ToInt32(this.Character_Count_Text.Text);
                        project.snippets[i].WordCount       = Convert.ToInt32(this.WordCount_Text.Text);

                        project.snippets[i].LastEdit = DateTime.Now.ToString("dd.Mm.yy");
                    }
                    i++;
                }
                if (isDocumentFound == false)
                {
                    //Matching document is NOT found
                    project.snippets.Add(GetSnippet());
                }
                #endregion

                string editedproject = JsonConvert.SerializeObject(project);
                File.WriteAllText(saveFile.FileName, editedproject);
                Update_Selection(project);

                //Saves the adress of the saved file
                SetLocation(saveFile.FileName);
            }
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            try
            {
                if (open.ShowDialog() == true)
                {
                    //Reads a Project_File file
                    Project_File project = GetProject(open.FileName);
                    Update_Writer(0, project);

                    //Stores the location of the opened file
                    SetLocation(open.FileName);
                    Directory directory = GetDirectory();
                    
                    string json = JsonConvert.SerializeObject(project);
                    directory.projectPaths.Add(open.FileName);
                    json = JsonConvert.SerializeObject(directory);
                    File.WriteAllText(loadDirectory, json);

                    Update_Projects();
                }
            }
            catch
            {
                //Error expected then the file is empty.
            }
        }
        #endregion

        #region Update Values
        private void Update_Writer(int index, Project_File project)
        {
            /*
             * Writes the information of an indexed document of a project
             * into the writer interface.
             */
            snippet Snippet = project.snippets[index];
            this.Name_Text.Text                 = project.snippets[index].Name;
            this.Writer_Main_Text.Text          = project.snippets[index].Text;
            this.Character_Count_Text.Text      = Convert.ToString(project.snippets[index].CharacterCount);
            this.WordCount_Text.Text            = Convert.ToString(project.snippets[index].WordCount);
            this.LastEdit_Text.Text             = project.snippets[index].LastEdit;
            this.Creation_Text.Text             = project.snippets[index].CreationDate;
            Update_Selection(project);
        }
        private void Update_Selection(Project_File project)
        {
            this.Snippet_Select.Items.Clear();
            foreach (snippet document in project.snippets)
            {
                if (this.Snippet_Select.Items.Contains(document.Name) == false)
                {
                    this.Snippet_Select.Items.Add(document.Name);
                }
            }            
        }
        private void Update_Projects()
        {
            Directory directory = GetDirectory();
            foreach (string path in directory.projectPaths)
            {
                if (this.Project_Select.Items.Contains(path) == false)
                {
                    Project_File project = GetProject(path);
                    this.Project_Select.Items.Add(project.ProjectName);
                }
            }
        }
        #endregion

        #region Apply Input Changes 
        private void Snippet_Select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Loads the relevant document file when the chapter select changes
            try
            {
                //Updates the writer text to reflect the chosen selection
                Directory directory     = GetDirectory();
                Project_File project    = GetProject(directory.lastAdress);
                int index = this.Snippet_Select.SelectedIndex;
                Update_Writer(index, project);

                //Saves the selected item for its future automatic loading
                directory.writerIndex = this.Snippet_Select.SelectedIndex;
                File.WriteAllText(loadDirectory, JsonConvert.SerializeObject(directory));
            }
            catch
            {
                /*
                 * NO CONSEQUENCES FOR FAILURE
                 * 
                 * Initial application will not find a project file.
                 */
            }
        }
        private void Writer_Main_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Set character count
            this.Character_Count_Text.Text = Convert.ToString(this.Writer_Main_Text.Text.Length);

            //Counts the words in the main text
            string text = (this.Writer_Main_Text.Text).Trim();
            int wordCount = 0, index = 0;
            bool isSameWord = false;
            while (index < text.Length)
            {
                if ((char.IsWhiteSpace(text[index]) == false) && (isSameWord == false))
                {
                    isSameWord = true;
                    wordCount++;
                }
                else
                {
                    if (char.IsWhiteSpace(text[index]) == true)
                    {
                        isSameWord = false;
                    }
                }
                index++;
            }
            this.WordCount_Text.Text = Convert.ToString(wordCount);
        }
        private void Name_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Writes the Name on the secondary Name display
            this.Name_Text.Text = this.Name_Text.Text;

        }
        private void Project_Select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Directory directory = GetDirectory();
                string projectPath  = directory.projectPaths[this.Project_Select.SelectedIndex];
                //Reads a Project_File file
                Project_File project = GetProject(projectPath);
                Update_Writer(0, project);

                //Stores the location of the opened file
                SetLocation(projectPath);
            }
            catch
            {
                //Error expected then the file is empty.
            }
        }
        #endregion

        #region Button Actions
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //Deletes a document file from the project
            Directory directory = GetDirectory();
            Project_File project = GetProject(directory.lastAdress);

            int index = this.Snippet_Select.SelectedIndex;
            project.snippets.RemoveAt(index);
            string editedproject = JsonConvert.SerializeObject(project);

            File.WriteAllText(directory.lastAdress, editedproject);
            this.Snippet_Select.Items.RemoveAt(index);
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Menu.Visibility = Visibility.Hidden;
        }
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            this.Menu.Visibility = Visibility.Visible;
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            this.Name_Text.Text = "";
            this.Writer_Main_Text.Text = "";
        }
        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            Directory directory     = GetDirectory();
            Project_File project    = new Project_File();
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = ".json";
            if (saveFile.ShowDialog() == true)
            {
                project.ProjectName = System.IO.Path.GetFileName(saveFile.FileName);
                string json = JsonConvert.SerializeObject(project);
                File.WriteAllText(saveFile.FileName, json);
                directory.projectPaths.Add(saveFile.FileName);
                json = JsonConvert.SerializeObject(directory);
                File.WriteAllText(loadDirectory, json);
            }
            Update_Projects();
        }
        private void ClearDirectory_Click(object sender, RoutedEventArgs e)
        {
            CreateDirectory();
        }
        #endregion

        #region GetClasses
        private Project_File GetProject(string path)
        {
            Project_File project = JsonConvert.DeserializeObject<Project_File>(File.ReadAllText(path));
            return project;
        }
        private snippet GetSnippet()
        {
            snippet Snippet = new snippet
            {
                 //Matching document IS found
                Text            = this.Writer_Main_Text.Text,
                Name            = this.Name_Text.Text,
                CharacterCount  = Convert.ToInt32(this.Character_Count_Text.Text),
                WordCount       = Convert.ToInt32(this.WordCount_Text.Text),
                CreationDate    = DateTime.Now.ToString("dd.MM.yy"),
                LastEdit        = DateTime.Now.ToString("dd.MM.yy"),
            };
            return Snippet;
        }
        private Directory GetDirectory()
        {
            Directory directory = JsonConvert.DeserializeObject<Directory>(File.ReadAllText(loadDirectory));
            return directory;
        }
        #endregion

        #region Miscellaneous Classes
        private void CreateDirectory()
        {
            Directory directory = new Directory()
            {
                writerIndex = 0,
                lastAdress = "",
            };
            string json = JsonConvert.SerializeObject(directory);
            string path = System.IO.Directory.GetCurrentDirectory() + "\\Directory.json";
            File.WriteAllText(path, json);
        }
        private void SetLocation(string path)
        {
            //Saves the location of the last project opened
            Directory directory = GetDirectory();
            directory.lastAdress = path;
            File.WriteAllText(loadDirectory, JsonConvert.SerializeObject(directory));
        }
        #endregion

        
    }
}
