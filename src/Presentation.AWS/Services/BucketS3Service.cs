using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using HAdministradora.Infra.CrossCutting.Aws.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HAdministradora.Infra.CrossCutting.Aws.Services
{
    public class BucketS3Service : IBucketS3Service
    {
        private string AcessSecret;
        private string AcessKey;
        private string BucketName;
        private readonly RegionEndpoint bucketRegion;
        private readonly IAmazonS3 _s3Client;
        private IConfiguration _config;


        public BucketS3Service(IConfiguration config)
        {

            _config = config;
            AcessSecret = _config.GetSection("AWS_S3_ACESS_SECRET").Value;
            AcessKey = _config.GetSection("AWS_S3_ACESS_KEY").Value;
            BucketName = _config.GetSection("AWS_S3_BUCKET_NAME").Value;
            bucketRegion = RegionEndpoint.USEast2;

            _s3Client = new AmazonS3Client(AcessKey, AcessSecret, bucketRegion);
        }
        #region Upload Files
        public async Task<string> UploadObjectAsync(Stream fileStream, string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(BucketName))
                {
                    return null;
                }

                string bucketLocation = await FindBucketLocationAsync(_s3Client, BucketName);
                if (string.IsNullOrEmpty(bucketLocation))
                {
                    if (!await CreateBucketAsync(_s3Client, BucketName))
                    {
                        return null;
                    }

                }

                var fileTransferUtility = new TransferUtility(_s3Client);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = BucketName,
                    Key = filePath,
                    InputStream = fileStream
                };

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                var bucketWithFileNameString = string.Concat(BucketName, "-", filePath);

                return await Task.FromResult(bucketWithFileNameString);

            }
            catch
            {

                return null;
            }

        }
        #endregion

        #region Download Files
        public async Task<Stream> DownloadObjectAsync(string fileName)
        {
            try
            {


                var transferUtility = new TransferUtility(_s3Client);
                var resposta = await transferUtility.OpenStreamAsync(BucketName, fileName);

                if (resposta != null)
                {
                    return resposta;
                }
                else
                {
                    return null;

                }

            }
            catch (AmazonS3Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Criar um bucket.
        private async Task<bool> CreateBucketAsync(IAmazonS3 clienteS3, string bucketName)
        {
            try
            {
                if (!await AmazonS3Util.DoesS3BucketExistV2Async(clienteS3, bucketName))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    PutBucketResponse putBucketResponse = await clienteS3.PutBucketAsync(putBucketRequest);
                }

                string bucketLocation = await FindBucketLocationAsync(clienteS3, bucketName);
                if (string.IsNullOrEmpty(bucketName))
                {

                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (AmazonS3Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Procurar um bucket e sua localização
        private async Task<string> FindBucketLocationAsync(IAmazonS3 client, string bucketName)
        {
            try
            {

                string bucketLocation;
                var request = new GetBucketLocationRequest()
                {
                    BucketName = bucketName
                };

                GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
                if (response != null && !string.IsNullOrEmpty(response.Location.ToString()))
                {
                    bucketLocation = response.Location.ToString();
                    return bucketName;
                }

                return null;

            }
            catch (AmazonS3Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Deletar file
        public async Task<bool> DeleteSingleObject(string filePath)
        {
            try
            {
                var fileObject = await FindAsyncSingleObject(filePath);
                if (fileObject == null)
                {
                    return false;
                }

                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = BucketName,
                    Key = filePath
                };

                await _s3Client.DeleteObjectAsync(deleteObjectRequest);

                return true;
            }
            catch (AmazonS3Exception ex)
            {

                return false;
            }
        }
        #endregion

        #region buscar file
        private async Task<GetObjectRequest> FindAsyncSingleObject(string filePath)
        {
            try
            {
                var objectRequest = new GetObjectRequest()
                {
                    BucketName = BucketName,
                    Key = filePath
                };
                var file = await _s3Client.GetObjectAsync(objectRequest);

                if (file != null)
                {
                    return await Task.FromResult(objectRequest);
                }
            }
            catch (AmazonS3Exception ex)
            {
                return null;
            }

            return null;

        }
        #endregion
    }
}
