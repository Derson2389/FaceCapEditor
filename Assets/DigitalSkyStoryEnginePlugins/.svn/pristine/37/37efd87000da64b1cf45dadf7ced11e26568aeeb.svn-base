using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalSky.Tracker;

public class ARFaceTrackBinding : TrackBinding
{
    public ARFaceTrackBinding(): base()
    {

    }

    public override void OnUpdateTrackData(TrackData data)
    {
        if (data == null || data.bindName != bindName)
            return;

        _normalizedValue = (data.blendshapeValue - limits.x) / (limits.y - limits.x);
        _normalizedValue = Mathf.Clamp01(_normalizedValue);
        if (inverted)
            _normalizedValue = 1f - _normalizedValue;

        // filter value
        _filteredValue = Filter(_normalizedValue);

        // set tracking value to TrackBindingTarget
        if (target != null)
        {
            target.SetBlendShape(_filteredValue * bindWeight * 100f);
        }
    }
}
