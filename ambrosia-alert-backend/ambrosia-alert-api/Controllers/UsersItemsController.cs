using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ambrosia_alert_api.Models;
using Microsoft.AspNetCore.Cors;

namespace ambrosia_alert_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersItemsController : ControllerBase
    {
        private readonly CloudTable _table;

        public UsersItemsController(IConfiguration configuration)
        {
            string? storageConnectionString = configuration.GetConnectionString("AzureStorage");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("AmbrosiaAppUsers");
            _table.CreateIfNotExistsAsync();
        }

        // GET: api/UsersItems
        [EnableCors("_myAllowSpecificOrigins")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserItem>>> GetUsersItems()
        {
            var query = new TableQuery<UserItem>();
            var users = new List<UserItem>();

            TableContinuationToken? token = null;

            do
            {
                TableQuerySegment<UserItem> segment = await _table.ExecuteQuerySegmentedAsync(query, token);
                users.AddRange(segment.Results);
                token = segment.ContinuationToken;
            } while (token != null);

            if (users == null || users.Count == 0)
            {
                return NotFound();
            }

            return users;
        }

        // GET: api/UsersItems/{partitionKey}/{rowKey}
        [EnableCors("_myAllowSpecificOrigins")]
        [HttpGet("{partitionKey}/{rowKey}")]
        public async Task<ActionResult<UserItem>> GetUserItem(string partitionKey, string rowKey)
        {
            var query = new TableQuery<UserItem>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey)
                )
            );

            TableContinuationToken? token = null;
            TableQuerySegment<UserItem> segment;
            try
            {
                segment = await _table.ExecuteQuerySegmentedAsync(query, token);
            }
            catch (Exception)
            {
                // Handle exceptions, such as table not found, as needed
                return NotFound();
            }

            if (segment.Results.Count == 0)
            {
                return NotFound();
            }

            return segment.Results.First();
        }

        // POST: api/UsersItems
        [EnableCors("_myAllowSpecificOrigins")]
        [HttpPost]
        public async Task<ActionResult<UserItem>> CreateUserItem(UserItem user)
        {
            try
            {
                // Set the partition key and row key based on your logic
                user.PartitionKey = user.LastName; // Replace with your actual partition key
                user.RowKey = user.Email;

                TableOperation operation = TableOperation.Insert(user);

                await _table.ExecuteAsync(operation);

                // Return the created student object with the generated row key
                return CreatedAtAction("GetUserItem", new { partitionKey = user.PartitionKey, rowKey = user.RowKey }, user);
            }
            catch (Exception)
            {
                // Handle exceptions, such as conflicts or table not found, as needed
                return BadRequest(); // You can return a different status code based on the specific error
            }
        }

        // DELETE: api/UsersItems/5
        //[HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUserItem(long id)
        // {
        //     if (_context.UsersItems == null)
        //     {
        //         return NotFound();
        //     }
        //     var userItem = await _context.UsersItems.FindAsync(id);
        //     if (userItem == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.UsersItems.Remove(userItem);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        // private bool UserItemExists(long id)
        // {
        //     return (_context.UsersItems?.Any(e => e.Id == id)).GetValueOrDefault();
        // }

    }
}
