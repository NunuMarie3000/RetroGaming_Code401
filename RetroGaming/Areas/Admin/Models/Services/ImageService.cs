using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using RetroGaming.Areas.Admin.Models.Interfaces;
using RetroGaming.Areas.Admin.Models.Options;
using Microsoft.Extensions.Options;
using System.Configuration;
using MessagePack.Formatters;
using RetroGaming.Areas.Admin.Models;

namespace RetroGaming.Areas.Admin.Models.Services
{
  public class ImageService: IImageService
  {
    private readonly AzureOptions _azureOptions;
    private readonly IConfiguration _configuration;

    public ImageService( IOptions<AzureOptions> azureOptions, IConfiguration configuration )
    {
      _azureOptions = azureOptions.Value;
      _configuration = configuration;
    }

    public async Task<AzureFile> UploadImageToAzure( IFormFile file )
    {
      BlobContainerClient container = new BlobContainerClient(_azureOptions.ConnectionString, _azureOptions.Container);

      await container.CreateIfNotExistsAsync();
      container.SetAccessPolicy(PublicAccessType.BlobContainer, null, null, default);

      BlobClient blob = container.GetBlobClient(file.FileName);

      using var stream = file.OpenReadStream();

      BlobUploadOptions options = new BlobUploadOptions()
      {
        HttpHeaders = new BlobHttpHeaders() { ContentType = file.ContentType }
      };

      if (!blob.Exists())
      {
        await blob.UploadAsync(stream, options);
      }

      AzureFile document = new AzureFile()
      {
        Name = file.FileName,
        Size = file.Length,
        Type = file.ContentType,
        Url = blob.Uri.ToString(),
      };

      return document;
    }

    public async void DeleteImageFromAzure( string blobName )
    {
      BlobContainerClient container = new BlobContainerClient(_azureOptions.ConnectionString, _azureOptions.Container);

      BlobClient blob = container.GetBlobClient(blobName);

      if (blob.Exists())
      {
        await blob.DeleteIfExistsAsync();
      }

    }

    public bool DoesImageExist(IFormFile file)
    {
      BlobContainerClient container = new BlobContainerClient(_azureOptions.ConnectionString, _azureOptions.Container);

      BlobClient blob = container.GetBlobClient(file.FileName);

      if (blob.Exists())
      {
        return true;
      }
      else
        return false;
    }
  }
}
