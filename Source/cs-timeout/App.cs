using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cs_timed_silver
{
    public class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(GetMainDictionary());
            Resources.MergedDictionaries.Add(GetLightThemeDictionary());

            LoadTheme(AppTheme.Light);

            var w = new MainWindow();

            ShutdownMode = ShutdownMode.OnMainWindowClose;
            MainWindow = w;

            w.Show();
        }

        internal ResourceDictionary MLight = null,
            MDark = null,
            MMain = null;
        internal ResourceDictionary GetLightThemeDictionary()
        {
            if (MLight == null)
            {
                MLight = new ResourceDictionary() { Source = new Uri("Themes/Light.xaml", UriKind.Relative) };
            }

            return MLight;
        }

        internal ResourceDictionary GetDarkThemeDictionary()
        {
            if (MDark == null)
            {
                MDark = new ResourceDictionary() { Source = new Uri("Themes/Dark.xaml", UriKind.Relative) };
            }

            return MDark;
        }

        internal ResourceDictionary GetMainDictionary()
        {
            if (MMain == null)
            {
                MMain = new ResourceDictionary() { Source = new Uri("AppResources.xaml", UriKind.Relative) };
            }

            return MMain;
        }

        internal void LoadTheme(AppTheme t)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Resources.MergedDictionaries.Count == 2)
                {
                    switch (t)
                    {
                        case AppTheme.Dark:
                            Resources.MergedDictionaries[1] = GetDarkThemeDictionary();
                            break;

                        default:
                            Resources.MergedDictionaries[1] = GetLightThemeDictionary();
                            break;
                    }
                }
                else if (Resources.MergedDictionaries.Count == 1)
                {
                    switch (t)
                    {
                        case AppTheme.Dark:
                            Resources.MergedDictionaries.Add(GetDarkThemeDictionary());
                            break;

                        default:
                            Resources.MergedDictionaries.Add(GetLightThemeDictionary());
                            break;
                    }
                }
                else
                {
                    Resources.MergedDictionaries.Clear();
                    Resources.MergedDictionaries.Add(GetMainDictionary());
                    LoadTheme(t);
                }
            }), System.Windows.Threading.DispatcherPriority.Normal); // how to process this after the ItemsControl has generated its elements?
        }
    }
}
