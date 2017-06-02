using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Util;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace LocationT3
{
    [Activity(Label = "LocationT3", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener, IOnMapReadyCallback
    {
        LocationManager mLocMgr;
        Button mButton;
        TextView mLatidude;
        TextView mLangtitude;
        TextView mProvider;
        string tag = "MainActivity";
        double mpLatitude;
        double mpLongtitude;
        Location location;

        double lt;
        double lo;

        private GoogleMap mMap;
        Button mBtnNormal;
        Button mBtnHybrid;
        Button mBtnSatellite;
        Button mBtnTerrain;

        Marker mMarker;

        public double pLt {
            set {
                lt = value;
            }
            get {
                return lt;
            }
        }

        public double pLo {
            set {
                lo = value;
            }
            get {
                return lo;
            }
        }
        public double getLatitude() {
            if (location != null) {
                mpLatitude = location.Latitude;
            }
            return mpLatitude;
        }

        public double getLongtitude() {
            if (location != null) {
                mpLongtitude = location.Longitude;
            }
            return mpLongtitude;
        }

       
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            mButton = FindViewById<Button>(Resource.Id.myButton);
            mLatidude = FindViewById<TextView>(Resource.Id.latitude);
            mLangtitude = FindViewById<TextView>(Resource.Id.langtitude);
            mProvider = FindViewById<TextView>(Resource.Id.provider);

            mBtnNormal = FindViewById<Button>(Resource.Id.btnNormal);
            mBtnHybrid = FindViewById<Button>(Resource.Id.btnHybrid);
            mBtnSatellite = FindViewById<Button>(Resource.Id.btnSatellite);
            mBtnTerrain = FindViewById<Button>(Resource.Id.btnTerrain);

            mBtnNormal.Click += MBtnNormal_Click;
            mBtnHybrid.Click += MBtnHybrid_Click;
            mBtnSatellite.Click += MBtnSatellite_Click;
            mBtnTerrain.Click += MBtnTerrain_Click;
            SetUpMap();
        }

        private void MBtnTerrain_Click(object sender, EventArgs e)
        {
            mMap.MapType = GoogleMap.MapTypeTerrain;
        }

        private void MBtnSatellite_Click(object sender, EventArgs e)
        {
            mMap.MapType = GoogleMap.MapTypeSatellite;
        }

        private void MBtnHybrid_Click(object sender, EventArgs e)
        {
            mMap.MapType = GoogleMap.MapTypeHybrid;
        }

        private void MBtnNormal_Click(object sender, EventArgs e)
        {
            mMap.MapType = GoogleMap.MapTypeNormal;
        }

        protected override void OnStart()
        {
            base.OnStart();
            Log.Debug(tag,"OnStart Called");
        }

        //this methode gets call every time the activity has start
        protected override void OnResume()
        {
            base.OnResume();

            // get the references to LocationManager
            mLocMgr = GetSystemService(Context.LocationService) as LocationManager;

            mButton.Click += delegate {

                mButton.Text = "Location Services Running";

                // pass in the provider(GPS)
                // the minimum time between updates(in second)
                // the minimum distance the user need to move to generate an update(in meters)
                // and ILocationListener(recall the class that implement ILocationListner, which is this class)
                /* if (mLocMgr.AllProviders.Contains(LocationManager.NetworkProvider) 
                     && mLocMgr.IsProviderEnabled(LocationManager.NetworkProvider)) {

                     mLocMgr.RequestLocationUpdates(LocationManager.NetworkProvider,2000,1,this);

                 }
                 else {
                     Toast.MakeText(this,"The Network Provider does not exits or is not enabled",ToastLength.Long).Show();
                 }*/

                // To get GetBestProviders option. This will determie the best provider
                // at application launch. Note that once providers has been set
                // it will stay the same until the next time this methode is called

                var locationCriteria = new Criteria();
                locationCriteria.Accuracy = Accuracy.Coarse;
                locationCriteria.PowerRequirement = Power.Medium;

                string locationProvider = mLocMgr.GetBestProvider(locationCriteria,true);
                mLocMgr.RequestLocationUpdates(locationProvider,2000,1,this);
                Log.Debug(tag,"Starting Location Updates with " + locationProvider.ToString());
            };
        }

        protected override void OnPause()
        {
            base.OnPause();

            // stop sending location update when the application goes into the background

            mLocMgr.RemoveUpdates(this);
            Log.Debug(tag,"Location updates paused because application is entering the background");
        }

        protected override void OnStop()
        {
            base.OnStop();
            Log.Debug(tag,"OnStop Called");
        }

        public void OnLocationChanged(Location location) {
            Log.Debug(tag,"Location Changed");
            mLatidude.Text = "Latitude     : " + location.Latitude.ToString();
            mLangtitude.Text = "Longtitude : " + location.Longitude.ToString();
            mProvider.Text = "Provider     : " + location.Provider.ToString();


            lt = location.Latitude;
            lo = location.Longitude;

            if (mMarker != null) {
                mMarker.Position.Latitude = location.Latitude;
                mMarker.Position.Longitude = location.Longitude;
            }

            
        }

        public void OnProviderDisabled(string provider) {
            Log.Debug(tag, provider + " disabled by the user");
        }

        public void OnProviderEnabled(string provider) {
            Log.Debug(tag, provider + " enabled by the user");
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extra) {
            Log.Debug(tag, provider + " availability has changed to " + status.ToString());
        }

        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;

            // Latitude and Longtitude
            LatLng latlang = new LatLng(-7.7542632,110.4101495);

            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlang,2);
            mMap.MoveCamera(camera);

            // Make a marker
            MarkerOptions options = new MarkerOptions();
            options.SetPosition(latlang);

            mMarker =  mMap.AddMarker(options);
            mMap.MarkerClick += MMap_MarkerClick;
        }

        private void MMap_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            LatLng pos = e.Marker.Position;
            mMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(pos,2));
        }
    }
}

