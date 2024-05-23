//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// 소스 코드는 개인 또는 상업 프로젝트에서 사용할 수 있습니다.
// 소스 코드는 재배포되거나 판매될 수 없습니다.
// 
// *** 불법 복제에 대한 주의 사항 ***
// 
// 이 자산을 불법 복제 사이트에서 받았다면, 유니티 애셋 스토어에서 구입해 주세요: https://assetstore.unity.com/packages/slug/60955?aid=1011lGnL
// 이 자산은 유니티 애셋 스토어에서만 합법적으로 구할 수 있습니다.
// 
// 저는 수백, 수천 시간 동안 이 소프트웨어와 다른 자산들을 개발하며 가족을 부양하는 인디 개발자입니다.
// 많은 사람들이 소프트웨어를 위해 많은 노력을 기울이는 동안 이를 도둑질하는 것은 매우 불쾌하고 무례하며 부도덕한 일입니다.
// 
// 감사합니다.
// 
// *** 불법 복제에 대한 주의 사항 끝 ***
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// 낮/밤 주기 프로필, 하루의 시간 및 이들이 모든 것에 미치는 영향을 포함하는 모든 필드와 속성을 포함
    /// </summary>
    [CreateAssetMenu(fileName = "WeatherMakerDayNightCycleProfile", menuName = "WeatherMaker/Day-Night Cycle Profile", order = 41)]
    public class WeatherMakerDayNightCycleProfileScript : ScriptableObject
    {
        #region 클래스들

        // 타임아웃 기능이 있는 웹 클라이언트 클래스
        private class WebClientWithTimeout : System.Net.WebClient
        {
            protected override System.Net.WebRequest GetWebRequest(Uri uri)
            {
                System.Net.WebRequest w = base.GetWebRequest(uri);
                w.Timeout = Timeout;
                return w;
            }

            /// <summary>
            /// 타임아웃 시간 (밀리초)
            /// </summary>
            public int Timeout { get; set; }
        }

        /// <summary>
        /// 태양 궤도 정보 클래스
        /// </summary>
        public class SunInfo
        {
            public DateTime DateTime; // 계산을 위한 날짜 및 시간
            public double Latitude; // 관찰자의 위도
            public double Longitude; // 관찰자의 경도
            public double AxisTilt; // 행성의 축 기울기
            public Vector3 UnitVectorUp; // 태양 위치 벡터
            public Vector3 UnitVectorDown; // 태양 위치 벡터의 반대 벡터
            public TimeSpan Dawn; // 새벽 시간
            public TimeSpan SunRise; // 일출 시간
            public TimeSpan SunSet; // 일몰 시간
            public TimeSpan Dusk; // 황혼 시간
            public double JulianDays; // 율리우스 날짜
            public double Declination; // 적위
            public double RightAscension; // 적경
            public double Azimuth; // 방위각
            public double Altitude; // 고도
            public double SolarMeanAnomaly; // 태양 평균 근점이각
            public double EclipticLongitude; // 황경
            public double SiderealTime; // 항성시
        }

        /// <summary>
        /// 달 궤도 정보 클래스
        /// </summary>
        public class MoonInfo
        {
            public SunInfo SunData; // 태양 데이터 참조
            public Vector3 UnitVectorUp; // 달 위치 벡터
            public Vector3 UnitVectorDown; // 달 위치 벡터의 반대 벡터
            public double Distance; // 달까지의 거리 (킬로미터)
            public double Phase; // 달의 위상
            public double PercentFull; // 만월 비율
            public double Angle; // 조명 각도
            public double Fraction; // 조명 비율
            public double Azimuth; // 방위각
            public double Altitude; // 고도
            public double RightAscension; // 적경
            public double Declination; // 적위
            public double LunarMeanAnomaly; // 달 평균 근점이각
            public double EclipticLongitude; // 황경
            public double SiderealTime; // 항성시
            public double ParallacticAngle; // 시차각
        }

        #endregion 클래스들

        /// <summary>
        /// 낮/밤 주기의 속도. 1은 실시간, 100은 100배 빠르며, 음수 값은 역방향으로 진행.
        /// </summary>
        [Header("Day/night cycle")]
        [Range(-100000, 100000.0f)]
        [Tooltip("주기의 낮 속도. 0으로 설정하면 주기가 멈추고 수동으로 제어할 수 있습니다. 속도가 1이면 주기가 실시간으로 진행됩니다. " +
            "속도가 100이면 정상 속도의 100배로 진행됩니다. 음수는 주기가 역방향으로 진행됩니다.")]
        public float Speed = 10.0f;

        /// <summary>
        /// 밤 주기의 속도. 낮 속도와 동일한 방식으로 작동.
        /// </summary>
        [Range(-100000, 100000.0f)]
        [Tooltip("주기의 밤 속도. 0으로 설정하면 주기가 멈추고 수동으로 제어할 수 있습니다. 속도가 1이면 주기가 실시간으로 진행됩니다. " +
            "속도가 100이면 정상 속도의 100배로 진행됩니다. 음수는 주기가 역방향으로 진행됩니다.")]
        public float NightSpeed = 10.0f;

        /// <summary>
        /// 주기가 업데이트되는 빈도. 높은 값은 그림자 깜박임을 줄이는 데 도움이 될 수 있습니다.
        /// </summary>
        [Tooltip("주기가 업데이트되는 빈도. 높은 값을 사용하면 그림자 깜박임 문제가 줄어들 수 있습니다. " +
            "일시적 안티앨리어싱을 켜는 것도 깜박임을 줄이는 좋은 방법이지만 설정을 조정해야 할 수 있습니다.")]
        [Range(0.0f, 10.0f)]
        public float UpdateInterval = 0.03f;

        private float accumulatedTime = 10.0f; // 업데이트를 위한 누적 시간

        /// <summary>
        /// 현재 하루의 시간 (초). 0은 자정, 43200은 정오, 86400은 다음 날 자정.
        /// </summary>
        [Range(0.0f, SecondsPerDay)]
        [Tooltip("현재 하루의 시간 (초).")]
        public float TimeOfDay = SecondsPerDay * 0.5f; // 기본값은 정오

#if UNITY_EDITOR
#pragma warning disable 0414

        [ReadOnlyLabel]
        [SerializeField]
        internal string TimeOfDayLabel = string.Empty;

#pragma warning restore 0414
#endif

        [Header("Date")]
        [Tooltip("태양과 달의 위치를 시뮬레이션하는 연도 - 이 값은 런타임 중 변경될 수 있습니다. " +
            "계산은 1900년 3월 1일부터 2100년 2월 28일까지의 날짜에 대해서만 정확합니다.")]
        [Range(1900, 2100)]
        public int Year = 2000;

        [Tooltip("태양과 달의 위치를 시뮬레이션하는 달 - 이 값은 런타임 중 변경될 수 있습니다.")]
        [Range(1, 12)]
        public int Month = 9;

        [Tooltip("태양과 달의 위치를 시뮬레이션하는 날 - 이 값은 런타임 중 변경될 수 있습니다.")]
        [Range(1, 31)]
        public int Day = 21;

        [Tooltip("하루가 끝날 때 날짜를 조정할지 여부. 날이 시작되고 끝날 때 정확한 태양 및 달 위치를 유지하는 데 중요하지만, 시간이 정적이면 끌 수 있습니다.")]
        public bool AdjustDateWhenDayEnds = true;

        [Tooltip("위도/경도에 대한 시간대 오프셋 (초). -1111로 설정하면 자동으로 계산됩니다 (입력란에 -1111을 입력한 후 탭키를 누르세요). -1111에 대한 참고: 에디터 모드에서는 웹 서비스를 사용합니다. 플레이 모드에서는 경도를 사용하여 빠르게 계산합니다.")]
        public int TimeZoneOffsetSeconds = -21600;

        [Header("Location")]
        [Range(-90.0f, 90.0f)]
        [Tooltip("카메라가 있는 행성의 위도 (도) - 북극 90에서 남극 -90까지")]
        public double Latitude = 40.7608; // 솔트 레이크 시티의 위도

        [Range(-180.0f, 180.0f)]
        [Tooltip("카메라가 있는 행성의 경도 (도). -180에서 180까지.")]
        public double Longitude = -111.8910; // 솔트 레이크 시티의 경도

        [Range(0.0f, 360.0f)]
        [Tooltip("행성의 기울기 (도) - 지구는 약 23.439도")]
        public float AxisTilt = 23.439f;

        [Header("Time of Day Mapping")]
        [Tooltip("낮, 새벽/황혼, 밤을 결정하는 그라디언트. 중심은 태양이 수평선에 있는 위치. 녹색 = 낮, 빨강 = 새벽/황혼, 파랑 = 밤. 새벽/황혼일 경우 오전 12시 이전은 새벽, 오후 12시 이후는 황혼.")]
        public Gradient DayDawnDuskNightGradient;

        [Tooltip("하늘의 색조를 설정하는 그라디언트, 중심은 태양이 수평선에 있는 위치.")]
        public Gradient SkyTintColor;

        [Tooltip("하늘에 추가하는 색을 설정하는 그라디언트, 중심은 태양이 수평선에 있는 위치.")]
        public Gradient SkyAddColor;

        [Header("Ambient colors")]
        [Tooltip("Unity의 내장된 앰비언트 색상을 낮, 새벽/황혼, 밤 앰비언트 색상으로 설정할지 여부.")]
        public WeatherMakerDayNightAmbientColorMode AmbientColorMode = WeatherMakerDayNightAmbientColorMode.AmbientColor;

        [Header("Ambient colors - day")]
        [Tooltip("낮 앰비언트 색상. 가장 오른쪽이 완전히 낮 - 알파 값은 강도로 사용.")]
        public Gradient DayAmbientColor;

        [Tooltip("추가 낮 앰비언트 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorIntensity = 0.05f;

        [Tooltip("낮 앰비언트 하늘 색상. 가장 오른쪽이 완전히 낮 - 알파 값은 강도로 사용.")]
        public Gradient DayAmbientColorSky;

        [Tooltip("추가 낮 앰비언트 하늘 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorSkyIntensity = 0.05f;

        [Tooltip("낮 앰비언트 지면 색상. 가장 오른쪽이 완전히 낮 - 알파 값은 강도로 사용.")]
        public Gradient DayAmbientColorGround;

        [Tooltip("추가 낮 앰비언트 지면 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorGroundIntensity = 0.05f;

        [Tooltip("낮 앰비언트 적도 색상. 가장 오른쪽이 완전히 낮 - 알파 값은 강도로 사용.")]
        public Gradient DayAmbientColorEquator;

        [Tooltip("추가 낮 앰비언트 적도 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float DayAmbientColorEquatorIntensity = 0.05f;

        [Header("Ambient colors - dawn/dusk")]
        [Tooltip("새벽/황혼 앰비언트 색상. 가장 오른쪽이 완전히 새벽 또는 황혼 - 알파 값은 강도로 사용.")]
        public Gradient DawnDuskAmbientColor;

        [Tooltip("추가 새벽/황혼 앰비언트 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorIntensity = 0.05f;

        [Tooltip("새벽/황혼 앰비언트 하늘 색상. 가장 오른쪽이 완전히 새벽 또는 황혼 - 알파 값은 강도로 사용.")]
        public Gradient DawnDuskAmbientColorSky;

        [Tooltip("추가 새벽/황혼 앰비언트 하늘 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorSkyIntensity = 0.05f;

        [Tooltip("새벽/황혼 앰비언트 지면 색상. 가장 오른쪽이 완전히 새벽 또는 황혼 - 알파 값은 강도로 사용.")]
        public Gradient DawnDuskAmbientColorGround;

        [Tooltip("추가 새벽/황혼 앰비언트 지면 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorGroundIntensity = 0.05f;

        [Tooltip("새벽/황혼 앰비언트 적도 색상. 가장 오른쪽이 완전히 새벽 또는 황혼 - 알파 값은 강도로 사용.")]
        public Gradient DawnDuskAmbientColorEquator;

        [Tooltip("추가 새벽/황혼 앰비언트 적도 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float DawnDuskAmbientColorEquatorIntensity = 0.05f;

        [Header("Ambient colors - night")]
        [Tooltip("밤 앰비언트 색상. 가장 오른쪽이 완전히 밤 - 알파 값은 강도로 사용.")]
        public Gradient NightAmbientColor;

        [Tooltip("추가 밤 앰비언트 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorIntensity = 0.05f;

        [Tooltip("밤 앰비언트 하늘 색상. 가장 오른쪽이 완전히 밤 - 알파 값은 강도로 사용.")]
        public Gradient NightAmbientColorSky;

        [Tooltip("추가 밤 앰비언트 하늘 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorSkyIntensity = 0.05f;

        [Tooltip("밤 앰비언트 지면 색상. 가장 오른쪽이 완전히 밤 - 알파 값은 강도로 사용.")]
        public Gradient NightAmbientColorGround;

        [Tooltip("추가 밤 앰비언트 지면 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorGroundIntensity = 0.05f;

        [Tooltip("밤 앰비언트 적도 색상. 가장 오른쪽이 완전히 밤 - 알파 값은 강도로 사용.")]
        public Gradient NightAmbientColorEquator;

        [Tooltip("추가 밤 앰비언트 적도 색상 강도")]
        [Range(0.0f, 10.0f)]
        public float NightAmbientColorEquatorIntensity = 0.05f;

        [Header("Sun modifiers")]
        [Tooltip("태양의 색조를 설정하는 그라디언트, 중심은 태양이 수평선에 있는 위치.")]
        public Gradient SunTintColorGradient;

        [Tooltip("태양 강도를 조절하는 그라디언트, 중심은 태양이 수평선에 있는 위치.")]
        public Gradient SunIntensityGradient;

        /// <summary>
        /// 하루의 시간을 TimeSpan 객체로 반환
        /// </summary>
        public TimeSpan TimeOfDayTimespan { get; private set; }

        /// <summary>
        /// 하루의 시간 범주
        /// </summary>
        public WeatherMakerTimeOfDayCategory TimeOfDayCategory { get; private set; }

        /// <summary>
        /// 완전히 낮인 경우 1
        /// </summary>
        public float DayMultiplier { get; private set; }

        /// <summary>
        /// 완전히 새벽 또는 황혼인 경우 1
        /// </summary>
        public float DawnDuskMultiplier { get; private set; }

        /// <summary>
        /// 완전히 밤인 경우 1
        /// </summary>
        public float NightMultiplier { get; private set; }

        /// <summary>
        /// 현재 태양 정보
        /// </summary>
        [NonSerialized]
        public readonly SunInfo SunData = new SunInfo();

        /// <summary>
        /// 현재 달 정보
        /// </summary>
        [NonSerialized]
        public readonly List<MoonInfo> MoonDatas = new List<MoonInfo>();

        /// <summary>
        /// 하루의 총 초 수
        /// </summary>
        public const float SecondsPerDay = 86400.0f;

        /// <summary>
        /// 정오 시간
        /// </summary>
        public const float HighNoonTimeOfDay = SecondsPerDay * 0.5f;

        /// <summary>
        /// 1도당 초 수
        /// </summary>
        public const float SecondsForOneDegree = SecondsPerDay / 360.0f;

        /// <summary>
        /// 동적 GI 업데이트를 위해 경과해야 하는 시간 (초)
        /// </summary>
        public const float DynamicGIUpdateThresholdSeconds = 300.0f;

        private DateTime prevDt;
        /// <summary>
        /// 현재 연도, 월, 일, 시간의 로컬 시간을 나타내는 DateTime 객체 반환
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
        /// TimeOfDay 속성에서 TimeSpan을 반환
        /// </summary>
        public TimeSpan TimeOfDayTimeSpan { get { return TimeSpan.FromSeconds(TimeOfDay); } set { TimeOfDay = (float)value.TotalSeconds; } }

        private float lastTimeOfDayForDynamicGIUpdate = -999999.0f;

        /// <summary>
        /// 율리우스 날짜를 System.DateTime으로 변환
        /// </summary>
        /// <param name="julianDate">율리우스 날짜</param>
        /// <returns>UTC 형식의 DateTime 객체</returns>
        public static DateTime JulianToDateTime(double julianDate)
        {
            double unixTime = (julianDate - 2440587.5) * 86400;
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTime);
            return dtDateTime;
        }

        /// <summary>
        /// 방위각과 고도를 단위 벡터로 변환
        /// </summary>
        /// <param name="azimuth">방위각</param>
        /// <param name="altitude">고도</param>
        /// <param name="vector">단위 벡터</param>
        public static void ConvertAzimuthAtltitudeToUnitVector(double azimuth, double altitude, ref Vector3 vector)
        {
            vector.y = (float)Math.Sin(altitude);
            float hyp = (float)Math.Cos(altitude);
            vector.z = hyp * (float)Math.Cos(azimuth);
            vector.x = hyp * (float)Math.Sin(azimuth);
        }

        /// <summary>
        /// 태양의 위치를 계산
        /// </summary>
        /// <param name="sunInfo">계산된 태양 정보를 받음</param>
        /// <param name="rotateYDegrees">Y축 주위로 회전할 각도</param>
        public static void CalculateSunPosition(SunInfo sunInfo, float rotateYDegrees)
        {
            // dateTime은 이미 UTC 형식이어야 함
            double d = (sunInfo.DateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / dayMs) + jDiff;
            double e = degreesToRadians * sunInfo.AxisTilt; // 지구의 기울기
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
                // 날짜 및 시간 범위를 초과할 경우 크래시 방지
            }
        }

        /// <summary>
        /// 달의 위치를 계산
        /// </summary>
        /// <param name="sunInfo">계산된 태양 정보</param>
        /// <param name="moonInfo">계산된 달 정보를 받음</param>
        /// <param name="rotateYDegrees">달을 y축 주위로 회전할 각도</param>
        public static void CalculateMoonPosition(SunInfo sunInfo, MoonInfo moonInfo, float rotateYDegrees)
        {
            double d = sunInfo.JulianDays;
            double e = degreesToRadians * sunInfo.AxisTilt; // 지구의 기울기
            double L = degreesToRadians * (218.316 + 13.176396 * d); // 황경
            double M = degreesToRadians * (134.963 + 13.064993 * d); // 평균 근점이각
            double F = degreesToRadians * (93.272 + 13.229350 * d); // 평균 거리
            double l = L + degreesToRadians * 6.289 * Math.Sin(M); // 경도
            double b = degreesToRadians * 5.128 * Math.Sin(F); // 위도
            double dist = 385001.0 - (20905.0 * Math.Cos(M)); // 달까지의 거리 (킬로미터)
            double ra = RightAscension(e, l, b);
            double dec = Declination(e, l, b);
            const double sunDistance = 149598000.0; // 지구와 태양 사이의 평균 거리
            double phi = Math.Acos(Math.Sin(sunInfo.Declination) * Math.Sin(dec) + Math.Cos(sunInfo.Declination) * Math.Cos(dec) * Math.Cos(sunInfo.RightAscension - ra));
            double inc = Math.Atan2(sunDistance * Math.Sin(phi), dist - sunDistance * Math.Cos(phi));
            double angle = Math.Atan2(Math.Cos(sunInfo.Declination) * Math.Sin(sunInfo.RightAscension - ra), Math.Sin(sunInfo.Declination) * Math.Cos(dec) - Math.Cos(sunInfo.Declination) * Math.Sin(dec) * Math.Cos(sunInfo.RightAscension - ra));
            double fraction = (1.0 + Math.Cos(inc)) * 0.5;
            double phase = 0.5 + (0.5 * inc * Math.Sign(angle) * (1.0 / Math.PI));
            double lw = -degreesToRadians * sunInfo.Longitude;
            phi = degreesToRadians * sunInfo.Latitude;
            double H = SiderealTime(d, lw) - ra;
            double h = Altitude(H, phi, dec);

            // "Astronomical Algorithms" 2nd edition by Jean Meeus (Willmann-Bell, Richmond) 1998의 14.1 공식.
            double pa = Math.Atan2(Math.Sin(H), Math.Tan(phi) * Math.Cos(dec) - Math.Sin(dec) * Math.Cos(H));
            h = h + AstroRefraction(h); // 굴절을 위한 고도 보정
            double azimuth = Azimuth(H, phi, dec);
            double altitude = h;
            ConvertAzimuthAtltitudeToUnitVector(azimuth, altitude, ref moonInfo.UnitVectorUp);

            // 달의 위치 설정 및 원점을 바라보도록 설정
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
            double c = degreesToRadians * (1.9148 * Math.Sin(m) + 0.02 * Math.Sin(2.0 * m) + 0.0003 * Math.Sin(3.0 * m)); // 중심 방정식
            double p = degreesToRadians * 102.9372; // 지구의 근일점
            return m + c + p + Math.PI;
        }

        private static double AstroRefraction(double h)
        {
            // 이 공식은 양의 고도에 대해서만 작동합니다.
            // h = -0.08901179이면 div/0가 발생합니다.
            h = (h < 0.0 ? 0.0 : h);

            // "Astronomical Algorithms" 2nd edition by Jean Meeus (Willmann-Bell, Richmond) 1998의 16.4 공식.
            // 1.02 / tan(h + 10.26 / (h + 5.10)) h는 도, 결과는 호 분 -> 라디안으로 변환:
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
                    // 현지 시간의 하루 시간을 UTC 시간으로 변환 - 빠르고 간단한 계산
                    double offsetSeconds = TimeZoneOffsetSeconds;
                    TimeSpan t = TimeSpan.FromSeconds(TimeOfDay - offsetSeconds);
                    SunData.DateTime = new DateTime(Year, Month, Day, 0, 0, 0, DateTimeKind.Utc) + t;
                    SunData.Latitude = Latitude;
                    SunData.Longitude = Longitude;
                    SunData.AxisTilt = AxisTilt;

                    // 태양의 위치를 계산하고 설정
                    if (sun.OrbitType == WeatherMakerOrbitType.FromEarth)
                    {
                        CalculateSunPosition(SunData, sun.RotateYDegrees);
                    }
                    SetCelestialObjectPosition(sun, SunData.UnitVectorDown);

                    // 태양 강도 및 그림자 강도를 계산
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

                // 강도는 만월에 비례하여 제곱으로 증가 - 이는 덜 채워진 경우 제곱만큼의 빛 감소를 의미
                // 태양 강도가 1에 가까워질수록 달의 빛 강도 감소
                // 지평선 아래로 떨어질 때 달 빛 감소
                dot = Mathf.Clamp(Vector3.Dot(MoonDatas[i].UnitVectorDown, Vector3.down) + 0.2f, 0.0f, 1.0f);
                dot = Mathf.Pow(dot, 0.25f);
                yPower = Mathf.Clamp((MoonDatas[i].UnitVectorUp.y + 0.2f) * 4.0f, 0.0f, 1.0f);
                moon.Light.color = moon.LightColor;
                moon.Light.intensity = moon.LightBaseIntensity * yPower * sunIntensityReducer * dot * moon.LightMultiplier;
                if (moon.OrbitType == WeatherMakerOrbitType.FromEarth)
                {
                    // 만월에서 멀어질수록 강도가 제곱으로 감소
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

            // 이벤트 전송
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
                        // 예외 무시
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
                // 하루 시간이 랩핑되는 경우 처리
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

            // 이벤트 전송
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
        /// 프로필 설정과 라이트 매니저 상태를 사용하여 낮/밤 주기 업데이트
        /// </summary>
        /// <param name="updateTimeOfDay">하루 시간을 업데이트할지 여부</param>
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
    /// 낮/밤 주기 앰비언트 색상 모드
    /// </summary>
    public enum WeatherMakerDayNightAmbientColorMode
    {
        /// <summary>
        /// 앰비언트 색상 사용 안함, 모두 검정색으로 설정. Unity 앰비언트 라이트 없음.
        /// </summary>
        None = 0,

        /// <summary>
        /// 앰비언트 색상을 앰비언트, 하늘, 적도 및 지면 색상으로 사용. Unity 평면 앰비언트 조명 사용.
        /// </summary>
        AmbientColor = 1,

        /// <summary>
        /// 앰비언트 하늘 색상만 사용, 나머지 앰비언트 색상은 검정색. Unity 평면 앰비언트 조명 사용.
        /// </summary>
        SkyOnly = 2,

        /// <summary>
        /// 앰비언트 하늘, 적도 및 지면 색상 사용. 앰비언트 색상은 검정색. Unity 삼중 조명 앰비언트 조명 사용.
        /// </summary>
        SkyEquatorGround = 4,

        /// <summary>
        /// 앰비언트 색상, 하늘, 적도 및 지면 색상 사용. 앰비언트 색상이 하늘, 적도 및 지면에 추가됨. Unity 삼중 조명 앰비언트 조명 사용.
        /// </summary>
        All = 8,

        /// <summary>
        /// 동적 GI 업데이트만 주기적으로 호출, DynamicGIUpdateThresholdSeconds 상수를 사용. 앰비언트 색상이나 설정을 변경하지 않음.
        /// </summary>
        DynamicGIUpdateOnly = 16,

        /// <summary>
        /// Unity 앰비언트 모드를 그대로 유지. 앰비언트 색상, 하늘, 적도 및 지면 색상 사용. 앰비언트 색상이 하늘, 적도 및 지면에 추가됨.
        /// </summary>
        AllWithUnityMode = 32,

        /// <summary>
        /// 기존 앰비언트 색상 설정을 사용, 낮/밤 주기 프로필 앰비언트 설정 무시.
        /// </summary>
        UnityAmbientSettings = 64
    }

    /// <summary>
    /// 하루 시간 범주
    /// </summary>
    [Flags]
    public enum WeatherMakerTimeOfDayCategory
    {
        /// <summary>
        /// 없음
        /// </summary>
        None = 0,

        /// <summary>
        /// 새벽
        /// </summary>
        Dawn = 1,

        /// <summary>
        /// 낮
        /// </summary>
        Day = 2,

        /// <summary>
        /// 황혼
        /// </summary>
        Dusk = 4,

        /// <summary>
        /// 밤
        /// </summary>
        Night = 8,

        /// <summary>
        /// 일출
        /// </summary>
        Sunrise = 16,

        /// <summary>
        /// 일몰
        /// </summary>
        Sunset = 32
    }
}
