using System;
using System.Web;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace AIn.Ventures.UI.Models
{
    public class ManageUIModel
    {
        public AIn.Ventures.Models.Category Catalogs { set; get; }
        public AIn.Ventures.Models.QpointCategory Categories { set; get; }
    }
}
