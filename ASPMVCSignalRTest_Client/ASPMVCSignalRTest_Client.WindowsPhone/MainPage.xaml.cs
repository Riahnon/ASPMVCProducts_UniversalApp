using ProductsAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ASPMVCSignalRTest_Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static readonly DependencyProperty APIClientProperty = DependencyProperty.Register("APIClient", typeof(ProductsAPIClientVM), typeof(MainPage), new PropertyMetadata(null));

        private ProductsAPIClientVM APIClient
        {
            get { return (ProductsAPIClientVM)GetValue(APIClientProperty); }
            set { SetValue(APIClientProperty, value); }
        }

        bool mLoggingOut; //Flag to know if user logout is accidental or voluntary
        public MainPage()
        {
            Windows.UI.ViewManagement.ApplicationView.TerminateAppOnFinalViewClose = true;
            this.APIClient = new ProductsAPIClientVM(this.Dispatcher, new ProductsAPIClient());
            this.APIClient.PropertyChanged += _OnAPIClientPropertyChanged;
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += _OnBackPressed;
        }

        private async void mLoginBtn_Tapped(object sender, RoutedEventArgs e)
        {
            if(await _Login())
                await _QueryProductLists();
        }

        private async void mRegisterBtn_Tapped(object sender, RoutedEventArgs e)
        {
            if(await _Register())
                await _QueryProductLists();
        }

        private async void mLogoutBtn_Tapped(object sender, RoutedEventArgs e)
        {
            await _Logout();
        }

        private async void mAddProductListBtn_Tapped(object sender, RoutedEventArgs e)
        {
            await _AddProductList();
        }

        private async void mAddProductEntryBtn_Tapped(object sender, RoutedEventArgs e)
        {
            await _AddProductEntry();
        }

        private async void ProductListDelete_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var lSender = sender as FrameworkElement;
            if (lSender == null || !(lSender.DataContext is ProductListVM))
                return;
            if (Object.ReferenceEquals(mListViewProductLists.SelectedItem, lSender.DataContext))
                mListViewProductLists.SelectedItem = null;
            var lProductList = ((ProductListVM)lSender.DataContext).Model;
            await _DeleteProductList(lProductList);
            
        }

        private async void ProductEntryDelete_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var lSender = sender as FrameworkElement;
            if (lSender == null || !(lSender.DataContext is ProductEntryVM))
                return;

            if (!(mListViewProductLists.SelectedItem is ProductListVM))
                return;

            await _DeleteProductEntry(((ProductListVM)mListViewProductLists.SelectedItem).Model, ((ProductEntryVM)lSender.DataContext).Model);
        }

        private async void mProductListNameTxtBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                await _AddProductList();
                mProductListNameTxtBox.Focus(FocusState.Keyboard);
            }
        }

        private async void mProductEntryNameTxtBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                await _AddProductEntry();
                mProductEntryNameTxtBox.Focus(FocusState.Keyboard);
            }
        }

        private void CanvasDelete_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _MarkDeteCandidate(sender, e);
        }

        private void CanvasDelete_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _MarkDeteCandidate(sender, e);
        }

        private void CanvasDelete_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _UnmarkDeteCandidate(sender, e);
        }

        private void CanvasDelete_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            _UnmarkDeteCandidate(sender, e);
        }

        private void CanvasDelete_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _UnmarkDeteCandidate(sender, e);
        }

        private void _MarkDeteCandidate(object sender, PointerRoutedEventArgs e)
        {
            var lSender = sender as FrameworkElement;
            if (lSender == null)
                return;

            var lBrush = new SolidColorBrush(Color.FromArgb(55, 255, 0, 0));
            if (Object.ReferenceEquals(lSender.DataContext, mListViewProductLists.SelectedItem))
                lBrush = new SolidColorBrush(Color.FromArgb(55, 255, 0, 0));
            while (lSender != null)
            {

                if (lSender is Grid)
                {
                    ((FrameworkElement)sender).CapturePointer(e.Pointer);
                    ((Grid)lSender).Background = lBrush;
                    break;
                }
                lSender = VisualTreeHelper.GetParent(lSender) as FrameworkElement;
            }
        }

        private void _UnmarkDeteCandidate(object sender, PointerRoutedEventArgs e)
        {
            var lSender = sender as DependencyObject;
            while (lSender != null)
            {
                if (lSender is Grid)
                {
                    ((FrameworkElement)sender).ReleasePointerCapture(e.Pointer);
                    ((Grid)lSender).Background = null;
                    break;
                }
                lSender = VisualTreeHelper.GetParent(lSender);
            }
        }

        private async Task<bool> _Login()
        {
            string lErrorMsg = null;
            try
            {
                if (String.IsNullOrEmpty(mUserNameTxtBox.Text) || string.IsNullOrEmpty(mPwdBox.Password))
                    return false;
                await APIClient.Login(new LoginRegisterDTO() { UserName = mUserNameTxtBox.Text, Password = mPwdBox.Password });
                return true;
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.Unauthorized).ToString()))
                    lErrorMsg = "Invalid username and/or password";
                else if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.Forbidden).ToString()))
                    lErrorMsg = "The server rejected the connection. Please update your client to the last version";
                else
                    lErrorMsg = "Error communicating with server";

            }

            if (lErrorMsg != null)
                await _ShowMessageBox_Ok(lErrorMsg);
            
            return false;
        }

        private async Task _Logout()
        {
            mLoggingOut = true;
            await APIClient.Logout();
            mLoggingOut = false;
        }

        private async Task<bool> _Register()
        {

            if (String.IsNullOrEmpty(mUserNameTxtBox.Text) || string.IsNullOrEmpty(mPwdBox.Password))
                return false;
            string lErrorMsg = null;
            try
            {
                await APIClient.RegisterUser(new LoginRegisterDTO() { UserName = mUserNameTxtBox.Text, Password = mPwdBox.Password });
                return true;
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.NotModified).ToString()))
                    lErrorMsg = "Invalid username. There's already an user with the given username";
                else if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.Forbidden).ToString()))
                    lErrorMsg = "The server rejected the connection. Please update your client to the last version";
                else
                    lErrorMsg = "Error communicating with server";
            }
            if (lErrorMsg != null)
                await _ShowMessageBox_Ok(lErrorMsg);

            return false;
        }

        private async Task _QueryProductLists()
        {
            string lErrorMsg = null;
            try
            {
                await this.APIClient.QueryProductLists();
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.Forbidden).ToString()))
                    lErrorMsg = "The server rejected the connection. Please update your client to the last version";
                else
                    lErrorMsg = "Error retrieving product lists from server";
            }
            if (lErrorMsg != null)
                await _ShowMessageBox_Ok(lErrorMsg);
        }

        private async Task _AddProductList()
        {
            if (String.IsNullOrEmpty(mProductListNameTxtBox.Text))
                return;
            string lErrorMsg = null;
            try
            {
                await APIClient.CreateProductList(new ProductListDTO() { Name = mProductListNameTxtBox.Text });
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.Conflict).ToString()))
                    lErrorMsg = "Invalid name. There's already a product list with the given name";
                else if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.Forbidden).ToString()))
                    lErrorMsg = "The server rejected the connection. Please update your client to the last version";
                else
                    lErrorMsg = "Error communicating with server";
            }
            if (lErrorMsg != null)
                await _ShowMessageBox_Ok(lErrorMsg);
        }

        private async Task _AddProductEntry()
        {
            if (!(mListViewProductLists.SelectedItem is ProductListVM))
                return;

            if (String.IsNullOrEmpty(mProductEntryNameTxtBox.Text))
                return;

            var lSelectedList = ((ProductListVM)mListViewProductLists.SelectedItem).Model;
            string lErrorMsg = null;
            try
            {
                await APIClient.CreateProductEntry(lSelectedList, new ProductEntryDTO() { ProductName = mProductEntryNameTxtBox.Text });
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.Conflict).ToString()))
                    lErrorMsg = "Invalid name. The given product is already in the list";
                else if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.Forbidden).ToString()))
                    lErrorMsg = "The server rejected the connection. Please update your client to the last version";
                else
                    lErrorMsg = "Error communicating with server";
            }
            if (lErrorMsg != null)
                await _ShowMessageBox_Ok(lErrorMsg);
        }

        private async Task _DeleteProductList(ProductListDTO aProductList)
        {
            string lErrorMsg = null;
            try
            {
                await APIClient.DeleteProductList(aProductList);
            }
            catch (Exception ex)
            {
                //In very high concurrent scenarios race conditions may ocurr, that lead the application to delete an already (just) deleted product list
                if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.NotFound).ToString()))
                {
                    //The given list was not found (probably already deleted in a race condition) Nothing is done
                }
                else if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.Forbidden).ToString()))
                    lErrorMsg = "The server rejected the connection. Please update your client to the last version";
                else
                    lErrorMsg = "Error communicating with server";
            }
            if (lErrorMsg != null)
                await _ShowMessageBox_Ok(lErrorMsg);
        }

        private async Task _DeleteProductEntry(ProductListDTO aProductList, ProductEntryDTO aProductEntry)
        {
            string lErrorMsg = null;
            try
            {
                await APIClient.DeleteProductEntry(aProductList, aProductEntry);
            }
            catch (Exception ex)
            {
                //In very high concurrent scenarios race conditions may ocurr, that lead the application to delete an already (just) deleted product entry
                if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.NotFound).ToString()))
                {
                    //The given product entry was not found (probably already deleted in a race condition) Nothing is done
                }
                else if (ex is HttpRequestException && ex.Message.Contains(((int)HttpStatusCode.Forbidden).ToString()))
                    lErrorMsg = "The server rejected the connection. Please update your client to the last version";
                else
                    lErrorMsg = "Error communicating with server";
            }
            if (lErrorMsg != null)
                await _ShowMessageBox_Ok(lErrorMsg);
        }

        private async void _OnAPIClientPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Dispatcher.HasThreadAccess)
            {
                switch (e.PropertyName)
                {
                    case "LoggedInUser":
                        //Connection is lost
                        if (this.APIClient.LoggedInUser == null && !mLoggingOut)
                        {
                            await _ShowMessageBox_Ok("Connection with API server was lost. Try reconnecting");
                        }
                        break;
                    case "IsBusy":
                        if (APIClient.IsBusy)
                        {
                            this.IsEnabled = false;
                            mCircleIsBusy.Visibility = Visibility.Visible;
                            mStackPanelIsBusy.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            this.IsEnabled = true;
                            mCircleIsBusy.Visibility = Visibility.Collapsed;
                            mStackPanelIsBusy.Visibility = Visibility.Collapsed;
                        }
                        break;
                }
            }
            else
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => _OnAPIClientPropertyChanged(sender, e));
            }
        }

        private async void _OnBackPressed ( object sender, BackPressedEventArgs e)
        { 
            //Not logged in
            if(object.ReferenceEquals(APIClient.LoggedInUser,null))
                return;

            if (object.ReferenceEquals(mListViewProductLists.SelectedItem, null))
            {
                e.Handled = true;
                await _Logout();
            }
            else 
            {
                e.Handled = true;
                mListViewProductLists.SelectedItem = null;
            }

        }
        private async Task _ShowMessageBox_Ok(string aMessage)
        {
            var lMsg = new MessageDialog(aMessage);
            lMsg.Commands.Add(new UICommand("Ok"));
            await lMsg.ShowAsync();
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
    }
}
