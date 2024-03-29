﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MangaApp.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MangaApp.Views 
{
    
    public partial class AboutPage : INotifyPropertyChanged
    {
        public int favouriteTapCount = 0;
        Host host = new Host();
        HttpClient client = new HttpClient();
        public List<User> userinfor;
        public List<Manga> dslh;
        public List<Manga> _employees;
        public List<Category> _Categorys;
        public  List<Manga> GetNElement(List<Manga> a, int n)
        {
            List<Manga> b = a.Take(5).ToList();
            return b;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
        }
        public List<Manga> Mangas
        {
            get { return _employees; }
            set { _employees = value; OnPropertyChanged(); }
        }
        public List<Manga> Mangas2
        {
            get { return _employees; }
            set { _employees = value; OnPropertyChanged(); }
        }
        public List<Manga> Mangas3
        {
            get { return _employees; }
            set { _employees = value; OnPropertyChanged(); }
        }
        public List<Category> Categorys
        {
            get { return _Categorys; }
            set { _Categorys = value; OnPropertyChanged(); }
        }
        public async void GetComment()
        {
            var kq = await client.GetStringAsync
                (host.url + "api/user/GetNotifyByUser?userID=" + User.userID.ToString());
            List<Notify> cmts = JsonConvert.DeserializeObject<List<Notify>>(kq);
            if ( cmts.Count == 0)
            {
                ntfno.IsVisible = true;
                ntfyes.IsVisible = false;
            }
            else
            {
                ntfyes.IsVisible = true;
                ntfno.IsVisible = false;
            }
            
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            getUser();
            GetComment();
        }
        public async Task<List<Manga>> LayDSLoaiHoa()
        {
            //List<Manga> dslh = null;
            HttpClient http = new HttpClient();
            var kq = await http.GetStringAsync
                (host.url+"api/manga/GetMangaList");
            return await Task.FromResult(JsonConvert.DeserializeObject<List<Manga>>(kq));
            //return await Task.FromResult(JsonConvert.DeserializeObject<List<Manga>>(kq).OrderByDescending(o => o.Liked).ToList());
            //lstdslh.ItemsSource = Mangas;
            //lstdslh2.ItemsSource = dslh;
            //this.BindingContext = this;
        }
        public async Task<List<Category>> GetCategory()
        {
            HttpClient http = new HttpClient();
            var kq = await http.GetStringAsync
                (host.url + "api/category/GetCategory");
            return await Task.FromResult(JsonConvert.DeserializeObject<List<Category>>(kq));
        }
        public async void getUser()
        {
            HttpClient http = new HttpClient();

            var kq = await http.GetStringAsync
                (host.url + "api/userInfor/GetUserByID?userID=" + User.userID.ToString());
            userinfor = JsonConvert.DeserializeObject<List<User>>(kq);
            username.Text = userinfor[0].userName;
        }
        public AboutPage()
        {
            InitializeComponent();
            //Mangas = LayDSLoaiHoa();
            Task.Run(async () =>
            {
                Mangas = await LayDSLoaiHoa();
                Categorys = await GetCategory();
                Mangas2 = Enumerable.Reverse(Mangas.Take(20)).ToList();
                lstdslh.ItemsSource = Mangas.OrderByDescending(o => o.Liked).Take(10).ToList();
                Mangas3 = GetNElement(Mangas, 5);

                //Carousel.ItemsSource = Mangas.Take(5);
                GetComment();
                getUser();
            });
            Device.StartTimer(TimeSpan.FromSeconds(2), (Func<bool>)(() =>
            {
                Carousel.Position = (Carousel.Position + 1) % 5;
                return true;
            }));

            this.BindingContext = this;
        }

         private void lstdslh_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
          
            //Manga manga = (Manga)lstdslh.SelectedItem;
            //Navigation.PushAsync(new DetailMangaPage(manga));
        }

        private async void PropertySelected(object sender, EventArgs e)
        {
            var manga = (sender as View).BindingContext as Manga;
            await this.Navigation.PushAsync(new DetailMangaPage(manga));


        }


        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            Manga manga= menuItem.CommandParameter as Manga;
            Favorite favorite = new Favorite();
            favorite.mangaID = manga.MangaID;
            favorite.userID = User.userID;
            var json = JsonConvert.SerializeObject(favorite);
            var noidung = new StringContent(json, Encoding.UTF8, "application/json");
            var apires = await client.PostAsync(host.url+"api/user/AddFavorite", noidung);
        }
        private async void ImgAddToWishlist_Tapped(object sender, EventArgs e)
        {
            
              var manga = (sender as View).BindingContext as Manga;
             Favorite favorite = new Favorite();
            favorite.mangaID = manga.MangaID;
            favorite.userID = User.userID;
            var json = JsonConvert.SerializeObject(favorite);
            var noidung = new StringContent(json, Encoding.UTF8, "application/json");
            var apires = await client.PostAsync(host.url + "api/user/AddFavorite", noidung);
             
            favouriteTapCount++;
            Image img = (Image)sender;
            img.Source = favouriteTapCount % 2 == 0 ? "FavouriteBlackIcon.png" : "FavouriteRedIcon.png";
        }







      

        
        /*private async void PropertySelected(object sender, EventArgs e)
        {
            var manga = (sender as View).BindingContext as Manga;
            await this.Navigation.PushAsync(new DetailMangaPage(manga));


        }*/

        private async void SelectType(object sender, EventArgs e)
        {
            var category = (sender as View).BindingContext as Category;
            //DisplayAlert("Test thu coi chon dc ko", category.categoryName, "yes", "no");
            var kq = await client.GetStringAsync
               (host.url + "api/manga/GetMangaByCategory?categoryID=" + category.categoryID.ToString());
            //Mangas = JsonConvert.DeserializeObject<List<Manga>>(kq);
            await Task.Run(async () =>
            {
                Mangas = JsonConvert.DeserializeObject<List<Manga>>(kq);
            });

            var view = sender as View;
            var parent = view.Parent as StackLayout;

            foreach (var child in parent.Children)
            {
                VisualStateManager.GoToState(child, "Normal");
                ChangeTextColor(child, "#707070");
            }

            VisualStateManager.GoToState(view, "Selected");
            ChangeTextColor(view, "#FFFFFF");
        }

        private void ChangeTextColor(View child, string hexColor)
        {
            var txtCtrl = child.FindByName<Label>("PropertyTypeName");

            if (txtCtrl != null)
                txtCtrl.TextColor = Color.FromHex(hexColor);
        }

        private void lstdslh_ItemSelected_1(object sender, SelectedItemChangedEventArgs e)
        {

               Manga manga = (Manga)lstdslh.SelectedItem;
               Navigation.PushAsync(new DetailMangaPage(manga));
        }

        private void Gotontf_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Notification());
        }

        private void Gotonsrch_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CategoryPage());

        }
    }
    /*private void Button_Clicked(object sender, EventArgs e)
    {
        testglobal.GlobalVariable = 12;
    }*/


    /*async private void lstdslh_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        HttpClient client = new HttpClient();
        Favorite addf = new Favorite();
        Manga lh = (Manga)lstdslh.SelectedItem;
        addf.mangaID = lh.MangaID;
        addf.userID = 7;
        var json = JsonConvert.SerializeObject(addf);
        var noidung = new StringContent(json, Encoding.UTF8, "application/json");
        var apires = await client.PostAsync("http://172.30.91.30/MangaApi/api/user/AddFavorite", noidung);
        string userID = await apires.Content.ReadAsStringAsync();
        int id = int.Parse(userID);

        await DisplayAlert("adsad", id.ToString(), "sd", "no");


    }*/

}
