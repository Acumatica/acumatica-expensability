using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI;
using Windows.UI.Xaml;

namespace Acumatica.ExpenseClaims.Converters
{
    class ExpenseClaimStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string status = value as string;
            switch (status)
            {
                case "Approved":
                    return "#6d69fe";
                case "On Hold":
                    return "#4d9ef9";
                case "Pending Approval":
                    return "#fe9923";
                case "Released":
                    return "#78e6fd";
                default:
                    return "#4d9ef9";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
