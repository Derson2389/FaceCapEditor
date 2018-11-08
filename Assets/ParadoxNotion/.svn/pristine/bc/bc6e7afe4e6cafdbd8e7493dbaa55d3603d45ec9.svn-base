//by xieheng

using Kalman;///tq
using System.Collections.Generic;
using UnityEngine;

namespace Slate
{

    public class CurveSmooth
    {
        class RotationDatas
        {
            public float x;
            public float y;
            public float z;
            public bool m = false;

            public RotationDatas(float x, float y, float z, bool m)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.m = m;
            }
        }

        public enum SmoothType
        {
            oneEuro = 0,
            average,
            kalMan,
        }

        public enum SmoothTypePost
        {
            oneEuro = 0,
            average,
            kalMan,
        }

        public static float GetSmoothRotationThreshold(List<Vector3> rotations)
        {
            float threshold = 0;

            float[] xs = new float[rotations.Count];
            float[] ys = new float[rotations.Count];
            float[] zs = new float[rotations.Count];

            for (int i = 1; i < rotations.Count - 1; i++)
            {
                Vector3 a = rotations[i - 1];
                Vector3 b = rotations[i];

                float x = Mathf.Abs(b.x - a.x);
                float y = Mathf.Abs(b.y - a.y);
                float z = Mathf.Abs(b.z - a.z);

                xs[i] = x > 359 ? 360 - x : x;
                ys[i] = y > 359 ? 360 - y : y;
                zs[i] = z > 359 ? 360 - z : z;
            }

            float tx = GetThreshold(xs);
            float ty = GetThreshold(ys);
            float tz = GetThreshold(zs);
            threshold = (tx + ty + tz) / 3.0f;

            return threshold;
        }
        public static MatrixKalmanWrapper kalman;

        public static OneEuroFilter<Quaternion> rotationFilter;
        public static float filterFrequency = 120.0f;
        public static float filterMinCutoff = 1.0f;
        public static float filterBeta = 0.0f;
        public static float filterDcutoff = 1.0f;
        public static int filterFrames = 10;
        public static List<Quaternion> SmoothRotationTQ(List<Vector3> rotations, int SmoothType = 0)
        {
            rotationFilter = new OneEuroFilter<Quaternion>(filterFrequency);
            List<Quaternion> inputQList = new List<Quaternion>();
            for (int i = 0; i < rotations.Count; i++)
            {
                Quaternion quat = new Quaternion();
                quat.eulerAngles = rotations[i];
                if (SmoothType == (int)SmoothTypePost.average)
                {
                    inputQList.Add(SmoothCamera.SmoothRotation(quat));
                }
                else if (SmoothType == (int)SmoothTypePost.oneEuro)
                {
                    rotationFilter.UpdateParams(filterFrequency, filterMinCutoff, filterBeta, filterDcutoff);
                    inputQList.Add(rotationFilter.Filter(quat));
                }

            }
            return inputQList;
        }

        public static void kalmanInit(Vector3 state)
        {
            kalman.SetWrapperState(state);
        }

        public static OneEuroFilter<Vector3> rotationVecFilter;

        public static OneEuroFilter<Vector3> rotationVecRTFilter;
        private static float ClampValue(float v1, float v2)
        {
            if(Mathf.Abs(v2-v1) > 180.0f)
            {
                if(v2 > v1)
                {
                    v2 -= 360.0f;
                }
                else
                {
                    v2 += 360.0f;
                }
            }

            return v2;
        }
        public static List<Vector3> SmoothRotationVectorTQ(List<Vector3> rotations, int smoothType = 0)
        {
            for(int i=1; i<rotations.Count; i++)
            {
                float x = ClampValue(rotations[i-1].x, rotations[i].x);
                float y = ClampValue(rotations[i - 1].y, rotations[i].y);
                float z = ClampValue(rotations[i - 1].z, rotations[i].z);

                rotations[i] = new Vector3(x, y, z);
            }
            if(smoothType == (int)SmoothType.oneEuro)
                rotationVecFilter = new OneEuroFilter<Vector3>(filterFrequency);
            if (smoothType == (int)SmoothType.kalMan)
            {
                kalman = new MatrixKalmanWrapper();
                kalman.SetWrapperState(rotations[0]);
            }
            List<Vector3> inputQList = new List<Vector3>();
            for (int i = 0; i < rotations.Count; i++)
            {
                Vector3 quaVector = new Vector3();
                quaVector = rotations[i];
                if (smoothType == (int)SmoothType.oneEuro)
                {
                    rotationVecFilter.UpdateParams(filterFrequency, filterMinCutoff, filterBeta, filterDcutoff);
                    inputQList.Add(rotationVecFilter.Filter(quaVector));
                }
                else if (smoothType == (int)SmoothType.kalMan)
                {
                    inputQList.Add(kalman.Update(quaVector));
                }
            }
            return inputQList;
        } 

