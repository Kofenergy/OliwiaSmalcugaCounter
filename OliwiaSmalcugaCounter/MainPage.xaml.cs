using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace OliwiaSmalcugaCounter
{
    public partial class MainPage : ContentPage
    {
        private CounterDatabase _database;
        private List<Counter> _counters;

        public MainPage()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadCounters();
            LoadPickerItems(); 
        }

        private void LoadPickerItems()
        {
            ColorPicker.ItemsSource = new List<string> { "Black", "LightGray", "LightBlue", "DarkBlue", "Purple", "Red", "Pink" };
            ColorPicker.SelectedItem = ColorPicker.ItemsSource[6]; 
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCounters();
        }

        private async void InitializeDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "counters.db");
            _database = new CounterDatabase(dbPath);
        }

        private async void LoadCounters()
        {
            _counters = await _database.GetCountersAsync();
            CountersCollectionView.ItemsSource = _counters;
        }

        private async void OnAddCounterClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CounterNameEntry.Text) &&
                int.TryParse(InitialValueEntry.Text, out int initialValue))
            {
                var selectedColor = ColorPicker.SelectedItem as string;
                if (selectedColor == null)
                {
                    await DisplayAlert("Błąd", "Proszę wybrać kolor.", "OK");
                    return;
                }

                var newCounter = new Counter
                {
                    Name = CounterNameEntry.Text,
                    Value = initialValue,
                    Color = selectedColor,
                    InitialValue = initialValue
                };
                await _database.SaveCounterAsync(newCounter);
                LoadCounters();
            }
        }

        private async void OnIncrementClicked(object sender, EventArgs e)
        {
            var counter = (Counter)((Button)sender).CommandParameter;
            counter.Value++;
            await _database.SaveCounterAsync(counter);
            LoadCounters();
        }

        private async void OnDecrementClicked(object sender, EventArgs e)
        {
            var counter = (Counter)((Button)sender).CommandParameter;
            counter.Value--;
            await _database.SaveCounterAsync(counter);
            LoadCounters();
        }

        private async void OnResetClicked(object sender, EventArgs e)
        {
            var counter = (Counter)((Button)sender).CommandParameter;
            counter.Value = counter.InitialValue; 
            await _database.SaveCounterAsync(counter);
            LoadCounters();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var counter = (Counter)((Button)sender).CommandParameter;
            await _database.DeleteCounterAsync(counter);
            LoadCounters();
        }
    }
}
