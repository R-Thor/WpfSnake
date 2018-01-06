using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfSnake
{
    /// <summary>
    /// Interaction logic for TestWindow_NestedGrid.xaml
    /// </summary>
    public partial class TestWindow_NestedGrid : Window
    {
        public TestWindow_NestedGrid()
        {
            InitializeComponent();
            Grid MainGrid = new Grid();
 
            MainGrid.Background = Brushes.LightCyan;
 
            MainGrid.ShowGridLines = true;
 
            this.Content = MainGrid;
 
            this.SizeToContent = SizeToContent.WidthAndHeight;
 
            //add m
 
            for (int i = 0; i < 2; i++)
 
            {
 
                RowDefinition row = new RowDefinition();
 
                row.Height = GridLength.Auto;
 
                MainGrid.RowDefinitions.Add(row);
 
            }
 
            for (int i = 0; i < 2; i++)
 
            {
 
                ColumnDefinition col = new ColumnDefinition();
 
                col.Width = GridLength.Auto;
 
                MainGrid.ColumnDefinitions.Add(col);
 
            }
 
            //Row0_Column0
 
            TextBlock mainTextBlock = new TextBlock();
 
            mainTextBlock.Text = "MainGrid_Row0_Column0";
 
            MainGrid.Children.Add(mainTextBlock);
 
            Grid.SetRow(mainTextBlock, 0);
 
            Grid.SetColumn(mainTextBlock, 0);
 
 
 
            //Row0_Column1
 
            mainTextBlock = new TextBlock();
 
            mainTextBlock.Text = "MainGrid_Row0_Column1";
 
            MainGrid.Children .Add (mainTextBlock);
 
            Grid.SetRow(mainTextBlock,0);
 
            Grid.SetColumn (mainTextBlock, 1);
 
 
 
            //Row1_Column0
 
            mainTextBlock = new TextBlock();
 
            mainTextBlock.Text = "MainGrid_Row1_Column0";
 
            MainGrid.Children.Add(mainTextBlock);
 
            Grid.SetRow(mainTextBlock, 1);
 
            Grid.SetColumn (mainTextBlock, 0);
 
 
 
 
 
            //second Grid
 
            Grid SecondGrid = new Grid();
 
            SecondGrid.ShowGridLines = true;
 
 
 
            for (int i = 0; i < 2; i++)
 
            {
 
                RowDefinition row = new RowDefinition();
 
                row.Height = GridLength.Auto ;
 
                SecondGrid.RowDefinitions.Add(row);
 
 
 
                ColumnDefinition col = new ColumnDefinition();
 
                col.Width = GridLength.Auto;
 
                SecondGrid.ColumnDefinitions.Add(col);
 
            }
 
 
 
            SecondGrid.Background = Brushes.LightCoral;
 
            Grid.SetRow(SecondGrid, 1);
 
            Grid.SetColumn(SecondGrid, 1);
 
            //add in the Main Grid
 
            MainGrid.Children.Add(SecondGrid);
 
 
 
            //Second Row0_Column0
 
            TextBlock secondTextBlock = new TextBlock();
 
            secondTextBlock.Text = "SecondGrid_Row0_Column0";
 
            SecondGrid.Children.Add(secondTextBlock);
 
            Grid.SetRow(secondTextBlock, 0);
 
            Grid.SetColumn(secondTextBlock, 0);
 
 
 
            //Second Row1_Column1
 
            secondTextBlock = new TextBlock();
 
            secondTextBlock.Text = "SecondGrid_Row1_Column1";
 
            SecondGrid.Children.Add(secondTextBlock);
 
            Grid.SetRow(secondTextBlock, 1);
 
            Grid.SetColumn(secondTextBlock, 1);
 

        }
    }
}
