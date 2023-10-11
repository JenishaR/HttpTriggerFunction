using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.EventGrid;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace HttpTriggerFunctionApp
{
    public static class HttpFunction
    {
        [FunctionName("HttpFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,           
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string firstname = req.Query["firstname"];
            string lastname = req.Query["lastname"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var payload = JsonConvert.DeserializeObject<object>(requestBody);
            var file = JsonConvert.SerializeObject(payload);
            //var validator = new PersonValidator();
            
            firstname = firstname ?? data?.firstname;
            lastname = lastname ?? data?.lastname;

            var validator = new PersonValidator();
            var person = new Person(firstname, lastname);

            var validationResult = validator.Validate(person);

            if (!validationResult.IsValid)
            {
                var errors = new List<string>();
                foreach (var error in validationResult.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
                log.LogInformation($"Validation Failed: {errors[0]}");
                return new BadRequestObjectResult(errors);
            }

            var storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=jehttptriggerfunctionapp;AccountKey=QYkY8Gr2as0Sr1UPetvEPIvCG9OKi2XUYO/9ZeO8vbZfeezCBP/P7FKlNLAnxLjiKacmhzgygP4v+ASttGUnkw==;EndpointSuffix=core.windows.net";

            var containerName = "demo";

            var blobName = $"Folder/file-{Guid.NewGuid()}.json";



            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

            await blobContainer.CreateIfNotExistsAsync();

            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);

            await blob.UploadTextAsync(file);

            string responseMessage =  $"Hello, {firstname}. This HTTP triggered function executed successfully.";
            log.LogInformation("Validation Passed");

            return new OkObjectResult(responseMessage);
        }
       
    }
}
