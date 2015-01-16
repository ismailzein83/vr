using System;

namespace TABS
{
    /// <summary>
    /// Defines the default Pricelist Import Option for a given supplier.
    /// </summary>
    [Serializable]
    public class PricelistImportOption : Components.BaseEntity
    {
        public override string Identifier { get { return "PricelistImportOption:" + Supplier; } }

        public static PricelistImportOption None = new PricelistImportOption();

        private string _ImporterName;
        private DateTime _LastUpdate = DateTime.Now;
        private ImportParameters _settings = new ImportParameters();

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set
            {
                if (this == None) throw new Exception("Cannot Modify this Instance. Please create a new Instance.");
                _LastUpdate = value;
            }
        }

        public string ImportParametersString
        {
            get
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                System.IO.MemoryStream byteStream = new System.IO.MemoryStream();
                binaryFormatter.Serialize(byteStream, _settings);
                return Convert.ToBase64String(byteStream.ToArray());
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    _settings = new ImportParameters();
                }
                else
                {
                    try
                    {
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        System.IO.MemoryStream byteStream = new System.IO.MemoryStream(Convert.FromBase64String(value));
                        _settings = (ImportParameters)binaryFormatter.Deserialize(byteStream);
                    }
                    catch
                    {
                        _settings = new ImportParameters();
                        _settings.CurrencyID = Supplier.CarrierProfile.Currency.Symbol;
                    }
                }
            }
        }

        public virtual string ImporterName
        {
            get { return _ImporterName; }
            set
            {
                if (this == None) throw new Exception("Cannot Modify this Instance. Please create a new Instance.");
                _ImporterName = value;
            }
        }

        public virtual CarrierAccount Supplier { get; set; }

        public virtual ImportParameters Options { get { return _settings; } }

        [Serializable]
        public class ImportParameters
        {
            public ImportParameters()
            {
                this.MaxRatesToImport = -1;
                this.CodeSheetZoneFormat = "{0}";
                this.RateSheetZoneFormat = "{0}";
                this.EffectiveDateFormat = "";
            }

            public TABS.PriceListType PricelistType { get; set; }
            public SupplierPriceListParser SupplierPriceListParser { get; set; }

            public string CurrencyID { get; set; }

            public int CodeSheetIndex { get; set; }
            public int CodeSheetFirstDataRow { get; set; }
            public string CodeSheetZoneFormat { get; set; }
            public int CodeSheetCodeIndex { get; set; }
            public TABS.CodeView CodeSheetCodeLayout { get; set; }
            public bool HasCodeRanges { get; set; }

            public bool HasCodePrefix { get; set; }
            public int CodeSheetPrefixIndex { get; set; }

            public int RateSheetIndex { get; set; }
            public int RateSheetFirstDataRow { get; set; }
            public string RateSheetZoneFormat { get; set; }
            public int RateIndex { get; set; }
            public int MaxRatesToImport { get; set; }
            public bool ShouldImportServices { get; set; }
            public int ServicesIndex { get; set; }

            public bool ShouldImportEffectiveDate { get; set; }
            public int EffectiveDateIndex { get; set; }
            public string EffectiveDateFormat { get; set; }

            public bool HasToDRates { get; set; }
            public int ToDOffPeakRateIndex { get; set; }
            public int ToDWeekendRateIndex { get; set; }

            public string PreProcessingScript { get; set; }
            public string PostProcessingScript { get; set; }

            public bool ShouldImportCodeEffectiveDate { get; set; }
            public bool CodeBEDFollowZoneBED { get; set; }
            public int CodeEffectiveDateIndex { get; set; }
            public string CodeEffectiveDateFormat { get; set; }
            public int EffectiveDateToReadFromIndex { get; set; } //1==Index/2==Date
            public DateTime SpecificEffectiveDate { get; set; }
            public override string ToString()
            {
                return string.Format(
@"PricelistType: {0}, 
Parser: {1}, 
Currency: {2}, 
CodeSheetIndex: {3}, 
CodeSheetFirstDataRow: {4}, 
CodeSheetZoneFormat: {5},
CodeSheetCodeIndex: {6},
CodeSheetCodeLayout: {7},
HasCodeRanges: {8},
HasCodePrefix: {9},
CodeSheetPrefixIndex: {10},
RateSheetIndex: {11},
RateSheetFirstDataRow: {12},
RateSheetZoneFormat: {13},
RateIndex: {14},
MaxRatesToImport: {15},
ShouldImportServices: {16},
ServicesIndex: {17},
EffectiveDateIndex: {18},
EffectiveDateFormat: {19},
HasToDRates: {20},
ToDOffPeakRateIndex: {21},
ToDWeekendRateIndex: {22},
ShouldImportCodeEffectiveDate: {23},
CodeED Follow ZoneED: {24},
CodeEffectiveDateIndex: {25},
CodeEffectiveDateFormat: {26},
PreProcessingScript: {27},
PostProcessingScript: {28}
",
    this.PricelistType,
    this.SupplierPriceListParser,
    this.CurrencyID,
    this.CodeSheetIndex,
    this.CodeSheetFirstDataRow,
    this.CodeSheetZoneFormat,
    this.CodeSheetCodeIndex,
    this.CodeSheetCodeLayout,
    this.HasCodeRanges,
    this.HasCodePrefix,
    this.CodeSheetPrefixIndex,
    this.RateSheetIndex,
    this.RateSheetFirstDataRow,
    this.RateSheetZoneFormat,
    this.RateIndex,
    this.MaxRatesToImport,
    this.ShouldImportServices,
    this.ServicesIndex,
    this.EffectiveDateIndex,
    this.EffectiveDateFormat,
    this.HasToDRates,
    this.ToDOffPeakRateIndex,
    this.ToDWeekendRateIndex,
    this.ShouldImportCodeEffectiveDate,
    this.CodeBEDFollowZoneBED,
    this.CodeEffectiveDateIndex,
    this.CodeEffectiveDateFormat,
    this.PreProcessingScript,
    this.PostProcessingScript
    );
            }
        }
    }
}
