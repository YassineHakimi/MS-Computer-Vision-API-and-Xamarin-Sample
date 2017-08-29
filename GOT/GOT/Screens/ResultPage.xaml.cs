using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GOT.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GOT.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultPage : ContentPage
    {
        public ResultPage()
        {
            InitializeComponent();
        }
        public ResultPage(Recognitionresult result)
        {
            InitializeComponent();
            foreach(var l in result.lines)
            label.Text += l.text + " ";
        }
    }
}