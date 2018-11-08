using System.Collections;
using System.Collections.Generic;
using System;

namespace DigitalSky.Tracker
{

    public interface ITracker
    {
        bool isInit { get; }

        string trackerName { get; }

        bool isTracking { get; }

        bool trackerActive { get; }

        bool Init();

        bool Open();

        void Close();

        void OnUpdate();

        void OnDestroy();

        void EnableTracking(bool enabled);

        bool AddListener(TrackRetargeter listener);

        void RemoveListener(TrackRetargeter listener);

        bool HasListener(TrackRetargeter listener);
    }
}

