using System;
using System.Collections.Generic;
using Readr7.Model.Entites;

namespace Readr7.Resources.DesignData
{
    public class MainViewDataModel
    {
        public Boolean IsAuthenticated { get; set; }

        public Feed Feed { get; set; }

        public MainViewDataModel()
        {
            IsAuthenticated = true;

            Feed = new Feed()
            {
                Items = new List<Item>()
                {
                    new Item(){
                        Title = "Short title",
                        Updated = 0,
                        Origin = new Origin(){
                            Title = "Michel"
                        },
                        Content = new ItemContent(){
                            Content = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                        }
                    },
                    new Item(){
                        Title = "Very long title, Very long title, Very long title, Very long title",
                        Updated = 0,
                        Origin = new Origin(){
                            Title = "very long author with a very long name"
                        },
                        Content = new ItemContent(){
                            Content = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                        }
                    }
                }
            };
        }
    }
}
