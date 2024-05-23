using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    //'WeatherMaker/Day-Night Cycle Profile' �޴� �׸��� �߰��ϰ� ���� �̸��� 'WeatherMakerDayNightCycleProfile'�� ����
    [CreateAssetMenu(fileName = "WeatherMakerDayNightCycleProfile", menuName = "WeatherMaker/Day-Night Cycle Profile", order = 41)]

    // Ÿ�Ӿƿ��� �ִ� �� Ŭ���̾�Ʈ Ŭ���� ����
    public class WeatherMakerDayNightCycleProfileScript : ScriptableObject
    {
        #region Classes

        private class WebClientWithTimeout : System.Net.WebClient
        {
            protected override System.Net.WebRequest GetWebRequest(Uri uri)
            {
                System.Net.WebRequest w = base.GetWebRequest(uri);
                w.Timeout = Timeout;    // Ÿ�Ӿƿ� ����
                return w;
            }
            public int Timeout { get; set; }    // Ÿ�Ӿƿ� �ð�
        }

        // �¾� ���� Ŭ���� ����
        public class SunInfo
        {
            public DateTime DateTime;   // ��¥ �� �ð�
            public double Latitude;         // ����
            public double Longitude;      // �浵
            public double AxisTilt;          // �� ����

            public Vector3 UnitVectorUp;    // ���� ���� (��)
            public Vector3 UnitVectorDown;  // ���� ���� (�Ʒ�)

            public TimeSpan Dawn;   // ���� �ð�
            public TimeSpan SunRise;    // ���� �ð�
            public TimeSpan SunSet;     // �ϸ� �ð�
            public TimeSpan Dusk;       // Ȳȥ �ð�

            public double JulianDays;       // ������ ��¥
            public double Declination;      // ����
            public double RightAscension;       // ����
            public double Azimuth;      // ������
            public double Altitude;     // ��
            public double SolarMeanAnomaly;     // ��� �¾� �̰�
            public double EclipticLongitude;        // Ȳ�� �浵
            public double SiderealTime;         // �׼� �ð�
        }

        // �� ���� Ŭ���� ����
        public class MoonInfo
        {
            public SunInfo SunData;     // �¾� ����

            public Vector3 UnitVectorUp;        // ���� ���� (��)
            public Vector3 UnitVectorDown;   // ���� ���� (�Ʒ�)

            public double Distance;     // �Ÿ�
            public double Phase;         //����
            public double PercentFull;  // ä���� ����
            public double Angle;    // ����
            public double Fraction;     // �м�
            public double Azimuth;  // ������
            public double Altitude;     // ��
            public double RightAscension;   // ����
            public double Declination;  // ����
            public double LunarMeanAnomaly; // ��� �� �̰�
            public double EclipticLongitude;    // Ȳ�� �浵
            public double SiderealTime;     // �׻� �ð�
            public double ParallacticAngle;     // ������
        }

        #endregion Classes

        [Header("Day/night cycle")]
        [Range(-100000, 100000.0f)]
        [Tooltip("The day speed of the cycle. Set to 0 to freeze the cycle and manually control it. At a speed of 1, the cycle is in real-time. " +
            "A speed of 100 is 100 times faster than normal. Negative numbers run the cycle backwards.")]
        // �� �ӵ��� ����( 0:�ֱⰡ ���� / ���:�ֱ⸦������ / ����:�ֱ⸦ ���������� )
        public float Speed = 10.0f;

        [Range(-100000, 100000.0f)]
        [Tooltip("The night speed of the cycle. Set to 0 to freeze the cycle and manually control it. At a speed of 1, the cycle is in real-time. " +
            "A speed of 100 is 100 times faster than normal. Negative numbers run the cycle backwards.")]
        // �� �ӵ��� ����( 0:�ֱⰡ ���� / ���:�ֱ⸦������ / ����:�ֱ⸦ ���������� )
        public float NightSpeed = 10.0f;

        [Tooltip("How often the update cycle updates. Use higher values if you have issues with shadow flickering, etc. Turning on temporal anti-aliasing " +
            "is also a good way to reduce flicker, though you may have to play with the settings.")]
        [Range(0.0f, 10.0f)]
        // �ֱ� ������Ʈ�� �󵵸� ����( ���� ���� �����ϸ� �׸��� ������ ���� �پ��)
        public float UpdateInterval = 0.03f;
        // ���� �ð� �ʱⰪ ����
        private float accumulatedTime = 10.0f;

        [Range(0.0f, SecondsPerDay)]
        [Tooltip("The current time of day in seconds (local time).")]
        // �Ϸ� �� ���� �ð��� �� ������ ��Ÿ��( �⺻ �� : �Ϸ��� ������ ������ ���� )
        public float TimeOfDay = SecondsPerDay * 0.5f;

#if UNITY_EDITOR

#pragma warning disable 0414

        [ReadOnlyLabel]
        [SerializeField]
        internal string TimeOfDayLabel = string.Empty;

#pragma warning restore 0414

#endif
        // ��¥ ����
        [Header("Date")]
        [Tooltip("�¾�� ���� ��ġ�� �ùķ��̼��� ���� - ��Ÿ�� ���� ����� �� �ֽ��ϴ�. " +
            "1900�� 3�� 1�Ϻ��� 2100�� 2�� 28�ϱ����� ��¥�� ���ؼ��� ��Ȯ�մϴ�")]
        [Range(1900, 2100)]
        public int Year = 2000;

        [Tooltip("�¾�� ���� ��ġ�� �ùķ��̼��� �� - ��Ÿ�� ���� ����� �� �ֽ��ϴ�")]
        [Range(1, 12)]
        public int Month = 9;

        [Tooltip("�¾�� ���� ��ġ�� �ùķ��̼��� �� - ��Ÿ�� ���� ����� �� �ֽ��ϴ�.")]
        [Range(1, 31)]
        public int Day = 21;

        [Tooltip("���� ���� �� ��¥�� �������� ����. ��¥�� ���۵ǰ� ���� �� ��Ȯ�� �¾�� �� ��ġ�� �����ϴ� �� �߿�������, �ð��� �����Ǿ� �ִٸ� �� �� �ֽ��ϴ�.")]
        public bool AdjustDateWhenDayEnds = true;

        [Tooltip("����/�浵�� �ð��� ������(��). -1111�� �����ϸ� �ڵ����� ���˴ϴ�(�ؽ�Ʈ �ʵ忡�� -1111�� �Է��� �� �� ����). -1111�� ���� ���� ����: ������ ��忡���� �� ���񽺰� ���˴ϴ�. ��� ��忡���� �浵�� ����Ͽ� ������ ����մϴ�..")]
        public int TimeZoneOffsetSeconds = -21600;

        // ��ġ ����
        [Header("Location")]
        [Range(-90.0f, 90.0f)]
        [Tooltip("ī�޶� ��ġ�� �༺�� ����(��) - �ϱ��� 90��, ������ -90��")]
        public double Latitude = 40.7608; // salt lake city latitude

        [Range(-180.0f, 180.0f)]
        [Tooltip("ī�޶� ��ġ�� �༺�� �浵(��) - 180���� -180����")]
        public double Longitude = -111.8910; // salt lake city longitude

        [Range(0.0f, 360.0f)]
        [Tooltip("�༺�� ����(��) - ������ �� 23.439��")]
        public float AxisTilt = 23.439f;

        // �ð��� ���� ���� ���
        [Header("Time of Day Mapping")]
        [Tooltip("��, ����/Ȳȥ �Ǵ� ���� �����մϴ�. �׷����Ʈ�� �߾��� �¾��� ���򼱿� �ִ� ��ġ�Դϴ�. ����� ��, �������� ����/Ȳȥ, �Ķ����� ���� ��Ÿ���ϴ�. ����/Ȳȥ�� ���, ���� 12�� ������ ����, ���� 12�� ���Ĵ� Ȳȥ�Դϴ�.")]
        public Gradient DayDawnDuskNightGradient;

        [Tooltip("�ϴ� ����, �׷����Ʈ�� �߾��� �¾��� ���򼱿� �ִ� ��ġ�Դϴ�.")]
        public Gradient SkyTintColor;

        [Tooltip("Sky add color, center of gradient is sun at horizon.")]
        public Gradient SkyAddColor;

        // �ں��Ʈ ���� ����
        [Header("Ambient colors")]
        [Tooltip("Unity ����� �ں��Ʈ ������ ��, ����/Ȳȥ �� �� �ں��Ʈ �������� �������� ����.")]
        public WeatherMakerDayNightAmbientColorMode AmbientColorMode = WeatherMakerDayNightAmbientColorMode.AmbientColor;

        [Header("Ambient colors - day")]
        [Tooltip("���� �ں��Ʈ ����, ���� �������� ������ ���� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient DayAmbientColor;

        [Tooltip("�߰� �� �ں��Ʈ ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ �ϴ� ����, ���� �������� ������ ���� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient DayAmbientColorSky;

        [Tooltip("�߰� �� �ں��Ʈ �ϴ� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorSkyIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ ���� ����, ���� �������� ������ ���� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient DayAmbientColorGround;

        [Tooltip("�߰� �� �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorGroundIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ ���� ����, ���� �������� ������ ���� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient DayAmbientColorEquator;

        [Tooltip("�߰� �� �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorEquatorIntensity = 0.05f;

        // ����/Ȳȥ�� �ں��Ʈ ���� ����
        [Header("Ambient colors - dawn/dusk")]
        [Tooltip("����/Ȳȥ �ں��Ʈ ����, ���� �������� ������ ���� �Ǵ� Ȳȥ�� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient DawnDuskAmbientColor;

        [Tooltip("�߰� ����/Ȳȥ �ں��Ʈ ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorIntensity = 0.05f;

        [Tooltip("����/Ȳȥ �ں��Ʈ �ϴ� ����, ���� �������� ������ ���� �Ǵ� Ȳȥ�� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient DawnDuskAmbientColorSky;

        [Tooltip("�߰� ����/Ȳȥ �ں��Ʈ �ϴ� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorSkyIntensity = 0.05f;

        [Tooltip("����/Ȳȥ �ں��Ʈ ���� ����, ���� �������� ������ ���� �Ǵ� Ȳȥ�� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient DawnDuskAmbientColorGround;

        [Tooltip("�߰� ����/Ȳȥ �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorGroundIntensity = 0.05f;

        [Tooltip("����/Ȳȥ �ں��Ʈ ���� ����, ���� �������� ������ ���� �Ǵ� Ȳȥ�� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient DawnDuskAmbientColorEquator;

        [Tooltip("�߰� ����/Ȳȥ �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorEquatorIntensity = 0.05f;

        // ���� �ں��Ʈ ���� ���� ���
        [Header("Ambient colors - night")]
        [Tooltip("�� �ں��Ʈ ����, ���� �������� ������ ���� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient NightAmbientColor;

        [Tooltip("�߰� �� �ں��Ʈ ���� ����")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ �ϴ� ����, ���� �������� ������ ���� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient NightAmbientColorSky;

        [Tooltip("�߰� �� �ں��Ʈ �ϴ� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorSkyIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ ���� ����, ���� �������� ������ ���� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient NightAmbientColorGround;

        [Tooltip("�߰� �� �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorGroundIntensity = 0.05f;

        [Tooltip("�� �ں��Ʈ ���� ����, ���� �������� ������ ���� ��Ÿ�� - ���Ĵ� ������ ��Ÿ��")]
        public Gradient NightAmbientColorEquator;

        [Tooltip("�߰� �� �ں��Ʈ ���� ���� ����")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorEquatorIntensity = 0.05f;

        // �¾� ������ ���� ���
        [Header("Sun modifiers")]
        [Tooltip("�¾��� ����, �׷����Ʈ�� �߾��� �¾��� ���򼱿� �ִ� ��ġ�Դϴ�.")]
        public Gradient SunTintColorGradient;

        [Tooltip("�¾� ������ �����ϴ� �׷����Ʈ, �׷����Ʈ�� �߾��� �¾��� ���򼱿� �ִ� ��ġ�Դϴ�.")]
        public Gradient SunIntensityGradient;

        // �Ϸ��� ���� �ð��� TimeSpan �������� ��Ÿ��
        public TimeSpan TimeOfDayTimespan { get; private set; }

        // ���� �ð���(��, ����/Ȳȥ, ��)�� ��Ÿ���� ������ ����
        public WeatherMakerTimeOfDayCategory TimeOfDayCategory { get; private set; }

        // �� �ð� ����� ��Ÿ���� ����
        public float DayMultiplier { get; private set; }

        // ����/Ȳȥ �ð� ����� ��Ÿ���� ����
        public float DawnDuskMultiplier { get; private set; }

        // �� �ð� ����� ��Ÿ���� ����
        public float NightMultiplier { get; private set; }

        // ����ȭ���� �ʴ� SunInfo Ŭ������ �ν��Ͻ��� ��Ÿ��. �¾翡 ���� ������ ����.
        [NonSerialized]
        public readonly SunInfo SunData = new SunInfo();

        // ����ȭ���� �ʴ� MoonInfo Ŭ������ �ν��Ͻ��� �����ϴ� ����Ʈ�� ��Ÿ��. �޿� ���� ������ ����.
        [NonSerialized]
        public readonly List<MoonInfo> MoonDatas = new List<MoonInfo>();

        // �Ϸ��� �� �ʸ� ����� ����. 86400�� (24�ð�)
        public const float SecondsPerDay = 86400.0f;

        // ���� �ð��� ����� ����. �Ϸ��� ������ 43200�� (12�ð�)
        public const float HighNoonTimeOfDay = SecondsPerDay * 0.5f;

        // 1���� �ش��ϴ� �ʸ� ����� ����. 240�� (4��)
        public const float SecondsForOneDegree = SecondsPerDay / 360.0f;

        // ���� GI ������Ʈ �Ӱ谪�� ����� ����. 300�� (5��)
        public const float DynamicGIUpdateThresholdSeconds = 300.0f;

        // ���� DateTime ���� �����ϴ� ����
        private DateTime prevDt;

        // DateTime �Ӽ� ���� - ����, ��, ��, �ð�, ��, �ʸ� �����ϰ� �������� �޼���
        public DateTime DateTime
        {
            get
            {
                // TimeOfDayTimeSpan ���� ������ ����, ��, �ϰ� �Բ� DateTime ��ü�� �����Ͽ� ��ȯ
                TimeSpan ts = TimeOfDayTimeSpan;
                return new DateTime(Year, Month, Day, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds, DateTimeKind.Local);
            }
            set
            {
                // �Էµ� DateTime ���� ���� �ð����� ��ȯ�Ͽ� ����, ��, ��, �ð��� ����
                DateTime dt = value.ToLocalTime();
                Year = dt.Year;
                Month = dt.Month;
                Day = dt.Day;
                TimeOfDay = (float)dt.TimeOfDay.TotalSeconds;
            }
        }

        // TimeOfDay�� TimeSpan �������� �������� �����ϴ� �Ӽ�
        public TimeSpan TimeOfDayTimeSpan 
        { 
            get { return TimeSpan.FromSeconds(TimeOfDay); }  // TimeOfDay�� �� ������ TimeSpan ��ü�� ��ȯ�Ͽ� ��ȯ
            set { TimeOfDay = (float)value.TotalSeconds; }  // TimeSpan ���� �� ������ ��ȯ�Ͽ� TimeOfDay�� ����
        }

        // ���� GI ������Ʈ�� ���� ������ �ð� �ʱⰪ ����
        private float lastTimeOfDayForDynamicGIUpdate = -999999.0f;

        /// <summary>
        /// ������ ��¥�� DateTime �������� ��ȯ�ϴ� �޼���
        /// </summary>
        /// <param name="julianDate">Julian date</param>    ������ ��¥
        /// <returns>DateTime with UTC kind</returns>    UTC ������ DateTime ��ü
        public static DateTime JulianToDateTime(double julianDate)
        {
            double unixTime = (julianDate - 2440587.5) * 86400; // ������ ��¥�� ���н� �ð����� ��ȯ
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc); // ���н� �ð��� ���� ��¥ ����
            dtDateTime = dtDateTime.AddSeconds(unixTime); // ���н� �ð���ŭ ���Ͽ� DateTime ��ü ����
            return dtDateTime; // ������ DateTime ��ü ��ȯ
        }

        /// <summary>
        /// �������� ���� ���� ���ͷ� ��ȯ�ϴ� �޼���
        /// </summary>
        /// <param name="azimuth">Azimuth</param>      ������
        /// <param name="altitude">Altitude</param>       �� 
        /// <param name="vector">Unit vector</param>    ���� ����
        public static void ConvertAzimuthAtltitudeToUnitVector(double azimuth, double altitude, ref Vector3 vector)
        {
            vector.y = (float)Math.Sin(altitude); // ���� ���ΰ��� y ���п� ����
            float hyp = (float)Math.Cos(altitude); // ���� �ڻ��ΰ��� �̿��Ͽ� hypotenuse ���
            vector.z = hyp * (float)Math.Cos(azimuth); // �������� �ڻ��ΰ��� hypotenuse�� ���Ͽ� z ���п� ����
            vector.x = hyp * (float)Math.Sin(azimuth); // �������� ���ΰ��� hypotenuse�� ���Ͽ� x ���п� ����
        }
        // -> ���� ���� �ֱ⸦ �����ϰ�, �¾�� ���� ��ġ�� ����ϸ� �پ��� �ð� ������ ��ȯ�ϴµ� ���Ǵ� �ڵ�



        /// <param name="sunInfo">Calculates and receives sun info, including position, etc. Parameters marked as calculation parameters need to be set first.</param>
        /// <param name="rotateYDegrees">Rotate around the Y axis</param>
        public static void CalculateSunPosition(SunInfo sunInfo, float rotateYDegrees)
        {
            // dateTime should already be UTC format
            double d = (sunInfo.DateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / dayMs) + jDiff;
            //double d = sunInfo.DateTime.ToOADate() + 2415018.5;
            double e = degreesToRadians * sunInfo.AxisTilt; // obliquity of the Earth
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
                // don't crash if date time is out of bounds
            }
        }

        /// <param name="sunInfo">Sun info, already calculated</param>
        /// <param name="moonInfo">Receives moon info</param>
        /// <param name="rotateYDegrees">Rotate the moon in the sky around the y axis by this degrees</param>
        public static void CalculateMoonPosition(SunInfo sunInfo, MoonInfo moonInfo, float rotateYDegrees)
        {
            double d = sunInfo.JulianDays;
            double e = degreesToRadians * sunInfo.AxisTilt; // obliquity of the Earth
            double L = degreesToRadians * (218.316 + 13.176396 * d); // ecliptic longitude
            double M = degreesToRadians * (134.963 + 13.064993 * d); // mean anomaly
            double F = degreesToRadians * (93.272 + 13.229350 * d); // mean distance
            double l = L + degreesToRadians * 6.289 * Math.Sin(M); // longitude
            double b = degreesToRadians * 5.128 * Math.Sin(F); // latitude
            double dist = 385001.0 - (20905.0 * Math.Cos(M)); // distance to the moon in km
            double ra = RightAscension(e, l, b);
            double dec = Declination(e, l, b);
            const double sunDistance = 149598000.0; // avg sun distance to Earth
            double phi = Math.Acos(Math.Sin(sunInfo.Declination) * Math.Sin(dec) + Math.Cos(sunInfo.Declination) * Math.Cos(dec) * Math.Cos(sunInfo.RightAscension - ra));
            double inc = Math.Atan2(sunDistance * Math.Sin(phi), dist - sunDistance * Math.Cos(phi));
            double angle = Math.Atan2(Math.Cos(sunInfo.Declination) * Math.Sin(sunInfo.RightAscension - ra), Math.Sin(sunInfo.Declination) * Math.Cos(dec) - Math.Cos(sunInfo.Declination) * Math.Sin(dec) * Math.Cos(sunInfo.RightAscension - ra));
            double fraction = (1.0 + Math.Cos(inc)) * 0.5;
            double phase = 0.5 + (0.5 * inc * Math.Sign(angle) * (1.0 / Math.PI));
            double lw = -degreesToRadians * sunInfo.Longitude;
            phi = degreesToRadians * sunInfo.Latitude;
            double H = SiderealTime(d, lw) - ra;
            double h = Altitude(H, phi, dec);

            // formula 14.1 of "Astronomical Algorithms" 2nd edition by Jean Meeus (Willmann-Bell, Richmond) 1998.
            double pa = Math.Atan2(Math.Sin(H), Math.Tan(phi) * Math.Cos(dec) - Math.Sin(dec) * Math.Cos(H));
            h = h + AstroRefraction(h); // altitude correction for refraction
            double azimuth = Azimuth(H, phi, dec);
            double altitude = h;
            ConvertAzimuthAtltitudeToUnitVector(azimuth, altitude, ref moonInfo.UnitVectorUp);

            // set moon position and look at the origin
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
            double c = degreesToRadians * (1.9148 * Math.Sin(m) + 0.02 * Math.Sin(2.0 * m) + 0.0003 * Math.Sin(3.0 * m)); // equation of center
            double p = degreesToRadians * 102.9372; // perihelion of the Earth
            return m + c + p + Math.PI;
        }

        private static double AstroRefraction(double h)
        {
            // the following formula works for positive altitudes only.
            // if h = -0.08901179 a div/0 would occur.
            h = (h < 0.0 ? 0.0 : h);

            // formula 16.4 of "Astronomical Algorithms" 2nd edition by Jean Meeus (Willmann-Bell, Richmond) 1998.
            // 1.02 / tan(h + 10.26 / (h + 5.10)) h in degrees, result in arc minutes -> converted to rad:
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
                    // convert local time of day to UTC time of day - quick and dirty calculation
                    double offsetSeconds = TimeZoneOffsetSeconds;
                    TimeSpan t = TimeSpan.FromSeconds(TimeOfDay - offsetSeconds);
                    SunData.DateTime = new DateTime(Year, Month, Day, 0, 0, 0, DateTimeKind.Utc) + t;
                    SunData.Latitude = Latitude;
                    SunData.Longitude = Longitude;
                    SunData.AxisTilt = AxisTilt;

                    // calculate and set sun position in sky
                    if (sun.OrbitType == WeatherMakerOrbitType.FromEarth)
                    {
                        CalculateSunPosition(SunData, sun.RotateYDegrees);
                    }
                    SetCelestialObjectPosition(sun, SunData.UnitVectorDown);

                    // calculate sun intensity and shadow strengths
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

                // intensity raises squared compare to moon fullness - this means less full is squared amount of less light
                // moon light intensity reduces as sun light intensity approaches 1
                // reduce moon light as it drops below horizon
                dot = Mathf.Clamp(Vector3.Dot(MoonDatas[i].UnitVectorDown, Vector3.down) + 0.2f, 0.0f, 1.0f);
                dot = Mathf.Pow(dot, 0.25f);
                yPower = Mathf.Clamp((MoonDatas[i].UnitVectorUp.y + 0.2f) * 4.0f, 0.0f, 1.0f);
                moon.Light.color = moon.LightColor;
                moon.Light.intensity = moon.LightBaseIntensity * yPower * sunIntensityReducer * dot * moon.LightMultiplier;
                if (moon.OrbitType == WeatherMakerOrbitType.FromEarth)
                {
                    // as moon goes away from full, intensity reduces squared
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

        // �־��� ī�޶� ���� �¾� ��ü�� ���¿� ���� ���� �ð��븦 ������Ʈ
        // ��,����/Ȳȥ,���� ����� ����
        private void UpdateDayMultipliers(CameraMode mode, WeatherMakerCelestialObjectScript sun, float l)
        {
            // DayDawnDuskNightGradient���� l ��ġ�� ������ ���Ͽ� ���� ���� ������
            Color lookup = DayDawnDuskNightGradient.Evaluate(l);

            // ������ ���� ���� �� ������ ����� ����
            DayMultiplier = lookup.g;   // ��� ����: �� ���
            DawnDuskMultiplier = lookup.r;  // ������ ����: ����/Ȳȥ ���
            NightMultiplier = lookup.b;     // �Ķ��� ����: �� ���

            // ���� �ð��� ī�װ��� �����ϰ� �ʱ�ȭ
            WeatherMakerTimeOfDayCategory current = TimeOfDayCategory;
            TimeOfDayCategory = WeatherMakerTimeOfDayCategory.None;

            // �� ����� 0���� ũ�� �ð��븦 ������ ����
            if (DayMultiplier > 0.0f)
            {
                TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Day;
            }

            // ����/Ȳȥ ����� 0���� ũ�� �ð��븦 ���� �Ǵ� Ȳȥ���� ����
            if (DawnDuskMultiplier > 0.0f)
            {
                if (TimeOfDay > (SecondsPerDay * 0.5))// �Ϸ��� ����(����) �����̸� Ȳȥ���� ����
                {
                    TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Dusk;
                }
                else     // �׷��� ������ �������� ����
                {
                    TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Dawn;
                }
            }

            // �� ����� 0���� ũ�� �ð��븦 ������ ����
            if (NightMultiplier > 0.0f)
            {
                TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Night;
            }

            // ī�޶� ��尡 OrthographicXY�� ���
            if (mode == CameraMode.OrthographicXY)
            {
                // �¾� ��ü�� z �� ������ Ȯ���Ͽ� �¾��� ���� ��ó�� �ִ��� Ȯ��
                float z = sun.transform.forward.z;
                if (z >= -0.1f && z <= 0.1f)
                {
                    if (TimeOfDay > (SecondsPerDay * 0.5f))   // �׷��� ������ ����� ����
                    {
                        TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Sunset;
                    }
                    else    // �׷��� ������ ����� ����
                    {
                        TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Sunrise;
                    }
                }
            }

            // ī�޶� ��尡 OrthographicXZ�� ���
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
                // �¾� ��ü�� y �� ������ Ȯ���Ͽ� �¾��� ���� ��ó�� �ִ��� Ȯ��
                float y = sun.transform.forward.y;
                if (y >= -0.05f && y <= 0.2f)
                {
                    if (TimeOfDay > (SecondsPerDay * 0.5f))   // �Ϸ��� ����(����) �����̸� �ϸ��� ����
                    {
                        TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Sunset;
                    }
                    else    // �׷��� ������ ����� ����
                    {
                        TimeOfDayCategory |= WeatherMakerTimeOfDayCategory.Sunrise;
                    }
                }
            }

            // send events
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
                        // eat exceptions
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
                // handle wrapping of time of day
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

            // send events
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
                } break;

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
        /// Update day night cycle using profile settings and light manager state
        /// </summary>
        /// <param name="updateTimeOfDay"></param>
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

    public enum WeatherMakerDayNightAmbientColorMode
    {
        None = 0,

        AmbientColor = 1,

        SkyOnly = 2,

        SkyEquatorGround = 4,

        All = 8,

        DynamicGIUpdateOnly = 16,

        AllWithUnityMode = 32,

        UnityAmbientSettings = 64
    }

    [Flags]
    public enum WeatherMakerTimeOfDayCategory
    {
        None = 0,

        Dawn = 1,

        Day = 2,

        Dusk = 4,

        Night = 8,

        Sunrise = 16,

        Sunset = 32
    }
}
