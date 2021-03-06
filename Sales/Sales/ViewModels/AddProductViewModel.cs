﻿
namespace Sales.ViewModels
{
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using System;
    using System.Collections.Generic;
    using Services;
    using System.Text;
    
    using Xamarin.Forms;
    using Sales.Common.Models;
    using System.Linq;
    using Plugin.Media;
    using Plugin.Media.Abstractions;

    public class AddProductViewModel:BaseViewModel
    {
        #region Attributes
        private Plugin.Media.Abstractions.MediaFile file;
        private bool isRunning;
        private bool isEnabled;
        private ApiService apiService;
        private ImageSource imageSource;
        #endregion


        #region Properties
        public string Description { get; set; }
        public string Price { get; set; }
        public string Remarks { get; set; }
        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.setValue(ref this.isRunning, value); }
        }
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { this.setValue(ref this.isEnabled, value); }
        }
        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set { this.setValue(ref this.imageSource, value); }
        }

        #endregion

        #region Constructors
        public AddProductViewModel()
        {
            this.apiService =new ApiService();
            this.IsEnabled = true;
            this.ImageSource = "noproduct";
        }
        #endregion


        #region Command
        public ICommand ChangeImageCommand
        {


            get
            {
                return new RelayCommand(ChangeImage);
            }
        }
        private async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();

            var source = await Application.Current.MainPage.DisplayActionSheet(
                Languages.ImageSource,
                Languages.Cancel,
                null,
                Languages.FromGallery,
                Languages.NewPicture);

            if (source == Languages.Cancel)
            {
                this.file = null;
                return;
            }

            if (source == Languages.NewPicture)
            {
                this.file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                this.file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (this.file != null)
            {
                this.ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
        }


        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Description))
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error,Languages.DescriptionError, Languages.Accept);
                return;
                    }
                       
            var price = decimal.Parse(this.Price);
            if (price<0)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.PriceError, Languages.Accept);
                return;
            }
            this.IsRunning = true;
            this.IsEnabled = false;
            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = false;
                await Application.Current.MainPage.DisplayAlert(Helpers.Languages.Error,
                    connection.Message, 
                    Helpers.Languages.Accept);
                return;
            }
            byte[] imageArray = null;
            if (this.file != null)
            {
                imageArray = FilesHelper.ReadFully(this.file.GetStream());
            }

            var product = new Product
            {
                Descripcion = this.Description,
                Precio = price,
                Remarks = this.Remarks,
                ImageArray = imageArray,


            };
                
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Post(url, prefix, controller,product);
            if (!response.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = false;
                await Application.Current.MainPage.DisplayAlert(Helpers.Languages.Error,
                    connection.Message,
                    Helpers.Languages.Accept);
                return;

            }

            var newProduct = (Product)response.Result;
            var viewModel = ProductsViewModel.GetInstance();
            viewModel.Products.Add(newProduct);
            //viewModel.Products = viewModel.Products.OrderBy(p => p.Descripcion).ToList();
            this.IsRunning = false;
            this.IsEnabled = false;
            await Application.Current.MainPage.Navigation.PopAsync();
        }


        #endregion
    }
}
