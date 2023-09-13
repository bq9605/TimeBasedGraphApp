using System.Text.Json.Serialization;
using System;
using System.Text.RegularExpressions;
using System.Formats.Asn1;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.ComponentModel;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public class GraphData
        {
            public DateTime DateStr { get; set; }
            public int CheckInCount { get; set; }
            public string Cabname { get; set; }
            public string PartId { get; set; }
            public string Schedule{ get; set; }
            public string OrderNumber { get; set; }
        }
        public class RawCSVData
        {
            public string id { get; set; }
            public string depttype { get; set; }
            public string partid { get; set; }
            public string partname { get; set; }
            public string ordernumber { get; set; }
            public string linenumber { get; set; }
            public string schedule { get; set; }
            public string checkin { get; set; }
            public string checkout { get; set; }
        }
        public class ViewModel: INotifyPropertyChanged
        {
            public List<RawCSVData> OriginData { get; set; }
            private List<GraphData> data;
            public List<GraphData> Data
            {
                get { return data; }
                set
                {
                    if (data != value)
                    {
                        data = value;
                        OnPropertyChanged(nameof(Data));
                    }
                }
            }

            private string selectedPartname;
            private string selectedPartid;
            private string selectedSchedule;
            private string selectedOrdernumber;
            public DateTime BeginDate { get; set; } = new DateTime();
            public DateTime EndDate { get; set; } = new DateTime();
            public List<string> PartItmes { get; } = new List<string>();
            public List<string> PartIdItems { get; } = new List<string>();
            public List<string> ScheduleItems { get; } = new List<string>();
            public List<string> OrderNumbers { get; } = new List<string>();
            public int SelectedTypeIndex { get; set; } // Add SelectedTypeIndex property
            public event PropertyChangedEventHandler PropertyChanged;

            public ViewModel()
            {

                // Read the file contents
                string rptFilePath = @"E:\parttrackerLastMonth.csv";
                FileStream fs = File.Open(rptFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var rptReader = new StreamReader(fs);
                rptReader.ReadLine();
                rptReader.ReadLine();
                Dictionary<DateTime, GraphData> dateHourContent = new Dictionary<DateTime, GraphData>();
                OriginData = new List<RawCSVData>(); // Initialize OriginData before the loop

                HashSet<string> uniquePartNames = new HashSet<string>(); // Use HashSet to store unique type values
                HashSet<string> uniquePartIds = new HashSet<string>(); // Use HashSet to store unique type values
                HashSet<string> uniqueSchedules = new HashSet<string>(); // Use HashSet to store unique type values
                HashSet<string> uniqueOrderNumbers = new HashSet<string>(); // Use HashSet to store unique type values
                uniquePartNames.Add("All");
                uniquePartIds.Add("All");
                uniqueSchedules.Add("All");
                uniqueOrderNumbers.Add("All");
                while (!rptReader.EndOfStream)
                {
                    string linedata = rptReader.ReadLine();

                    string[] fields = Regex.Split(linedata, @",(?=(?:[^""]*""[^""]*"")*[^""]*$)");

                    if (fields.Length == 9)
                    {
                        var rawcsvdata = new RawCSVData{ id = fields[0],
                            depttype = fields[1], partid = fields[2], partname = fields[3], ordernumber = fields[4],
                            linenumber=fields[5], schedule=fields[6],checkin=fields[7],checkout = fields[8]
                        };
                        OriginData.Add(rawcsvdata);
                        
                    }
                    
                   
                }
                for(int i = 0; i < OriginData.Count(); i++)
                {
                    if (DateTime.TryParse(OriginData[i].checkin, out DateTime date))
                    {
                        DateTime dateHour = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                        if (dateHourContent.ContainsKey(dateHour))
                        {
                            dateHourContent[dateHour].CheckInCount++;
                        }
                        else
                        {
                            // Create a new instance of GraphData and add it to the dictionary
                            GraphData newData = new GraphData { CheckInCount = 1 };
                            dateHourContent[dateHour] = newData;
                        }
                    }
                    // Add unique type values to the HashSet
                    if (!string.IsNullOrEmpty(OriginData[i].partname))
                    {
                        uniquePartNames.Add(OriginData[i].partname);
                    }
                    if (!string.IsNullOrEmpty(OriginData[i].partid))
                    {
                        uniquePartIds.Add(OriginData[i].partid);
                    }
                    if (!string.IsNullOrEmpty(OriginData[i].schedule))
                    {
                        uniqueSchedules.Add(OriginData[i].schedule);
                    }
                    if (!string.IsNullOrEmpty(OriginData[i].ordernumber))
                    {
                        uniqueOrderNumbers.Add(OriginData[i].ordernumber);
                    }
                }
                
                PartItmes = uniquePartNames.ToList();
                PartItmes.Sort();
                PartIdItems = uniquePartIds.ToList();
                PartIdItems.Sort();
                ScheduleItems = uniqueSchedules.ToList();
                ScheduleItems.Sort();
                OrderNumbers = uniqueOrderNumbers.ToList();
                OrderNumbers.Sort();
                Data = dateHourContent.Select(kvp => new GraphData { DateStr = kvp.Key, CheckInCount = kvp.Value.CheckInCount,Cabname = kvp.Value.Cabname,OrderNumber=kvp.Value.OrderNumber,PartId = kvp.Value.PartId,Schedule = kvp.Value.Schedule }).ToList();
                BeginDate = dateHourContent.Keys.Min();
                EndDate = dateHourContent.Keys.Max();
            }
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public string SelectedPartName
            {
                get { return selectedPartname; }
                set
                {
                    selectedPartname = value;
                    // Perform any additional logic or actions when the selected value changes
                    // For example, you can display a message or update other properties based on the selected value
                }
            }
            public string SelectedPartID
            {
                get { return selectedPartid; }
                set
                {
                    selectedPartid = value;
                    // Perform any additional logic or actions when the selected value changes
                    // For example, you can display a message or update other properties based on the selected value
                }
            }
            public string SelectedSchedule
            {
                get { return selectedSchedule; }
                set
                {
                    selectedSchedule = value;
                    // Perform any additional logic or actions when the selected value changes
                    // For example, you can display a message or update other properties based on the selected value
                }
            }
            public string SelectedOrderNumber
            {
                get { return selectedOrdernumber; }
                set
                {
                    selectedOrdernumber = value;
                    // Perform any additional logic or actions when the selected value changes
                    // For example, you can display a message or update other properties based on the selected value
                }
            }



        }
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new ViewModel();
        }
        public void changeFilter()
        {
            var viewModel = (ViewModel)this.BindingContext;
            Dictionary<DateTime, GraphData> dateHourContent = new Dictionary<DateTime, GraphData>();
            for (int i = 0; i < viewModel.OriginData.Count(); i++)
            {
                if (DateTime.TryParse(viewModel.OriginData[i].checkin, out DateTime date))
                {
                    DateTime dateHour = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                    if(viewModel.SelectedPartName != "All" && viewModel.SelectedPartName != null)
                    {
                        if (viewModel.OriginData[i].partname != viewModel.SelectedPartName)
                        {
                            continue;
                        }
                    }
                    if (viewModel.SelectedPartID != "All" && viewModel.SelectedPartID != null)
                    {
                        if (viewModel.OriginData[i].partid != viewModel.SelectedPartID)
                        {
                            continue;
                        }
                    }
                    if (viewModel.SelectedSchedule != "All" && viewModel.SelectedSchedule != null)
                    {
                        if (viewModel.OriginData[i].schedule != viewModel.SelectedSchedule)
                        {
                            continue;
                        }
                    }
                    if (viewModel.SelectedOrderNumber != "All" && viewModel.SelectedOrderNumber != null)
                    {
                        if (viewModel.OriginData[i].ordernumber != viewModel.SelectedOrderNumber)
                        {
                            continue;
                        }
                    }
                    if (date < viewModel.BeginDate || date > viewModel.EndDate)
                    {
                        continue;
                    }

                    if (dateHourContent.ContainsKey(dateHour))
                    {
                            dateHourContent[dateHour].CheckInCount++;
                    }
                    else
                    {
                        // Create a new instance of GraphData and add it to the dictionary
                        GraphData newData = new GraphData { CheckInCount = 1 };
                        dateHourContent[dateHour] = newData;
                    }
                }
                
            }

            viewModel.Data = dateHourContent.Select(kvp => new GraphData { DateStr = kvp.Key, CheckInCount = kvp.Value.CheckInCount, Cabname = kvp.Value.Cabname, OrderNumber = kvp.Value.OrderNumber, PartId = kvp.Value.PartId, Schedule = kvp.Value.Schedule }).ToList();
        }
        public void DatePickerFrom_DateSelected(object sender, DateChangedEventArgs e)
        {
            // Handle the "From" date selection event here
            DateTime selectedDate = e.NewDate;
            var viewModel = (ViewModel)this.BindingContext;
            viewModel.BeginDate = selectedDate;
            changeFilter();
            // Perform any necessary actions based on the selected date
        }

        public void DatePickerTo_DateSelected(object sender, DateChangedEventArgs e)
        {
            // Handle the "To" date selection event here
            DateTime selectedDate = e.NewDate;
            var viewModel = (ViewModel)this.BindingContext;
            viewModel.EndDate = selectedDate;
            changeFilter();
            // Perform any necessary actions based on the selected date
        }

        public void CabnamePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle the "Type" picker selection event here
            var picker = (Picker)sender;
            var cabName = picker.SelectedItem;
            var viewModel = (ViewModel)this.BindingContext;
            viewModel.SelectedPartName = cabName.ToString();
            changeFilter();
       }
        public void PartIdPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle the "Type" picker selection event here
            var picker = (Picker)sender;
            var partId = picker.SelectedItem;
            var viewModel = (ViewModel)this.BindingContext;
            viewModel.SelectedPartID = partId.ToString();
            changeFilter();
        }
        public void SchedulePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle the "Type" picker selection event here
            var picker = (Picker)sender;
            var schedule = picker.SelectedItem;
            var viewModel = (ViewModel)this.BindingContext;
            viewModel.SelectedSchedule = schedule.ToString();
            changeFilter();
        }
        public void OrdernumberPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle the "Type" picker selection event here
            var picker = (Picker)sender;
            var ordernumber = picker.SelectedItem;
            var viewModel = (ViewModel)this.BindingContext;
            viewModel.SelectedOrderNumber = ordernumber.ToString();
            changeFilter();
        }
    }
}