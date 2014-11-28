using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Core;
using Windows.UI.Xaml.Controls;

namespace Acumatica.ExpenseClaims.Converters
{
    public class ItemClickEventArgsConverter : EventArgsToParameterConverterBase<ItemClickEventArgs>
    {
        protected override object Convert(ItemClickEventArgs args)
        {
            return args.ClickedItem;
        }
    }
}
