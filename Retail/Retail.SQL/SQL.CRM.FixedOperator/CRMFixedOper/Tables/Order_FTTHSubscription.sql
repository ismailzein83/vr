CREATE TABLE [CRMFixedOper].[Order_FTTHSubscription] (
    [ID]           BIGINT         NOT NULL,
    [NearbyNumber] NVARCHAR (255) NULL,
    [PhoneNumber]  NVARCHAR (255) NULL,
    [DPNumber]     NVARCHAR (255) NULL,
    [FDBNumber]    NVARCHAR (255) NULL,
    [IMSId]        INT            NULL,
    [IMSName]      NVARCHAR (255) NULL,
    [MSANId]       INT            NULL,
    [MSANName]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

