using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Alexan.PhotoAlbums
{
    public class Routes : IRouteProvider 
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/PhotoAlbums",
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"},
                            {"controller", "PhotoAlbumAdmin"},
                            {"action", "Summary"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/PhotoAlbums/Create/{type}",
                        new RouteValueDictionary {
                                {"area", "Alexan.PhotoAlbums"},
                            {"controller", "PhotoAlbumAdmin"},
                            {"action", "Create"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/PhotoAlbums/{albumId}",
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"},
                            {"controller", "PhotoAlbumAdmin"},
                            {"action", "Item"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/PhotoAlbums/{albumId}/Edit",
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"},
                            {"controller", "PhotoAlbumAdmin"},
                            {"action", "Edit"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/PhotoAlbums/{albumId}/Remove/{deleteFolder}",
                        new RouteValueDictionary {
                            {"deleteFolder", false},
                            {"area", "Alexan.PhotoAlbums"},
                            {"controller", "PhotoAlbumAdmin"},
                            {"action", "Remove"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"}
                        },
                        new MvcRouteHandler())
                },
                //Photo
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/PhotoAlbums/{albumId}/Photo/Add",
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"},
                            {"controller", "PhotoAdmin"},
                            {"action", "Add"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/PhotoAlbums/{albumId}/Photo/UploadFile",
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"},
                            {"controller", "PhotoAdmin"},
                            {"action", "UploadFile"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/PhotoAlbums/Photo/{imageId}/Edit",
                        new RouteValueDictionary {
                                {"area", "Alexan.PhotoAlbums"},
                            {"controller", "PhotoAdmin"},
                            {"action", "Edit"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/PhotoAlbums/Photo/{photoId}/Remove/{deleteFile}",
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAdmin"},
                            {"controller", "PhotoAdmin"},
                            {"action", "Remove"},
                            {"deleteFile", false}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"}
                        },
                        new MvcRouteHandler())
                },
                //ImageHandling
                new RouteDescriptor {
                    Route = new Route(
                        "PhotoAlbums/{photoId}/Thumb/{width}/{height}",
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAdmin"},
                            {"controller", "ImageHandling"},
                            {"action", "Thumb"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Alexan.PhotoAlbums"}
                        },
                        new MvcRouteHandler())
                },
            };
        }
    }
}