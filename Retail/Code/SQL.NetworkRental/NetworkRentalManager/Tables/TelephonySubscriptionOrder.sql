CREATE TABLE [NetworkRentalManager].[TelephonySubscriptionOrder] (
    [ID]            BIGINT         NOT NULL,
    [PhoneNumber]   VARCHAR (50)   NULL,
    [SwitchId]      INT            NULL,
    [SwitchName]    NVARCHAR (255) NULL,
    [SwitchType]    NVARCHAR (255) NULL,
    [CabinetId]     INT            NULL,
    [CabinetName]   NVARCHAR (255) NULL,
    [MDFName]       NVARCHAR (255) NULL,
    [DPName]        NVARCHAR (255) NULL,
    [PrimaryPort]   NVARCHAR (255) NULL,
    [SecondaryPort] NVARCHAR (255) NULL,
    [CreatedTime]   DATETIME       NULL,
    [RatePlan]      BIGINT         NULL,
    CONSTRAINT [PK_TelephonySubscriptionOrder] PRIMARY KEY CLUSTERED ([ID] ASC)
);

