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

namespace ambrosia_alert_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersItemsController : ControllerBase
    {
        private readonly CloudTable? _table;

        public UsersItemsController(IConfiguration configuration)
        {
            string? storageConnectionString = configuration.GetConnectionString("AzureStorage");
            //string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=datctable1;AccountKey=i2NdpOOkCX3MuBvbXrNB8Hqgo3CTk1zVQaVrDCHPj5LrkC+le3IwbPyXOHpmzOKxlPnNw5nC2+zv+AStc9Y4VQ==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("AmbrosiaAppUsers");
            _table.CreateIfNotExistsAsync();
        }

        // GET: api/UsersItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserItem>>> GetUsersItems()
        {
            var query = new TableQuery<UserItem>();
            var students = new List<UserItem>();

            TableContinuationToken? token = null;

            do
            {
                TableQuerySegment<UserItem> segment = await _table.ExecuteQuerySegmentedAsync(query, token);
                students.AddRange(segment.Results);
                token = segment.ContinuationToken;
            } while (token != null);

            if (students == null || students.Count == 0)
            {
                return NotFound();
            }

            return students;
        }


        // GET: api/UsersItems/{partitionKey}/{rowKey}
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
            catch (Exception ex)
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




        // PUT: api/StudentsItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        // public async Task<IActionResult> PutStudentsItem(long id, StudentsItem studentsItem)
        // {
        //     if (id != studentsItem.Id)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(studentsItem).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!StudentsItemExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // POST: api/StudentsItems
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
            catch (Exception ex)
            {
                // Handle exceptions, such as conflicts or table not found, as needed
                return BadRequest(); // You can return a different status code based on the specific error
            }
        }

        // DELETE: api/StudentsItems/5
        //[HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteStudentsItem(long id)
        // {
        //     if (_context.StudentsItems == null)
        //     {
        //         return NotFound();
        //     }
        //     var studentsItem = await _context.StudentsItems.FindAsync(id);
        //     if (studentsItem == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.StudentsItems.Remove(studentsItem);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        // private bool StudentsItemExists(long id)
        // {
        //     return (_context.StudentsItems?.Any(e => e.Id == id)).GetValueOrDefault();
        // }

    }
}
