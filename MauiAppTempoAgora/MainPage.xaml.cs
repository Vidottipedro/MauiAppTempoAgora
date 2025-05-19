using MauiAppTempoAgora.Models;
using System.Diagnostics;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        private object txt_cidade;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";

                        dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nacer do sol: {t.sunrise} \n" +
                                         $"Por do sol: {t.sunset} \n" +
                                         $"Temp max: {t.temp_max} \n" +
                                         $"Temp min: {t.temp_min} \n";
                        lbl_res.Text = dados_previsao;

                        string mapa = $"https://embed.windy.com/embed.html?" +
                            $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                            $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                            $"&lat={t.lat.ToString().Replace(",", ".")}&lon={t.lon.ToString().Replace(",", ".")}";

                        wv_mapa.Source = mapa;

                        Debug.WriteLine(mapa);

                    }
                    else
                    {
                        lbl_res.Text = "Sem dadoss de Precisão";
                    }

                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "ok");

            }

        }

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10)
                    );


                Location? local = await Geolocation.Default.GetLocationAsync(request);

                if (local != null)
                {
                    string local_disp = $"Latitde: {local.Latitude} \n" +
                                        $"Longitude: {local.Longitude} \n";

                    lbl_coords.Text = local_disp;

                    //pega nome da cidade que está nas coordenadas.
                    GetCidade(local.Latitude, local.Longitude);

                }
                else
                {
                    lbl_coords.Text = "Nenhuma localização";
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: dispositivo não Suporta", fnsEx.Message, "ok");


            }
            catch (FeatureNotEnabledException fneEX)
            {
                await DisplayAlert("Error: Localização Desabilitada", fneEX.Message, "ok");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Error: Permissão da Localização", pEx.Message, "ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "ok");
            }
        }

        private async void GetCidade(double latitude, double longitude)
        {
            try
            {

                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);

                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "ok");
            }
        }



    }

}