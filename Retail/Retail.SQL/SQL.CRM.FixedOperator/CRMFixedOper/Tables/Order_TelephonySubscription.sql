CREATE TABLE [CRMFixedOper].[Order_TelephonySubscription] (
    [ID]            BIGINT         NOT NULL,
    [PhoneNumber]   NVARCHAR (255) NULL,
    [NearbyNumber]  NVARCHAR (255) NULL,
    [SwitchId]      INT            NULL,
    [SwitchName]    NVARCHAR (255) NULL,
    [SwitchType]    NVARCHAR (255) NULL,
    [CabinetId]     INT            NULL,
    [CabinetName]   NVARCHAR (255) NULL,
    [MDFName]       NVARCHAR (255) NULL,
    [DPName]        NVARCHAR (255) NULL,
    [PrimaryPort]   NVARCHAR (255) NULL,
    [SecondaryPort] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

