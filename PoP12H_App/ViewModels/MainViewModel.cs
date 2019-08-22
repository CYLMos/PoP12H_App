using Caliburn.Micro;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json.Linq;
using PoP12H_App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;

namespace PoP12H_App.ViewModels
{
    public class MainViewModel : Screen, INotifyPropertyChanged
    {
        private List<PoPModel> _popData;
        private ObservableCollection<string> _regionName;
        private string _selectedRegion;

        public MainViewModel()
        {
            // Init regions in Tainan
            this.initRegion();

            string result = this.getPoPData(SelectedRegion);
            this.updatePoPData(result);
        }

        private TimeData formateTime(string data)
        {
            string[] time = data.Split(new char[] { 'T', ':', '-', ' '});

            if(time.Length != 6)
            {
                throw new Exception("Time formate of time data from API is wrong~!");
            }

            TimeData timeData = new TimeData
            {
                Year = time[0],
                Month = time[1],
                Date = time[2],
                Hour = time[3],
                Minute = time[4]
            };

            return timeData;

        }

        private void initRegion()
        {
            ObservableCollection<string> regions = new ObservableCollection<string>();

            // Add regions in Tainan
            regions.Add("安南區");
            regions.Add("中西區");
            regions.Add("安平區");
            regions.Add("東區");
            regions.Add("南區");
            regions.Add("北區");
            regions.Add("白河區");
            regions.Add("後壁區");
            regions.Add("鹽水區");
            regions.Add("新營區");
            regions.Add("東山區");
            regions.Add("北門區");
            regions.Add("柳營區");
            regions.Add("學甲區");
            regions.Add("下營區");
            regions.Add("六甲區");
            regions.Add("南化區");
            regions.Add("將軍區");
            regions.Add("楠西區");
            regions.Add("麻豆區");
            regions.Add("官田區");
            regions.Add("佳里區");
            regions.Add("大內區");
            regions.Add("七股區");
            regions.Add("玉井區");
            regions.Add("善化區");
            regions.Add("西港區");
            regions.Add("山上區");
            regions.Add("安定區");
            regions.Add("新市區");
            regions.Add("左鎮區");
            regions.Add("新化區");
            regions.Add("永康區");
            regions.Add("歸仁區");
            regions.Add("關廟區");
            regions.Add("龍崎區");
            regions.Add("仁德區");

            RegionName = regions;
            SelectedRegion = "南區";
        }

        private string getPoPData(string region)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"http://opendata.cwb.gov.tw/api/v1/rest/datastore/F-D0047-077?Authorization=CWB-E8B85DC9-4A7C-4161-8FAC-F2E73581FA63&locationName={region}&elementName=PoP12h");

            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json";

            string result = "";

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            return result;
        }

        private void updatePoPData(string data)
        {
            try
            {
                JObject json = JObject.Parse(data);
                JArray results = (JArray)json["records"]["locations"][0]["location"][0]["weatherElement"][0]["time"];
                List<PoPModel> poPModels = new List<PoPModel>();

                for (int i = 0; i < results.Count; i++)
                {
                    var item = (JObject)results[i];

                    JArray elementValues = (JArray)item["elementValue"];
                    var elementValue = (JObject)elementValues[0];

                    TimeData start = this.formateTime(item["startTime"].ToString());
                    TimeData end = this.formateTime(item["endTime"].ToString());

                    PoPModel model = new PoPModel
                    {
                        startTime = start.Month + "/" + start.Date + " " + start.Hour + ":" + start.Minute,
                        endTime = end.Month + "/" + end.Date + " " + end.Hour + ":" + end.Minute,
                        value = int.Parse(elementValue["value"].ToString()),
                        measures = elementValue["measures"].ToString()
                    };


                    poPModels.Add(model);
                }

                PoPData = poPModels;
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }

        public SeriesCollection SeriesCollection
        {
            get;
            set;
        }

        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public List<PoPModel> PoPData
        {
            get
            {
                return _popData;
            }
            set
            {
                _popData = value;

                SeriesCollection = new SeriesCollection();
                Labels = new List<string>();
                ChartValues<int> values = new ChartValues<int>();

                foreach (PoPModel poPModel in PoPData)
                {
                    values.Add(poPModel.value);
                    Labels.Add(poPModel.startTime + "~\n" + poPModel.endTime);
                }

                SeriesCollection.Add(new ColumnSeries
                {
                    Title = "Raining Probability",
                    Values = values
                });

                Formatter = x => x + "%";

                NotifyOfPropertyChange(() => _popData);
                OnPropertyChanged("SeriesCollection");
            }
        }

        public ObservableCollection<string> RegionName
        {
            get
            {
                return _regionName;
            }
            set
            {
                _regionName = value;
            }
        }
        public string SelectedRegion
        {
            get
            {
                return _selectedRegion;
            }
            set
            {
                string result = this.getPoPData(value);
                this.updatePoPData(result);
                _selectedRegion = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class TimeData
        {
            public string Year { get; set; }
            public string Month { get; set; }
            public string Date { get; set; }
            public string Hour { get; set; }
            public string Minute { get; set; }

        }
       
    }
}
