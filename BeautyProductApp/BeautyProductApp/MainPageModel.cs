using BeautyProductApp.Models;
using BeautyProductApp.Services;
using FFImageLoading;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace BeautyProductApp
{
    public class MainPageModel : BaseModel
    {
        public IProductService service;
        public MainPageModel(IProductService productService)
        {
            service = productService;
        }

        public ObservableCollection<Products> _ProductsList = new ObservableCollection<Products>();
        public ObservableCollection<Products> ProductsList
        {
            get => _ProductsList;
            set
            {
                if (_ProductsList != value)
                {
                    _ProductsList = value;
                    RaisePropertyChanged(nameof(ProductsList));
                }
            }
        }
        public ObservableCollection<Products> TempData
        {
            get; set;
        } = new ObservableCollection<Products>();

        private bool isRefresh;
        public bool IsToRefresh
        {
            get => isRefresh;
            set
            {
                isRefresh = value;
                RaisePropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged();
            }
        }

        private bool _errorMessage;
        public bool ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                RaisePropertyChanged();
            }
        }

        private string _SearchText;
        public string SearchText
        {
            get => _SearchText;
            set
            {
                _SearchText = value;
                RaisePropertyChanged();
            }
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            if (CrossConnectivity.Current.IsConnected)
            {
                await GetProductsData();
            }
            else
            {
                await CoreMethods.DisplayAlert("Internet", "Internet Required", "Cancel");
                ErrorMessage = true;
            }
        }

        public Command RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsToRefresh = true;
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        await GetProductsData(true);
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("Internet", "Internet Required", "Cancel");
                        ErrorMessage = true;
                    }
                    IsToRefresh = false;
                });
            }
        }

        public Command SortByName
        {
            get
            {
                return new Command(() =>
                {
                    if (TempData != null && TempData.Any())
                    {
                        var sortedList = TempData.OrderBy(p => p.product_name).ToList();
                        ProductsList = new ObservableCollection<Products>(sortedList);
                    }

                });
            }
        }

        public Command SortByBrand
        {
            get
            {
                return new Command(() =>
                {
                    if (TempData != null && TempData.Any())
                    {
                        var sortedList = TempData.OrderBy(p => p.brand_name).ToList();
                        ProductsList = new ObservableCollection<Products>(sortedList);
                    }
                });
            }
        }

        public Command SortByRating
        {
            get
            {
                return new Command(() =>
                {
                    if (TempData != null && TempData.Any())
                    {
                        var sortedList = TempData.OrderByDescending(p => p.hazard_rating).ToList();
                        ProductsList = new ObservableCollection<Products>(sortedList);
                    }
                });
            }
        }


        public Command SearchTextCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsLoading = true;

                    try
                    {
                        if (string.IsNullOrEmpty(SearchText))
                        {

                            if (TempData != null)
                            {
                                ProductsList = new ObservableCollection<Products>(TempData);

                                ErrorMessage = false;
                            }
                            else
                            {
                                ProductsList = new ObservableCollection<Products>();

                                ErrorMessage = true;
                            }
                        }
                        else
                        {

                            if (TempData != null && TempData.Count > 0)
                            {
                                var filteredProducts = TempData
                                    .Where(x =>
                                        x.brand_name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                        x.product_name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                        (x.hazard_rating_string != null && x.hazard_rating_string.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                                        (x.hazard_rating_category != null && x.hazard_rating_category.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                    ).ToList();

                                if (filteredProducts.Any())
                                {
                                    ProductsList = new ObservableCollection<Products>(filteredProducts);
                                    ErrorMessage = false;
                                }
                                else
                                {
                                    ProductsList = new ObservableCollection<Products>();
                                    ErrorMessage = true;
                                }

                            }
                            else
                            {
                                ProductsList = new ObservableCollection<Products>();
                                ErrorMessage = true;
                            }
                        }
                        if (ProductsList.Count > 0)
                        {
                            ErrorMessage = false;
                        }
                    }
                    catch (Exception ex)
                    {

                        ProductsList = new ObservableCollection<Products>();
                        ErrorMessage = true;

                    }
                    finally
                    {
                        IsLoading = false;
                    }
                    RaisePropertyChanged("ProductsList");
                });
            }
        }





        private async Task GetProductsData(bool isCallFromRefreshCommand = false)
        {
            if (isCallFromRefreshCommand)
            {
                IsLoading = false;

                if (ProductsList != null && ProductsList.Count != 0)
                    ProductsList.Clear();

            }
            else
            {
                IsLoading = true;
            }
            ErrorMessage = false;
            try
            {
                var Result = await service.GetProductsAsync("https://td-ios-coding-challenge.s3.amazonaws.com/product_data/products.json");
                if (Result != null)
                {
                    if (Result.products != null && Result.products.Count != 0)
                    {
                        foreach (var Product in Result.products)
                        {
                            Product.hazard_rating_string = GetStringByInteger(Product.hazard_rating);
                            ProductsList.Add(Product);
                        }
                        TempData = ProductsList;
                    }
                }

                else
                {
                    ErrorMessage = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = true;
                RaisePropertyChanged();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}


