-- 1. Create schema
IF schema_id('lw') IS NULL
    EXECUTE('CREATE SCHEMA [lw]')

-- 2. Create tables
CREATE TABLE [lw].[Events] (
    [EventId] [int] NOT NULL IDENTITY,
    [TenantId] [int],
    [ApplicationId] [int],
    [Code] [nvarchar](max),
    [Name] [nvarchar](max),
    CONSTRAINT [PK_lw.Events] PRIMARY KEY ([EventId])
)

CREATE TABLE [lw].[Webhooks] (
    [WebhookId] [int] NOT NULL IDENTITY,
    [EventId] [int] NOT NULL,
    [Name] [nvarchar](max),
    [Url] [nvarchar](max),
    [ContentType] [int] NOT NULL,
    [Payload] [nvarchar](max),
    CONSTRAINT [PK_lw.Webhooks] PRIMARY KEY ([WebhookId])
)

CREATE TABLE [lw].[WebhookLog] (
    [LogId] [int] NOT NULL IDENTITY,
    [Created] [datetime] NOT NULL,
    [Updated] [datetime],
    [Webhook_WebhookId] [int],
    CONSTRAINT [PK_lw.WebhookLog] PRIMARY KEY ([LogId])
)

-- 3. Create indexes
CREATE INDEX [IX_EventId] ON [lw].[Webhooks]([EventId])
CREATE INDEX [IX_Webhook_WebhookId] ON [lw].[WebhookLog]([Webhook_WebhookId])

-- 4. Add foreign keys
ALTER TABLE [lw].[Webhooks] ADD CONSTRAINT [FK_lw.Webhooks_lw.Events_EventId] FOREIGN KEY ([EventId]) REFERENCES [lw].[Events] ([EventId]) ON DELETE CASCADE
ALTER TABLE [lw].[WebhookLog] ADD CONSTRAINT [FK_lw.WebhookLog_lw.Webhooks_Webhook_WebhookId] FOREIGN KEY ([Webhook_WebhookId]) REFERENCES [lw].[Webhooks] ([WebhookId])