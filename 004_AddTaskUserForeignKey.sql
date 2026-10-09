USE TaskManagementDb;
GO

ALTER TABLE Tasks
ADD CONSTRAINT FK_Tasks_Users
    FOREIGN KEY (UserId)
    REFERENCES Users(Id);
GO