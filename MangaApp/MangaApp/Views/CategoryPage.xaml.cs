﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MangaApp.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MangaApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoryPage : ContentPage
    {
        string searchname="";
        string searchcategory="";
        HttpClient http = new HttpClient();
        public List<Category> dslh;
        Host host = new Host();

        async void GetManga()
        {
            var kq = await http.GetStringAsync
                (host.url + "api/manga/GetMangaList");
            var dslh = JsonConvert.DeserializeObject<List<Manga>>(kq);
            lstdslh.ItemsSource = dslh;
        }
        async void GetCategory ()
        {
            var kq = await http.GetStringAsync
                (host.url + "api/category/GetCategory");
            dslh = JsonConvert.DeserializeObject<List<Category>>(kq);
            categoryPicker.ItemsSource = dslh;
            //
            //    categoryPicker.SelectedIndex = dslh.FindIndex(item => item.categoryID == i);

        }
        
        public CategoryPage()
        {
            InitializeComponent();
            GetCategory();
            //autopicked(i);
            GetManga();
        }

        private void invoke(object v)
        {
            throw new NotImplementedException();
        }

        private async void categoryPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            Category selected = picker.SelectedItem as Category;
            searchcategory = selected.categoryID.ToString();
            if (srch.Text == "" || srch.Text is null)
            {
                var kq = await http.GetStringAsync
                 (host.url + "api/manga/GetMangaByCategory?categoryID=" + selected.categoryID.ToString());
                var dslh = JsonConvert.DeserializeObject<List<Manga>>(kq);
                lstdslh.ItemsSource = dslh;
            }
            else
            {
                var kq = await http.GetStringAsync
                    (host.url + "api/manga/findmanga?name=" + searchname + "&categoryID=" + searchcategory);
                var dslh = JsonConvert.DeserializeObject<List<Manga>>(kq);
                lstdslh.ItemsSource = dslh;
            }
        }
        private void lstdslh_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            Manga manga = (Manga)lstdslh.SelectedItem;
            Navigation.PushAsync(new DetailMangaPage(manga));
        }
        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            //MenuItem menuItem = (MenuItem)sender;
            //Manga manga = menuItem.CommandParameter as Manga;
            var manga = (sender as View).BindingContext as Manga;
            Favorite favorite = new Favorite();
            favorite.mangaID = manga.MangaID;
            favorite.userID = User.userID;
            var json = JsonConvert.SerializeObject(favorite);
            var noidung = new StringContent(json, Encoding.UTF8, "application/json");
            var apires = await http.PostAsync(host.url + "api/user/AddFavorite", noidung);
        }
        async void OnActionSheetSimpleClicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("ActionSheet: Send to?", "Cancel", null, "Email", "Twitter", "Facebook");
            if (action == "Email")
            await DisplayAlert("pick", action, "yes", "no");
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (srch.Text != "")
            {
                if (!(srch.Text is null))
                {
                    
                        if (searchcategory == "")
                        {
                            var kq = await http.GetStringAsync
                        (host.url + "api/manga/findmanga?name=" + searchname);
                            var dslh = JsonConvert.DeserializeObject<List<Manga>>(kq);
                            lstdslh.ItemsSource = dslh;
                        }
                        else
                        {
                            var kq = await http.GetStringAsync
                            (host.url + "api/manga/findmanga?name=" + searchname + "&categoryID=" + searchcategory);
                            var dslh = JsonConvert.DeserializeObject<List<Manga>>(kq);
                            lstdslh.ItemsSource = dslh;
                        }
                    
                }
                else
                {
                    GetManga();
                }
            }
            else
            {
                GetManga();
            }
           /* var kq = await http.GetStringAsync
                (host.url + "api/manga/findmanga?name="+searchname+"&categoryID="+searchcategory);
            var dslh = JsonConvert.DeserializeObject<List<Manga>>(kq);
            lstdslh.ItemsSource = dslh;*/
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchname = srch.Text;  
        }

        private async void MenuItem_Clicked_1(object sender, EventArgs e)
        {
            //MenuItem menuItem = (MenuItem)sender;
            //Manga manga = menuItem.CommandParameter as Manga;
            var manga = (sender as View).BindingContext as Manga;
            Follow favorite = new Follow();
            favorite.mangaID = manga.MangaID;
            favorite.userID = User.userID;
            var json = JsonConvert.SerializeObject(favorite);
            var noidung = new StringContent(json, Encoding.UTF8, "application/json");
            var apires = await http.PostAsync(host.url + "api/follow/AddFollow", noidung);
        }

        private void Navigate_Tapped(object sender, EventArgs e)
        {
            var manga = (sender as View).BindingContext as Manga;
            Navigation.PushAsync(new DetailMangaPage(manga));
        }

        private void srch_Completed(object sender, EventArgs e)
        {

        }
    }
}