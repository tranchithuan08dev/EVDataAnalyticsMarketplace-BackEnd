ALTER TABLE Users
ADD CONSTRAINT FK_Users_Organization
FOREIGN KEY (OrganizationId) REFERENCES Organizations(OrganizationId);
GO

CREATE TABLE DataQualityFlags (
    FlagId            BIGINT IDENTITY(1,1) PRIMARY KEY,
    DatasetVersionId  UNIQUEIDENTIFIER NOT NULL,
    FlagType          NVARCHAR(50) NOT NULL, -- 'red', 'yellow', 'green'
    Message           NVARCHAR(2000) NOT NULL, -- "Phát hiện PII (email) tại dòng 512"
    DetailsJson       NVARCHAR(MAX) NULL,      
    ProcessedBy       NVARCHAR(100) NULL,      -- Tên của ML service, ví dụ: 'ML-PII-Scanner'
    CreatedAt         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_DataQualityFlags_Version 
        FOREIGN KEY (DatasetVersionId) 
        REFERENCES dbo.DatasetVersions(DatasetVersionId) 
        ON DELETE CASCADE
);
GO
CREATE INDEX IX_DataQualityFlags_VersionId ON DataQualityFlags(DatasetVersionId);
GO

ALTER TABLE Payments
ADD CONSTRAINT FK_Payments_Purchase
FOREIGN KEY (PurchaseId) REFERENCES Purchases(PurchaseId)
ON DELETE SET NULL;
GO