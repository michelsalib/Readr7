using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Readr7.Model
{
    public class Tag
    {
        public readonly static Tag All = new Tag { Id = "/All" };

        public String Id { get; set; }

        public String Name
        {
            get
            {
                return Id.Substring(Id.LastIndexOf('/')+1);
            }
        }

        public bool IsFolder
        {
            get
            {
                return Id.Contains("/lable/");
            }
        }
    }
}
