﻿using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Wino.Calendar.Args;
using Wino.Calendar.Views.Abstract;
using Wino.Core.Domain.Entities.Calendar;
using Wino.Core.Domain.Enums;
using Wino.Core.Domain.Models.Calendar;
using Wino.Messaging.Client.Calendar;

namespace Wino.Calendar.Views
{
    public sealed partial class CalendarPage : CalendarPageAbstract,
        IRecipient<ScrollToDateMessage>,
        IRecipient<GoNextDateRequestedMessage>,
        IRecipient<GoPreviousDateRequestedMessage>
    {
        private DateTime? selectedDateTime;
        public CalendarPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        public void Receive(ScrollToDateMessage message) => CalendarControl.NavigateToDay(message.Date);

        public void Receive(GoNextDateRequestedMessage message) => CalendarControl.GoNextRange();

        public void Receive(GoPreviousDateRequestedMessage message) => CalendarControl.GoPreviousRange();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is CalendarPageNavigationArgs args)
            {
                if (args.RequestDefaultNavigation)
                {
                    // Go today.
                    WeakReferenceMessenger.Default.Send(new LoadCalendarMessage(DateTime.Now.Date, CalendarInitInitiative.App));
                }
                else
                {
                    // Go specified date.
                    WeakReferenceMessenger.Default.Send(new LoadCalendarMessage(args.NavigationDate, CalendarInitInitiative.User));
                }
            }
        }

        private void CellSelected(object sender, TimelineCellSelectedArgs e)
        {
            // Selected date is in Local kind.
            selectedDateTime = e.ClickedDate;

            // TODO: Popup is not positioned well on daily view.
            TeachingTipPositionerGrid.Width = e.CellSize.Width;
            TeachingTipPositionerGrid.Height = e.CellSize.Height;

            Canvas.SetLeft(TeachingTipPositionerGrid, e.PositionerPoint.X);
            Canvas.SetTop(TeachingTipPositionerGrid, e.PositionerPoint.Y);

            var testCalendarItem = new CalendarItem
            {
                CalendarId = Guid.Parse("9ead7613-dacb-4163-8d33-2e32e65008a1"),
                StartDate = selectedDateTime.Value,
                DurationInSeconds = TimeSpan.FromMinutes(30).TotalSeconds,
                CreatedAt = DateTime.UtcNow,
                Description = "Test Description",
                Location = "Poland",
                Title = "Test event",
                Id = Guid.NewGuid()
            };

            //WeakReferenceMessenger.Default.Send(new CalendarEventAdded(testCalendarItem));

            NewEventTip.IsOpen = true;
        }

        private void CellUnselected(object sender, TimelineCellUnselectedArgs e)
        {
            NewEventTip.IsOpen = false;
            selectedDateTime = null;
        }

        private void CreateEventTipClosed(TeachingTip sender, TeachingTipClosedEventArgs args)
        {
            // Reset the timeline selection when the tip is closed.
            CalendarControl.ResetTimelineSelection();
        }

        private void AddEventClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (selectedDateTime == null) return;

            // var eventEndDate = selectedDateTime.Value.Add(EventTimePicker.Time);

            // Create the event.
            // WeakReferenceMessenger.Default.Send(new CalendarEventAdded(new CalendarItem(selectedDateTime.Value, eventEndDate)));
        }

        private void QuickEventAccountSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuickEventAccountSelectorFlyout.Hide();
        }
    }
}
