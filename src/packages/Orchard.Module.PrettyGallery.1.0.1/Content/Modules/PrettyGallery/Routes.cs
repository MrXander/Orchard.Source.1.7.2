using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace PrettyGallery {
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "PrettyGallery/PrettyGallery/UpdateFileInfo",
                        new RouteValueDictionary {
                            {"area", "PrettyGallery"},
                            {"controller", "PrettyGallery"},
                            {"action", "UpdateFileInfo"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "PrettyGallery"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 10,
                    Route = new Route(
                        "PrettyGallery/Image/Resize/{width}/{height}/{*imagePath}",
                        new RouteValueDictionary {
                            {"area", "PrettyGallery"},
                            {"controller", "Image"},
                            {"action", "Resize"},
                            {"width", ""},
                            {"height", ""},
                            {"imagePath", ""}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "PrettyGallery"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}