        public static void SmoothRotationEx(List<Vector3> rotations, float threshold, int tolerant = 0)
        {
            //threshold = Mathf.Min(359.999999f, Mathf.Max(0.0f, threshold));
            //tolerant  = Mathf.Max(0, tolerant);

            //List<RotationDatas> datas = GenerateRotationDatas(rotations, threshold);

            //int length = datas.Count;
            //int sidx = -1;

            //for (int i = 0; i < length; i++)
            //{
            //    rotations[i] = new Vector3(datas[i].x, datas[i].y, datas[i].z);

            //    if (datas[i].m && sidx < 0)
            //    {
            //        sidx = i;
            //    }

            //    if (sidx >= 0 && (!datas[i].m || i == length - 1))
            //    {
            //        SlerpQ(datas, sidx, i, rotations, SlerpMode.Atan);
            //        sidx = -1;
            //    }
            //}
            rotations = EuroSmoothQ(rotations);
        }

        private static List<Vector3> _rotations = new List<Vector3>();
        private static List<Vector3> _smoothes  = new List<Vector3>();
        private static int _currentFrameIndex = 0;
       
        public static void BeginSmoothRotationRT()
        {
            _rotations.Clear();
            _smoothes.Clear();
            _currentFrameIndex = 0;
            rotationVecRTFilter = new OneEuroFilter<Vector3>(filterFrequency);
            kalman = new MatrixKalmanWrapper();
            preRotation = Vector3.zero;
            SetSmoothFrames();
            resetKalmanInit();
        }

        public static void SetSmoothFrames()
        { 
            int smoothFrames = UnityEditor.EditorPrefs.GetInt("SmoothFrames", 10);
            SmoothCamera.StartSmooth(smoothFrames);
        }

        public static void SetSmoothParam(float frequency, float beta, float minCutoff, float dCutoff)
        {
            filterFrequency = frequency;
            filterBeta = beta;
            filterMinCutoff = minCutoff;           
            filterDcutoff = dCutoff;
        }

        public static Vector3 preRotation = Vector3.zero;
        public static bool kalManInit = true;
        public static void resetKalmanInit() {
            kalManInit = false;
        }

        public static Vector3 SmoothRotationVectorTQRt(Vector3 rotation, int smoothType)
        {   
            float x = ClampValue(preRotation.x, rotation.x);
            float y = ClampValue(preRotation.y, rotation.y);
            float z = ClampValue(preRotation.z, rotation.z);
            
            rotation = new Vector3(x, y, z);
            preRotation = rotation;
            Vector3 temp = new Vector3();
            if (smoothType == (int)SmoothType.oneEuro)
            {
                rotationVecRTFilter.UpdateParams(filterFrequency, filterMinCutoff, filterBeta, filterDcutoff);
                temp = rotationVecRTFilter.Filter(rotation);
            }
            else if(smoothType == (int)SmoothType.kalMan)
            {
                if (kalManInit)
                {
                    kalman.SetWrapperState(rotation);
                    kalManInit = false;
                }
                else
                {
                    temp = kalman.Update(rotation);
                }
            }
            
            return temp;
        }

        public static Vector3 SmoothRotationRT(Vector3 rotation, float threshold, int delayFrames = 10)
        {
            _rotations.Add(rotation);
            //_smoothes.Add(rotation);

            //if (_smoothes.Count < delayFrames)
            //    return _rotations[0];

            //List<RotationDatas> datas = GenerateRotationDatas(_rotations, threshold);
            //RemoveUnusedDatas(datas, delayFrames);

            //int length = datas.Count;
            //int sidx = -1;

            //for (int i = 0; i < length; i++)
            //{
            //    _smoothes[i] = new Vector3(datas[i].x, datas[i].y, datas[i].z);

            //    if (!datas[i].m)
            //    {
            //        _rotations[i] = new Vector3(datas[i].x, datas[i].y, datas[i].z);
            //    }

            //    if (datas[i].m && sidx < 0)
            //    {
            //        sidx = i;
            //    }

            //    if (sidx >= 0 && (!datas[i].m || i == length - 1))
            //    {
            //        SlerpQ(datas, sidx, i, _smoothes, SlerpMode.Linear);
            //        sidx = -1;
            //    }
            //}

            //return _smoothes[_currentFrameIndex++];
            List<Vector3> smoothes = EuroSmoothQ(_rotations);
            return smoothes[_currentFrameIndex++];
        }

        private static List<RotationDatas> GenerateRotationDatas(List<Vector3> rotations, float threshold)
        {
            int length = rotations.Count;

            List<RotationDatas> datas = new List<RotationDatas>();
            datas.Add(new RotationDatas(rotations[0].x, rotations[0].y, rotations[0].z, false));

            for (int i = 1; i < length; i++)
            {
                datas.Add(new RotationDatas(datas[i - 1].x, datas[i - 1].y, datas[i - 1].z, false));

                float dx = Mathf.Abs(rotations[i].x - rotations[i - 1].x);
                float dy = Mathf.Abs(rotations[i].y - rotations[i - 1].y);
                float dz = Mathf.Abs(rotations[i].z - rotations[i - 1].z);

                dx = dx > 359 ? 360 - dx : dx;
                dy = dy > 359 ? 360 - dy : dy;
                dz = dz > 359 ? 360 - dz : dz;

                if (dx > threshold || dy > threshold || dz > threshold)
                {
                    datas[i].x = rotations[i].x;
                    datas[i].y = rotations[i].y;
                    datas[i].z = rotations[i].z;
                    datas[i].m = true;

                    datas[i - 1].m = true;
                }
            }
            //datas[length - 1].m = false;

            return datas;
        }

