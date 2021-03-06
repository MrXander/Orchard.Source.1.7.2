﻿using Alexan.PhotoAlbums.Services;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Alexan.PhotoAlbums
{
    public class AdminMenu : INavigationProvider
    {
        private IAlbumService _albumService;
        public AdminMenu(IAlbumService albumService)
        {
            _albumService = albumService;
        }
        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Photo Albums"), "2.6", BuildMenu);
        }

        private void BuildMenu(NavigationItemBuilder menu)
        {
            menu.Add(T("Manage Albums"), "2",
                           item => item.Action("Summary", "PhotoAlbumAdmin", new { area = "Alexan.PhotoAlbums" }).Permission(Permissions.ManageAlbums));
        }
    }
}