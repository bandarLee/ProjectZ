//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// �ҽ� �ڵ�� ���� �Ǵ� ��� ������Ʈ���� ����� �� �ֽ��ϴ�.
// �ҽ� �ڵ�� ������ǰų� �Ǹŵ� �� �����ϴ�.
// 
// *** �ҹ� ������ ���� ���� ���� ***
// 
// �� �ڻ��� �ҹ� ���� ����Ʈ���� �޾Ҵٸ�, ����Ƽ �ּ� ������ ������ �ּ���: https://assetstore.unity.com/packages/slug/60955?aid=1011lGnL
// �� �ڻ��� ����Ƽ �ּ� �������� �չ������� ���� �� �ֽ��ϴ�.
// 
// ���� ����, ��õ �ð� ���� �� ����Ʈ����� �ٸ� �ڻ���� �����ϸ� ������ �ξ��ϴ� �ε� �������Դϴ�.
// ���� ������� ����Ʈ��� ���� ���� ����� ����̴� ���� �̸� �������ϴ� ���� �ſ� �����ϰ� �����ϸ� �ε����� ���Դϴ�.
// 
// �����մϴ�.
// 
// *** �ҹ� ������ ���� ���� ���� �� ***
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// ��/�� �ֱ� ������, �Ϸ��� �ð� �� �̵��� ��� �Ϳ� ��ġ�� ������ �����ϴ� ��� �ʵ�� �Ӽ��� ����
    /// </summary>
    [CreateAssetMenu(fileName = "WeatherMakerDayNightCycleProfile", menuName = "WeatherMaker/Day-Night Cycle Profile", order = 41)]
    public class WeatherMakerDayNightCycleProfileScript : ScriptableObject
    {
        #region Ŭ������

        // Ÿ�Ӿƿ� ����� �ִ� �� Ŭ���̾�Ʈ Ŭ����
        private class WebClientWithTimeout : System.Net.WebClient
        {
            protected override System.Net.WebRequest GetWebRequest(Uri uri)
            {
                System.Net.WebRequest w = base.GetWebRequest(uri);
                w.Timeout = Timeout;
                return w;
            }

            /// <summary>
            /// Ÿ�Ӿƿ� �ð� (�и���)
            /// </summary>
            public int Timeout { get; set; }
        }

        /// <summary>
        /// �¾� �˵� ���� Ŭ����
        /// </summary>
        public class SunInfo
        {
            public DateTime DateTime; // ����� ���� ��¥ �� �ð�
            public double Latitude; // �������� ����
            public double Longitude; // �������� �浵
            public double AxisTilt; // �༺�� �� ����
            public Vector3 UnitVectorUp; // �¾� ��ġ ����
            public Vector3 UnitVectorDown; // �¾� ��ġ ������ �ݴ� ����
            public TimeSpan Dawn; // ���� �ð�
            public TimeSpan SunRise; // ���� �ð�
            public TimeSpan SunSet; // �ϸ� �ð�
            public TimeSpan Dusk; // Ȳȥ �ð�
            public double JulianDays; // �����콺 ��¥
            public double Declination; // ����
            public double RightAscension; // ����
            public double Azimuth; // ������
            public double Altitude; // ��
            public double SolarMeanAnomaly; // �¾� ��� �����̰�
            public double EclipticLongitude; // Ȳ��
            public double SiderealTime; // �׼���
        }

        /// <summary>
        /// �� �˵� ���� Ŭ����
        /// </summary>
        public class MoonInfo
        {
            public SunInfo SunData; // �¾� ������ ����
            public Vector3 UnitVectorUp; // �� ��ġ ����
            public Vector3 UnitVectorDown; // �� ��ġ ������ �ݴ� ����
            public double Distance; // �ޱ����� �Ÿ� (ų�ι���)
            public double Phase; // ���� ����
            public double PercentFull; // ���� ����
            public double Angle; // ���� ����
            public double Fraction; // ���� ����
            public double Azimuth; // ������
            public double Altitude; // ��
            public double RightAscension; // ����
            public double Declination; // ����
            public double LunarMeanAnomaly; // �� ��� �����̰�
            public double EclipticLongitude; // Ȳ��
            public double SiderealTime; // �׼���
            public double ParallacticAngle; // ������
        }

        #endregion Ŭ������

        /// <summary>
        /// ��/�� �ֱ��� �ӵ�. 1�� �ǽð�, 100�� 100�� ������, ���� ���� ���������� ����.
        /// </summary>
        [Header("Day/night cycle")]
        [Range(-100000, 100000.0f)]
        [Tooltip("�ֱ��� �� �ӵ�. 0���� �����ϸ� �ֱⰡ ���߰� �������� ������ �� �ֽ��ϴ�. �ӵ��� 1�̸� �ֱⰡ �ǽð����� ����˴ϴ�. " +
            "�ӵ��� 100�̸� ���� �ӵ��� 100��� ����˴ϴ�. ������ �ֱⰡ ���������� ����˴ϴ�.")]
        public float Speed = 10.0f;

        /// <summary>
        /// �� �ֱ��� �ӵ�. �� �ӵ��� ������ ������� �۵�.
        /// </summary>
        [Range(-100000, 100000.0f)]
        [Tooltip("�ֱ��� �� �ӵ�. 0���� �����ϸ� �ֱⰡ ���߰� �������� ������ �� �ֽ��ϴ�. �ӵ��� 1�̸� �ֱⰡ �ǽð����� ����˴ϴ�. " +
            "�ӵ��� 100�̸� ���� �ӵ��� 100��� ����˴ϴ�. ������ �ֱⰡ ���������� ����˴ϴ�.")]
        public float NightSpeed = 10.0f;

        /// <summary>
        /// �ֱⰡ ������Ʈ�Ǵ� ��. ���� ���� �׸��� �������� ���̴� �� ������ �� �� �ֽ��ϴ�.
        /// </summary>
        [Tooltip("�ֱⰡ ������Ʈ�Ǵ� ��. ���� ���� ����ϸ� �׸��� ������ ������ �پ�� �� �ֽ��ϴ�. " +
            "�Ͻ��� ��Ƽ�ٸ������ �Ѵ� �͵� �������� ���̴� ���� ��������� ������ �����ؾ� �� �� �ֽ��ϴ�.")]
        [Range(0.0f, 10.0f)]
        public float UpdateInterval = 0.03f;

        private float accumulatedTime = 10.0f; // ������Ʈ�� ���� ���� �ð�

        /// <summary>
        /// ���� �Ϸ��� �ð� (��). 0�� ����, 43200�� ����, 86400�� ���� �� ����.
        /// </summary>
        [Range(0.0f, SecondsPerDay)]
        [Tooltip("���� �Ϸ��� �ð� (��).")]
        public float TimeOfDay = SecondsPerDay * 0.5f; // �⺻���� ����

#if UNITY_EDITOR
#pragma warning disable 0414

        [ReadOnlyLabel]
        [SerializeField]
        internal string TimeOfDayLabel = string.Empty;

#pragma warning restore 0414
#endif

        [Header("Date")]
        [Tooltip("�¾�� ���� ��ġ�� �ùķ��̼��ϴ� ���� - �� ���� ��Ÿ�� �� ����� �� �ֽ��ϴ�. " +
            "����� 1900�� 3�� 1�Ϻ��� 2100�� 2�� 28�ϱ����� ��¥�� ���ؼ��� ��Ȯ�մϴ�.")]
        [Range(1900, 2100)]
        public int Year = 2000;

        [Tooltip("�¾�� ���� ��ġ�� �ùķ��̼��ϴ� �� - �� ���� ��Ÿ�� �� ����� �� �ֽ��ϴ�.")]
        [Range(1, 12)]
        public int Month = 9;

        [Tooltip("�¾�� ���� ��ġ�� �ùķ��̼��ϴ� �� - �� ���� ��Ÿ�� �� ����� �� �ֽ��ϴ�.")]
        [Range(1, 31)]
        public int Day = 21;

        [Tooltip("�Ϸ簡 ���� �� ��¥�� �������� ����. ���� ���۵ǰ� ���� �� ��Ȯ�� �¾� �� �� ��ġ�� �����ϴ� �� �߿�������, �ð��� �����̸� �� �� �ֽ��ϴ�.")]
        public bool AdjustDateWhenDayEnds = true;

        [Tooltip("����/�浵�� ���� �ð��� ������ (��). -1111�� �����ϸ� �ڵ����� ���˴ϴ� (�Է¶��� -1111�� �Է��� �� ��Ű�� ��������). -1111�� ���� ����: ������ ��忡���� �� ���񽺸� ����մϴ�. �÷��� ��忡���� �浵�� ����Ͽ� ������ ����մϴ�.")]
        public int TimeZoneOffsetSeconds = -21600;

        [Header("Location")]
        [Range(-90.0f, 90.0f)]
        [Tooltip("ī�޶� �ִ� �༺�� ���� (��) - �ϱ� 90���� ���� -90����")]
        public double Latitude = 40.7608; // ��Ʈ ����ũ ��Ƽ�� ����

        [Range(-180.0f, 180.0f)]
        [Tooltip("ī�޶� �ִ� �༺�� �浵 (��). -180���� 180����.")]
        public double Longitude = -111.8910; // ��Ʈ ����ũ ��Ƽ�� �浵

        [Range(0.0f, 360.0f)]
        [Tooltip("�༺�� ���� (��) - ������ �� 23.439��")]
        public float AxisTilt = 23.439f;

        [Header("Time of Day Mapping")]
        [Tooltip("��, ����/Ȳȥ, ���� �����ϴ� �׶���Ʈ. �߽��� �¾��� ���򼱿� �ִ� ��ġ. ��� = ��, ���� = ����/Ȳȥ, �Ķ� = ��. ����/Ȳȥ�� ��� ���� 12�� ������ ����, ���� 12�� ���Ĵ� Ȳȥ.")]
        public Gradient DayDawnDuskNightGradient;

        [Tooltip("�ϴ��� ������ �����ϴ� �׶���Ʈ, �߽��� �¾��� ���򼱿� �ִ� ��ġ.")]
        public Gradient SkyTintColor;

        [Tooltip("�ϴÿ� �߰��ϴ� ���� �����ϴ� �׶���Ʈ, �߽��� �¾��� ���򼱿� �ִ� ��ġ.")]
        public Gradient SkyAddColor;

        [Header("Ambient colors")]
        [Tooltip("Unity�� ����� �ں��Ʈ ������ ��, ����/Ȳȥ, �� �ں��Ʈ �������� �������� ����.")]
        public WeatherMakerDayNightAmbientColorMode AmbientColorMode = WeatherMakerDayNightAmbientColorMode.AmbientColor;

        [Header("Ambient colors - day")]
        [Tooltip("�� �ں��Ʈ ����. ���� �������� ������ �� - ���� ���� ������ ���.")]
        public Gradient DayAmbientColor;

        [Tooltip("�߰� �� �ں��Ʈ ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ �ϴ� ����. ���� �������� ������ �� - ���� ���� ������ ���.")]
        public Gradient DayAmbientColorSky;

        [Tooltip("�߰� �� �ں��Ʈ �ϴ� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorSkyIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ ���� ����. ���� �������� ������ �� - ���� ���� ������ ���.")]
        public Gradient DayAmbientColorGround;

        [Tooltip("�߰� �� �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorGroundIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ ���� ����. ���� �������� ������ �� - ���� ���� ������ ���.")]
        public Gradient DayAmbientColorEquator;

        [Tooltip("�߰� �� �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorEquatorIntensity = 0.05f;

        [Header("Ambient colors - dawn/dusk")]
        [Tooltip("����/Ȳȥ �ں��Ʈ ����. ���� �������� ������ ���� �Ǵ� Ȳȥ - ���� ���� ������ ���.")]
        public Gradient DawnDuskAmbientColor;

        [Tooltip("�߰� ����/Ȳȥ �ں��Ʈ ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorIntensity = 0.05f;

        [Tooltip("����/Ȳȥ �ں��Ʈ �ϴ� ����. ���� �������� ������ ���� �Ǵ� Ȳȥ - ���� ���� ������ ���.")]
        public Gradient DawnDuskAmbientColorSky;

        [Tooltip("�߰� ����/Ȳȥ �ں��Ʈ �ϴ� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorSkyIntensity = 0.05f;

        [Tooltip("����/Ȳȥ �ں��Ʈ ���� ����. ���� �������� ������ ���� �Ǵ� Ȳȥ - ���� ���� ������ ���.")]
        public Gradient DawnDuskAmbientColorGround;

        [Tooltip("�߰� ����/Ȳȥ �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorGroundIntensity = 0.05f;

        [Tooltip("����/Ȳȥ �ں��Ʈ ���� ����. ���� �������� ������ ���� �Ǵ� Ȳȥ - ���� ���� ������ ���.")]
        public Gradient DawnDuskAmbientColorEquator;

        [Tooltip("�߰� ����/Ȳȥ �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorEquatorIntensity = 0.05f;

        [Header("Ambient colors - night")]
        [Tooltip("�� �ں��Ʈ ����. ���� �������� ������ �� - ���� ���� ������ ���.")]
        public Gradient NightAmbientColor;

        [Tooltip("�߰� �� �ں��Ʈ ���� ����")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ �ϴ� ����. ���� �������� ������ �� - ���� ���� ������ ���.")]
        public Gradient NightAmbientColorSky;

        [Tooltip("�߰� �� �ں��Ʈ �ϴ� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorSkyIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ ���� ����. ���� �������� ������ �� - ���� ���� ������ ���.")]
        public Gradient NightAmbientColorGround;

        [Tooltip("�߰� �� �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorGroundIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ ���� ����. ���� �������� ������ �� - ���� ���� ������ ���.")]
        public Gradient NightAmbientColorEquator;

        [Tooltip("�߰� �� �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorEquatorIntensity = 0.05f;

        [Header("Sun modifiers")]
        [Tooltip("�¾��� ������ �����ϴ� �׶���Ʈ, �߽��� �¾��� ���򼱿� �ִ� ��ġ.")]
        public Gradient SunTintColorGradient;

        [Tooltip("�¾� ������ �����ϴ� �׶���Ʈ, �߽��� �¾��� ���򼱿� �ִ� ��ġ.")]
        public Gradient SunIntensityGradient;

        /// <summary>
        /// �Ϸ��� �ð��� TimeSpan ��ü�� ��ȯ
        /// </summary>
        public TimeSpan TimeOfDayTimespan { get; private set; }

        /// <summary>
        /// �Ϸ��� �ð� ����
        /// </summary>
        public WeatherMakerTimeOfDayCategory TimeOfDayCategory { get; private set; }

        /// <summary>
        /// ������ ���� ��� 1
        /// </summary>
        public float DayMultiplier { get; private set; }

        /// <summary>
        /// ������ ���� �Ǵ� Ȳȥ�� ��� 1
        /// </summary>
        public float DawnDuskMultiplier { get; private set; }

        /// <summary>
        /// ������ ���� ��� 1
        /// </summary>
        public float NightMultiplier { get; private set; }

        /// <summary>
        /// ���� �¾� ����
        /// </summary>
        [NonSerialized]
        public readonly SunInfo SunData = new SunInfo();

        /// <summary>
        /// ���� �� ����
        /// </summary>
        [NonSerialized]
        public readonly List<MoonInfo> MoonDatas = new List<MoonInfo>();

        /// <summary>
        /// �Ϸ��� �� �� ��
        /// </summary>
        public const float SecondsPerDay = 86400.0f;

        /// <summary>
        /// ���� �ð�
        /// </summary>
        public const float HighNoonTimeOfDay = SecondsPerDay * 0.5f;

        /// <summary>
        /// 1���� �� ��
        /// </summary>
        public const float SecondsForOneDegree = SecondsPerDay / 360.0f;

        /// <summary>
        /// ���� GI ������Ʈ�� ���� ����ؾ� �ϴ� �ð� (��)
        /// </summary>
        public const float DynamicGIUpdateThresholdSeconds = 300.0f;

        private DateTime prevDt;
        /// <summary>
        /// ���� ����, ��, ��, �ð��� ���� �ð��� ��Ÿ���� DateTime ��ü ��ȯ
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                TimeSpan ts = TimeOfDayTimeSpan;
                return new DateTime(Year, Month, Day, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds, DateTimeKind.Local);
            }
            set
            {
                DateTime dt = value.ToLocalTime();
                Year = dt.Year;
                Month = dt.Month;
                Day = dt.Day;
                TimeOfDay = (float)dt.TimeOfDay.TotalSeconds;
            }
        }

        /// <summary>
        /// TimeOfDay �Ӽ����� TimeSpan�� ��ȯ
        /// </summary>
        public TimeSpan TimeOfDayTimeSpan { get { return TimeSpan.FromSeconds(TimeOfDay); } set { TimeOfDay = (float)value.TotalSeconds; } }

        private float lastTimeOfDayForDynamicGIUpdate = -999999.0f;

        /// <summary>
        /// �����콺 ��¥�� System.DateTime���� ��ȯ
        /// </summary>
        /// <param name="julianDate">�����콺 ��¥</param>
        /// <returns>UTC ������ DateTime ��ü</returns>
        public static DateTime JulianToDateTime(double julianDate)
        {
            double unixTime = (julianDate - 2440587.5) * 86400;
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTime);
            return dtDateTime;
        }

        /// <summary>
        /// �������� ���� ���� ���ͷ� ��ȯ
        /// </summary>
        /// <param name="azimuth">������</param>
        /// <param name="altitude">��</param>
        /// <param name="vector">���� ����</param>
        public static void ConvertAzimuthAtltitudeToUnitVector(double azimuth, double altitude, ref Vector3 vector)
        {
            vector.y = (float)Math.Sin(altitude);
            float hyp = (float)Math.Cos(altitude);
            vector.z = hyp * (float)Math.Cos(azimuth);
            vector.x = hyp * (float)Math.Sin(azimuth);
        }

        /// <summary>
        /// �¾��� ��ġ�� ���
        /// </summary>
        /// <param name="sunInfo">���� �¾� ������ ����</param>
        /// <param name="rotateYDegrees">Y�� ������ ȸ���� ����</param>
        public static void CalculateSunPosition(SunInfo sunInfo, float rotateYDegrees)
        {
            // dateTime�� �̹� UTC �����̾�� ��
            double d = (sunInfo.DateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / dayMs) + jDiff;
            double e = degreesToRadians * sunInfo.AxisTilt; // ������ ����
            double m = SolarMeanAnomaly(d);
            double l = EclipticLongitude(m);
            double dec = Declination(e, l, 0);
            double ra = RightAscension(e, l, 0);
            double lw = -degreesToRadians * sunInfo.Longitude;
            double phi = degreesToRadians * sunInfo.Latitude;
            double h = SiderealTime(d, lw) - ra;
            double azimuth = Azimuth(h, phi, dec);
            double altitude = Altitude(h, phi, dec);
            ConvertAzimuthAtltitudeToUnitVector(azimuth, altitude, ref sunInfo.UnitVectorUp);

            sunInfo.UnitVectorUp = Quaternion.AngleAxis(rotateYDegrees, Vector3.up) * sunInfo.UnitVectorUp;
            sunInfo.UnitVectorDown = -sunInfo.UnitVectorUp;
            sunInfo.JulianDays = d;
            sunInfo.Declination = dec;
            sunInfo.RightAscension = ra;
            sunInfo.Azimuth = azimuth;
            sunInfo.Altitude = altitude;
            sunInfo.SolarMeanAnomaly = m;
            sunInfo.EclipticLongitude = l;
            sunInfo.SiderealTime = h;

            double n = JulianCycle(d, lw);
            double ds = ApproxTransit(0, lw, n);
            double jnoon = SolarTransit(ds, m, l);
            double jSunSet = JulianDateForSunAltitude(-0.8 * (Math.PI / 180.0), lw, phi, dec, n, m, l);
            double jSunRise = jnoon - (jSunSet - jnoon);
            double jDusk = JulianDateForSunAltitude(-10.0 * (Math.PI / 180.0), lw, phi, dec, n, m, l);
            double jDawn = jnoon - (jDusk - jnoon);

            try
            {
                sunInfo.Dawn = JulianToDateTime(jDawn).TimeOfDay;
                sunInfo.Dusk = JulianToDateTime(jDusk).TimeOfDay;
                sunInfo.SunRise = JulianToDateTime(jSunRise).TimeOfDay;
                sunInfo.SunSet = JulianToDateTime(jSunSet).TimeOfDay;
            }
            catch
            {
                // ��¥ �� �ð� ������ �ʰ��� ��� ũ���� ����
            }
        }

        /// <summary>
        /// ���� ��ġ�� ���
        /// </summary>
        /// <param name="sunInfo">���� �¾� ����</param>
        /// <param name="moonInfo">���� �� ������ ����</param>
        /// <param name="rotateYDegrees">���� y�� ������ ȸ���� ����</param>
        public static void CalculateMoonPosition(SunInfo sunInfo, MoonInfo moonInfo, float rotateYDegrees)
        {
            double d = sunInfo.JulianDays;
            double e = degreesToRadians * sunInfo.AxisTilt; // ������ ����
            double L = degreesToRadians * (218.316 + 13.176396 * d); // Ȳ��
            double M = degreesToRadians * (134.963 + 13.064993 * d); // ��� �����̰�
            double F = degreesToRadians * (93.272 + 13.229350 * d); // ��� �Ÿ�
            double l = L + degreesToRadians * 6.289 * Math.Sin(M); // �浵
            double b = degreesToRadians * 5.128 * Math.Sin(F); // ����
            double dist = 385001.0 - (20905.0 * Math.Cos(M)); // �ޱ����� �Ÿ� (ų�ι���)
            double ra = RightAscension(e, l, b);
            double dec = Declination(e, l, b);
            const double sunDistance = 149598000.0; // ������ �¾� ������ ��� �Ÿ�
            double phi = Math.Acos(Math.Sin(sunInfo.Declination) * Math.Sin(dec) + Math.Cos(sunInfo.Declination) * Math.Cos(dec) * Math.Cos(sunInfo.RightAscension - ra));
            double inc = Math.Atan2(sunDistance * Math.Sin(phi), dist - sunDistance * Math.Cos(phi));
            double angle = Math.Atan2(Math.Cos(sunInfo.Declination) * Math.Sin(sunInfo.RightAscension - ra), Math.Sin(sunInfo.Declination) * Math.Cos(dec) - Math.Cos(sunInfo.Declination) * Math.Sin(dec) * Math.Cos(sunInfo.RightAscension - ra));
            double fraction = (1.0 + Math.Cos(inc)) * 0.5;
            double phase = 0.5 + (0.5 * inc * Math.Sign(angle) * (1.0 / Math.PI));
            double lw = -degreesToRadians * sunInfo.Longitude;
            phi = degreesToRadians * sunInfo.Latitude;
            double H = SiderealTime(d, lw) - ra;
            double h = Altitude(H, phi, dec);

            // "Astronomical Algorithms" 2nd edition by Jean Meeus (Willmann-Bell, Richmond) 1998�� 14.1 ����.
            double pa = Math.Atan2(Math.Sin(H), Math.Tan(phi) * Math.Cos(dec) - Math.Sin(dec) * Math.Cos(H));
            h = h + AstroRefraction(h); // ������ ���� �� ����
            double azimuth = Azimuth(H, phi, dec);
            double altitude = h;
            ConvertAzimuthAtltitudeToUnitVector(azimuth, altitude, ref moonInfo.UnitVectorUp);

            // ���� ��ġ ���� �� ������ �ٶ󺸵��� ����
            moonInfo.UnitVectorUp = Quaternion.AngleAxis(rotateYDegrees, Vector3.up) * moonInfo.UnitVectorUp;
            moonInfo.UnitVectorDown = -moonInfo.UnitVectorUp;
            moonInfo.Distance = dist;
            moonInfo.Phase = phase;
            moonInfo.PercentFull = 1.0 - Math.Abs((0.5 - phase) * 2.0);
            moonInfo.Angle = angle;
            moonInfo.Fraction = fraction;
            moonInfo.Azimuth = azimuth;
            moonInfo.Altitude = altitude;
            moonInfo.RightAscension = ra;
            moonInfo.Declination = dec;
            moonInfo.LunarMeanAnomaly = M;
            moonInfo.EclipticLongitude = L;
            moonInfo.SiderealTime = H;
            moonInfo.ParallacticAngle = pa;
        }

        private const double degreesToRadians = Math.PI / 180.0;
        private const double dayMs = 1000.0 * 60.0 * 60.0 * 24.0;
        private const double j0 = 0.0009;
        private const double j1970 = 2440587.5;
        private const double j2000 = 2451545.0;
        private const double jDiff = (j1970 - j2000);

        private static double RightAscension(double e, double l, double b)
        {
            return Math.Atan2(Math.Sin(l) * Math.Cos(e) - Math.Tan(b) * Math.Sin(e), Math.Cos(l));
        }

        private static double Declination(double e, double l, double b)
        {
            return Math.Asin(Math.Sin(b) * Math.Cos(e) + Math.Cos(b) * Math.Sin(e) * Math.Sin(l));
        }

        private static double Azimuth(double h, double phi, double dec)
        {
            return Math.Atan2(Math.Sin(h), Math.Cos(h) * Math.Sin(phi) - Math.Tan(dec) * Math.Cos(phi));
        }

        private static double Altitude(double h, double phi, double dec)
        {
            return Math.Asin(Math.Sin(phi) * Math.Sin(dec) + Math.Cos(phi) * Math.Cos(dec) * Math.Cos(h));
        }

        private static double SiderealTime(double d, double lw)
        {
            return degreesToRadians * (280.16 + 360.9856235 * d) - lw;
        }

        private static double SolarMeanAnomaly(double d)
        {
            return degreesToRadians * (357.5291 + 0.98560028 * d);
        }

        private static double EclipticLongitude(double m)
        {
            double c = degreesToRadians * (1.9148 * Math.Sin(m) + 0.02 * Math.Sin(2.0 * m) + 0.0003 * Math.Sin(3.0 * m)); // �߽� ������
            double p = degreesToRadians * 102.9372; // ������ ������
            return m + c + p + Math.PI;
        }

        private static double AstroRefraction(double h)
        {
            // �� ������ ���� ���� ���ؼ��� �۵��մϴ�.
            // h = -0.08901179�̸� div/0�� �߻��մϴ�.
            h = (h < 0.0 ? 0.0 : h);

            // "Astronomical Algorithms" 2nd edition by Jean Meeus (Willmann-Bell, Richmond) 1998�� 16.4 ����.
            // 1.02 / tan(h + 10.26 / (h + 5.10)) h�� ��, ����� ȣ �� -> �������� ��ȯ:
            return 0.0002967 / Math.Tan(h + 0.00312536 / (h + 0.08901179));
        }

        private static double JulianCycle(double d, double lw)
        {
            return Math.Round(d - j0 - lw / (2 * Math.PI));
        }

        private static double ApproxTransit(double Ht, double lw, double n)
        {
            return j0 + (Ht + lw) / (2 * Math.PI) + n;
        }

        private static double SolarTransit(double ds, double M, double L)
        {
            return j2000 + ds + 0.0053 * Math.Sin(M) - 0.0069 * Math.Sin(2 * L);
        }

        private static double HourAngle(double h, double phi, double d)
        {
            return Math.Acos((Math.Sin(h) - Math.Sin(phi) * Math.Sin(d)) / (Math.Cos(phi) * Math.Cos(d)));
        }

        private static double JulianDateForSunAltitude(double h, double lw, double phi, double dec, double n, double M, double L)
        {
            double w = HourAngle(h, phi, dec);
            double a = ApproxTransit(w, lw, n);
            return SolarTransit(a, M, L);
        }

        private static double CorrectAngle(double angleInRadians)
        {
            if (angleInRadians < 0)
            {
                return (2 * Math.PI) + angleInRadians;
            }
            else if (angleInRadians > 2 * Math.PI)
            {
                return angleInRadians - (2 * Math.PI);
            }
            else
            {
                return angleInRadians;
            }
        }

#if UNITY_EDITOR

        private DateTime lastTimeZoneCheck = DateTime.MinValue;

#endif

        private void SetCelestialObjectPosition(WeatherMakerCelestialObjectScript obj, Vector3 transformForward)
        {
            if (obj.OrbitType == WeatherMakerOrbitType.CustomOrthographic || obj.OrbitType == WeatherMakerOrbitType.CustomPerspective)
            {
                return;
            }
            obj.transform.forward = (transformForward == Vector3.zero ? obj.transform.forward : transformForward);
        }

        private void UpdateSuns(CameraMode mode, IList<WeatherMakerCelestialObjectScript> suns)
        {
            foreach (WeatherMakerCelestialObjectScript sun in suns)
            {
                if (sun == null)
                {
                    return;
                }
                else if (mode == CameraMode.OrthographicXY && !sun.OrbitTypeIsPerspective)
                {
                    if (!sun.OrbitTypeIsCustom)
                    {
                        sun.transform.rotation = Quaternion.AngleAxis(180.0f + ((TimeOfDay / SecondsPerDay) * 360.0f), Vector3.right);
                    }
                    if (sun.LightMode != WeatherMakerCelestialObjectLightMode.None)
                    {
                        float dot = Mathf.Clamp(Vector3.Dot(sun.transform.forward, Vector3.forward) + 0.5f, 0.0f, 1.0f);
                        sun.Light.intensity = sun.LightBaseIntensity * dot;
                    }
                }
                else if (mode == CameraMode.OrthographicXZ && !sun.OrbitTypeIsPerspective)
                {
                    if (!sun.OrbitTypeIsCustom)
                    {
                        sun.transform.rotation = Quaternion.AngleAxis(180.0f + ((TimeOfDay / SecondsPerDay) * 360.0f), Vector3.forward);
                    }
                    if (sun.LightMode != WeatherMakerCelestialObjectLightMode.None)
                    {
                        float dot = Mathf.Clamp(Vector3.Dot(sun.transform.forward, Vector3.up) + 0.5f, 0.0f, 1.0f);
                        sun.Light.intensity = sun.LightBaseIntensity * dot;
                    }
                }
                else
                {
                    // ���� �ð��� �Ϸ� �ð��� UTC �ð����� ��ȯ - ������ ������ ���
                    double offsetSeconds = TimeZoneOffsetSeconds;
                    TimeSpan t = TimeSpan.FromSeconds(TimeOfDay - offsetSeconds);
                    SunData.DateTime = new DateTime(Year, Month, Day, 0, 0, 0, DateTimeKind.Utc) + t;
                    SunData.Latitude = Latitude;
                    SunData.Longitude = Longitude;
                    SunData.AxisTilt = AxisTilt;

                    // �¾��� ��ġ�� ����ϰ� ����
                    if (sun.OrbitType == WeatherMakerOrbitType.FromEarth)
                    {
                        CalculateSunPosition(SunData, sun.RotateYDegrees);
                    }
                    SetCelestialObjectPosition(sun, SunData.UnitVectorDown);

                    // �¾� ���� �� �׸��� ������ ���
                    float l = sun.GetGradientLookup();
                    float sunIntensityLookup = SunIntensityGradient.Evaluate(l).grayscale;
                    sunIntensityLookup *= sunIntensityLookup;
                    sun.Light.color = sun.LightColor * SunTintColorGradient.Evaluate(l);
                    sun.Light.shadowStrength = sun.LightBaseShadowStrength;
                    if (sun.LightMode != WeatherMakerCelestialObjectLightMode.None)
                    {
                        sun.Light.intensity = sun.LightBaseIntensity * sunIntensityLookup;
                    }
                }
            }
        }

        private void UpdateMoons(CameraMode mode, WeatherMakerCelestialObjectScript sun, IList<WeatherMakerCelestialObjectScript> moons)
        {
            float dot, yPower;
            float sunIntensityReducer = (sun == null ? 1.0f : 1.0f - Mathf.Min(1.0f, Mathf.Pow(sun.Light.intensity, 0.2f)));
            while (MoonDatas.Count > moons.Count)
            {
                MoonDatas.RemoveAt(MoonDatas.Count - 1);
            }
            while (MoonDatas.Count < moons.Count)
            {
                MoonDatas.Add(new MoonInfo());
            }

            for (int i = 0; i < moons.Count; i++)
            {
                WeatherMakerCelestialObjectScript moon = moons[i];
                if (moon == null)
                {
                    continue;
                }
                else if (mode == CameraMode.OrthographicXY && !moon.OrbitTypeIsPerspective)
                {
                    moon.transform.rotation = Quaternion.AngleAxis(((TimeOfDay / SecondsPerDay) * 360.0f), Vector3.right);
                    dot = Mathf.Clamp(Vector3.Dot(moon.transform.forward, Vector3.forward) + 0.5f, 0.0f, 1.0f);
                }
                else if (mode == CameraMode.OrthographicXZ && !moon.OrbitTypeIsPerspective)
                {
                    moon.transform.rotation = Quaternion.AngleAxis(((TimeOfDay / SecondsPerDay) * 360.0f), Vector3.forward);
                    dot = Mathf.Clamp(Vector3.Dot(moon.transform.forward, Vector3.up) + 0.5f, 0.0f, 1.0f);
                }
                else if (moon.OrbitType == WeatherMakerOrbitType.FromEarth)
                {
                    CalculateMoonPosition(SunData, MoonDatas[i], moon.RotateYDegrees);
                }

                if (!moon.OrbitTypeIsCustom)
                {
                    SetCelestialObjectPosition(moon, MoonDatas[i].UnitVectorDown);
                }

                // ������ ������ ����Ͽ� �������� ���� - �̴� �� ä���� ��� ������ŭ�� �� ���Ҹ� �ǹ�
                // �¾� ������ 1�� ����������� ���� �� ���� ����
                // ���� �Ʒ��� ������ �� �� �� ����
                dot = Mathf.Clamp(Vector3.Dot(MoonDatas[i].UnitVectorDown, Vector3.down) + 0.2f, 0.0f, 1.0f);
                dot = Mathf.Pow(dot, 0.25f);
                yPower = Mathf.Clamp((MoonDatas[i].UnitVectorUp.y + 0.2f) * 4.0f, 0.0f, 1.0f);
                moon.Light.color = moon.LightColor;
                moon.Light.intensity = moon.LightBaseIntensity * yPower * sunIntensityReducer * dot * moon.LightMultiplier;
                if (moon.OrbitType == WeatherMakerOrbitType.FromEarth)
                {
                    // �������� �־������� ������ �������� ����
                    moon.Light.intensity *= ((float)MoonDatas[i].PercentFull * (float)MoonDatas[i].PercentFull);
                }
                moon.Light.shadowStrength = moon.LightBaseShadowStrength;
            }
        }

        private void ProcessCelestialObject(WeatherMakerCelestialObjectScript obj,
            Dictionary<string, float> directionalLightIntensityModifiers,
            Dictionary<string, float> directionalLightShadowModifier)
        {
            if (obj != null)
            {
                foreach (KeyValuePair<string, float> multiplier in directionalLightIntensityModifiers)
                {
                    obj.Light.intensity *= multiplier.Value;
                }
                foreach (KeyValuePair<string, float> multiplier in directionalLightShadowModifier)
                {
                    obj.Light.shadowStrength *= multiplier.Value;
                }
                obj.UpdateObject();
            }
        }

        private void UpdateLightIntensitiesAndShadows(WeatherMakerCelestialObjectScript sun, ICollection<WeatherMakerCelestialObjectScript> moons,
            Dictionary<string, float> directionalLightIntensityModifiers,
            Dictionary<string, float> directionalLightShadowModifier)
        {
            ProcessCelestialObject(sun, directionalLightIntensityModifiers, directionalLightShadowModifier);
            foreach (WeatherMakerCelestialObjectScript obj in moons)
            {
                ProcessCelestialObject(obj, directionalLightIntensityModifiers, directionalLightShadowModifier);
            }
        }

        private void UpdateDayMultipliers(CameraMode mode, WeatherMakerCelestialObjectScript sun, float l)
        {
            Color lookup = DayDawnDuskNightGradient.Evaluate(l);
            DayMultiplier = lookup.g;
            DawnDuskMultiplier = lookup.r;
            NightMultiplier = lookup.b;
            WeatherMakerTimeOfDayCategory current = TimeOfDayCategory;
            TimeOfDayCategory = WeatherMakerTimeOfDayCategory.None;
            if (DayMultiplier > 0.0f)
            {
                TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Day;
            }
            if (DawnDuskMultiplier > 0.0f)
            {
                if (TimeOfDay > (SecondsPerDay * 0.5))
                {
                    TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Dusk;
                }
                else
                {
                    TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Dawn;
                }
            }
            if (NightMultiplier > 0.0f)
            {
                TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Night;
            }

            if (mode == CameraMode.OrthographicXY)
            {
                float z = sun.transform.forward.z;
                if (z >= -0.1f && z <= 0.1f)
                {
                    if (TimeOfDay > (SecondsPerDay * 0.5f))
                    {
                        TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Sunset;
                    }
                    else
                    {
                        TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Sunrise;
                    }
                }
            }
            else if (mode == CameraMode.OrthographicXZ)
            {
                float x = sun.transform.forward.x;
                if (x >= -0.1f && x <= 0.1f)
                {
                    if (TimeOfDay > (SecondsPerDay * 0.5f))
                    {
                        TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Sunset;
                    }
                    else
                    {
                        TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Sunrise;
                    }
                }
            }
            else
            {
                float y = sun.transform.forward.y;
                if (y >= -0.05f && y <= 0.2f)
                {
                    if (TimeOfDay > (SecondsPerDay * 0.5f))
                    {
                        TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Sunset;
                    }
                    else
                    {
                        TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Sunrise;
                    }
                }
            }

            // �̺�Ʈ ����
            if ((current & WeatherMakerTimeOfDayCategory.Day) == WeatherMakerTimeOfDayCategory.None && (TimeOfDayCategory & WeatherMakerTimeOfDayCategory.Day) != WeatherMakerTimeOfDayCategory.None)
            {
                WeatherMakerScript.Instance.DayBegin.Invoke(this);
            }
            if ((current & WeatherMakerTimeOfDayCategory.Night) == WeatherMakerTimeOfDayCategory.None && (TimeOfDayCategory & WeatherMakerTimeOfDayCategory.Night) != WeatherMakerTimeOfDayCategory.None)
            {
                WeatherMakerScript.Instance.NightBegin.Invoke(this);
            }
            if ((current & WeatherMakerTimeOfDayCategory.Dawn) == WeatherMakerTimeOfDayCategory.None && (TimeOfDayCategory & WeatherMakerTimeOfDayCategory.Dawn) != WeatherMakerTimeOfDayCategory.None)
            {
                WeatherMakerScript.Instance.DawnBegin.Invoke(this);
            }
            if ((current & WeatherMakerTimeOfDayCategory.Dusk) == WeatherMakerTimeOfDayCategory.None && (TimeOfDayCategory & WeatherMakerTimeOfDayCategory.Dusk) != WeatherMakerTimeOfDayCategory.None)
            {
                WeatherMakerScript.Instance.DuskBegin.Invoke(this);
            }
            if ((current & WeatherMakerTimeOfDayCategory.Sunrise) == WeatherMakerTimeOfDayCategory.None && (TimeOfDayCategory & WeatherMakerTimeOfDayCategory.Sunrise) != WeatherMakerTimeOfDayCategory.None)
            {
                WeatherMakerScript.Instance.SunriseBegin.Invoke(this);
            }
            if ((current & WeatherMakerTimeOfDayCategory.Sunrise) != WeatherMakerTimeOfDayCategory.None && (TimeOfDayCategory & WeatherMakerTimeOfDayCategory.Sunrise) == WeatherMakerTimeOfDayCategory.None)
            {
                WeatherMakerScript.Instance.SunriseEnd.Invoke(this);
            }
            if ((current & WeatherMakerTimeOfDayCategory.Sunset) == WeatherMakerTimeOfDayCategory.None && (TimeOfDayCategory & WeatherMakerTimeOfDayCategory.Sunset) != WeatherMakerTimeOfDayCategory.None)
            {
                WeatherMakerScript.Instance.SunsetBegin.Invoke(this);
            }
            if ((current & WeatherMakerTimeOfDayCategory.Sunset) != WeatherMakerTimeOfDayCategory.None && (TimeOfDayCategory & WeatherMakerTimeOfDayCategory.Sunset) == WeatherMakerTimeOfDayCategory.None)
            {
                WeatherMakerScript.Instance.SunsetEnd.Invoke(this);
            }
        }

        private void UpdateTimeZone()
        {
            if (TimeZoneOffsetSeconds == -1111)
            {
                TimeZoneOffsetSeconds = (int)(Longitude * 24 / 360) * 3600;

#if UNITY_EDITOR

                if ((DateTime.UtcNow - lastTimeZoneCheck).TotalSeconds > 10.0)
                {
                    lastTimeZoneCheck = DateTime.UtcNow;
                    WebClientWithTimeout c = new WebClientWithTimeout();
                    c.Timeout = 3000;
                    TimeSpan unixTimeSpan = new DateTime(Year, Month, Day, 1, 1, 1, DateTimeKind.Utc) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    string url = "http://api.timezonedb.com/v2/get-time-zone?by=position&lat=" + Latitude + "&lng=" + Longitude + "&time=" + (long)unixTimeSpan.TotalSeconds + "&key=1H9B390ZKKPX";
                    try
                    {
                        c.DownloadStringCompleted += (o, e) =>
                        {
                            string xml = e.Result;
                            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(xml, @"\<gmtOffset\>(?<gmtOffset>.*?)\</gmtOffset\>");
                            if (m.Success)
                            {
                                TimeZoneOffsetSeconds = int.Parse(m.Groups["gmtOffset"].Value);
                                WeatherMakerScript.QueueOnMainThread(() =>
                                {
                                    SerializationHelper.SetDirty(this);
                                });
                            }
                        };
                        c.DownloadStringAsync(new System.Uri(url));
                    }
                    catch
                    {
                        // ���� ����
                    }
                }

#endif

            }
        }

        private void UpdateTimeOfDay(bool updateTimeOfDay)
        {
            UpdateTimeZone();
            prevDt = DateTime;

#if UNITY_EDITOR

            TimeOfDayLabel = DateTime.Today.Add(TimeSpan.FromSeconds(TimeOfDay)).ToString("hh:mm tt");

#endif

            if (!updateTimeOfDay || !Application.isPlaying)
            {
                return;
            }

            if (NightMultiplier != 1.0f && Speed != 0.0f)
            {
                TimeOfDay += (Speed * accumulatedTime);
            }
            else if (NightMultiplier == 1.0f && NightSpeed != 0.0f)
            {
                TimeOfDay += (NightSpeed * accumulatedTime);
            }
            if (AdjustDateWhenDayEnds)
            {
                // �Ϸ� �ð��� ���εǴ� ��� ó��
                if (TimeOfDay < 0.0f)
                {
                    TimeOfDay += SecondsPerDay;
                    DateTime dt = new DateTime(Year, Month, Day) - TimeSpan.FromDays(1.0) + TimeSpan.FromSeconds(TimeOfDay);
                    Year = dt.Year;
                    Month = dt.Month;
                    Day = dt.Day;
                }
                else if (TimeOfDay >= SecondsPerDay)
                {
                    TimeOfDay -= SecondsPerDay;
                    DateTime dt = new DateTime(Year, Month, Day) + TimeSpan.FromDays(1.0) + TimeSpan.FromSeconds(TimeOfDay);
                    Year = dt.Year;
                    Month = dt.Month;
                    Day = dt.Day;
                }
            }
            else if (TimeOfDay < 0.0f)
            {
                TimeOfDay += SecondsPerDay;
            }
            else if (TimeOfDay >= SecondsPerDay)
            {
                TimeOfDay -= SecondsPerDay;
            }
            TimeOfDayTimespan = TimeSpan.FromSeconds(TimeOfDay);

            // �̺�Ʈ ����
            if (prevDt.Year != Year)
            {
                WeatherMakerScript.Instance.YearChanged.Invoke(this);
            }
            if (prevDt.Month != Month)
            {
                WeatherMakerScript.Instance.MonthChanged.Invoke(this);
            }
            if (prevDt.Day != Day)
            {
                WeatherMakerScript.Instance.DayChanged.Invoke(this);
            }
            if (prevDt.Hour != TimeOfDayTimespan.Hours)
            {
                WeatherMakerScript.Instance.HourChanged.Invoke(this);
            }
            if (prevDt.Minute != TimeOfDayTimespan.Minutes)
            {
                WeatherMakerScript.Instance.MinuteChanged.Invoke(this);
            }
            if (prevDt.Second != TimeOfDayTimespan.Seconds)
            {
                WeatherMakerScript.Instance.SecondChanged.Invoke(this);
            }
        }

        private void UpdateAmbientColors(float l)
        {

#if UNITY_EDITOR

            if (DayAmbientColor == null)
            {
                return;
            }

#endif

            Color dayAmbient = DayAmbientColor.Evaluate(l);
            Color dayAmbientSky = DayAmbientColorSky.Evaluate(l);
            Color dayAmbientGround = DayAmbientColorGround.Evaluate(l);
            Color dayAmbientEquator = DayAmbientColorEquator.Evaluate(l);
            Color dawnDuskAmbient = DawnDuskAmbientColor.Evaluate(l);
            Color dawnDuskAmbientSky = DawnDuskAmbientColorSky.Evaluate(l);
            Color dawnDuskAmbientGround = DawnDuskAmbientColorGround.Evaluate(l);
            Color dawnDuskAmbientEquator = DawnDuskAmbientColorEquator.Evaluate(l);
            Color nightAmbient = NightAmbientColor.Evaluate(l);
            Color nightAmbientSky = NightAmbientColorSky.Evaluate(l);
            Color nightAmbientGround = NightAmbientColorGround.Evaluate(l);
            Color nightAmbientEquator = NightAmbientColorEquator.Evaluate(l);
            Color externalAmbient = Color.clear;

            if (WeatherMakerFullScreenCloudsScript.Instance != null && WeatherMakerFullScreenCloudsScript.Instance.CloudProfile != null &&
                WeatherMakerFullScreenCloudsScript.Instance.CloudProfile.AuroraProfile != null && WeatherMakerFullScreenCloudsScript.Instance.CloudProfile.AuroraProfile.AuroraEnabled)
            {
                externalAmbient = WeatherMakerFullScreenCloudsScript.Instance.CloudProfile.AuroraProfile.AnimationAmbientColor * NightMultiplier * Mathf.Max(NightAmbientColorIntensity, NightAmbientColorSkyIntensity);
            }

            Color ambientLight = (dayAmbient * DayMultiplier * dayAmbient.a * DayAmbientColorIntensity) +
                (dawnDuskAmbient * DawnDuskMultiplier * dawnDuskAmbient.a * DawnDuskAmbientColorIntensity) +
                (nightAmbient * NightMultiplier * nightAmbient.a * NightAmbientColorIntensity) +
                externalAmbient;
            Color ambientLightSky = (dayAmbientSky * DayMultiplier * dayAmbientSky.a * DayAmbientColorSkyIntensity) +
                (dawnDuskAmbientSky * DawnDuskMultiplier * dawnDuskAmbientSky.a * DawnDuskAmbientColorSkyIntensity) +
                (nightAmbientSky * NightMultiplier * nightAmbientSky.a * NightAmbientColorSkyIntensity) +
                externalAmbient;
            Color ambientLightGround = (dayAmbientGround * DayMultiplier * dayAmbientGround.a * DayAmbientColorGroundIntensity) +
                (dawnDuskAmbientGround * DawnDuskMultiplier * dawnDuskAmbientGround.a * DawnDuskAmbientColorGroundIntensity) +
                (nightAmbientGround * NightMultiplier * nightAmbientGround.a * NightAmbientColorGroundIntensity) +
                externalAmbient;
            Color ambientLightEquator = (dayAmbientEquator * DayMultiplier * dayAmbientEquator.a * DayAmbientColorEquatorIntensity) +
                (dawnDuskAmbientEquator * DawnDuskMultiplier * dawnDuskAmbientEquator.a * DawnDuskAmbientColorEquatorIntensity) +
                (nightAmbientEquator * NightMultiplier * nightAmbientEquator.a * NightAmbientColorEquatorIntensity) +
                externalAmbient;

            Shader.SetGlobalFloat(WMS._WeatherMakerDayMultiplier, DayMultiplier);
            Shader.SetGlobalFloat(WMS._WeatherMakerDayMultiplierInv, 1.0f - DayMultiplier);
            Shader.SetGlobalFloat(WMS._WeatherMakerDawnDuskMultiplier, DawnDuskMultiplier);
            Shader.SetGlobalFloat(WMS._WeatherMakerDawnDuskMultiplierInv, 1.0f - DawnDuskMultiplier);
            Shader.SetGlobalFloat(WMS._WeatherMakerNightMultiplier, NightMultiplier);
            Shader.SetGlobalFloat(WMS._WeatherMakerNightMultiplierInv, 1.0f - NightMultiplier);
            if (WeatherMakerLightManagerScript.Instance != null)
            {
                float lookup = (WeatherMakerLightManagerScript.Instance.SunPerspective != null ? WeatherMakerLightManagerScript.Instance.SunPerspective.GetGradientLookup() :
                    (WeatherMakerLightManagerScript.Instance.SunOrthographic != null ? WeatherMakerLightManagerScript.Instance.SunOrthographic.GetGradientLookup() : 0.0f));
                Color tintColor = WeatherMakerLightManagerScript.EvaluateGradient(SkyTintColor, lookup);
                Color addColor = WeatherMakerLightManagerScript.EvaluateGradient(SkyAddColor, lookup);
                Shader.SetGlobalColor(WMS._WeatherMakerSkyTintColor2, tintColor * tintColor.a);
                Shader.SetGlobalColor(WMS._WeatherMakerSkyAddColor, addColor * addColor.a);
            }

            switch (AmbientColorMode)
            {
                case WeatherMakerDayNightAmbientColorMode.AmbientColor:
                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                    RenderSettings.ambientLight = ambientLight;
                    RenderSettings.ambientSkyColor = ambientLight;
                    RenderSettings.ambientEquatorColor = ambientLight;
                    RenderSettings.ambientGroundColor = ambientLight;
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColor, ambientLight);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorSky, ambientLight);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorGround, ambientLight);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorEquator, ambientLight);
                    break;

                case WeatherMakerDayNightAmbientColorMode.SkyOnly:
                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                    RenderSettings.ambientLight = Color.black;
                    RenderSettings.ambientSkyColor = ambientLightSky;
                    RenderSettings.ambientEquatorColor = Color.black;
                    RenderSettings.ambientGroundColor = Color.black;
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColor, Color.black);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorSky, ambientLightSky);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorGround, Color.black);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorEquator, Color.black);
                    break;

                case WeatherMakerDayNightAmbientColorMode.SkyEquatorGround:
                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
                    RenderSettings.ambientLight = Color.black;
                    RenderSettings.ambientSkyColor = ambientLightSky;
                    RenderSettings.ambientEquatorColor = ambientLightEquator;
                    RenderSettings.ambientGroundColor = ambientLightGround;
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColor, Color.black);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorSky, ambientLightSky);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorGround, ambientLightGround);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorEquator, ambientLightEquator);
                    break;

                case WeatherMakerDayNightAmbientColorMode.All:
                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
                    goto case WeatherMakerDayNightAmbientColorMode.AllWithUnityMode;

                case WeatherMakerDayNightAmbientColorMode.AllWithUnityMode:
                {
                    float maxColorComponent = ambientLight.maxColorComponent;
                    RenderSettings.ambientLight = ambientLight;
                    RenderSettings.ambientSkyColor = (ambientLightSky = (ambientLightSky.maxColorComponent >= maxColorComponent ? ambientLightSky : ambientLight));
                    RenderSettings.ambientGroundColor = (ambientLightGround = (ambientLightGround.maxColorComponent >= maxColorComponent ? ambientLightGround : ambientLight));
                    RenderSettings.ambientEquatorColor = (ambientLightEquator = (ambientLightEquator.maxColorComponent >= maxColorComponent ? ambientLightEquator : ambientLight));
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColor, ambientLight);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorSky, ambientLightSky);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorGround, ambientLightGround);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorEquator, ambientLightEquator);
                }
                break;

                case WeatherMakerDayNightAmbientColorMode.DynamicGIUpdateOnly:
                    if (Mathf.Abs(TimeOfDay - lastTimeOfDayForDynamicGIUpdate) > DynamicGIUpdateThresholdSeconds)
                    {
                        lastTimeOfDayForDynamicGIUpdate = TimeOfDay;
                        DynamicGI.UpdateEnvironment();
                    }
                    goto case WeatherMakerDayNightAmbientColorMode.UnityAmbientSettings;

                case WeatherMakerDayNightAmbientColorMode.None:
                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                    RenderSettings.ambientLight = Color.black;
                    RenderSettings.ambientSkyColor = Color.black;
                    RenderSettings.ambientEquatorColor = Color.black;
                    RenderSettings.ambientGroundColor = Color.black;
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColor, Color.black);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorSky, Color.black);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorGround, Color.black);
                    Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorEquator, Color.black);
                    break;

                case WeatherMakerDayNightAmbientColorMode.UnityAmbientSettings:
                    if (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Flat)
                    {
                        Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColor, RenderSettings.ambientLight);
                        Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorSky, RenderSettings.ambientLight);
                        Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorGround, RenderSettings.ambientLight);
                        Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorEquator, RenderSettings.ambientLight);
                    }
                    else
                    {
                        Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColor, RenderSettings.ambientLight);
                        Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorSky, RenderSettings.ambientSkyColor);
                        Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorGround, RenderSettings.ambientEquatorColor);
                        Shader.SetGlobalColor(WMS._WeatherMakerAmbientLightColorEquator, RenderSettings.ambientGroundColor);
                    }
                    break;
            }
        }

        /// <summary>
        /// ������ ������ ����Ʈ �Ŵ��� ���¸� ����Ͽ� ��/�� �ֱ� ������Ʈ
        /// </summary>
        /// <param name="updateTimeOfDay">�Ϸ� �ð��� ������Ʈ���� ����</param>
        public void UpdateFromProfile(bool updateTimeOfDay)
        {
            if (WeatherMakerScript.Instance == null)
            {
                return;
            }

            Camera mainCamera = WeatherMakerScript.Instance.MainCamera;
            WeatherMakerCelestialObjectScript sun = WeatherMakerLightManagerScript.SunForCamera(mainCamera);
            if (sun == null)
            {
                return;
            }

            accumulatedTime += Time.deltaTime;
            if (accumulatedTime > UpdateInterval)
            {
                Dictionary<string, float> directionalLightIntensityModifiers = WeatherMakerLightManagerScript.Instance.DirectionalLightIntensityMultipliers;
                Dictionary<string, float> directionalLightShadowModifier = WeatherMakerLightManagerScript.Instance.DirectionalLightShadowIntensityMultipliers;
                CameraMode mode = WeatherMakerScript.ResolveCameraMode();
                IList<WeatherMakerCelestialObjectScript> suns = WeatherMakerLightManagerScript.Instance.Suns;
                IList<WeatherMakerCelestialObjectScript> moons = WeatherMakerLightManagerScript.Instance.Moons;
                float l = sun.GetGradientLookup();
                UpdateAmbientColors(l);
                UpdateTimeOfDay(updateTimeOfDay);
                UpdateSuns(mode, suns);
                UpdateDayMultipliers(mode, sun, l);
                UpdateMoons(mode, sun, moons);
                accumulatedTime = 0.0f;
                UpdateLightIntensitiesAndShadows(sun, moons, directionalLightIntensityModifiers, directionalLightShadowModifier);
            }
        }
    }

    /// <summary>
    /// ��/�� �ֱ� �ں��Ʈ ���� ���
    /// </summary>
    public enum WeatherMakerDayNightAmbientColorMode
    {
        /// <summary>
        /// �ں��Ʈ ���� ��� ����, ��� ���������� ����. Unity �ں��Ʈ ����Ʈ ����.
        /// </summary>
        None = 0,

        /// <summary>
        /// �ں��Ʈ ������ �ں��Ʈ, �ϴ�, ���� �� ���� �������� ���. Unity ��� �ں��Ʈ ���� ���.
        /// </summary>
        AmbientColor = 1,

        /// <summary>
        /// �ں��Ʈ �ϴ� ���� ���, ������ �ں��Ʈ ������ ������. Unity ��� �ں��Ʈ ���� ���.
        /// </summary>
        SkyOnly = 2,

        /// <summary>
        /// �ں��Ʈ �ϴ�, ���� �� ���� ���� ���. �ں��Ʈ ������ ������. Unity ���� ���� �ں��Ʈ ���� ���.
        /// </summary>
        SkyEquatorGround = 4,

        /// <summary>
        /// �ں��Ʈ ����, �ϴ�, ���� �� ���� ���� ���. �ں��Ʈ ������ �ϴ�, ���� �� ���鿡 �߰���. Unity ���� ���� �ں��Ʈ ���� ���.
        /// </summary>
        All = 8,

        /// <summary>
        /// ���� GI ������Ʈ�� �ֱ������� ȣ��, DynamicGIUpdateThresholdSeconds ����� ���. �ں��Ʈ �����̳� ������ �������� ����.
        /// </summary>
        DynamicGIUpdateOnly = 16,

        /// <summary>
        /// Unity �ں��Ʈ ��带 �״�� ����. �ں��Ʈ ����, �ϴ�, ���� �� ���� ���� ���. �ں��Ʈ ������ �ϴ�, ���� �� ���鿡 �߰���.
        /// </summary>
        AllWithUnityMode = 32,

        /// <summary>
        /// ���� �ں��Ʈ ���� ������ ���, ��/�� �ֱ� ������ �ں��Ʈ ���� ����.
        /// </summary>
        UnityAmbientSettings = 64
    }

    /// <summary>
    /// �Ϸ� �ð� ����
    /// </summary>
    [Flags]
    public enum WeatherMakerTimeOfDayCategory
    {
        /// <summary>
        /// ����
        /// </summary>
        None = 0,

        /// <summary>
        /// ����
        /// </summary>
        Dawn = 1,

        /// <summary>
        /// ��
        /// </summary>
        Day = 2,

        /// <summary>
        /// Ȳȥ
        /// </summary>
        Dusk = 4,

        /// <summary>
        /// ��
        /// </summary>
        Night = 8,

        /// <summary>
        /// ����
        /// </summary>
        Sunrise = 16,

        /// <summary>
        /// �ϸ�
        /// </summary>
        Sunset = 32
    }
}
