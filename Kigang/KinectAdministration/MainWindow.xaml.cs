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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassLibraryDatabase;
//http://www.youtube.com/watch?v=jqkehT2F6SE&feature=related
//http://msdn.microsoft.com/en-us/library/cc716735.aspx
//http://msdn.microsoft.com/en-us/vbasic/dd776540.aspx
namespace KinectAdministration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private CollectionViewSource therapeutViewSource;
        private KigangContext context;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Instantiate the ObjectContext.
            context = new KigangContext();

            // Define a query that returns orders for a customer.
            // Because lazy loading is on by default, SalesOrderDetails
            // related to a SalesOrderHeader will be loaded when the query
            // is executed.

            //var query = from o in context.Therapeuten
            //            select o;

            // Execute the query and bind the result to the OrderItems control.
            //this.orderItemsGrid.DataContext = ((ObjectQuery)query).Execute(MergeOption.AppendOnly);
            //dataGridThearpeuten.DataContext = ((ObjectQuery)query).Execute(MergeOption.AppendOnly);
            therapeutViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("therapeutViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            context.Therapeuten.ToList();
            therapeutViewSource.Source = context.Therapeuten.Local;




            System.Windows.Data.CollectionViewSource patientViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("patientViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            context.Patients.ToList();
            patientViewSource.Source = context.Patients.Local;
            System.Windows.Data.CollectionViewSource therapieViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("therapieViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            context.Therapien.ToList();
            therapieViewSource.Source = context.Therapien.Local;
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var context = new KigangContext();
            var therapeuten = context.Therapeuten.Local;
        }
        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            therapeutViewSource.View.MoveCurrentToNext();
        }
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            context.SaveChanges();
        }
        private void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            if (therapeutViewSource.View.CurrentPosition != 0)
                therapeutViewSource.View.MoveCurrentToPrevious();
        }
        private void buttonPatientSave_Click(object sender, RoutedEventArgs e)
        {
            context.SaveChanges();
        }
        private void buttonPatientDelete_Click(object sender, RoutedEventArgs e)
        {
            context.Patients.Remove((Patient)patientDataGrid.SelectedItem);
        }

        private void buttonSaveTherapy_Click(object sender, RoutedEventArgs e)
        {
            context.SaveChanges();
        }

        private void buttonDeleteTherapy_Click(object sender, RoutedEventArgs e)
        {
            context.Therapien.Remove((Therapie)therapieDataGrid.SelectedItem);
        }
    }
}
