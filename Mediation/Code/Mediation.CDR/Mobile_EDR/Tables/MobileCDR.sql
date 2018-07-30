CREATE TABLE [Mobile_EDR].[MobileCDR] (
    [Id]                              BIGINT         IDENTITY (1, 1) NOT NULL,
    [call_reference]                  BIGINT         NULL,
    [record_type]                     INT            NULL,
    [record_type_name]                NVARCHAR (100) NULL,
    [calling_number]                  VARCHAR (100)  NULL,
    [called_number]                   VARCHAR (100)  NULL,
    [call_duration]                   INT            NULL,
    [setup_time]                      DATETIME       NULL,
    [calling_imsi]                    VARCHAR (50)   NULL,
    [calling_imei]                    VARCHAR (50)   NULL,
    [called_imsi]                     VARCHAR (100)  NULL,
    [called_imei]                     VARCHAR (100)  NULL,
    [connect_date_time]               DATETIME       NULL,
    [disconnect_date_time]            DATETIME       NULL,
    [switch_id]                       INT            NULL,
    [setup_timestamp]                 BIGINT         NULL,
    [connect_timestamp]               BIGINT         NULL,
    [disconnect_timestamp]            BIGINT         NULL,
    [unique_identifier]               VARCHAR (50)   NULL,
    [file_name]                       NVARCHAR (200) NULL,
    [calling_first_ci]                NVARCHAR (255) NULL,
    [calling_last_ci]                 NVARCHAR (255) NULL,
    [called_first_ci]                 NVARCHAR (255) NULL,
    [called_last_ci]                  NVARCHAR (255) NULL,
    [intermediate_charging_indicator] INT            NULL,
    [intermediate_record_number]      INT            NULL,
    [global_call_reference]           NVARCHAR (50)  NULL,
    [cause_for_termination]           INT            NULL
);






































GO
CREATE NONCLUSTERED INDEX [IX_MobileCDR_Id]
    ON [Mobile_EDR].[MobileCDR]([Id] ASC);


GO
CREATE CLUSTERED INDEX [IX_MobileCDR_SetupTime_Id]
    ON [Mobile_EDR].[MobileCDR]([setup_time] ASC, [Id] ASC);

