using LandmarkAI.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LandmarkAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png *.jpg *.jpeg)|*.png;*.jpg;*.jpeg";
            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(fileName));

                MakePredictionAsync(fileName);
            }
        }

        private async void MakePredictionAsync(string fileName)
        {
            string url = "https://westeurope.api.cognitive.microsoft.com/customvision/v3.0/Prediction/f5a8d1f4-e30f-4d99-bdda-76763e9706c2/classify/iterations/Iteration1/image";
            string prediction_key = "be13cb597a7e4cb8bf198619b001693b";
            string content_type = "application/octet-stream";

            byte[] file = File.ReadAllBytes(fileName);

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Prediction-Key", prediction_key);

                using(var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                    var response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();

                        List<Prediction> predictions = (List<Prediction>)JsonConvert.DeserializeObject<CustomVision>(responseString).predictions;

                        lvPredictions.ItemsSource = predictions;

                        string predicate = predictions.Find(p => p.probability == predictions.Max(pre => pre.probability)).tagName;

                        MessageBox.Show("This is: " + predicate);
                    }
                    else
                    {
                        MessageBox.Show("Cann't Predict this image please select another one!");
                    }

                }
            }
        }
    }
}
