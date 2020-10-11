using System;
namespace consumerFunction01.DataModel
{
    public class Wng
    {
        public string TransactionId { get; set; }
        public int InternalVin { get; set; }
        public DateTime OccurrenceDate { get; set; }
        public string ActionCode { get; set; }
        public Getwnginfo getwnginfo { get; set; }
        public string vehicleInformation { get; set; }
        public DateTime onetimePassAuthenticationDateTime { get; set; }
        public Userservicelist[] userServiceList { get; set; }
    }

    public class Getwnginfo
    {
        public string resultCode { get; set; }
        public Wnginformationlist[] wngInformationList { get; set; }
    }

    public class Wnginformationlist
    {
        public string vin { get; set; }
        public int internalVin { get; set; }
        public DateTime centerReceivedTime { get; set; }
        public string groupDivision { get; set; }
        public string triggerNumber { get; set; }
        public object triggerNumberPosition { get; set; }
        public Warningfileanalysisdetail[] warningFileAnalysisDetail { get; set; }
        public object vdtSoftwareNumber { get; set; }
        public object triggerTime { get; set; }
        public DateTime occurrenceDate { get; set; }
        public int relativeOccurrenceTime { get; set; }
        public DateTime fileTimestamp { get; set; }
        public DateTime sortDate { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public int relevanceSpecifiedValue { get; set; }
    }

    public class Warningfileanalysisdetail
    {
        public string languageCode { get; set; }
        public Info info { get; set; }
    }

    public class Info
    {
        public string talkGuideURL { get; set; }
        public string electronicOmURL { get; set; }
        public string warningLightName { get; set; }
        public string WarningLightIconFileName { get; set; }
        public string meterDisplay { get; set; }
        public string centerDisplay { get; set; }
        public string severityRating { get; set; }
    }

    public class Userservicelist
    {
        public int internalUserId { get; set; }
        public string serviceNameList { get; set; }
        public DateTime startDateTime { get; set; }
        public object endDateTime { get; set; }
    }
}
