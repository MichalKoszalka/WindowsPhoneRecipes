using Pyszności.Common;
using Pyszności.Controller;
using Pyszności.DataSource;
using Pyszności.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Pyszności
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string FirstPivotView = "PopularRecipes";
        private const string SecondPivotView = "FavouriteRecipes";

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            
           // ListBox1.DataContext = NewRecipes;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if(localSettings.Values.ContainsKey("value"))
            {
                searchBox.Text = localSettings.Values["value"].ToString();
            }
            myIndeterminateProbar.Visibility = Visibility.Visible;
            var popularRecipes = await RestController.GetRecipesAsync("");
            this.defaultViewModel[FirstPivotView] = popularRecipes;
            myIndeterminateProbar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            myIndeterminateProbar.Visibility = Visibility.Visible;
            var popularRecipes = await RestController.GetRecipesAsync(searchBox.Text);
            this.defaultViewModel[FirstPivotView] = popularRecipes;
            myIndeterminateProbar.Visibility = Visibility.Collapsed;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = ((Recipe)e.ClickedItem);
            if (!Frame.Navigate(typeof(BasicPage1), item))
            {
                throw new Exception();
            }
        }

        private async void SearchAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(searchBox.Visibility == Visibility.Collapsed)
            {
                searchBox.Visibility = Visibility.Visible;
                searchBox.Focus(FocusState.Keyboard);
            }
            else
            {

                myIndeterminateProbar.Visibility = Visibility.Visible;
                searchBox.Visibility = Visibility.Collapsed;
                Debug.WriteLine(searchBox.Text);
                var recipes = await RestController.GetRecipesAsync(searchBox.Text);
                this.defaultViewModel[FirstPivotView] = recipes;
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            var favouriteRecipes = SampleDataSource.GetFavouriteRecipes();
            this.defaultViewModel[SecondPivotView] = favouriteRecipes;
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["value"] = searchBox.Text;
        }
    }
}
