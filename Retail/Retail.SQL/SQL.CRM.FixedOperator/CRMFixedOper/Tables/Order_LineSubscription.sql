CREATE TABLE [CRMFixedOper].[Order_LineSubscription] (
    [ID]                BIGINT         NOT NULL,
    [TelephonySelected] BIT            NULL,
    [DataSelected]      BIT            NULL,
    [PhoneNumber]       VARCHAR (50)   NULL,
    [InternetUsername]  NVARCHAR (255) NULL,
    [CreatedTime]       DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

