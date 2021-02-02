﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.Locations;
using AContext = Android.Content.Context;


namespace Shiny.Locations
{
    public class LocationServicesGpsManagerImpl : AbstractGpsManager
    {
        readonly LocationManager client;
        public LocationServicesGpsManagerImpl(IAndroidContext context) : base(context)
            => this.client = context.GetSystemService<LocationManager>(AContext.LocationService);


        public override IObservable<IGpsReading?> GetLastReading()
        {
            var criteria = new Criteria
            {
                BearingRequired = false,
                AltitudeRequired = false,
                SpeedRequired = true
            };
            var loc = this.client.GetLastKnownLocation(this.client.GetBestProvider(criteria, false));
            if (loc != null)
            {
                var reading = new GpsReading(loc);
                return Observable.Return(reading);
            }
            return Observable.Empty<IGpsReading>();
        }


        protected override Task RequestLocationUpdates(GpsRequest request)
        {
            var criteria = new Criteria
            {
                BearingRequired = false,
                AltitudeRequired = false,
                SpeedRequired = true
            };

            this.client.RequestLocationUpdates(
                (long)request.Interval.TotalMilliseconds,
                (float)(request.MinimumDistance?.TotalMeters ?? 0),
                criteria,
                this.Callback,
                null
            );
            return Task.CompletedTask;
        }


        protected override Task RemoveLocationUpdates()
        {
            this.client.RemoveUpdates(this.Callback);
            return Task.CompletedTask;
        }
    }
}
