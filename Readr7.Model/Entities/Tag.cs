using System;

namespace Readr7.Model.Entites
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
