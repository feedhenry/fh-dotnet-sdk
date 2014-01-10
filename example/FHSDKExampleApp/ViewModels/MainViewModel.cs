using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FHSDKExampleApp.Resources;
using System.Threading.Tasks;
using FHSDK.FHHttpClient;
using FHSDK;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace FHSDKExampleApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<TweetViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<TweetViewModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public async Task LoadData()
        {
            FHResponse fhres = await FH.Act("getTweets", new Dictionary<string, object>());
            JObject tweets = fhres.GetResponseAsJObject();
            JArray tweetsArr = (JArray)tweets["tweets"];
            for (int i = 0; i < tweetsArr.Count; i++)
            {
                string content = (string) tweetsArr[i]["text"];
                this.Items.Add(new TweetViewModel(content));
            }

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}