        private static void SlerpQ(List<RotationDatas> datas, int sidx, int cidx, List<Vector3> vectors, SlerpMode mode)
        {
            int length = cidx - sidx;

            Quaternion Z = Quaternion.Euler(datas[sidx].x, datas[sidx].y, datas[sidx].z);
            Quaternion D = Quaternion.Euler(datas[cidx].x, datas[cidx].y, datas[cidx].z);

            for (int i = 1; i < length; i++)
            {
                float weight = GetSlerpWeight(i, length, mode);
                Quaternion Q = Quaternion.Slerp(Z, D, weight);

                Vector3 A = Q.eulerAngles;
                Vector3 B = A;
                Vector3 P = vectors[sidx + i - 1];

                B.x = Mathf.Abs(P.x - A.x) > 180 && P.x > 180 ? A.x + 360 : A.x;
                B.y = Mathf.Abs(P.y - A.y) > 180 && P.y > 180 ? A.y + 360 : A.y;
                B.z = Mathf.Abs(P.z - A.z) > 180 && P.z > 180 ? A.z + 360 : A.z;

                A.x = Mathf.Abs(P.x - B.x) < 180 ? B.x : A.x - 360;
                A.y = Mathf.Abs(P.y - B.y) < 180 ? B.y : A.y - 360;
                A.z = Mathf.Abs(P.z - B.z) < 180 ? B.z : A.z - 360;

                vectors[sidx + i] = A;
            }
        }

        private enum SlerpMode
        {
            Linear,
            Atan
        }

        private static readonly float a = 1.5f;
        private static readonly float b = a * 2;
        private static readonly float c = Mathf.Atan(a) * 2;

        private static float GetSlerpWeight(int index, int length, SlerpMode mode)
        {
            float weight = 1;
            length = Mathf.Max(1, length);

            if (mode == SlerpMode.Linear)
            {
                weight = index / (float)length;
            }
            else if (mode == SlerpMode.Atan)
            {
                float x = b * (index / (float)length - 0.5f);
                float y = Mathf.Atan(x);

                weight = y / c + 0.5f;
            }

            return weight;
        }

        private static void RemoveUnusedDatas(List<RotationDatas> datas, int delayFrames)
        {
            if (datas.Count - delayFrames > 100)
            {
                int length = datas.Count - delayFrames - 100;

                datas.RemoveRange(0, length);
                _rotations.RemoveRange(0, length);
                _smoothes.RemoveRange(0, length);
                _currentFrameIndex -= length;
            }
        }

        private static float GetThreshold(float[] values)
        {
            float threshold = 0.0f;
            float G = 0.0f;

            for (float t = 0.0f; t < 3.0f; t += 0.01f)
            {
                float bu = 0.0f;
                float bw = 0.0f;
                float fu = 0.0f;
                float fw = 0.0f;

                foreach (float v in values)
                {
                    if (v < t)
                    {
                        fu += v;
                        fw += 1;
                    }
                    else
                    {
                        bu += v;
                        bw += 1;
                    }
                }

                fu = fu / Mathf.Max(1, fw);
                fw = fw / values.Length;
                bu = bu / Mathf.Max(1, bw);
                bw = bw / values.Length;

                float g = fw * bw * Mathf.Pow(fu - bu, 2);
                if (g > G)
                {
                    G = g;
                    threshold = t;
                }
            }

            return threshold;
        }

        private const float MINCUTOFF = 1f;

        private static List<Vector3> EuroSmoothQ(List<Vector3> rotations, float beta = 0f, float frequency = 120f)
        {
            List<Vector3> smoothes = new List<Vector3>();
            foreach (Vector3 r in rotations)
            {
                Vector3 s = new Vector3(r.x, r.y, r.z);
                smoothes.Add(s);
            }

            for (int i = 1; i < rotations.Count; i++)
            {
                Vector3 r = rotations[i];
                Vector3 s = smoothes[i - 1];

                float x = GetEuroValue(r.x, s.x, beta, frequency);
                float y = GetEuroValue(r.y, s.y, beta, frequency);
                float z = GetEuroValue(r.z, s.z, beta, frequency);

                smoothes[i] = new Vector3(x, y, z);
            }

            return smoothes;
        }

        private static float GetEuroValue(float a, float b, float beta, float frequency)
        {
            float dcutoff = MINCUTOFF + beta * Mathf.Abs(a - b);
            float tao = 1.0f / (2 * Mathf.PI * dcutoff);
            float alpha = 1.0f / (1 + tao * frequency);

            return alpha * a + (1 - alpha) * b;
        }
    }
}