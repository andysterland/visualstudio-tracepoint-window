namespace TracePointsToolWindow
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for TracePointHostWindow1Control.
    /// </summary>
    public partial class TracePointHostWindow1Control : UserControl
    {
        private List<TracePointMessage> _tracepointCollection;
        private CollectionViewSource _collectionViewSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TracePointHostWindow1Control"/> class.
        /// </summary>
        public TracePointHostWindow1Control()
        {
            InitializeComponent();

            _tracepointCollection = new List<TracePointMessage>();
        }

        private void RefreshDataGrid()
        {
            _collectionViewSource = new CollectionViewSource();
            _collectionViewSource.Source = _tracepointCollection;
            _collectionViewSource.Filter += TracePoint_Filter;

            gridTracePoints.DataContext = _collectionViewSource;
        }

        public void AddTracePoint(TracePointMessage TracePoint)
        {
            _tracepointCollection.Add(TracePoint);
            RefreshDataGrid();
        }
        
        private void TracePoint_Filter(object sender, FilterEventArgs e)
        {
            if (e != null && e.Item != null)
            {
                TracePointMessage tp = e.Item as TracePointMessage;
                string filter = txtFilter.Text;
                e.Accepted = tp.IsMatch(filter);
            }
        }
        public void ClearAllTracePoints()
        {
            _tracepointCollection.Clear();
            RefreshDataGrid();
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshDataGrid();
        }

        private void tglClearOnSessionStart_Checked(object sender, RoutedEventArgs e)
        {
            Config.ClearOnSessionStart = true;
        }

        private void tglClearOnSessionStart_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.ClearOnSessionStart = false;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearAllTracePoints();
        }
    }
}