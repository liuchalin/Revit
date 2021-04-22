using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace MyTool.ViewModel
{
    public class ValidationExceptonBehavior : Behavior<FrameworkElement>
    {
        private Dictionary<UIElement, int> exceptionCountDict;
        private Dictionary<UIElement, NotifyAdorner> adornerDict;

        protected override void OnAttached()
        {
            exceptionCountDict = new Dictionary<UIElement, int>();
            adornerDict = new Dictionary<UIElement, NotifyAdorner>();
            this.AssociatedObject.AddHandler(Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(this.OnValidationError));
        }

        void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            try
            {
                IValidationExceptionHandler handler = GetValidationExceptionHandler();
                UIElement element = e.OriginalSource as UIElement;

                if (handler == null || element == null)
                {
                    return;
                }

                if (e.Action == ValidationErrorEventAction.Added)
                {
                    if (exceptionCountDict.ContainsKey(element))
                    {
                        exceptionCountDict[element]++;
                    }
                    else
                    {
                        exceptionCountDict.Add(element, 1);
                    }
                }
                else if (e.Action == ValidationErrorEventAction.Removed)
                {
                    if (exceptionCountDict.ContainsKey(element))
                    {
                        exceptionCountDict[element]--;
                    }
                    else
                    {
                        exceptionCountDict.Add(element, 0);
                    }
                }

                if (exceptionCountDict[element] <= 0)
                {
                    HideAdorner(element);
                }
                else
                {
                    ShowAdorner(element, e.Error.ErrorContent.ToString());
                }

                int totalExceptionCount = 0;
                foreach (KeyValuePair<UIElement, int> kvp in exceptionCountDict)
                {
                    totalExceptionCount += kvp.Value;
                }

                handler.IsValid = totalExceptionCount <= 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        IValidationExceptionHandler GetValidationExceptionHandler()
        {
            if (this.AssociatedObject.DataContext is IValidationExceptionHandler)
            {
                var handler = this.AssociatedObject.DataContext as IValidationExceptionHandler;
                return handler;
            }
            return null;
        }

        void ShowAdorner(UIElement element, string errorMessage)
        {
            if (adornerDict.ContainsKey(element))
            {
                adornerDict[element].ChangeToolTip(errorMessage);
            }
            else
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
                NotifyAdorner adorner = new NotifyAdorner(element, errorMessage);
                adornerLayer.Add(adorner);
                adornerDict.Add(element, adorner);
            }
        }

        void HideAdorner(UIElement element)
        {
            if (adornerDict.ContainsKey(element))
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
                adornerLayer.Remove(adornerDict[element]);
                adornerDict.Remove(element);
            }
        }
    }
}
