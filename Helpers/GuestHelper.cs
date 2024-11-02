using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace MarkRestaurant.Helpers
{
    public static class GuestHelper
    {
        public static HtmlString GetFullName(this IHtmlHelper htmlHelper, User guest)
        {
            if (guest is null)
                return new HtmlString(new StringBuilder($" Fail ").ToString());

            else if (guest.Name == "")
                return new HtmlString(new StringBuilder($" Not entered ").ToString());

            else
                return new HtmlString(new StringBuilder($"{guest.Surname} {guest.Name} {guest.MiddleName}").ToString());
            
        }
    }
}
