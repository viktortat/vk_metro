﻿using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Windows;

namespace VK_Metro.Models
{
    public class MainPageModel : INotifyPropertyChanged
    {
        public MainPageModel()
        {
            this.Init();
        }
        public void Init()
        {
            this.vkFriend = new ObservableCollection<VKFriendModel>();
            this.vkUsers = new ObservableCollection<VKFriendModel>();
            this.vkMessage = new ObservableCollection<VKMessageModel>();
            this.IsDataLoaded = false;
        }

        private ObservableCollection<VKFriendModel> vkFriend;
        private ObservableCollection<VKFriendModel> vkUsers;
        private ObservableCollection<VKMessageModel> vkMessage;
        public bool IsDataLoaded { get; set; }

        public string TitleImageUri
        {
            get
            {
                var darkVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
                if (darkVisibility == Visibility.Visible)
                {
                    return "/VK_Metro;component/Images/dark/VK_logotype.png";
                }
                else
                {
                    return "/VK_Metro;component/Images/light/VK_logotype_Light.png";
                }
            }
        }

        public IEnumerable VKFriend
        { 
            get
            {
                return from item in this.vkFriend
                       group item by item.groupIndex into n
                       orderby n.Key
                       select new GroupFriends<string, VKFriendModel>(n);
            }
        }

        public IEnumerable VKMessage
        {
            get
            {
                return from item in this.vkMessage
                       group item by item.uid into n
                       orderby n.Key
                       select new GroupDialogs<string, VKMessageModel>(n);
            }
        }

        public string GetPhoto(string uid)
        {

            foreach (var friend in vkFriend)
            {
                if (friend.uid == uid)
                    return friend.photo;
            }
            foreach (var user in vkUsers)
            {
                if (user.uid == uid)
                    return user.photo;
            }
            App.VK.GetUser(uid, result =>
            {
                VKFriendModel user = (VKFriendModel)result;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (user.photo == "http://vk.com/images/deactivated_c.gif")
                        user.photo = "/VK_Metro;component/Images/deactivated_c.png";
                    this.vkUsers.Add(user);
                    this.NotifyPropertyChanged("VKMessage");
                });
            }, error =>
            {
            });
            return "";
        }
        public string GetName(string uid)
        {

            foreach (var friend in vkFriend)
            {
                if (friend.uid == uid)
                    return friend.name;
            }
            foreach (var user in vkUsers)
            {
                if (user.uid == uid)
                    return user.name;
            }
            App.VK.GetUser(uid, result =>
            {
                VKFriendModel user = (VKFriendModel)result;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (user.photo == "http://vk.com/images/deactivated_c.gif")
                        user.photo = "/VK_Metro;component/Images/deactivated_c.png";
                    this.vkUsers.Add(user);
                    this.NotifyPropertyChanged("VKMessage");
                });
            }, error =>
            {
            });
            return "";
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

        /// <summary>
        /// Добавить
        /// </summary>
        /// <param name="VKFriends"></param>
        public void AddFriend(VKFriendModel[] VKFriends)
        {
            foreach (var friend in VKFriends)
                this.vkFriend.Add(friend);
            this.NotifyPropertyChanged("VKFriend");
        }
        public void AddMessage(VKMessageModel[] VKMessage)
        {
            foreach (var message in VKMessage)
                this.vkMessage.Add(message);
            this.NotifyPropertyChanged("VKMessage");
        }
    }
}
