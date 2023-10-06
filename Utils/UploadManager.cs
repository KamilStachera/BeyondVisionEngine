extern alias amazonS3;
extern alias amazonPolly;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Amazon.Polly;
using Amazon.Polly.Model;
using amazonS3::Amazon.S3;

namespace BeyondVisionEngine.Utils
{
    public class UploadManager
    {
        private const string _accessKeyID = "AKIA6C7W7IHAGW4EIZNP";
        private const string _secretKey = "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";
        private const string _bucketName = "beyond-vision-gamedata";
        private IAmazonS3 _s3Client;
        private AmazonPollyClient _pollyClient;
        private List<TaskAwaiter<StartSpeechSynthesisTaskResponse>> _responseAwaiters = new List<TaskAwaiter<StartSpeechSynthesisTaskResponse>>();

        public UploadManager()
        {
            _s3Client = amazonS3::Amazon.AWSClientFactory.CreateAmazonS3Client(_accessKeyID, _secretKey, amazonS3::Amazon.RegionEndpoint.EUCentral1);
            _pollyClient = new AmazonPollyClient(_accessKeyID,_secretKey, amazonPolly::Amazon.RegionEndpoint.EUCentral1);
        }

        public void GenerateAndUploadDialogFile(string gameId, string locationId, string dialogId, int voiceId, string dialog)
        {
            var speechRequest = new StartSpeechSynthesisTaskRequest
            {
                Text = dialog,
                OutputFormat = OutputFormat.Mp3,
                VoiceId = _pollyClient.DescribeVoices(new DescribeVoicesRequest()).Voices[voiceId].Id,
                OutputS3BucketName = _bucketName,
                OutputS3KeyPrefix = $@"{gameId}/{locationId}/{dialogId}"
            };

            _responseAwaiters.Add(_pollyClient.StartSpeechSynthesisTaskAsync(speechRequest).GetAwaiter());
        }

        public bool CheckIfAllFilesWereUploaded()
        {
            return _responseAwaiters.All(responseAwaiter => responseAwaiter.IsCompleted);
        }

        public void UploadJsonToS3(string json, string gameId)
        {
            var transferUtility = new amazonS3::Amazon.S3.Transfer.TransferUtility(_s3Client);
            var transferRequest = new amazonS3::Amazon.S3.Transfer.TransferUtilityUploadRequest
            {
                Key =  $"{gameId}/gameJson.txt", 
                BucketName = _bucketName, 
                FilePath = $"{gameId}.txt"
            };
            transferUtility.Upload(transferRequest);
        }

    }
}
