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
using CookBook.Models;
using SQLite;
using System.Collections.ObjectModel;

namespace CookBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public int openClose = 0;
        public bool dualclick = false;
        List<Recipe> recipeList;

        public MainWindow()
        {
            InitializeComponent();
            recipeList = new List<Recipe>();
            readRecipeDatabase();

        }

        public void readRecipeDatabase()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.databasePath))
            {
                conn.CreateTable<Recipe>();
                recipeList = (conn.Table<Recipe>().ToList()).OrderBy(c => c.Name).ToList();
            }
            if (recipeList != null)
            {

                ObservableCollection<Recipe> recipeListObs = new ObservableCollection<Recipe>(recipeList);
                recipeListView.ItemsSource = recipeList;
            }
        }

        private void showtestRecipe(object sender, RoutedEventArgs e)
        {

        }
        //Moje_przepisy otwiera i zamyka pasek kategorii
        private void my_recipes_button(object sender, RoutedEventArgs e)
        {
            readRecipeDatabase();
            if (dualclick == true)
            {
                openClose++;
                dualclick = false;        
            }

            if (openClose % 2 == 0)
            {

                openClose++;
                categoryFilter.Visibility = Visibility.Visible;
                titleFilter.Visibility = Visibility.Visible;
                filteredSearch_button.Visibility = Visibility.Visible;

            }
            else
            {
                
                openClose = 2;
                categoryFilter.Visibility = Visibility.Hidden;
                titleFilter.Visibility = Visibility.Hidden;
                filteredSearch_button.Visibility = Visibility.Hidden;
                recipeListView.Visibility = Visibility.Hidden;
            }

        }

        private void Grid_hideButtons(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource != MR_button)
            {
                    dualclick = true;
                    categoryFilter.Visibility = Visibility.Hidden;
                    titleFilter.Visibility = Visibility.Hidden;
                    filteredSearch_button.Visibility = Visibility.Hidden;
                    recipeListView.Visibility = Visibility.Hidden;
            }
        }

        //Dodawanie przepisu
        private void add_new_button(object sender, RoutedEventArgs e)
        {
            categoryFilter.Visibility = Visibility.Hidden;
            titleFilter.Visibility = Visibility.Hidden;
            Hide();
          
            Window1 window1 = new Window1();
            if (this.WindowState == WindowState.Maximized)
            {
                window1.WindowState = WindowState.Maximized;
            }

            window1.ShowDialog();
            ShowDialog();
        }
        //zamykanie programu
        private void close_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void recipeListView_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            Recipe selectedRecipe = (Recipe)recipeListView.SelectedItem;

            if (selectedRecipe != null)
            {
                Window1 editrecipe = new Window1(selectedRecipe);
                editrecipe.ShowDialog();
            }
        }
        public List<string> categoriesFilter = new List<string>();
        private void categoryFilter_Loaded(object sender, RoutedEventArgs e)
        {
            categoriesFilter.Add("Italian");
            categoriesFilter.Add("French");
            categoriesFilter.Add("Spanish");
            categoriesFilter.Add("Polish");
            categoriesFilter.Add("Other");
            var combo = sender as ComboBox;
            combo.ItemsSource = categoriesFilter;
            combo.SelectedIndex = 0;
        }

        private void filteredSearch_Click(object sender, RoutedEventArgs e)
        {
            string categoryfilter = categoryFilter.Text;
            string titlefilter = titleFilter.Text;
            using (SQLite.SQLiteConnection connection = new SQLite.SQLiteConnection(App.databasePath))
            {
                connection.CreateTable<Recipe>();
                if (string.IsNullOrEmpty(titlefilter))
                {
                    recipeListView.Visibility = Visibility.Visible;
                    recipeList = (connection.Table<Recipe>().ToList()).OrderBy(t => t.Name).Where
                        (c => c.Category == categoryfilter).ToList();
                }
                else
                {
                    recipeListView.Visibility = Visibility.Visible;
                    recipeList = (connection.Table<Recipe>().ToList()).OrderBy(t => t.Name).Where
                        (c => c.Category == categoryfilter).Where(x => x.Name.Contains(titlefilter)).ToList();
                }
            }

            if (recipeList != null)
            {
                recipeListView.ItemsSource = recipeList;
            }
        }
    }
    public partial class Window1 : Window
    {

        public List<string> categories = new List<string>();
        Recipe recipe;
        public Window1(Recipe _recipe)
        {
            InitializeComponent();

            this.recipe = _recipe;
            titleBox.Text = _recipe.Name;
            categoryBox.Text = _recipe.Category;
            ingredientBox.AppendText(_recipe.Ingredients);
            methodBox.AppendText(_recipe.Method);
            M_button.Visibility = Visibility.Visible;
            D_button.Visibility = Visibility.Visible;
            A_button.Visibility = Visibility.Hidden;
        }

        //powrót do poprzedniego okna
        private void back_button(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            
        }

        private void add_Button(object sender, RoutedEventArgs e)
        {
            string ingredientText = new TextRange(ingredientBox.Document.ContentStart, 
                ingredientBox.Document.ContentEnd).Text;
            string methodText = new TextRange(methodBox.Document.ContentStart, 
                methodBox.Document.ContentEnd).Text;
            Recipe recipe = new Recipe()
            {
                Name = titleBox.Text,
                Category = categoryBox.Text,
                Ingredients = ingredientText,
                Method = methodText
            }; 

            using (SQLiteConnection connection = new SQLiteConnection(App.databasePath))
            {
                connection.CreateTable<Recipe>();
                connection.Insert(recipe);
            }


            Close();

        }

        private void modify_Button(object sender, RoutedEventArgs e)
        {
            string ingredientText = new TextRange(ingredientBox.Document.ContentStart, 
                ingredientBox.Document.ContentEnd).Text;
            string methodText = new TextRange(methodBox.Document.ContentStart, 
                methodBox.Document.ContentEnd).Text;

            recipe.Name = titleBox.Text;
            recipe.Category = categoryBox.Text;
            recipe.Ingredients = ingredientText;
            recipe.Method = methodText;

            using (SQLiteConnection connection = new SQLiteConnection(App.databasePath))
            {
                connection.CreateTable<Recipe>();
                connection.Update(recipe);
            }
            Close();

        }

        private void delete_Button(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(App.databasePath))
            {
                connection.CreateTable<Recipe>();
                connection.Delete(recipe);
            }
            Close();
        }

        private void categoryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = sender as ComboBox;
            string name = selectedCategory.SelectedItem as string;            
        }

        private void categoryBox_Loaded(object sender, RoutedEventArgs e)
        {
            categories.Add("Italian"); categories.Add("French"); categories.Add("Spanish");
            categories.Add("Polish"); categories.Add("Other");
            var combo = sender as ComboBox;
            combo.ItemsSource = categories;
            combo.SelectedIndex = 0;
        }
    }
}
