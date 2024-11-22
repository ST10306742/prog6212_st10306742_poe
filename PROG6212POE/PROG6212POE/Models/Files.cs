using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models
{
    public class Files
        //part 3 
        //issue : files not uploading to Files table yet
    {
        [Key]
        public int fileId { get; set; }
        public string fileName { get; set; } //original name
        public string UniqueFileName { get; set; } // Stored name
        public byte[] Data { get; set; }
        public long Length { get; set; }
        public string ContentType { get; set; }

        //link to claims model
        public int ClaimId { get; set; } //fk to show whom the files belong to
        public Claims claims { get; set; }
    }
}
    