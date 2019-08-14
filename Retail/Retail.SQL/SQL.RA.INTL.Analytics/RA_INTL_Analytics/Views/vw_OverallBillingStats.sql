
CREATE VIEW [RA_INTL_Analytics].[vw_OverallBillingStats]
AS
SELECT        voice.[BatchStart], voice.[OperatorID], voice.[NumberOfCDRs], 0 AS NumberOfSMSs, voice.[TrafficDirection], voice.[Rate], voice.[TotalRevenue], voice.[TotalIncome], voice.[TotalDurationInSeconds], 
                         voice.[TotalSaleDurationInSeconds], voice.[CurrencyID]
/*		,voice.[MaximumDurationInSeconds]*/ FROM RA_INTL_Analytics.BillingStatsDaily AS voice
UNION ALL
SELECT        sms.[BatchStart], sms.[OperatorID], 0 AS NumberOfCDRs, sms.[NumberOfSMSs], sms.[TrafficDirection], sms.[Rate], sms.[TotalRevenue], sms.[TotalIncome], 0 AS TotalDurationInSeconds, 
                         0 AS TotalSaleDurationInSeconds, sms.[CurrencyID]
/*		, sms.[DestinationMobileCountry]*/ FROM RA_INTL_SMSAnalytics.BillingStatsDaily AS sms