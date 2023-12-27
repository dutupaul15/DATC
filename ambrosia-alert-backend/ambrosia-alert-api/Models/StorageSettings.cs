using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace ambrosia_alert_api
{
    public class AzureTableSettings
    {
        public AzureTableSettings(string storageAccount,
                                       string storageKey,
                                       string tableName)
        {
            if (string.IsNullOrEmpty(storageAccount))
                throw new ArgumentNullException(storageAccount);

            if (string.IsNullOrEmpty(storageKey))
                throw new ArgumentNullException(storageKey);

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(tableName);

            StorageAccount = storageAccount;
            StorageKey = storageKey;
            TableName = tableName;
        }

        public string StorageAccount { get; }
        public string StorageKey { get; }
        public string TableName { get; }
    }
}