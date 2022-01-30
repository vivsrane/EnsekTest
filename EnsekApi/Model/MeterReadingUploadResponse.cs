using System.Collections.Generic;

namespace EnsekApi
{
    public class MeterReadingUploadResponse
    {
        public int SuccessfulUploadRecords { get; set; }
        public int FailedUploadRecords { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}