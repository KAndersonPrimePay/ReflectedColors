using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xamarin.Forms;

namespace ReflectedColors
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var colorFields = typeof(Color).GetRuntimeFields()
                .Where(f => f.IsPublic && f.IsStatic)
                .Select(f => CreateColorView((Color)f.GetValue(null), f.Name));

            foreach (var field in colorFields)
            {
                stackLayout.Children.Add(field);
            }
        }

        private Frame CreateColorView(Color color, string name)
        {
            return new Frame
            {
                Content = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 15,
                    Children =
                    {
                        new BoxView
                        {
                            Color = color,
                        },
                        new Label
                        {
                            Text=name,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            FontAttributes = FontAttributes.Bold,
                            VerticalOptions = LayoutOptions.Center
                        }
                        /*new Label
                        {
                            BackgroundColor = name == "Silver" ? Color.Black : Color.Silver,
                            Text = name,
                            TextColor = name == "Transparent" ? Color.Black : color,
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        } */
                    }
                }
            };
        }
    }
}
