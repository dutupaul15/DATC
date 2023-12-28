using Azure;
using Azure.Data.Tables;

namespace ambrosia_alert_api.Models;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

public class UserItem : TableEntity
{
    // Properties from ITableEntity
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password {get; set;}


    public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
    {
        TableEntity.ReadUserObject(this, properties, operationContext);
    }
    public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
    {
        return TableEntity.WriteUserObject(this, operationContext);
    }
}
