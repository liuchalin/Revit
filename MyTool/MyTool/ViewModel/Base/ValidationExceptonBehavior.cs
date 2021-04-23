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
        Dictionary<UIElement, int> _exceptionCount;
        Dictionary<UIElement, NotifyAdorner> _adornerCache;

        protected override void OnAttached()
        {
            _exceptionCount = new Dictionary<UIElement, int>();
            _adornerCache = new Dictionary<UIElement, NotifyAdorner>();
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
                    if (_exceptionCount.ContainsKey(element))
                    {
                        _exceptionCount[element]++;
                    }
                    else
                    {
                        _exceptionCount.Add(element, 1);
                    }
                }
                else if (e.Action == ValidationErrorEventAction.Removed)
                {
                    if (_exceptionCount.ContainsKey(element))
                    {
                        _exceptionCount[element]--;
                    }
                    else
                    {
                        _exceptionCount.Add(element, 0);
                    }
                }

                if (_exceptionCount[element] <= 0)
                {
                    HideAdorner(element);
                }
                else
                {
                    ShowAdorner(element, e.Error.ErrorContent.ToString());
                }

                int totalExceptionCount = 0;
                foreach (KeyValuePair<UIElement, int> kvp in _exceptionCount)
                {
                    totalExceptionCount += kvp.Value;
                }

                handler.IsValid = totalExceptionCount == 0;
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
            if (_adornerCache.ContainsKey(element))
            {
                _adornerCache[element].ChangeToolTip(errorMessage);
            }
            else
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
                NotifyAdorner adorner = new NotifyAdorner(element, errorMessage);
                adornerLayer.Add(adorner);
                _adornerCache.Add(element, adorner);
            }
        }

        void HideAdorner(UIElement element)
        {
            if (_adornerCache.ContainsKey(element))
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
                adornerLayer.Remove(_adornerCache[element]);
                _adornerCache.Remove(element);
            }
        }
    }
}
