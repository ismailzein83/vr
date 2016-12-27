CREATE TABLE [NP_IVSwitch].[codec_defs] (
    [codec_id]              INT           NOT NULL,
    [pt]                    INT           NULL,
    [fs_name]               VARCHAR (20)  NULL,
    [description]           VARCHAR (50)  NULL,
    [encoding_type]         CHAR (1)      NULL,
    [bits_per_sec]          INT           NULL,
    [bits_per_sample]       INT           NULL,
    [ms_per_frame]          INT           NULL,
    [clock_rate]            INT           NULL,
    [default_ms_per_packet] INT           NULL,
    [avl_min_ptime]         INT           NULL,
    [avl_max_ptime]         INT           NULL,
    [passthru]              INT           NULL,
    [display_order]         INT           NULL,
    [mos]                   VARCHAR (20)  NULL,
    [note]                  VARCHAR (200) NULL,
    CONSTRAINT [codec_defs_pk_codec_id] PRIMARY KEY CLUSTERED ([codec_id] ASC)
);

